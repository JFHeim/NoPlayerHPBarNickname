using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;

#pragma warning disable CS0618
namespace NoPlayerHPBarNickname
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        #region values

        private const string ModName = "NoPlayerHPBarNickname",
            ModVersion = "1.0.0",
            ModGUID = "com.Frogger." + ModName;

        private static readonly Harmony harmony = new(ModGUID);
        public static Plugin _self;

        #endregion
        #region ConfigSettings

        static string ConfigFileName = "com.Frogger.TowerDefense.cfg";
        DateTime LastConfigChange;

        public static readonly ConfigSync configSync = new(ModName)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        public static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = _self.Config.Bind(group, name, value, description);
            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        void SetCfgValue<T>(Action<T> setter, ConfigEntry<T> config)
        {
            setter(config.Value);
            config.SettingChanged += (_, _) => setter(config.Value);
        }

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        #endregion
        #region Config

        public void SetupWatcherOnConfigFile()
        {
            FileSystemWatcher fileSystemWatcherOnConfig = new(Paths.ConfigPath, ConfigFileName);
            fileSystemWatcherOnConfig.Changed += ConfigChanged;
            fileSystemWatcherOnConfig.IncludeSubdirectories = true;
            fileSystemWatcherOnConfig.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            fileSystemWatcherOnConfig.EnableRaisingEvents = true;
        }

        private void ConfigChanged(object sender, FileSystemEventArgs e)
        {
            if ((DateTime.Now - LastConfigChange).TotalSeconds <= 2.0)
            {
                return;
            }

            LastConfigChange = DateTime.Now;
            try
            {
                Config.Reload();
                Debug("Reloading Config...");
            }
            catch
            {
                DebugError("Can't reload Config");
            }
        }

        private void UpdateConfiguration()
        {
            hideBar = hideBarConfig.Value;
            hideNick = hideNickConfig.Value;
            Debug("Configuration Received");
        }

        #endregion
        #region values

        internal static ConfigEntry<bool> hideNickConfig;
        internal static ConfigEntry<bool> hideBarConfig;
        internal static bool hideNick;
        internal static bool hideBar;

        #endregion


        private void Awake()
        { 
            #region config

            Config.SaveOnConfigSet = false;
            
            _ = configSync.AddLockingConfigEntry(config("General", "Lock Configuration", true, ""));
            
            hideNickConfig = config("General", "Hide Nick", false, "");
            hideBarConfig = config("General", "Hide Bar", true, "");

            SetupWatcherOnConfigFile();
            Config.ConfigReloaded += (_, _) => { UpdateConfiguration(); };
            Config.SaveOnConfigSet = true;
            Config.Save();

            #endregion


            _self = this;
            harmony.PatchAll(typeof(Pacth));
        }

        #region Patch

        [HarmonyPatch]
        public static class Pacth
        {
            [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud)), HarmonyPrefix]
            private static bool EnemyHudShowHudPacth(Character c)
            {
                if (c.IsPlayer())
                {
                    return false;
                }

                return true;
            }
        }

        #endregion
        
        #region tools

        public static void Debug(string msg)
        {
            _self.Logger.LogMessage(msg);
        }

        public static void DebugError(string msg)
        {
            _self.Logger.LogError($"{msg} Write to the developer and moderator if this happens often.");
        }

        #endregion

    }
}