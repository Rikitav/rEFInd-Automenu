using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Installation;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Installing
{
    public static partial class InstallingOperations
    {
        public static DirectoryInfo? InstallFormalizationThemeDirectory(DirectoryInfo ThemeDir, DirectoryInfo RefindInstallationDir) => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo?>("Installing formalization theme", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            if (!ThemeDir.Exists)
            {
                throw new DirectoryNotFoundException("Theme directory does not exist");
            }

            DirectoryInfo? formalizationThemePath = null;
            try
            {
                formalizationThemePath = ThemeRepacker.CopyDirectory(ThemeDir, RefindInstallationDir, null);
            }
            catch (Exception Exc)
            {
                log.Error("Failed to install formalization theme", Exc);
                ctrl.Error("Failed due unhandled exception");
            }

            return ctrl.Success(ThemeDir.Name, formalizationThemePath);
        });
    }
}
