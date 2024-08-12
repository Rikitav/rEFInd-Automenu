using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public RefindConfiguration ParseConfigurationFile(DirectoryInfo EspRefindDir) => ConsoleProgram.Interface.ExecuteAndReturn<RefindConfiguration>("Parsing config file", commands, (ctrl) =>
        {
            // Getting config file path
            string configFilePath = Path.Combine(EspRefindDir.FullName, "refind.conf");
            RefindConfiguration? conf = null;

            try
            {
                // Parsing configuration information from file
                log.InfoFormat("Parsing config file - ", configFilePath);
                conf = ConfigurationFileParser.FromFile(configFilePath);
            }
            catch (Exception exc)
            {
                // Parsing error
                log.Error("Failed to parse config file", exc);
                return ctrl.Error("Failed to parse config file - " + exc.Message);
            }

            // null check???
            if (conf == null)
            {
                log.Error("Configuration instance was null");
                return ctrl.Error("Configuration instance was null");
            }

            // Done
            log.Info("Config file parsed successfully");
            return ctrl.Success(string.Empty, conf);
        });
    }
}
