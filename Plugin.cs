using BepInEx;
using BepInEx.Configuration;

namespace NoPlayerHPBarNickname;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string
        ModName = "NoPlayerHPBarNickname",
        ModVersion = "1.5.2",
        ModAuthor = "Frogger",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    public static ConfigEntry<int> mobs_nameDistance;
    public static ConfigEntry<int> mobs_barDistance;
    public static ConfigEntry<int> mobs_alertedSignDistance;
    public static ConfigEntry<int> mobs_awareSignDistance;
    public static ConfigEntry<int> mobs_starsDistance;
    public static ConfigEntry<int> players_nameDistance;
    public static ConfigEntry<int> players_barDistance;
    public static ConfigEntry<int> anyHudMaxShowDistance;


    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);

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

        anyHudMaxShowDistance = config("Other", "Any hud max show distance. Warning: Read description", 0,
            "This overrides vanilla hud max show distance. Set to 0 to disable and keep vanilla value. Vanilla value is 30.");
    }
}