using rEFInd_Automenu.Installation;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public string InstallFormalizationThemeDirectory(DirectoryInfo ThemeDir, DirectoryInfo RefindInstallationDir) => ConsoleProgram.Interface.ExecuteAndReturn<string>("Installing formalization theme", commands, (ctrl) =>
        {
            string formalizationThemePath = string.Empty;
            try
            {
                formalizationThemePath = ThemeRepacker.CopyDirectory(ThemeDir, RefindInstallationDir);
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
