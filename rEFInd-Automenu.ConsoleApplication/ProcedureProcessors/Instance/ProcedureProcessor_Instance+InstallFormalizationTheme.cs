using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.Parsing;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Installing;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.TypesExtensions;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        public static void InstallFormalizationTheme(InstanceArgumentsInfo argumentsInfo)
        {
            // Working
            log.Info("Started installation formalization theme for current rEFInd instatnce (\'Instance --InstallTheme\' flag)");

            if (argumentsInfo.InstallTheme == null)
                throw new ArgumentNullException(nameof(argumentsInfo.InstallTheme), "\"Formalization directory\" argument is null or empty");

            if (!argumentsInfo.InstallTheme.Exists)
                throw new DirectoryNotFoundException(string.Format("Formalization directory doesnt exist in \"{0}\"", argumentsInfo.InstallTheme.FullName));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Removing theme if exist
            if (EspRefindDir.GetSubDirectory("theme").Exists)
            {
                ConsoleProgram.Interface.Execute("Removing old theme", ConsoleProgram.CommonCommands, (ctrl) =>
                {
                    try
                    {
                        log.Warn("Theme directory is already exist. Directory will be deleted");
                        EspRefindDir.GetSubDirectory("theme").Delete(true);
                        log.Info("OLD Theme directory was deleted");
                    }
                    catch (Exception exc)
                    {
                        log.Error("Failed to delete old formalization theme directory", exc);
                        ctrl.Error("Failed to delete old formalization theme directory");
                    }
                });
            }

            // Installing formaliztion theme
            DirectoryInfo? formalizationThemePath = InstallingOperations.InstallFormalizationThemeDirectory(
                argumentsInfo.InstallTheme, // ThemeDir
                EspRefindDir);              // RefindInstallationDir

            // Correcting config file to have\add 'include' keyword
            ConsoleProgram.Interface.Execute("Config correction", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                // Getting config file path
                string configFilePath = Path.Combine(EspRefindDir.FullName, "refind.conf");

                // Parsing configuration information from file
                log.Info("Parsing config file");
                RefindConfiguration conf = ConfigurationFileParser.FromFile(configFilePath);

                // Modifying config
                log.Info("Modifying configuration information");
                conf.Global.Include = @"theme/theme.conf";

                // reassigning menu netry icons
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(conf, EspRefindDir.FullName);
                configurationBuilder.ThemeDirectory = formalizationThemePath;
                configurationBuilder.AssignLoaderIcons();

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                configurationBuilder.WriteConfigurationToFile("refind.conf");
            });
        }
    }
}
