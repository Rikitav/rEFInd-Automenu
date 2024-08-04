using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;

#pragma warning disable CA1416 // Проверка совместимости платформы
namespace rEFInd_Automenu.Extensions
{
    public static class DirectoryExtensions
    {
        public static bool IsEmpty(this DirectoryInfo path)
        {
            return !path.EnumerateFileSystemInfos().Any();
        }

        public static void Empty(this DirectoryInfo Target)
        {
            foreach (FileInfo file in Target.GetFiles())
                file.Delete();

            foreach (DirectoryInfo subDirectory in Target.GetDirectories())
                subDirectory.Delete(true);
        }

        public static DirectoryInfo CopyTo(this DirectoryInfo SourceDir, string DestinationDirPath, bool Recursive = false)
        {
            return CopyTo(SourceDir.FullName, DestinationDirPath, Recursive);
        }

        public static DirectoryInfo CopyTo(string SourceDirPath, string DestinationDirPath, bool Recursive = false)
        {
            if (string.IsNullOrEmpty(SourceDirPath))
                throw new ArgumentNullException(nameof(SourceDirPath));

            if (string.IsNullOrEmpty(DestinationDirPath))
                throw new ArgumentNullException(nameof(DestinationDirPath));

            if (!Directory.Exists(SourceDirPath))
                throw new DirectoryNotFoundException($"Source directory not found : {SourceDirPath}");

            DirectoryInfo DestinationDir = Directory.CreateDirectory(DestinationDirPath);
            foreach (string file in Directory.EnumerateFiles(SourceDirPath))
                File.Copy(file, Path.Combine(DestinationDirPath, Path.GetFileName(file)), true);

            if (Recursive)
            {
                foreach (string subDir in Directory.EnumerateDirectories(SourceDirPath))
                    CopyTo(subDir, Path.Combine(DestinationDirPath, Path.GetDirectoryName(subDir) ?? string.Empty), true);
            }

            return DestinationDir;
        }

        public static void GrantAccessControl(this DirectoryInfo directory)
        {
            DirectorySecurity security = directory.GetAccessControl();
            FileSystemAccessRule accessRule = new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);

            security.AddAccessRule(accessRule);
            directory.SetAccessControl(security);
        }

        public static bool TryGrantAccessControl(this DirectoryInfo directory)
        {
            try
            {
                directory.GrantAccessControl();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<DirectoryInfo> CopyToAsync(this DirectoryInfo SourceDir, string DestinationDirPath, bool Recursive = false)
        {
            return await Task.Run(() => SourceDir.CopyTo(DestinationDirPath, Recursive));
        }

        public static DirectoryInfo GetSubDirectory(this DirectoryInfo SourceDir, string DirName)
        {
            return new DirectoryInfo(Path.Combine(SourceDir.FullName, DirName));
        }

        public static DirectoryInfo GetTempDirectory()
        {
            string TmpDirName = "tmp" + Random.Shared.Next(4096, 65535).ToString("X");
            string TempDirPath = Path.Combine(Path.GetTempPath());
            return new DirectoryInfo(Path.Combine(TempDirPath, TmpDirName));
        }
    }
}
