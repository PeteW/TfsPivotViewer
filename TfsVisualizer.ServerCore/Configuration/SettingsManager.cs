using System;
using System.Configuration;

namespace TfsVisualizer.ServerCore.Configuration
{
    public class SettingsManager
    {
        public static string TfsUrl
        {
            get { return GetAppSetting("TfsServerUrl", "http://xxx:8080/tfs/"); }
        }

        /// <summary>
        /// return a setting from .config
        /// If its missing supply an optional default
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string GetAppSetting(string setting, string defaultValue = null)
        {
            try
            {
                var test = ConfigurationManager.AppSettings[setting];
                if (test == null)
                    throw new Exception(string.Format("Unable to find an appsetting defined for [{0}]. Maybe the path to the appSettings file is incorrect, or maybe the specific setting is missing within the Appsettings file.", setting));
                return test;
            }
            catch
            {
                if (!string.IsNullOrEmpty(defaultValue))
                    return defaultValue;
                throw;
            }
        }

    }
}