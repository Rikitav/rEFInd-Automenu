using log4net;
using Microsoft.Win32;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.RedistryExplorer;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CA1416
namespace rEFInd_Automenu.Booting
{
    public static class BcdEditBridge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BcdEditBridge));
        public const string RefindBootloaderTag = "custom:0x72666e64";

        /// <summary>
        /// Rewrites the path and description values ​​of the {bootmgr} boot entry to the specified ones
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="TargetPath"></param>
        public static void SetBootMgr(string Name, string TargetPath)
        {
            log.InfoFormat("Setting bootmgr loader info. Description - {0}, Path - {1}", Name, TargetPath);
            ExecuteBcdedit("/set {bootmgr} description " + Name.Quotation('\"'));
            ExecuteBcdedit("/set {bootmgr} path " + TargetPath.Quotation('\"'));
            //ExecuteBcdedit(string.Join(' ', "/set", "{bootmgr}", RefindBootloaderTag, "1"));
        }

        /*
        public static IEnumerable<string> EnumerateFirmwareBootmgr()
        {
            string? ExecData = ExecuteBcdedit("/enum {fwbootmgr}", true);
            if (string.IsNullOrWhiteSpace(ExecData))
            {
                log.Error("Failed to enumerate fwbootmgr. Empty bcdedit data");
                throw new BcdeditException("Failed to enumerate fwbootmgr. Empty bcdedit data");
            }

            foreach (Match GuidMatch in Regex.Matches(ExecData, @"\{(.+)\}", RegexOptions.Multiline).Cast<Match>())
            {
                if (!GuidMatch.Success)
                    continue;

                string GuidValue = GuidMatch.Groups[1].Value;
                if (Guid.TryParse(GuidValue, out _))
                {
                    log.InfoFormat("Matched fwbootmgr entry {0}", GuidValue);
                    yield return GuidValue;
                }
            }
        }
        */

        /// <summary>
        /// Searches the registry for an entry about the backup {bootmgr} boot entry
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsBootmgrBackuped([NotNullWhen(true)]out Guid? Value)
        {
            Value = null;

            log.Info("Checking the existance of a backup copy of the bootmgr boot entry");
            Guid? BackupedBootmgrIdentificator = ProgramRegistry.BackupedBootmgrIdentificator;

            if (BackupedBootmgrIdentificator == null)
            {
                log.Warn("BootmgrBackupEntryGuid is not found or holding incorrect data");
                return false;
            }

            if (BackupedBootmgrIdentificator == Guid.Empty)
            {
                log.Warn("BootmgrBackupEntryGuid is holding empty GUID");
                ProgramRegistry.Branch.DeleteValue(nameof(ProgramRegistry.BackupedBootmgrIdentificator));
                log.Info("BootmgrBackupEntryGuid was deleted");
                return false;
            }

            Value = BackupedBootmgrIdentificator;
            return true;
        }

        /// <summary>
        /// Backups an unmodified boot entry {bootmgr}
        /// </summary>
        /// <exception cref="BootmgrBackupException"></exception>
        public static void BackupBootmgr()
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
            string? Output = ExecuteBcdedit("/copy {bootmgr} /d \"Windows boot manager\"", true);
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
            ProgramRegistry.BackupedBootmgrIdentificator = new Guid(BackupGuidMatch.Groups[1].Value);
        }

        /// <summary>
        /// Runs the BcdEdit program with the specified parameters
        /// </summary>
        /// <param name="Params"></param>
        /// <param name="ReadOutput"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static string? ExecuteBcdedit(string Params, bool ReadOutput = false)
        {
            using Process BcdEditProc = new Process()
            {
                StartInfo = new ProcessStartInfo("bcdedit.exe")
                {
                    Arguments = Params,
                    CreateNoWindow = true,
                    RedirectStandardOutput = ReadOutput,
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)
                }
            };

            log.InfoFormat("Bcdedit is executing with parameters - {0}", Params);
            BcdEditProc.Start();
            BcdEditProc.WaitForExit();

            if (BcdEditProc.ExitCode != 0)
            {
                log.ErrorFormat("Failed to execute bcdedit (ExitCode : {0})", BcdEditProc.ExitCode);
                throw new Win32Exception(BcdEditProc.ExitCode, "BcdEdit execution failed");
            }

            if (ReadOutput)
            {
                StringBuilder outputBuilder = new StringBuilder();
                while (!BcdEditProc.StandardOutput.EndOfStream)
                    outputBuilder.AppendLine(BcdEditProc.StandardOutput.ReadLine());

                return outputBuilder.ToString();
            }

            return null;
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
