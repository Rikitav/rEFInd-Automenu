using log4net;
using Microsoft.Win32;
using rEFInd_Automenu.Booting;
using System;

#pragma warning disable CA1416
namespace rEFInd_Automenu.RegistryExplorer
{
    public class ProgramRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BcdEditBridge));
        public static readonly object NotFoundValue = 0xBAD + 0xC0DE;
        public static readonly RegistryKey Branch = Registry.LocalMachine
            .CreateSubKey("SOFTWARE")
            .CreateSubKey("rEFInd Automenu");

        public static Guid? BackupedBootmgrIdentificator
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(BackupedBootmgrIdentificator));
                object? value = Branch.GetValue(nameof(BackupedBootmgrIdentificator), NotFoundValue);
                if (value == null || value == NotFoundValue)
                {
                    log.ErrorFormat("Failed to get {0} registry key", nameof(BackupedBootmgrIdentificator));
                    return null;
                }

                if (Guid.TryParse((string)value, out var GuidValue))
                {
                    log.ErrorFormat("{0} registry key value equals {1}", nameof(BackupedBootmgrIdentificator), GuidValue);
                    return GuidValue;
                }

                log.ErrorFormat("Failed to parse {0} registry key value", nameof(BackupedBootmgrIdentificator));
                return null;
            }

            set
            {
                if (value != null)
                    Branch.SetValue(nameof(BackupedBootmgrIdentificator), value.Value.ToString());
            }
        }

        public static bool LogInterfaceExecution
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(LogInterfaceExecution));
                object? value = Branch.GetValue(nameof(LogInterfaceExecution), NotFoundValue);
                if (value == NotFoundValue)
                {
                    log.ErrorFormat("Failed to get {0} registry key", nameof(LogInterfaceExecution));
                    return false;
                }

                int KeyValue = ((int?)value) ?? 0;
                log.ErrorFormat("{0} registry key value equals {1}", nameof(LogInterfaceExecution), KeyValue == 1);
                return KeyValue == 1;
            }
        }

        public static bool LogInterfaceResults
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(LogInterfaceResults));
                object? value = Branch.GetValue(nameof(LogInterfaceResults), NotFoundValue);
                if (value == NotFoundValue)
                {
                    log.ErrorFormat("Failed to get {0} registry key", nameof(LogInterfaceResults));
                    return false;
                }

                int KeyValue = ((int?)value) ?? 0;
                log.ErrorFormat("{0} registry key value equals {1}", nameof(LogInterfaceResults), KeyValue == 1);
                return KeyValue == 1;
            }
        }

        public static bool PreferMountvolEspSearch
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(PreferMountvolEspSearch));
                object? value = Branch.GetValue(nameof(PreferMountvolEspSearch), NotFoundValue);
                if (value == NotFoundValue)
                {
                    log.ErrorFormat("Failed to get {0} registry key", nameof(PreferMountvolEspSearch));
                    return false;
                }

                int KeyValue = ((int?)value) ?? 0;
                log.ErrorFormat("{0} registry key value equals {1}", nameof(PreferMountvolEspSearch), KeyValue == 1);
                return KeyValue == 1;
            }
        }

        public static bool PreferLoadersEspScan
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(PreferLoadersEspScan));
                object? value = Branch.GetValue(nameof(PreferLoadersEspScan), NotFoundValue);
                if (value == NotFoundValue)
                {
                    log.ErrorFormat("Failed to get {0} registry key", nameof(PreferLoadersEspScan));
                    return false;
                }

                int KeyValue = ((int?)value) ?? 0;
                log.ErrorFormat("{0} registry key value equals {1}", nameof(PreferLoadersEspScan), KeyValue == 1);
                return KeyValue == 1;
            }
        }
    }
}
