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
        ModVersion = "1.5.0",
        ModAuthor = "Frogger",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    public static Plugin _self;

    //internal static ConfigEntry<bool> showNick;
    //internal static ConfigEntry<bool> showBar;
    internal static ConfigEntry<int> mobs_nameDistance;
    internal static ConfigEntry<int> mobs_barDistance;
    internal static ConfigEntry<int> mobs_alertedSignDistance;
    internal static ConfigEntry<int> mobs_awareSignDistance;
    internal static ConfigEntry<int> mobs_starsDistance;
    internal static ConfigEntry<int> players_nameDistance;
    internal static ConfigEntry<int> players_barDistance;


    private void Awake()
    {
        _self = this;
        Config.SaveOnConfigSet = false;
        SetupWatcherOnConfigFile();

        configSync.AddLockingConfigEntry(config("General", "Lock Configuration", true, ""));
        mobs_barDistance = config("Mobs", "Mobs healthBar distance", 6,
            "If mob is more than this distance from player, its health bar will be hidden. Set to 0 to hide health bar completely");
        mobs_nameDistance = config("Mobs", "Mobs name distance", 2,
            "If mob is more than this distance from player, its name will be hidden. Set to 0 to hide name completely");
        mobs_alertedSignDistance = config("Mobs", "Mobs alert sign distance", 5,
            "If mob is more than this distance from player, its alert sign will be hidden. Set to 0 to hide alert sign completely");
        mobs_starsDistance = config("Mobs", "Mobs stars distance", 5,
            "If mob is more than this distance from player, its stars will be hidden. Set to 0 to hide stars completely");
        mobs_awareSignDistance = config("Mobs", "Mobs aware sign distance", 5,
            "If mob is more than this distance from player, its aware sign will be hidden. Set to 0 to hide aware sign completely");

        players_barDistance = config("Players", "Players healthBar distance", 6,
            "If player is more than this distance from player, his/her health bar will be hidden. Set to 0 to hide health bar completely");
        players_nameDistance = config("Players", "Players name distance", 2,
            "If player is more than this distance from player, his/her name will be hidden. Set to 0 to hide name completely");

        Config.ConfigReloaded += (_, _) => UpdateConfiguration();
        Config.SettingChanged += (_, _) => UpdateConfiguration();
        Config.SaveOnConfigSet = true;
        Config.Save();


        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), ModGUID);
    }

    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))] [HarmonyPostfix] [HarmonyWrapSafe]
        private static void UpdateHuds(EnemyHud __instance, Player player)
        {
            try
            {
                if (!__instance || __instance.m_huds == null || __instance.m_huds.Count <= 0 ||
                    !player || !m_localPlayer) return;

                foreach (var hud in __instance.m_huds)
                {
                    var character = hud.Key;
                    var data = hud.Value;
                    var nickObj = data?.m_name?.transform.gameObject;
                    var healthObj = data?.m_gui?.transform.Find("Health")?.gameObject;
                    if (!nickObj || !healthObj) continue;
                    var distance = Utils.DistanceXZ(m_localPlayer.transform.position, character.transform.position);
                    int nickDistance, healthDistance;
                    if (character.IsPlayer())
                    {
                        nickDistance = players_nameDistance.Value;
                        healthDistance = players_barDistance.Value;
                    } else
                    {
                        nickDistance = mobs_nameDistance.Value;
                        healthDistance = mobs_barDistance.Value;
                        var guiTransform = data.m_gui.transform;
                        var Alerted = guiTransform.Find("Alerted").gameObject;
                        Alerted.SetActive(mobs_alertedSignDistance.Value != 0 && distance < mobs_alertedSignDistance.Value);
                        
                        var showStars = mobs_starsDistance.Value != 0 && distance < mobs_starsDistance.Value;
                        var level_2 = guiTransform.Find("level_2").gameObject;
                        var level_3 = guiTransform.Find("level_3").gameObject;
                        level_2.SetActive(showStars);
                        level_3.SetActive(showStars);
                    }

                    nickObj.SetActive(nickDistance != 0 && distance < nickDistance);
                    healthObj.SetActive(healthDistance != 0 && distance < healthDistance);
                }
            }
            catch (Exception)
            {
                //idk why there is an exception here, but lets ignore it, okay?
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