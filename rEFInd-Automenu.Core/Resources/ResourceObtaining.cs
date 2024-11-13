using rEFInd_Automenu.Win32;
using System;

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

        [Flags]
        public enum Warnings
        {
            None = 1,
            NoInternet = 2,
            LocalDisallowed = 4
        }

        public static Method GetMethodForVersion(Version TargetVersion)
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

        public static Method GetMethod(bool DownloadLatest, out Warnings warnings)
        {
            warnings = Warnings.None;
            /*
            if (!ProgramConfiguration.Instance.AllowCreateLocalResource)
            {
                warnings = Warnings.LocalDisallowed;
                return Method.ExtractEmbedded;
            }
            */

            // Getting latest locally saved rEFInd BinArchive
            string LocalBinPath = LocalResourceManager.GetLatestBinArchiveFullPath(out Version? LocalLatestVerInfo);

            if (DownloadLatest)
            {
                if (Win32Utilities.IsInternetConnectionAvailable())
                {
                    if (string.IsNullOrEmpty(LocalBinPath))
                        return Method.DownloadLatest;

                    Version LatestVerInfo = WebResourceManager.GetLatestVersion().Result;
                    return LatestVerInfo > LocalLatestVerInfo
                        ? Method.DownloadLatest
                        : Method.LocalStorage;
                }
                else
                {
                    warnings = Warnings.NoInternet;
                }
            }

            if (string.IsNullOrEmpty(LocalBinPath))
                return Method.ExtractEmbedded;

            return EmbeddedResourceManager.rEFIndBin_VersionInResources > LocalLatestVerInfo
                ? Method.ExtractEmbedded
                : Method.LocalStorage;
        }
    }
}
