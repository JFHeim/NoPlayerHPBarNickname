using NoPlayerHPBarNickname.Config;

namespace NoPlayerHPBarNickname;

[BepInEx.BepInPlugin(Consts.ModGuid, Consts.ModName, Consts.ModVersion)]
public class Plugin : BepInEx.BaseUnityPlugin
{
    private void Awake()
    {
        Log.Initialize(this);

        new Harmony(Consts.ModGuid).PatchAll();
        ConfigsContainer.InitializeConfiguration(this);
    }
}
