using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;

namespace TBRBooker.Business
{
   

    public class Config
    {
        private static string ConfigFilePath = Settings.Inst().WorkingDir + "\\config\\TBRBookerConfig.txt";
        private const char KVPSeperator = ':';
        private static Config _Instance { get; set; }

        public Dictionary<string, string> LookupTable { get; set; }

        public static string Lookup(string key)
        {
            if (_Instance == null)
                _Instance = CreateInstance();

            if (_Instance.LookupTable.ContainsKey(key))
                return _Instance.LookupTable[key];
            else
                throw new Exception($"Configuration key not found: {key}.");

        }

        private static Config CreateInstance()
        {
            var config = new Config() { LookupTable = new Dictionary<string, string>() };

            try
            {
                foreach (var line in File.ReadAllLines(Path.GetFullPath(ConfigFilePath)))
                {
                    var kvp = line.Split(KVPSeperator);
                    config.LookupTable.Add(kvp[0], kvp[1]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading config file {ConfigFilePath}: {ex.Message}");
            }

            return config;
        }



    }
}
