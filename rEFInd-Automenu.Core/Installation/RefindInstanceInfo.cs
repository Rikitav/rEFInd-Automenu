using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace rEFInd_Automenu.Installation
{
    public struct RefindInstanceInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RefindInstanceInfo));

        public Version LoaderVersion { get; set; }

        public RefindInstanceInfo(Version version)
        {
            LoaderVersion = version;
        }

        public static void Write(RefindInstanceInfo info, string EspRefindDir)
        {
            string InfoJsonFilePath = Path.Combine(EspRefindDir, "info.json");
            log.InfoFormat("Writing new {0} to {1}", nameof(RefindInstanceInfo), InfoJsonFilePath);

            if (File.Exists(InfoJsonFilePath))
            {
                log.Info("Original file gets overwriten");
                File.Delete(Path.Combine(EspRefindDir, "info.json"));
            }

            JsonSerializer serializer = JsonSerializer.Create();
            serializer.Formatting = Formatting.Indented;

            // Writing
            using (TextWriter writer = new StreamWriter(InfoJsonFilePath))
            {
                serializer.Serialize(writer, info);
                log.Info(nameof(RefindInstanceInfo) + " was writed successfully");
            }
        }

        public static RefindInstanceInfo? Read(string EspRefindDir)
        {
            string InfoJsonFilePath = Path.Combine(EspRefindDir, "info.json");
            log.InfoFormat("Reading {0} data from {1}", nameof(RefindInstanceInfo), InfoJsonFilePath);

            if (!File.Exists(InfoJsonFilePath))
            {
                log.Error("info file doesnt exists");
                return null;
            }

            JsonSerializer serializer = JsonSerializer.Create();
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new VersionConverter());

            // Reading
            using (TextReader reader = new StreamReader(InfoJsonFilePath))
            {
                object? TmpObj = serializer.Deserialize(reader, typeof(RefindInstanceInfo));
                if (TmpObj == null)
                {
                    log.Error("No data wwas recivied from info file");
                    return null;
                }

                RefindInstanceInfo InfoTmp = (RefindInstanceInfo)TmpObj;
                log.InfoFormat("Info file was readed successfully - installed version {0}", InfoTmp.LoaderVersion);
                return InfoTmp;
            }
        }
    }
}
