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

        static string ConfigFileName = "com.Frogger.NoPlayerHPBarNickname.cfg";
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

        #region values

        internal static ConfigEntry<bool> showNickConfig;
        internal static ConfigEntry<bool> showBarConfig;
        internal static bool showNick;
        internal static bool showBar;

        #endregion


        private void Awake()
        {
            _self = this;

            #region config

            Config.SaveOnConfigSet = false;
            SetupWatcherOnConfigFile();

            _ = configSync.AddLockingConfigEntry(config("General", "Lock Configuration", true, ""));

            showNickConfig = config("General", "Show Nick", true, "");
            showBarConfig = config("General", "Show Bar", false, "");

            Config.ConfigReloaded += (_, _) => { UpdateConfiguration(); };
            Config.SettingChanged += (_, _) => { UpdateConfiguration(); };
            Config.SaveOnConfigSet = true;
            Config.Save();

            #endregion


            harmony.PatchAll();
        }

        #region Patch

        [HarmonyPatch]
        public static class Pacth
        {
            [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud)), HarmonyPostfix]
            private static void EnemyHudShowHudPacth(Character c)
            {
                if (!c.IsPlayer()) return;
                if (!EnemyHud.instance.m_huds.TryGetValue(c, out EnemyHud.HudData hud)) return;

                hud.m_name.transform.gameObject.SetActive(showNick);
                hud.m_gui.transform.Find("Health").gameObject.SetActive(showBar);
            }
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
            showBar = showBarConfig.Value;
            showNick = showNickConfig.Value;
            Debug("Configuration Received");
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