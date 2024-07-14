using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace rEFInd_Automenu.Installation
{
    public static class InstallManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstallManager));
        public static readonly string[] KnownBinArchiveDirectoryNames = new string[]
        {
            "icons",
            "drivers_*",
            "tools_*"
        };

        /// <summary>
        /// Copies resource archive loader and directories
        /// </summary>
        /// <param name="RefindArchiveFolder"></param>
        /// <param name="DestinationFolder"></param>
        /// <param name="Arch"></param>
        /// <param name="Overwrite"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void CopyResourceArchive(DirectoryInfo RefindArchiveFolder, DirectoryInfo DestinationFolder, EnvironmentArchitecture Arch, string DestinationLoadersPattern)
        {
            log.InfoFormat("Started resource archive copying from \"{0}\" to \"{1}\"", RefindArchiveFolder.FullName, DestinationFolder.FullName);

            // Checking any loaders existing
            if (!RefindArchiveFolder.EnumerateFiles("refind_*.efi").Any())
            {
                log.Error("Resource archive does not contain any loaders");
                throw new FileNotFoundException("Loader file wasnt found on given resource directory");
            }

            // Moving resources
            CopyLoaders(RefindArchiveFolder, DestinationFolder, Arch, DestinationLoadersPattern);
            CopyDirectories(RefindArchiveFolder, DestinationFolder, Arch);
        }

        public static void CopyDirectories(DirectoryInfo ArchiveDir, DirectoryInfo DestinationDir, EnvironmentArchitecture Arch)
        {
            if (Arch.HasFlag(EnvironmentArchitecture.None))
            {
                foreach (string DirEntry in KnownBinArchiveDirectoryNames)
                {
                    foreach (DirectoryInfo loaderDir in ArchiveDir.EnumerateDirectories(DirEntry))
                    {
                        loaderDir.CopyTo(Path.Combine(DestinationDir.FullName, loaderDir.Name), true);
                        log.InfoFormat("Copied resource directory \"{0}\"", loaderDir.Name);
                    }
                }
            }

            foreach (EnvironmentArchitecture EnumFlag in Arch.GetType().GetEnumValues().Cast<EnvironmentArchitecture>())
            {
                if (EnumFlag == EnvironmentArchitecture.None || !Arch.HasFlag(EnumFlag))
                    continue;

                foreach (string DirEntry in KnownBinArchiveDirectoryNames)
                {
                    string EntryDirName = DirEntry.Replace("*", EnumFlag.GetArchPostfixString());
                    log.InfoFormat("Coopying entry directory \"{0}\"", EntryDirName);
                    if (!ArchiveDir.GetSubDirectory(EntryDirName).Exists)
                    {
                        log.WarnFormat("\"{0}\" entry wasnt found in archive directory", EntryDirName);
                        continue;
                    }

                    try
                    {
                        ArchiveDir
                            .GetSubDirectory(EntryDirName)
                            .CopyTo(Path.Combine(DestinationDir.FullName, EntryDirName));
                        log.InfoFormat("Copied entry directory \"{0}\"", EntryDirName);
                    }
                    catch (Exception exc)
                    {
                        log.Error("Failed to copy entry directory \"" + EntryDirName + "\"", exc);
                        continue;
                    }
                }
            }
        }

        public static void CopyLoaders(DirectoryInfo ArchiveDir, DirectoryInfo DestinationDir, EnvironmentArchitecture Arch, string DestinationLoadersPattern)
        {
            if (Arch.HasFlag(EnvironmentArchitecture.None))
            {
                foreach (FileInfo loaderFile in ArchiveDir.EnumerateFiles("refind_*.efi"))
                {
                    string ArchPostfix = loaderFile.Name.Substring("refind_".Length, loaderFile.Name.Length - loaderFile.Name.IndexOf(".efi"));
                    string DestLoaderName = string.Format(DestinationLoadersPattern, ArchPostfix);
                    loaderFile.CopyTo(Path.Combine(DestinationDir.FullName, DestLoaderName));
                    log.InfoFormat("Copied resource loader \"{0}\"", loaderFile.Name);
                }
            }

            foreach (EnvironmentArchitecture EnumFlag in Arch.GetType().GetEnumValues().Cast<EnvironmentArchitecture>())
            {
                if (EnumFlag == EnvironmentArchitecture.None || !Arch.HasFlag(EnumFlag))
                    continue;

                string LoaderName = string.Format("refind_{0}.efi", EnumFlag.GetArchPostfixString());
                if (!File.Exists(Path.Combine(ArchiveDir.FullName, LoaderName)))
                {
                    log.WarnFormat("{0} loader wasnt found in archive directory");
                    continue;
                }

                string DestinationLoaderName = string.Format(DestinationLoadersPattern, EnumFlag.GetArchPostfixString());
                try
                {
                    log.InfoFormat("Coopying resource loader \"{0}\"", LoaderName);
                    File.Copy(
                        Path.Combine(ArchiveDir.FullName, LoaderName),
                        Path.Combine(DestinationDir.FullName, DestinationLoaderName),
                        true);

                    log.InfoFormat("Copied resource loader \"{0}\"", LoaderName);
                }
                catch (Exception exc)
                {
                    log.Error("Failed to copy resource loader \"" + LoaderName + "\"", exc);
                    continue;
                }
            }
        }

        /// <summary>
        /// Extracts resource archive contained in Stream to temporary directory
        /// </summary>
        /// <param name="ArchiveStream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public static DirectoryInfo ExtractBinArchive(Stream ArchiveStream)
        {
            // NullStream check
            if (ArchiveStream == null || ArchiveStream == Stream.Null)
            {
                log.Error("Resource archive stream was null");
                throw new InvalidDataException(nameof(ArchiveStream) + " was Empty");
            }

            // Crating temporary directory
            DirectoryInfo TmpDir = DirectoryExtensions.GetTempDirectory();
            log.InfoFormat("Temporary extraction directory - {0}", TmpDir.FullName);

            using (ZipArchive Archive = new ZipArchive(ArchiveStream))
            {
                Archive.ExtractToDirectory(TmpDir.FullName);
                DirectoryInfo RefindResourceDir = TmpDir.EnumerateDirectories().First();

                log.InfoFormat("Resource archive \"{0}\" was successfully extracted", RefindResourceDir.Name);
                return RefindResourceDir;
            }
        }
    }
}
