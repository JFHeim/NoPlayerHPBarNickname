using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using static Player;

namespace NoPlayerHPBarNickname;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string
        ModName = "NoPlayerHPBarNickname",
        ModVersion = "1.2.0",
        ModAuthor = "Frogger",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    public static Plugin _self;

    internal static ConfigEntry<bool> showNick;
    internal static ConfigEntry<bool> showBar;
    internal static ConfigEntry<int> showNickMinDistance;
    internal static ConfigEntry<int> showBarMinDistance;
    internal static ConfigEntry<bool> appliesToMobs;

    private void Awake()
    {
        _self = this;
        Config.SaveOnConfigSet = false;
        SetupWatcherOnConfigFile();

        configSync.AddLockingConfigEntry(config("General", "Lock Configuration", true, ""));

        appliesToMobs = config("General", "Applies to Mobs", false,
            "Should also mobs bars and nicknames be shown/hidden as players");
        showNick = config("General", "Show Nick", true, "");
        showBar = config("General", "Show Bar", false, "");
        showBarMinDistance = config("General", "Show Bar Min Distance", 2,
            "If target is less than this distance from player, bar will be shown anyway");
        showNickMinDistance = config("General", "Show Nick Min Distance", 4,
            "If target is less than this distance from player, nickName will be shown anyway");

        Config.ConfigReloaded += (_, _) => UpdateConfiguration();
        Config.SettingChanged += (_, _) => UpdateConfiguration();
        Config.SaveOnConfigSet = true;
        Config.Save();


        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);
    }

    [HarmonyPatch]
    public static class Pacth
    {
        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))] [HarmonyPostfix]
        private static void UpdateHuds(EnemyHud __instance, Player player)
        {
            if (!player || !m_localPlayer) return;

            foreach (var hud in __instance.m_huds)
            {
                var character = hud.Key;
                var data = hud.Value;
                var nickObj = data.m_name?.transform.gameObject;
                var healthObj = data.m_gui?.transform.Find("Health").gameObject;
                if (!nickObj || !healthObj) continue;
                if (!character.IsPlayer() && !appliesToMobs.Value)
                {
                    nickObj.SetActive(true);
                    healthObj.SetActive(true);
                    continue;
                }

                var distance = Utils.DistanceXZ(m_localPlayer.transform.position, character.transform.position);

                nickObj.SetActive(showNick.Value || distance < showNickMinDistance.Value);

                healthObj.SetActive(showBar.Value || distance < showBarMinDistance.Value);
            }
        }
    }

    #region ConfigSettings

    private static readonly string ConfigFileName = "com.Frogger.NoPlayerHPBarNickname.cfg";
    private DateTime LastConfigChange;

    public static readonly ConfigSync configSync = new(ModName)
        { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

    public static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
        bool synchronizedSetting = true)
    {
        var configEntry = _self.Config.Bind(group, name, value, description);
        var syncedConfigEntry = configSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
        return configEntry;
    }

    private ConfigEntry<T> config<T>(string group, string name, T value, string description,
        bool synchronizedSetting = true)
    {
        return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
    }

    private void SetCfgValue<T>(Action<T> setter, ConfigEntry<T> config)
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
        if ((DateTime.Now - LastConfigChange).TotalSeconds <= 2.0) return;

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

    private void UpdateConfiguration() { Debug("Configuration Received"); }

    #endregion

    #region tools

    public static void Debug(string msg) { _self.Logger.LogMessage(msg); }

    public static void DebugError(string msg)
    {
        _self.Logger.LogError($"{msg} Write to the developer and moderator if this happens often.");
    }

    #endregion
}