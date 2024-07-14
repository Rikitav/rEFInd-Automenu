using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Resources
{
    public static partial class EmbeddedResourceManager
    {
        public static readonly string rEFIndBin_ResourceName = "refind-bin_embedded.zip";
        public static readonly Version rEFIndBin_VersionInResources = new Version(0, 14, 0);

        private static readonly ILog log = LogManager.GetLogger(typeof(EmbeddedResourceManager));

        public static async Task<Stream> GetArchiveStream()
        {
#pragma warning disable CS8603
            await Task.Yield();
            log.Info("Embedded resource requested");
            log.InfoFormat("Embedded resource version - \"{0}\"", rEFIndBin_VersionInResources);

            // Path combining
            string rEFIndBin_ResourcePath = typeof(EmbeddedResourceManager).Namespace + "." + rEFIndBin_ResourceName;
            log.InfoFormat("Assembly resource path - \"{0}\"", rEFIndBin_ResourcePath);

            // Extracting resource archive from current assembly embedded resources
            log.Info("Getting embedded resource stream");
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
            Stream? ResourceStream = CurrentAssembly.GetManifestResourceStream(rEFIndBin_ResourcePath);

            // NullStream checking
            if (ResourceStream == null || ResourceStream == Stream.Null)
            {
                log.Info("Resource was not found in assemblies embedded resource");
                return ResourceStream;
            }

            // alright
            return ResourceStream;
#pragma warning restore CS8603
        }
    }
}
