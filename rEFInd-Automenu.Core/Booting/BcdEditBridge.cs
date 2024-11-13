using log4net;
using rEFInd_Automenu.TypesExtensions;
using rEFInd_Automenu.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace rEFInd_Automenu.Booting
{
    public class BcdEditBridge : Win32ApplicationBridge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BcdEditBridge));
        public const string RefindBootloaderTag = "custom:0x72666e64";

        public BcdEditBridge()
            : base("bcdedit.exe") { }

        /// <summary>
        /// Setts given boot option as first in boot order in FwBootmgr
        /// </summary>
        /// <param name="BootOptName"></param>
        public void SetBootOrderFirst(string BootOptName)
        {
            log.InfoFormat("Setting {0} BootEntry as first on FwBootmMgr boot order", BootOptName);
            Execute("/set {fwbootmgr} displayOrder " + BootOptName.Quotation('{', '}') + " /addfirst");
            log.Info("Boot order changed successfully");
        }

        /// <summary>
        /// Rewrites the path and description values ​​of the {bootmgr} boot entry to the specified ones
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="TargetPath"></param>
        public void SetBootOptionPath(string BootOptName, string Name, string TargetPath)
        {
            log.InfoFormat("Setting bootmgr loader info. BootOpt - {0} Description - {1}, Path - {2}", BootOptName, Name, TargetPath);

            // Setting boot option loader path
            string SetLoaderPathArgs = string.Join(' ',
                "/set", BootOptName.Quotation('{', '}'), // boot option name
                "path", TargetPath.Quotation('\"'));     // property value

            // Setting boot option description (entry name)
            string SetDescriptionArgs = string.Join(' ',
                "/set", BootOptName.Quotation('{', '}'), // boot option name
                "description", Name.Quotation('\"'));    // property value

            // Setting boot option custom boot tag
            /*
            string SetCustomBootTagArgs = string.Join(' ',
                "/set", BootOptName.Quotation('{', '}'), // boot option name
                RefindBootloaderTag, "1"));              // custom property value
            */

            Execute(SetLoaderPathArgs);
            Execute(SetDescriptionArgs);
            //Execute(SetCustomBootTagArgs);
        }

        /// <summary>
        /// Searches the registry for an entry about the backup {bootmgr} boot entry
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsBootmgrBackuped([NotNullWhen(true)] out Guid? Value)
        {
            log.Info("Checking the existance of a backup copy of the bootmgr boot entry");
            Value = RegistryExplorer.Instance.BackupedBootmgrIdentificator;

            if (Value == null)
            {
                log.Warn("BootmgrBackupEntryGuid is not found or holding incorrect data");
                return false;
            }

            if (Value == Guid.Empty)
            {
                log.Warn("BootmgrBackupEntryGuid is holding empty GUID");
                RegistryExplorer.Instance.DeleteRegistryValue(nameof(RegistryExplorer.BackupedBootmgrIdentificator));
                log.Info("BootmgrBackupEntryGuid was deleted");
                Value = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Backups an unmodified boot entry {bootmgr}
        /// </summary>
        /// <exception cref="BootmgrBackupException"></exception>
        public void BackupBootmgr()
        {
            log.InfoFormat("Looking for already saved backup");
            if (IsBootmgrBackuped(out Guid? BackupedGuid)) // entry may be alreday backuped
            {
                log.InfoFormat("Bootmgr is already backuped. Entry identificator : {0}", BackupedGuid);
                return;
            }

            // Trying to backup
            log.InfoFormat("Backuping default bootmgr");

            // Executing bcdedit to copy existing bootmgr
            string? Output = Execute("/copy {bootmgr} /d \"Windows boot manager\"", true);
            if (string.IsNullOrWhiteSpace(Output))
            {
                // Output of bcdedit was empty
                log.Error("Failed to backup bootmgr. Bcdedit returned empty data");
                throw new BootmgrBackupException("Bcdedit returned empty data");
            }

            // Matching backupped entry GUID
            Match BackupGuidMatch = Regex.Match(Output, @"(\{.+\})");
            if (!BackupGuidMatch.Success)
            {
                log.Error("Failed to backup bootmgr. Failed to match GUID from bcdedit output");
                log.ErrorFormat("Bcdedit output was - \"{0}\"", Output);
                throw new BootmgrBackupException("Failed to match GUID from bcdedit output");
            }

            log.InfoFormat("Bootmgr was succesfully backuped. Entry identificator - {0}", BackupGuidMatch.Groups[1].Value);
            log.Info("Writing backuped boot entry GUID to registry");
            RegistryExplorer.Instance.BackupedBootmgrIdentificator = new Guid(BackupGuidMatch.Groups[1].Value);
        }

        public class BcdeditException : Exception
        {
            public BcdeditException(string message)
                : base(message) { }

            public BcdeditException(Exception inner)
                : base("Failed to execute bcdedit", inner) { }

            public BcdeditException(string message, Exception inner)
                : base(message, inner) { }
        }

        public class BootmgrBackupException : Exception
        {
            public BootmgrBackupException(string message)
                : base(message) { }

            public BootmgrBackupException(Exception inner)
                : base("Failed to backup bootmgr", inner) { }

            public BootmgrBackupException(string message, Exception inner)
                : base(message, inner) { }
        }
    }
}