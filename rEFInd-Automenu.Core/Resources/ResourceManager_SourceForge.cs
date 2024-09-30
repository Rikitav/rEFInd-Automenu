using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Resources
{
    public static class WebResourceManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebResourceManager));
        private static Version? _LatestVersion = null;
        private static Version[]? _AvailableVersionsList = null;

        /// <summary>
        ///  Checks the availability and accessibility of a web page
        /// </summary>
        /// <param name="PageUri"></param>
        /// <returns></returns>
        public static async Task<bool> CheckWebpageAvailablity(string PageUri)
        {
            log.Info("Checking web page availablity");
            log.InfoFormat("Testing \"{0}\" webpage uri", PageUri);
            using HttpClient TesterClient = new HttpClient();

            HttpResponseMessage responce = await TesterClient.GetAsync(PageUri);
            if (!responce.IsSuccessStatusCode)
            {
                log.Error("Webpage was not found");
                return false;
            }

            log.Info("Tested webpage existing");
            return true;
        }

        /// <summary>
        /// Searches the specifications for the latest available version number of rEFInd
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static async Task<Version> GetLatestVersion()
        {
            log.Info("Getting latest available refind resource archive version");
            if (_LatestVersion != null)
            {
                log.InfoFormat("Latest hashed version is {0}", _LatestVersion);
                return _LatestVersion;
            }

            using HttpClient httpGetSpec = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            log.Info("Requesting latest refind specifications file");
            using StreamReader readerGetSpec = new StreamReader(await httpGetSpec.GetStreamAsync(@"https://sourceforge.net/p/refind/code/ci/master/tree/refind.spec?format=raw"));

            log.Info("Searching \"Version\" line");
            while (!readerGetSpec.EndOfStream)
            {
                string? SpecLine = readerGetSpec.ReadLine();
                if (string.IsNullOrEmpty(SpecLine))
                    continue;

                if (SpecLine.StartsWith("Version"))
                {
                    log.Info("Version line was found");
                    _LatestVersion = Version.Parse(SpecLine.AsSpan("Version: ".Length));
                    log.InfoFormat("Latest available version - {0}", _LatestVersion);
                    return _LatestVersion;
                }
            }

            log.Error("Failed to find version line in specifications file");
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Downloads the rEFInd resource archive of a specific version from the SourceForge repository
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static async Task<Stream> DownloadArchiveByVersion(Version version)
        {
            log.InfoFormat("Downloading rEFInd resource archive from SourceForge of version {0}", version);
            using HttpClient Client = new HttpClient();
            Client.Timeout = TimeSpan.FromMinutes(5);

            string RequestUri = string.Format(@"https://sourceforge.net/projects/refind/files/{0}/refind-bin-{0}.zip/download", version);
            log.InfoFormat("Request URI for downloading - \"{0}\"", RequestUri);
            return await Client.GetStreamAsync(RequestUri);
        }

        /// <summary>
        /// Downloads the rEFInd куыщгксу archive of the latest available version from the SourceForge repository
        /// </summary>
        /// <returns></returns>
        public static async Task<Stream> DownloadArchiveStream()
        {
            log.Info("Downloading latest available rEFInd resource archive from SourceForge");
            using HttpClient Client = new HttpClient();

            log.InfoFormat("Request URI for downloading - \"{0}\"", @"https://sourceforge.net/projects/refind/files/latest/download");
            return await Client.GetStreamAsync(@"https://sourceforge.net/projects/refind/files/latest/download");
        }

        public static async Task<Version[]> GetVersionsList()
        {
            log.Info("Getting a list of available versions of rEFInd on SourceForge");
            if (_AvailableVersionsList != null)
                return _AvailableVersionsList;

            using HttpClient Client = new HttpClient();
            Client.Timeout = TimeSpan.FromMinutes(1);

            log.InfoFormat("Request URI for enumerating - \"{0}\"", @"https://sourceforge.net/projects/refind/files/");
            string pageString = await Client.GetStringAsync(@"https://sourceforge.net/projects/refind/files/");
            Regex trFinderRegex = new Regex(@"<tr title=\u0022(.+)\u0022 class=\u0022folder \u0022>");

            log.Info("Enumerating versions");
            Version DefaultVer = new Version(0, 0);
            _AvailableVersionsList = trFinderRegex
                .Matches(pageString)
                .Select(x => Version.TryParse(x.Groups[1].Value, out Version? ver) ? ver : DefaultVer)
                .Where(x => x != DefaultVer)
                .ToArray();

            log.InfoFormat("Enumerating finished. Available versions : {0}", string.Join<Version>("; ", _AvailableVersionsList));
            return _AvailableVersionsList;
        }
    }
}
