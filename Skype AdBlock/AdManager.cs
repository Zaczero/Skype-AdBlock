using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Skype_AdBlock
{
    public class AdManager
    {
        private enum TrustedZoneType
        {
            Intranet = 1,
            TrustedSites = 2,
            Internet = 3,
            RestrictedSites = 4,
        }

        private const string RegKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\ZoneMap\\Domains\\skype.com\\apps";

        private readonly string _skypeBaseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Skype";

        public bool GetAdStatus()
        {
            var rk = Registry.CurrentUser.OpenSubKey(RegKeyPath);

            // RegKey not found
            if (rk == null)
            {
                return true;
            }

            var trustedZoneValue = rk.GetValue("*");

            // Value not found
            if (trustedZoneValue == null)
            {
                return true;
            }

            return (TrustedZoneType)trustedZoneValue != TrustedZoneType.RestrictedSites;
        }

        public void DisableAds()
        {
            KillSkype();

            // Block ad connection
            var rk = Registry.CurrentUser.CreateSubKey(RegKeyPath);
            rk.SetValue("*", (int)TrustedZoneType.RestrictedSites);
            rk.Dispose();

            // Block ad placeholder
            foreach (var configPath in GetConfigPaths())
            {
                File.SetAttributes(configPath, FileAttributes.Normal);
                File.WriteAllText(configPath, File.ReadAllText(configPath).Replace("<AdvertPlaceholder>1</AdvertPlaceholder>", "<AdvertPlaceholder>0</AdvertPlaceholder>"));
                File.SetAttributes(configPath, FileAttributes.ReadOnly);
            }
        }

        public void EnableAds()
        {
            KillSkype();

            // Restore ad connection
            var rk = Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            rk?.DeleteValue("*", false);
            rk.Dispose();

            // Restore ad placeholder
            foreach (var configPath in GetConfigPaths())
            {
                File.SetAttributes(configPath, FileAttributes.Normal);
                File.WriteAllText(configPath, File.ReadAllText(configPath).Replace("<AdvertPlaceholder>0</AdvertPlaceholder>", "<AdvertPlaceholder>1</AdvertPlaceholder>"));
                File.SetAttributes(configPath, FileAttributes.ReadOnly);
            }
        }

        private static void KillSkype()
        {
            var skypeProcesses = Process.GetProcessesByName("Skype");
            foreach (var process in skypeProcesses)
            {
                process.Kill();
                process.WaitForExit();
            }
        }

        private IEnumerable<string> GetConfigPaths()
        {
            // Scan all dirs for config files
            var dirs = Directory.GetDirectories(_skypeBaseDir);
            foreach (var dir in dirs)
            {
                var cfgPath = dir + "\\config.xml";
                if (File.Exists(cfgPath))
                {
                    yield return cfgPath;
                }
            }
        }
    }
}
