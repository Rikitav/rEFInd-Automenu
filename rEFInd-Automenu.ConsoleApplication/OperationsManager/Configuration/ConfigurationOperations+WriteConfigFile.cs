using rEFInd_Automenu.Configuration;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration
{
    public static partial class ConfigurationOperations
    {
        public static void WriteConfigFile(DirectoryInfo refindDir, ConfigurationFileBuilder configurationBuilder) => ConsoleProgram.Interface.Execute("Writing config file", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            string EfiConfFile = Path.Combine(refindDir.FullName, "refind.conf");
            if (File.Exists(EfiConfFile))
                File.Delete(EfiConfFile);

            configurationBuilder.WriteConfigurationToFile(EfiConfFile);
        });
    }
}
