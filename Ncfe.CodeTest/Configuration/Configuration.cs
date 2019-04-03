using System;
using System.Configuration;

namespace Ncfe.CodeTest.Configuration
{
    public class Configuration : IConfiguration
    {
        private static class ConfigNames
        {
            public const string RecentFailoverTimeSpan = "RecentFailoverTimeSpan";
            public const string FailoverRecordsCountThreshold = "FailoverRecordsCountThreshold";
            public const string IsFailoverModeEnabled = "IsFailoverModeEnabled";
        }
        public TimeSpan RecentFailoverPeriod => GetConfigValue<TimeSpan>(ConfigNames.RecentFailoverTimeSpan, s => TimeSpan.Parse(s));

        public int FailoverRecordsCountThreshold => GetConfigValue<int>(ConfigNames.FailoverRecordsCountThreshold, s => int.Parse(s));

        public bool IsFailoverModeEnabled => GetConfigValue<bool>(ConfigNames.IsFailoverModeEnabled, s => bool.Parse(s));

        private T GetConfigValue<T>(string appSettingName, Func<string, T> parse)
        {
            var setting = ConfigurationManager.AppSettings[appSettingName];
            if (string.IsNullOrEmpty(setting))
                throw new ApplicationException($"Config value {appSettingName} not set.");

            T value;
            try
            {
                value = parse(setting);
            }
            catch
            {
                throw new ApplicationException($"Config value {appSettingName} cannot be parsed to {typeof(T).Name}.");
            }

            return value;
        }
    }
}
