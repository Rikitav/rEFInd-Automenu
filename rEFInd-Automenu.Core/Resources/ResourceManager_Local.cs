using log4net;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Resources
{
    public static partial class LocalResourceManager
    {
        public static readonly string VersionsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rEFInd Automenu", "Versions");
        public static readonly DirectoryInfo VersionsDirectory = Directory.CreateDirectory(VersionsDirectoryPath);

        public static readonly ILog log = LogManager.GetLogger(typeof(LocalResourceManager));
        public static EventHandler? LocalStorageUpdated;

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
            if (!AnyLocalResourceExist())
            {
                // No resource archives
                log.Error("No locally saved resources");
                throw new EmptyLocalDatabaseException();
            }

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
        public static FileStream GetLatestBinArchiveStream()
        {
            // Checking for any locally saved resource
            if (!AnyLocalResourceExist())
            {
                // No resource archives
                log.Error("No locally saved resources");
                throw new EmptyLocalDatabaseException();
            }

            // Formating path for latest resource
            string LatestFullPath = GetLatestBinArchiveFullPath(out _);
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
        public static string GetLatestBinArchiveFullPath(out Version verInfo)
        {
            // Ordering files on local database by they version and get the last one
            FileInfo? LastBin = VersionsDirectory
                .EnumerateFiles("refind-bin-*.zip")
                .OrderBy(ArchivePath => Version.Parse(ArchivePath.Name.AsSpan("refind-bin-".Length, ArchivePath.Name.Length - "refind-bin-.zip".Length)))
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
        public static async Task SaveBinArchive(Version version, Stream BinStream)
        {
            // Getting if resource archive already exists on local database with given version
            string BinPath = Path.Combine(VersionsDirectory.FullName, string.Format("refind-bin-{0}.zip", version));
            log.InfoFormat("Resource archive of version \"{0}\" is saving to file \"{1}\"", version, BinPath);
            if (File.Exists(BinPath))
            {
                // Resource already exists
                log.WarnFormat("Resource archive of version {0} already exists in local database. Skipping saving", version);
                return;
            }

            try
            {
                // Creating file and writing data stream into
                log.InfoFormat("Saving resource archive of version {0} to local database", version);
                using FileStream BinFile = File.Create(BinPath);
                await BinStream.CopyToAsync(BinFile);
                log.InfoFormat("Resource archive was succesfully saved", version);
                LocalStorageUpdated?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception exc)
            {
                // Something went wrong on resource saving
                log.Warn("Failed to save resource archive because of exception", exc);
            }
        }

        public static Version[] GetSavedArchivesVersionsList()
        {
            log.Info("Getting a list of locally saved versions of rEFInd");
            Version[] versions = VersionsDirectory
                .EnumerateFiles("refind-bin-*.zip")
                .Select(ArchivePath => Version.Parse(ArchivePath.Name.AsSpan("refind-bin-".Length, ArchivePath.Name.Length - "refind-bin-.zip".Length)))
                .ToArray();

            log.InfoFormat("Enumerating finished. Locally saved versions : {0}", string.Join<Version>("; ", versions));
            return versions;
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
