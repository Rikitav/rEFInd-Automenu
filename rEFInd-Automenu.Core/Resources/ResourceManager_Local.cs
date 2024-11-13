using log4net;
using rEFInd_Automenu.RuntimeConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Resources
{
    public static partial class LocalResourceManager
    {
        public static readonly string VersionsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rEFInd Automenu", "Versions");
        public static readonly DirectoryInfo VersionsDirectory = Directory.CreateDirectory(VersionsDirectoryPath);
        public static event Action<Version>? LocalStorageUpdated;

        private static readonly ILog log = LogManager.GetLogger(typeof(LocalResourceManager));

        /// <summary>
        /// Checking for any resource on local storage
        /// </summary>
        /// <returns></returns>
        public static bool AnyLocalResourceExist()
        {
            // Any locally saved resource archives
            return VersionsDirectory
                .EnumerateFiles("refind-bin-*.zip")
                .Any();
        }

        /// <summary>
        /// Checking for resource archive eith speccific version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool VersionExists(Version? version)
        {
            // if version null, the no resources
            if (version == null)
                return false;

            // Formatting path and getting his existing
            string VerName = string.Format("refind-bin-{0}.zip", version);
            return File.Exists(Path.Combine(VersionsDirectoryPath, VerName));
        }

        /// <summary>
        /// Opening resource archive stream by speccific version located on local storage
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        /// <exception cref="EmptyLocalDatabaseException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static FileStream GetBinArchiveByVersionStreamByVersino(Version version)
        {
            log.InfoFormat("Getting bin archive FileStream of version {0}", version);

            // Checking for any locally saved resource
            LRM_Helper.ThrowIfEmptyDataBase();

            // Formating path for asked resource
            string VersionedPath = GetBinArchiveFullPathByVersion(version);
            if (!File.Exists(VersionedPath))
            {
                // Does not exists
                log.ErrorFormat("No resource archives with version \"{0}\"", version);
                throw new FileNotFoundException(VersionedPath);
            }

            // Opening new readonly FileStream for asked resource archive
            log.InfoFormat("Opening resource archive stream from - {0}", VersionedPath);
            return new FileStream(VersionedPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Opening resource archive stream with latest available version on local storage
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EmptyLocalDatabaseException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static FileStream GetLatestBinArchiveStream([NotNullWhen(true)] out Version? version)
        {
            log.Info("Opening new readonly FileStream for latest versioned resource archive");

            // Checking for any locally saved resource
            LRM_Helper.ThrowIfEmptyDataBase();

            // Formating path for latest resource
            string LatestFullPath = GetLatestBinArchiveFullPath(out version);
            if (string.IsNullOrEmpty(LatestFullPath))
            {
                log.Info("Failed to get resource archive path. Returned path was empty");
                throw new FileNotFoundException();
            }

            // Opening new readonly FileStream for latest versioned resource archive
            log.InfoFormat("Opening resource archive stream from - {0}", LatestFullPath);
            return new FileStream(LatestFullPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Checking path for resource archive with speccific version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string GetBinArchiveFullPathByVersion(Version version)
        {
            // Formating path for resource archive with version of "version" param
            string VerName = string.Format("refind-bin-{0}.zip", version);
            if (!File.Exists(VerName))
                return string.Empty;

            return Path.Combine(VersionsDirectory.FullName, VerName);
        }

        /// <summary>
        /// Checking path for resource archive with latest availlable version
        /// </summary>
        /// <param name="verInfo"></param>
        /// <returns></returns>
        public static string GetLatestBinArchiveFullPath(out Version? verInfo)
        {
            // Ordering files on local database by they version and get the last one
            FileInfo? LastBin = LRM_Helper
                .EnumerateArchives()
                .OrderBy(a => LRM_Helper.ParseVersionFromFileName(a.Name))
                .LastOrDefault();

            if (LastBin == null)
            {
                verInfo = new Version(0, 0, 0);
                log.Error("Failed to get resource archive FileInfo");
                return string.Empty;
            }

            // Assigning out "verInfo" param
            verInfo = Version.Parse(LastBin.Name.AsSpan("refind-bin-".Length, LastBin.Name.Length - "refind-bin-.zip".Length));
            return LastBin.FullName;
        }

        /// <summary>
        /// Asynchronously saves stream with resource archive obtained from another ssources to local storage
        /// </summary>
        /// <param name="version"></param>
        /// <param name="BinStream"></param>
        /// <returns></returns>
        public static async Task<string> SaveBinArchive(Version version, Stream BinStream)
        {
            // Starting
            log.InfoFormat("Resource archive of version \"{0}\" is saving as local resource", version);

            // Checking local resources writing right
            if (!ProgramConfiguration.Instance.AllowCreateLocalResource)
            {
                log.Warn("Creating local resources is disallowed for current session");
                return string.Empty;
            }

            // Getting if resource archive already exists on local database with given version
            string BinPath = Path.Combine(VersionsDirectory.FullName, string.Format("refind-bin-{0}.zip", version));

            // Checking if such file is already exists
            if (File.Exists(BinPath))
            {
                // Resource already exists
                log.WarnFormat("Resource archive already exists in local database");
                return string.Empty;
            }

            try
            {
                // Creating file and writing data stream into
                log.InfoFormat("Saving resource archive of version {0} to local database", version);
                using (FileStream BinFile = File.Create(BinPath))
                {
                    if (BinStream.CanSeek)
                        BinStream.Seek(0, SeekOrigin.Begin);

                    await BinStream.CopyToAsync(BinFile);
                    await BinFile.FlushAsync();
                }

                log.InfoFormat("Resource archive was succesfully saved");
                LocalStorageUpdated?.Invoke(version);
                return BinPath;
            }
            catch (Exception exc)
            {
                // Something went wrong on resource saving
                log.Warn("Failed to save resource archive because of exception", exc);
                return string.Empty;
            }
        }

        public static Version[] GetSavedArchivesVersionsList()
        {
            log.Info("Getting a list of locally saved versions of rEFInd");
            Version[] versions = LRM_Helper
                .EnumerateArchives()
                .Select(a => LRM_Helper.ParseVersionFromFileName(a.Name))
                .OrderBy(x => x)
                .Reverse()
                .ToArray();

            log.InfoFormat("Enumerating finished. Locally saved versions : {0}", string.Join<Version>("; ", versions));
            return versions;
        }

        private static class LRM_Helper
        {
            public static Version ParseVersionFromFileName(string Name)
            {
                int start = Name.Length - "refind-bin-".Length + 1;
                int length = start - ".zip".Length - 1;
                return Version.Parse(Name.AsSpan(start, length));
            }

            public static void ThrowIfEmptyDataBase()
            {
                if (!AnyLocalResourceExist())
                {
                    log.Error("No locally saved resources");
                    throw new EmptyLocalDatabaseException();
                }
            }

            public static IEnumerable<FileInfo> EnumerateArchives()
            {
                return VersionsDirectory.EnumerateFiles("refind-bin-*.zip");
            }
        }

        public class EmptyLocalDatabaseException : Exception
        {
            public EmptyLocalDatabaseException()
                : base() { }

            public EmptyLocalDatabaseException(string Message)
                : base(Message) { }
        }
    }
}
