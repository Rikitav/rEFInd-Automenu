using rEFInd_Automenu.Configuration;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration
{
    public static partial class ConfigurationOperations
    {
        public static ConfigurationFileBuilder GetConfigurationFileBuilder(DirectoryInfo refindDir, bool IsDynamicPlatform) => ConsoleProgram.Interface.ExecuteAndReturn<ConfigurationFileBuilder>("Getting configuration platform", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(
                refindDir.FullName);

            // Configuring platform
            if (!IsDynamicPlatform)
                configurationBuilder.ConfigureStaticPlatform();
            else
                configurationBuilder.ConfigureDynamicPlatform();

            // Success
            return ctrl.Success(IsDynamicPlatform ? "Dynamic platform" : "Static platform", configurationBuilder);
        });
    }
}
