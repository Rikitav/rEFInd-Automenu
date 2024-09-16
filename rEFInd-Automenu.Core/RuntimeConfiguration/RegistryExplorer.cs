using log4net;
using Microsoft.Win32;
using rEFInd_Automenu.Booting;
using System;

#pragma warning disable CA1416
namespace rEFInd_Automenu.RuntimeConfiguration
{
    public class RegistryExplorer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BcdEditBridge));
        public static readonly RegistryKey Branch = Registry.LocalMachine
            .CreateSubKey(@"SOFTWARE\rEFInd Automenu");

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

        public static ApplicationTheme ApplicationTheme
        {
            get
            {
                log.InfoFormat("Getting {0} registry key", nameof(ApplicationTheme));
                object? value = Branch.GetValue(nameof(ApplicationTheme));

                if (value == null)
                {
                    log.ErrorFormat("Failed to get {0} registry key. Not found", nameof(ApplicationTheme));
                    return ApplicationTheme.Auto;
                }

                if (value is string EnumValue)
                {
                    if (!Enum.TryParse(EnumValue, out ApplicationTheme type))
                    {
                        log.ErrorFormat("Failed to parse value of {0} registry key. Key value : {1}", nameof(ApplicationTheme), EnumValue);
                        return ApplicationTheme.Auto;
                    }

                    log.ErrorFormat("{0} registry key value equals {1}", nameof(ApplicationTheme), value);
                    return type;
                }

                return ApplicationTheme.Auto;
            }
        }
    }
}
