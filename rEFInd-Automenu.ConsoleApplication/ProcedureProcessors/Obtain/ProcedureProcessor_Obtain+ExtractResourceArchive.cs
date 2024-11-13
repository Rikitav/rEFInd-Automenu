using rEFInd_Automenu.Resources;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Obtain
{
    public static partial class ProcedureProcessor_Obtain
    {
        private static void ExtractResourceArchive()
        {
            // Working
            log.Info("Extracting resource archive (Obtain -Embedded flag)");

            // Setting variables
            string FileName = string.Format("rEFInd-bin-{0}.zip", EmbeddedResourceManager.rEFIndBin_VersionInResources.ToString());
            string OutputFile = Path.Combine(Environment.CurrentDirectory, FileName);

            // Extracting embedded resource archive
            ConsoleProgram.Interface.Execute("Extracting resource archive", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                if (File.Exists(OutputFile))
                {
                    // Archive with this version or name already exists
                    ctrl.Warning("Archive already exists");
                    log.Warn("Archive already exists");
                    return;
                }

                // Opening resource stream and cretaing new file to write
                using Stream ResArchiveStream = EmbeddedResourceManager.GetArchiveStream().Result;
                if (ResArchiveStream == null)
                {
                    // Archive with this version or name already exists
                    ctrl.Error("ResArchiveStream is null");
                    log.Warn("ResArchiveStream is null");
                    return;
                }

                LocalResourceManager.SaveBinArchive(EmbeddedResourceManager.rEFIndBin_VersionInResources, ResArchiveStream).Wait();
                using (FileStream file = File.Create(OutputFile))
                    ResArchiveStream.CopyTo(file);

                log.Info("Resource archive succesfully extracted");
                ctrl.Success(FileName);
            });
        }
    }
}
