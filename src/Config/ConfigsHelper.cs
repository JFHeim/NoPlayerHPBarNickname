using BepInEx;
using BepInEx.Configuration;
using Managers.ServerSync;

// ReSharper disable InconsistentNaming
namespace NoPlayerHPBarNickname.Config;

public partial class ConfigsContainer
{
    private static bool _isInitialized = false;
    private static BaseUnityPlugin _plugin = null!;
    private static ConfigSync configSync = null!;
    private static DateTime _lastConfigChange = DateTime.MinValue;

    public static void InitializeConfiguration(BaseUnityPlugin plugin)
    {
        if (_isInitialized)
        {
            Log.Error("ConfigsContainer is already initialized");
            return;
        }

        configSync = new ConfigSync(Consts.ModGuid)
            { DisplayName = Consts.ModName, CurrentVersion = Consts.ModVersion, MinimumRequiredVersion = Consts.ModVersion };

        _plugin = plugin;
        plugin.Config.SaveOnConfigSet = false;
        _ = new ConfigsContainer();
        SetupWatcher();
        plugin.Config.SaveOnConfigSet = true;
        plugin.Config.Save();

        _isInitialized = true;
    }

    private static void SetupWatcher()
    {
        FileSystemWatcher fileSystemWatcher = new(Paths.ConfigPath, $"{_plugin.Info.Metadata.GUID}.cfg");
        fileSystemWatcher.Changed += ConfigChanged;
        fileSystemWatcher.Created += ConfigChanged;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private static void ConfigChanged(object sender, FileSystemEventArgs eventArgs)
    {
        if ((DateTime.Now - _lastConfigChange).TotalSeconds <= 2) return;
        _lastConfigChange = DateTime.Now;

        try { _plugin.Config.Reload(); }
        catch (Exception ex) { Log.Error(ex, "Unable reload config"); }
    }

    // ReSharper disable once InconsistentNaming
    public static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
    {
        ConfigDescription extendedDescription = new(
            description.Description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
            description.AcceptableValues, description.Tags);
        var configEntry = _plugin.Config.Bind(group, name, value, extendedDescription);

        var syncedConfigEntry = configSync.AddConfigEntry(configEntry);
        syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

        return configEntry;
    }

    // ReSharper disable once InconsistentNaming
    public static ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
        => config(group, name, value, new ConfigDescription(description), synchronizedSetting);
}
