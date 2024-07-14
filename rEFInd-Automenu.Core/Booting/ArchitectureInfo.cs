using log4net;
using System;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.Booting
{
    [Flags]
    public enum EnvironmentArchitecture
    {
        None = 2,
        AMD64 = 4,
        ARM64 = 8,
        X86 = 16
    }

    public static class ArchitectureInfo
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ArchitectureInfo));

        public static EnvironmentArchitecture Current
        {
            get => RuntimeInformation.OSArchitecture switch
            {
                Architecture.X86 => EnvironmentArchitecture.X86,
                Architecture.X64 => EnvironmentArchitecture.AMD64,
                Architecture.Arm64 => EnvironmentArchitecture.ARM64,
                _ => EnvironmentArchitecture.None
            };
        }

        public static string CurrentPostfix
        {
            get => Current switch
            {
                EnvironmentArchitecture.ARM64 => "aa64",
                EnvironmentArchitecture.AMD64 => "x64",
                EnvironmentArchitecture.X86 => "ia32",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Defines the processor architecture of current device
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static bool IsCurrentPlatformSupported()
        {
            log.Info("Checking current processor architecture installation capability");
            if (Current == EnvironmentArchitecture.None)
            {
                log.FatalFormat("Current \"{0}\" processor architecture is not supported for rEFInd installation", RuntimeInformation.OSArchitecture);
                //throw new PlatformNotSupportedException($"{RuntimeInformation.OSArchitecture} Architecture is not supported for installation");
                return false;
            }

            log.Info("Current processor architecture is capable for installation");
            return true;
        }

        /// <summary>
        /// Defines a EFI-loader postfix for a given architecture
        /// </summary>
        /// <param name="Arch"></param>
        /// <returns></returns>
        public static string GetArchPostfixString(this EnvironmentArchitecture Arch)
        {
            switch (Arch)
            {
                case EnvironmentArchitecture.X86:
                    return "ia32";

                case EnvironmentArchitecture.AMD64:
                    return "x64";

                case EnvironmentArchitecture.ARM64:
                    return "aa64";

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Defines a architecture for a given postfix
        /// </summary>
        /// <param name="ArchitecturePostfix"></param>
        /// <returns></returns>
        public static EnvironmentArchitecture FromPostfix(string ArchitecturePostfix)
        {
            switch (ArchitecturePostfix.ToLower())
            {
                case "ia32":
                    return EnvironmentArchitecture.X86;

                case "x64":
                    return EnvironmentArchitecture.AMD64;

                case "aa64":
                    return EnvironmentArchitecture.ARM64;

                default:
                    return EnvironmentArchitecture.None;
            }
        }
    }
}
