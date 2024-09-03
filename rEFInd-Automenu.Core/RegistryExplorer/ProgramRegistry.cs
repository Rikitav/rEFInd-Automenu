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
        //public static readonly object NotFoundValue = 0xBAD + 0xC0DE;
        public static readonly RegistryKey Branch = Registry.LocalMachine
            .CreateSubKey("SOFTWARE")
            .CreateSubKey("rEFInd Automenu");

        //*
        public static Guid? BackupedBootmgrIdentificator
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(BackupedBootmgrIdentificator));
                object? value = Branch.GetValue(nameof(BackupedBootmgrIdentificator));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(BackupedBootmgrIdentificator));
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
        //*/

        public static bool LogInterfaceExecution
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(LogInterfaceExecution));
                object? value = Branch.GetValue(nameof(LogInterfaceExecution));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(LogInterfaceExecution));
                    return false;
                }

                if (value is int KeyValue)
                {
                    log.ErrorFormat("{0} registry key value equals {1}", nameof(LogInterfaceExecution), KeyValue == 1);
                    return KeyValue > 0;
                }

                return false;
            }
        }

        public static bool LogInterfaceResults
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(LogInterfaceResults));
                object? value = Branch.GetValue(nameof(LogInterfaceResults));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(LogInterfaceResults));
                    return false;
                }

                if (value is int KeyValue)
                {
                    log.ErrorFormat("{0} registry key value equals {1}", nameof(LogInterfaceResults), KeyValue == 1);
                    return KeyValue > 0;
                }

                return false;
            }
        }

        public static bool PreferMountvolEspSearch
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(PreferMountvolEspSearch));
                object? value = Branch.GetValue(nameof(PreferMountvolEspSearch));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(PreferMountvolEspSearch));
                    return false;
                }

                if (value is int KeyValue)
                {
                    log.ErrorFormat("{0} registry key value equals {1}", nameof(PreferMountvolEspSearch), KeyValue == 1);
                    return KeyValue > 0;
                }

                return false;
            }
        }

        public static bool PreferBootmgrBooting
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(PreferBootmgrBooting));
                object? value = Branch.GetValue(nameof(PreferBootmgrBooting));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(PreferBootmgrBooting));
                    return false;
                }

                if (value is int KeyValue)
                {
                    log.ErrorFormat("{0} registry key value equals {1}", nameof(PreferBootmgrBooting), KeyValue == 1);
                    return KeyValue > 0;
                }

                return false;
            }
        }

        public static LoaderScannerType LoaderScannerType
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(LoaderScannerType));
                object? value = Branch.GetValue(nameof(LoaderScannerType));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(LoaderScannerType));
                    return LoaderScannerType.Undetermined;
                }

                if (value is string EnumValue)
                {
                    if (!Enum.TryParse(EnumValue, out LoaderScannerType type))
                    {
                        log.ErrorFormat("Failed to parse value of {0} registry key. Key value : {1}", nameof(LoaderScannerType), EnumValue);
                        return LoaderScannerType.Undetermined;
                    }

                    log.ErrorFormat("{0} registry key value equals {1}", nameof(LoaderScannerType), value);
                    return type;
                }

                return LoaderScannerType.Undetermined;
            }
        }
    }

    public enum LoaderScannerType
    {
        Undetermined,
        EspDirectoryEnumerator,
        FwBootmgrRecordParser,
        NvramLoadOptionReader
    }
}
