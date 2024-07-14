using System;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.Resources
{
    public class ResourceObtaining
    {
        public enum Method
        {
            DownloadLatest,  // Download latest from internet
            ExtractEmbedded, // Extract from application's embedded resource
            LocalStorage     // Extract from locally saved bin archive
        }

        public static Method GetVersionedResource(Version TargetVersion)
        {
            // Requested version is exist on local storage
            if (LocalResourceManager.VersionExists(TargetVersion))
                return Method.LocalStorage;

            // Requested version is available in embedded resources
            if (TargetVersion == EmbeddedResourceManager.rEFIndBin_VersionInResources)
                return Method.ExtractEmbedded;

            // Download from SOurceForge
            return Method.DownloadLatest;
        }

        public static Method GetResource(bool DownloadLatest)
        {
            // Getting latest locally saved rEFInd BinArchive
            string LocalBinPath = LocalResourceManager.GetLatestBinArchiveFullPath(out Version LocalLatestVerInfo);

            if (DownloadLatest)
            {
                if (NativeMethods.InternetGetConnectedState(out _, 0))
                {
                    if (string.IsNullOrEmpty(LocalBinPath))
                        return Method.DownloadLatest;

                    Version LatestVerInfo = WebResourceManager.GetLatestVersion().Result;
                    return LatestVerInfo > LocalLatestVerInfo
                        ? Method.DownloadLatest
                        : Method.LocalStorage;
                }
            }

            if (string.IsNullOrEmpty(LocalBinPath))
                return Method.ExtractEmbedded;

            return EmbeddedResourceManager.rEFIndBin_VersionInResources > LocalLatestVerInfo
                ? Method.ExtractEmbedded
                : Method.LocalStorage;
        }

        private static class NativeMethods
        {
            [DllImport("wininet.dll")]
            public static extern bool InternetGetConnectedState(out int Description, int ReservedValue);
        }
    }
}
