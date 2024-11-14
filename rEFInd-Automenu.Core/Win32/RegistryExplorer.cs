using log4net;
using Microsoft.Win32;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.RuntimeConfiguration;
using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1416
namespace rEFInd_Automenu.Win32
{
    public class RegistryExplorer
    {
        private readonly ILog log = LogManager.GetLogger(typeof(BcdEditBridge));
        private static RegistryExplorer? _instance;
        private readonly RegistryKey? _RegistryBranch;

        public static RegistryExplorer Instance
        {
            get => _instance ??= new RegistryExplorer();
        }

        private RegistryExplorer()
        {
            if (!AllowReadWrite())
                return;

            _RegistryBranch = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\rEFInd Automenu");
            AppDomain.CurrentDomain.ProcessExit += (_, _) => _RegistryBranch.Close();
        }

        [MemberNotNullWhen(true, nameof(_RegistryBranch))]
        private bool AllowReadWrite()
        {
            return ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite;
        }

        public bool ShowLargeHeader
        {
            get
            {
                if (!ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite)
                    return false;

                int KeyStateValue = GetRegistryValue<int>(nameof(ShowLargeHeader), 0);
                log.InfoFormat("{0} registry key value equals {1}", nameof(BackupedBootmgrIdentificator), KeyStateValue);
                return KeyStateValue > 0;
            }
        }

        public Guid? BackupedBootmgrIdentificator
        {
            get
            {
                if (!ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite)
                    return Guid.Empty;

                string GuidStringValue = GetRegistryValue<string>(nameof(BackupedBootmgrIdentificator), string.Empty);
                if (GuidStringValue == string.Empty)
                    return null;

                if (!Guid.TryParse(GuidStringValue, out Guid GuidValue))
                {
                    log.ErrorFormat("Failed to parse GUID from registry key");
                    return null;
                }

                log.InfoFormat("{0} registry key value equals {1}", nameof(BackupedBootmgrIdentificator), GuidValue);
                return GuidValue;
            }

            set
            {
                if (!ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite)
                    return;

                if (value != null)
                    _RegistryBranch?.SetValue(nameof(BackupedBootmgrIdentificator), value.Value.ToString());
            }
        }

        public ApplicationTheme ApplicationTheme
        {
            get
            {
                if (!ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite)
                    return ApplicationTheme.Auto;

                string EnumStringValue = GetRegistryValue<string>(nameof(ApplicationTheme), "Auto");
                if (EnumStringValue == "Auto")
                    return ApplicationTheme.Auto;

                if (!Enum.TryParse(EnumStringValue, true, out ApplicationTheme EnumValue))
                {
                    log.ErrorFormat("Failed to parse value of {0} registry key. Key value : {1}", nameof(ApplicationTheme), EnumValue);
                    return ApplicationTheme.Auto;
                }

                log.InfoFormat("{0} registry key value equals {1}", nameof(ApplicationTheme), EnumValue);
                return EnumValue;
            }

            set
            {
                if (!ProgramConfiguration.Instance.AllowWindowsRegistryReadWrite)
                    return;

                if (Enum.IsDefined(value))
                    _RegistryBranch?.SetValue(nameof(ApplicationTheme), value.ToString());
            }
        }

        public void DeleteRegistryValue(string keyName)
        {
            _RegistryBranch?.DeleteValue(keyName);
        }

        private T GetRegistryValue<T>(string keyName, T defaultValue)
        {
            log.InfoFormat("Getting {0} registry key", keyName);
            object? value = _RegistryBranch?.GetValue(keyName, null);

            if (value == null)
            {
                log.ErrorFormat("\"{0}\" registry key was not found", keyName);
                return defaultValue;
            }

            if (value is not T keyValue)
            {
                log.ErrorFormat("\"{0}\" registry key has incorrect format", keyName);
                return defaultValue;
            }

            return keyValue;
        }
    }
}
