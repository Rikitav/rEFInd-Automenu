using CommandLine;
using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.GlobalConfig;
using rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Install;
using rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance;
using rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.MenuEntries;
using rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Obtain;
using rEFInd_Automenu.Resources;
using rEFInd_Automenu.RuntimeLogging;
using rEFInd_Automenu.Win32;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.Plasma.Tasks.Management;
using System.Reflection;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.ConsoleApplication
{
    public static class ConsoleProgram
    {
        // Logger
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ConsoleProgram));

        // Interface fields
        public static readonly bool IsElevatedProcess = Win32Utilities.ProcessHasAdminRigths();
        public static readonly object SyncLockObject = new object();
        public static readonly TaskShell Interface = new TaskShell();

        public static  ConsoleControllerCommands CommonCommands
        {
            get
            {
                ConsoleControllerCommands commands = new ConsoleControllerCommands();
                commands.SetLockHandle(SyncLockObject);
                return commands;
            }
        }

        public static ConsoleControllerProgressBarCommands ProgressCommands
        {
            get
            {
                ConsoleControllerProgressBarCommands commands = new ConsoleControllerProgressBarCommands();
                commands.SetLockHandle(SyncLockObject);
                return commands;
            }
        }

        // Initiliazation
        static ConsoleProgram()
        {
            // Writing program header
            WriteHeader();

            // Initilazing runtime
            CoreLogging.InitLogFile();
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            Interface.AfterControllerError += AfterControllerExecution;
            Interface.AfterControllerWarning += AfterControllerExecution;
            Interface.AfterControllerFaulted += AfterControllerFaulted;

            // Configuring TaskShell
            Interface.HandleMethod = ExceptionHandleMethod.AsError;
            Interface.ExitOnExceptionalError = false;
        }

        private static void Main(string[] args)
        {
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

            // This statement need for correct output formating
            if (AnyEnvironmentError)
                Console.WriteLine();

            // Processing arguments
            ProcessArguments(args);
            Environment.Exit(0);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.IsTerminating)
                Logger.Fatal("Process was terminated by unhandled exception", args.ExceptionObject as Exception);
        }

        private static void AfterControllerExecution(object sender, TaskShellEventArgs args)
        {
            if (args.TerminationRequested)
                Environment.Exit(1);
        }

        private static void AfterControllerFaulted(object sender, TaskShellEventArgs args)
        {
            Logger.Fatal("Process was terminated by unhandled exception", args.FaultedException);
            Environment.Exit(1);
        }

        private static void WriteHeader()
        {
            // In case you like those fancy headers, you can turn it on trough registry key!
            if (RegistryExplorer.Instance.ShowLargeHeader)
            {
                // Writing sexy header
                Console.WriteLine();
                Console.WriteLine(@"   ______   ______   ______  __   __   __   _____    ");
                Console.WriteLine(@"  /\  == \ /\  ___\ /\  ___\/\ \ /\ '-.\ \ /\  __-.  ");
                Console.WriteLine(@"  \ \  __< \ \  __\ \ \  __\\ \ \\ \ \-.  \\ \ \/\ \ ");
                Console.WriteLine(@"   \ \_\ \_\\ \_____\\ \_\   \ \_\\ \_\\'\_\\ \____- ");
                Console.WriteLine(@"    \/_/ /_/ \/_____/ \/_/    \/_/ \/_/ \/_/ \/____/ ");
            }

            // Getting versions
            Version bootManagerVersion = EmbeddedResourceManager.rEFIndBin_VersionInResources;
            Version automenuVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(2, 0, 0, 0);
            int MaxVersionLength = new Version[] { bootManagerVersion, automenuVersion }.Select(v => v.ToString().Length).Max();

            // Program and Copyright header
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("rEFInd boot manager {0} (C) Roderick W. Smith", bootManagerVersion.ToString().PadRight(MaxVersionLength));
            Console.WriteLine("rEFInd Automenu     {0} (C) Rikitav", automenuVersion.ToString().PadRight(MaxVersionLength));
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void ProcessArguments(string[] args)
        {
            // Arguments parser
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
                InstallArgumentsInfo, InstanceArgumentsInfo, GetArgumentsInfo,
                GlobalConfigurationArgumentsInfo, EntriesConfigurationArgumentsInfo>(args);

            if (parserResult.Errors.Any())
            {
                Logger.Error("Arguments was not parsed");
                ArgumentsHelper.WriteHelp(parserResult);
                Environment.Exit(1);
            }

            switch (args.ElementAt(0).ToLower())
            {
                case "install":
                case "setup":
                case "make":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Installing rEFInd");
                        parserResult.WithParsed<InstallArgumentsInfo>(info =>
                        {
                            ArgumentsHelper.LogArguments(info, Logger);
                            if (!ArgumentsHelper.ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind install --help\" for details.");
                                Environment.Exit(1);
                            }

                            ProcedureProcessor_Install.Execute(info);
                        });
                        break;
                    }

                case "instance":
                case "present":
                case "existent":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Touching instance");
                        parserResult.WithParsed<InstanceArgumentsInfo>(info =>
                        {
                            ArgumentsHelper.LogArguments(info, Logger);
                            if (!ArgumentsHelper.ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind instance --help\" for details.");
                                Environment.Exit(1);
                            }

                            ProcedureProcessor_Instance.Execute(info);
                        });
                        break;
                    }

                case "get":
                case "obtain":
                case "gain":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Getting object");
                        parserResult.WithParsed<GetArgumentsInfo>(info =>
                        {
                            ArgumentsHelper.LogArguments(info, Logger);
                            if (!ArgumentsHelper.ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind get --help\" for details.");
                                Environment.Exit(1);
                            }

                            ProcedureProcessor_Obtain.Execute(info);
                        });
                        break;
                    }

                case "configuration":
                case "config":
                case "settings":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Changing config");
                        parserResult.WithParsed<GlobalConfigurationArgumentsInfo>(info =>
                        {
                            ArgumentsHelper.LogArguments(info, Logger);
                            if (!ArgumentsHelper.ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind configuration --help\" for details.");
                                Environment.Exit(1);
                            }

                            ProcedureProcessor_GlobalConfig.Execute(info);
                        });
                        break;
                    }

                case "menuentry":
                case "bootoption":
                case "record":
                    {
                        ConsoleInterfaceWriter.WriteHeader("Changing config");
                        parserResult.WithParsed<EntriesConfigurationArgumentsInfo>(info =>
                        {
                            ArgumentsHelper.LogArguments(info, Logger);
                            if (!ArgumentsHelper.ValidMainArgumentsCount(info, args))
                            {
                                ConsoleInterfaceWriter.WriteWarning("You can only specify one main argument from a group. Type \"refind menuentry --help\" for details.");
                                Environment.Exit(1);
                            }

                            ProcedureProcessor_MenuEntries.Execute(info);
                        });
                        break;
                    }
            }
        }
    }
}