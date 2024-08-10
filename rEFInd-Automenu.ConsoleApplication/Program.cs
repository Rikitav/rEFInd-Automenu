using CommandLine;
using CommandLine.Text;
using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.FunctionalWorkers;
using rEFInd_Automenu.Logging;
using rEFInd_Automenu.RegistryExplorer;
using rEFInd_Automenu.Resources;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.Plasma.Tasks.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace rEFInd_Automenu.ConsoleApplication
{
    internal static class ConsoleProgram
    {
        // Logger
        public static readonly ILog Logger = LogManager.GetLogger(typeof(ConsoleProgram));

        public static bool IsElevatedProcess
        {
            get
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // Interface fields
        internal static readonly TaskShell Interface = new TaskShell()
        {
            HandleMethod = ExceptionHandleMethod.AsError,
            ExitOnExceptionalError = false,
        };

        private static void Main(string[] args)
        {
            // Initilazing interface
            CoreLogging.InitLogFile();
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            Interface.AfterControllerError += AfterControllerError;

            //
            // Program header
            Version[] TitleVersions = new Version[] { EmbeddedResourceManager.rEFIndBin_VersionInResources, Assembly.GetExecutingAssembly().GetName().Version ?? new Version(2, 0, 0, 0) };
            int MaxVersionLength = TitleVersions.Select(v => v.ToString().Length).Max();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("rEFInd boot manager {0, -" + MaxVersionLength + "} (C) Roderick W. Smith", TitleVersions[0]);
            Console.WriteLine("rEFInd Automenu     {0, -" + MaxVersionLength + "} (C) Rikitav", TitleVersions[1]);
            Console.WriteLine();
            Console.ResetColor();

            // This var need for correct output formating
            bool AnyEnvironmentError = false;

            // Checking UEFI availablity
            if (!FirmwareInterface.Available)
            {
                AnyEnvironmentError = true;
                Logger.Warn("The program was launched on a system that does not support UEFI. Some features will be disabled");
                ConsoleInterfaceWriter.WriteWarning("Firmware interface is not available on current system. Some functions will be disabled");
            }

            // Checking processor architecture
            if (!ArchitectureInfo.IsCurrentPlatformSupported())
            {
                AnyEnvironmentError = true;
                Logger.Warn(RuntimeInformation.OSArchitecture + " Architecture is not capable for rEFInd installation. Some features will be disabled");
                ConsoleInterfaceWriter.WriteWarning(RuntimeInformation.OSArchitecture + " Architecture is not capable for rEFInd installation. Some features will be disabled");
            }

            // Checking administrator rights
            if (!IsElevatedProcess)
            {
                AnyEnvironmentError = true;
                Logger.Warn("The program was launched without administrator rights. Some features will be disabled");
                ConsoleInterfaceWriter.WriteWarning("The program was launched without administrator rights. Some features will be disabled");
            }

            if (AnyEnvironmentError)
                Console.WriteLine();

            // CMD arguments Parser
            //StringWriter ParseErrorsWriter = new StringWriter();
            Parser ArgsParser = new Parser(With =>
            {
                With.HelpWriter = null; //ParseErrorsWriter;
                With.EnableDashDash = true;
                With.AllowMultiInstance = false;
                With.CaseSensitive = false;
                With.CaseInsensitiveEnumValues = false;
                With.AutoVersion = false;
            });

            // Parsing command line arguments
            ParserResult<object> parserResult = ArgsParser.ParseArguments<
                InstallArgumentsInfo, InstanceArgumentsInfo, GetArgumentsInfo>(args);

            if (parserResult.Errors.Any())
            {
                Logger.Error("Arguments was not parsed");
                //ConsoleInterfaceWriter.WriteWarning(ParseErrorsWriter.ToString());
                WriteHelp(parserResult);
                Environment.Exit(1);
            }

            switch (args.ElementAt(0).ToLower())
            {
                case "install":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Installing rEFInd");
                        parserResult.WithParsed<InstallArgumentsInfo>(info =>
                        {
                            LogArguments(info);
                            if (!ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind install --help\" for details.");
                                Environment.Exit(1);
                            }

                            InstallArgumentsWorker.Execute(info);
                        });
                        break;
                    }

                case "instance":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Touching instance");
                        parserResult.WithParsed<InstanceArgumentsInfo>(info =>
                        {
                            LogArguments(info);
                            if (!ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind instance --help\" for details.");
                                Environment.Exit(1);
                            }

                            InstanceArgumentsWorker.Execute(info);
                        });
                        break;
                    }

                case "get":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Getting object");
                        parserResult.WithParsed<GetArgumentsInfo>(info =>
                        {
                            LogArguments(info);
                            if (!ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind get --help\" for details.");
                                Environment.Exit(1);
                            }

                            GetArgumentsWorker.Execute(info);
                        });
                        break;
                    }

                case "config":
                    {
                        // Can be implemented
                        break;
                    }
            }

            Environment.Exit(0);
        }

        private static void AfterControllerError(object sender, TaskShellEventArgs args)
        {
            if (args.TerminationRequested)
                Environment.Exit(1);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.IsTerminating)
                Logger.Fatal("Process was terminated by unhandled exception", args.ExceptionObject as Exception);
        }

        private static void ProcessExit(object? sender, EventArgs args)
        {
            MountVolBribge.UnmountEsp();
            ProgramRegistry.Branch.Close();
        }

        public static TCommands GetControllerCommands<TCommands>(object? LockHandle = null) where TCommands : IConsoleInterfacenterfaceCommands, new()
        {
            TCommands ctrl = new TCommands();
            if (LockHandle != null)
                ctrl.SetLockHandle(LockHandle);

            return ctrl;
        }

        public static bool ValidMainArgumentsCount(object argumentsInfo, string[] args)
        {
            int MainArgsCount = argumentsInfo.GetType().GetProperties().Count(prop =>
            {
                OptionAttribute? optionAttribute = prop.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute == null)
                    return false;

                if (string.IsNullOrEmpty(optionAttribute.Group))
                    return false;

                return 
                    args.Contains("-" + optionAttribute.ShortName, StringComparer.CurrentCultureIgnoreCase) ||
                    args.Contains("--" + optionAttribute.LongName, StringComparer.CurrentCultureIgnoreCase);
            });

            return MainArgsCount == 1;
        }

        public static void WriteHelp<TArguments>(ParserResult<TArguments> parserResult)
        {
            HelpText Help = HelpText.AutoBuild(parserResult);
            Help.Copyright = string.Empty;
            Help.Heading = string.Empty;
            Help.AddEnumValuesToHelpText = true;

            string HelpString = Help.ToString();
            ConsoleInterfaceWriter.WriteWarning("\n" + HelpString.TrimStart('\n', '\r'));
        }

        private static void LogArguments(object ArgumentsInfo)
        {
            // 
            Type ObjType = ArgumentsInfo.GetType();
            PropertyInfo[] Props = ObjType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // 
            string[] FieldNames = Props.Select(x => x.Name).ToArray();
            string[] TypeNames = Props.Select(x => x.PropertyType.FullName ?? "NullPropertyName").ToArray();
            string[] Values = Props.Select(x => x.GetValue(ArgumentsInfo)?.ToString() ?? "<NULL>").ToArray();

            // 
            int FieldNamesPadding = FieldNames.Select(x => x.Length).Max();
            int TypeNamesPadding = TypeNames.Select(x => x.Length).Max();
            int ValuesPadding = Values.Select(x => x.Length).Max();

            // 
            StringBuilder builder = new StringBuilder("From \"" + ObjType.Name + "\" Parsed :\n");
            builder.AppendLine(string.Format("{0} | {1} | {2}", "Field".PadRight(FieldNamesPadding), "Type".PadRight(TypeNamesPadding), "Value".PadRight(ValuesPadding)));
            builder.AppendLine(string.Format("{0} | {1} | {2}", new string(' ', FieldNamesPadding), new string(' ', TypeNamesPadding), new string(' ', ValuesPadding)));

            for (int i = 0; i < Values.Length; i++)
            {
                builder.AppendLine(string.Format("{0} | {1} | {2}",
                    FieldNames[i].PadRight(FieldNamesPadding),
                    TypeNames[i].PadRight(TypeNamesPadding),
                    Values[i].PadRight(ValuesPadding)));
            }

            Logger.Info(builder.ToString());
        }
    }
}