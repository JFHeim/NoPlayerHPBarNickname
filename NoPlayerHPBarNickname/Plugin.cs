using BepInEx;
using BepInEx.Configuration;

namespace NoPlayerHPBarNickname;

[BepInPlugin(ModGuid, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string
        ModName = "NoPlayerHPBarNickname",
        ModVersion = "1.6.0",
        ModAuthor = "Frogger",
        ModGuid = $"com.{ModAuthor}.{ModName}";

    public static ConfigEntry<int> MobsNameDistanceConfig         = null!;
    public static ConfigEntry<int> MobsBarDistanceConfig          = null!;
    public static ConfigEntry<int> MobsAlertedSignDistanceConfig  = null!;
    public static ConfigEntry<int> MobsStarsDistanceConfig        = null!;
    public static ConfigEntry<int> PlayersNameDistanceConfig      = null!;
    public static ConfigEntry<int> PlayersBarDistanceConfig       = null!;
    public static ConfigEntry<int> AnyHudMaxShowDistanceConfig    = null!;


    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGuid);

        MobsBarDistanceConfig            = config("Mobs",    "Mobs healthBar distance",      6, "If mob is more than this distance from player, its health bar will be hidden. Set to 0 to hide health bar completely");
        MobsNameDistanceConfig           = config("Mobs",    "Mobs name distance",           2, "If mob is more than this distance from player, its name will be hidden. Set to 0 to hide name completely");
        MobsAlertedSignDistanceConfig    = config("Mobs",    "Mobs alert sign distance",     5, "If mob is more than this distance from player, its alert sign will be hidden. Set to 0 to hide alert sign completely");
        MobsStarsDistanceConfig          = config("Mobs",    "Mobs stars distance",          5, "If mob is more than this distance from player, its stars will be hidden. Set to 0 to hide stars completely");

        PlayersBarDistanceConfig         = config("Players", "Players healthBar distance",   6, "If player is more than this distance from player, his/her health bar will be hidden. Set to 0 to hide health bar completely");
        PlayersNameDistanceConfig        = config("Players", "Players name distance",        2, "If player is more than this distance from player, his/her name will be hidden. Set to 0 to hide name completely");
    
        AnyHudMaxShowDistanceConfig      = config("Other",   "Any hud max show distance. Warning: Read description", 0, "This overrides vanilla hud max show distance. Set to 0 to disable and keep vanilla value. Vanilla value is 30.");
    }
}