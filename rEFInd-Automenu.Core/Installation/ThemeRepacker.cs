using log4net;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.TypesExtensions;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rEFInd_Automenu.Installation
{
    public static class ThemeRepacker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ThemeRepacker));

        public static DirectoryInfo CopyDirectory(DirectoryInfo ThemeDir, DirectoryInfo DestinationDir, RefindConfiguration? parentConfiguration = null)
        {
            // Theme dir should exist
            if (!ThemeDir.Exists)
            {
                log.Error("Theme directory doesnt exists");
                throw new DirectoryNotFoundException();
            }

            // Should have config files
            if (!ThemeDir.EnumerateFiles("*.conf").Any())
            {
                log.Error("Theme directory doesnt contain any configuration file");
                throw new FileNotFoundException("Theme directory doesnt contain any configuration file");
            }

            // But only one
            if (ThemeDir.EnumerateFiles("*.conf").Count() > 1)
            {
                log.Error("Theme directory contains more than one configuration files");
                throw new FileNotFoundException("Theme directory contains more than one configuration files");
            }

            // Creating new configuration instance to create config file
            log.InfoFormat("Installing formalization theme from directory - {0}", ThemeDir.FullName);
            RefindConfiguration configuration = parentConfiguration ?? new RefindConfiguration();
            configuration.Global = new RefindGlobalConfigurationInfo();

            // Searching for "conf" file
            string ThemeConf = ThemeDir.EnumerateFiles("*.conf").First().FullName;
            log.InfoFormat("Reading formalization theme configuration file {0}", ThemeConf);

            // Opening for read
            using (StreamReader ConfReader = File.OpenText(ThemeConf))
            {
                while (!ConfReader.EndOfStream)
                {
                    // Reading config file line
                    string? ConfLine = ConfReader.ReadLine();
                    
                    // not null or empty
                    if (string.IsNullOrEmpty(ConfLine))
                        continue;

                    // not comment
                    if (ConfLine.StartsWith("#"))
                        continue;

                    // Parsing via expression
                    log.InfoFormat("Reading line \"{0}\"", ConfLine);
                    Match LineMatch = Regex.Match(ConfLine, @"(\S+) (.+)");
                    
                    // Checking success
                    if (!LineMatch.Success)
                    {
                        log.Error("Failed to parse config line");
                        continue;
                    }

                    // Assigning token value
                    log.InfoFormat("Parsing token - {0}", LineMatch.Groups[1].Value);

                    switch (LineMatch.Groups[1].Value)
                    {
                        case "icons_dir":
                            {
                                try
                                {
                                    // Building path for formalization File or Directory
                                    string? EntryPath = BuildEntryPath(ThemeDir.FullName, LineMatch.Groups[2].Value, false);

                                    // Error check
                                    if (string.IsNullOrEmpty(EntryPath))
                                    {
                                        log.Error("Failed to format token value");
                                        break;
                                    }

                                    // Copying finded entry
                                    log.InfoFormat("Copying entry to theme installation directory - {0}", EntryPath);
                                    DirectoryExtensions.CopyTo(EntryPath, Path.Combine(DestinationDir.FullName, "theme", "icons"));

                                    // Assigning new value to config file
                                    configuration.Global.IconsDirectory = "theme/icons";
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", LineMatch.Groups[1].Value, configuration.Global.IconsDirectory);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "banner":
                            {
                                try
                                {
                                    // Building path for formalization File or Directory
                                    string? EntryPath = BuildEntryPath(ThemeDir.FullName, LineMatch.Groups[2].Value, true);

                                    // Error check
                                    if (string.IsNullOrEmpty(EntryPath))
                                    {
                                        log.Error("Failed to format token value");
                                        break;
                                    }

                                    // Copying finded entry
                                    log.InfoFormat("Copying entry to theme installation directory - {0}", EntryPath);
                                    File.Copy(EntryPath, Path.Combine(DestinationDir.FullName, "theme", Path.GetFileName(EntryPath)));

                                    // Assigning new value to config file
                                    configuration.Global.Banner = "theme/" + Path.GetFileName(EntryPath);
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", LineMatch.Groups[1].Value, configuration.Global.Banner);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "selection_big":
                            {
                                try
                                {
                                    // Building path for formalization File or Directory
                                    string? EntryPath = BuildEntryPath(ThemeDir.FullName, LineMatch.Groups[2].Value, true);

                                    // Error check
                                    if (string.IsNullOrEmpty(EntryPath))
                                    {
                                        log.Error("Failed to format token value");
                                        break;
                                    }

                                    // Copying finded entry
                                    log.InfoFormat("Copying entry to theme installation directory - {0}", EntryPath);
                                    File.Copy(EntryPath, Path.Combine(DestinationDir.FullName, "theme", Path.GetFileName(EntryPath)));

                                    // Assigning new value to config file
                                    configuration.Global.SelectionBig = "theme/" + Path.GetFileName(EntryPath);
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", LineMatch.Groups[1].Value, configuration.Global.SelectionBig);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "selection_small":
                            {
                                try
                                {
                                    // Building path for formalization File or Directory
                                    string? EntryPath = BuildEntryPath(ThemeDir.FullName, LineMatch.Groups[2].Value, true);

                                    // Error check
                                    if (string.IsNullOrEmpty(EntryPath))
                                    {
                                        log.Error("Failed to format token value");
                                        break;
                                    }

                                    // Copying finded entry
                                    log.InfoFormat("Copying entry to theme installation directory - {0}", EntryPath);
                                    File.Copy(EntryPath, Path.Combine(DestinationDir.FullName, "theme", Path.GetFileName(EntryPath)));

                                    // Assigning new value to config file
                                    configuration.Global.SelectionSmall = "theme/" + Path.GetFileName(EntryPath);
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", LineMatch.Groups[1].Value, configuration.Global.SelectionSmall);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "font":
                            {
                                try
                                {
                                    // Building path for formalization File or Directory
                                    string? EntryPath = BuildEntryPath(ThemeDir.FullName, LineMatch.Groups[2].Value, true);

                                    // Error check
                                    if (string.IsNullOrEmpty(EntryPath))
                                    {
                                        log.Error("Failed to format token value");
                                        break;
                                    }

                                    // Copying finded entry
                                    log.InfoFormat("Copying entry to theme installation directory - {0}", EntryPath);
                                    File.Copy(EntryPath, Path.Combine(DestinationDir.FullName, "theme", Path.GetFileName(EntryPath)));

                                    // Assigning new value to config file
                                    configuration.Global.FontImage = "theme/" + Path.GetFileName(EntryPath);
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", LineMatch.Groups[1].Value, configuration.Global.FontImage);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "big_icon_size":
                            {
                                try
                                {
                                    // Parsing numeric value of token
                                    int bis = int.Parse(LineMatch.Groups[2].Value);

                                    // Assigning new value to config file
                                    configuration.Global.BigIconSize = bis;
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", nameof(configuration.Global.BigIconSize), bis);
                                }
                                catch (Exception Exc)
                                {
                                    // Error occured
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        case "small_icon_size":
                            {
                                try
                                {
                                    // Parsing numeric value of token
                                    int sis = int.Parse(LineMatch.Groups[2].Value);

                                    // Assigning new value to config file
                                    configuration.Global.SmallIconSize = sis;
                                    log.InfoFormat("Assigned new value to configuration entry \"{0}\" - {1}", nameof(configuration.Global.SmallIconSize), sis);
                                }
                                catch (Exception Exc)
                                {
                                    log.Error("Failed to parse configuration token", Exc);
                                }

                                break;
                            }

                        default:
                            {
                                // Token name was not recognized
                                log.Error("Token name was not recognized");
                                break;
                            }
                    }
                }
            }

            // Creating new configuration file
            DirectoryInfo ThemePath = DestinationDir.GetSubDirectory("theme");
            ConfigurationFileBuilder.WriteConfigurationToFile(configuration, Path.Combine(ThemePath.FullName, "theme.conf"));
            return ThemePath;
        }

        private static string? BuildEntryPath(string ThemeDirFullPath, string EntryValue, bool IsFile)
        {
            // This function spliting token value by slashes
            // and then tries to find resource in theme directory
            // by building path starting from last name

            string[] Pathes = EntryValue.Split('/', '\\').Reverse().ToArray();
            StringBuilder PathBuilder = new StringBuilder();

            foreach (string PathNode in Pathes)
            {
                PathBuilder.Insert(0, PathNode + "/");
                string TstPath = Path.Combine(ThemeDirFullPath, PathBuilder.ToString().TrimEnd('/'));
                if (IsFile && File.Exists(TstPath) || Directory.Exists(TstPath))
                {
                    return TstPath;
                }
            }

            return null;
        }
    }
}
