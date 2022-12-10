using BepInEx;
using HarmonyLib;

#pragma warning disable CS0618
namespace NoPlayerHPBarNickname
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class Plugin : BaseUnityPlugin
    {
        #region values
        private const string ModName = "NoPlayerHPBarNickname", ModVersion = "1.0.0", ModGUID = "com.Frogger." + ModName;
        private static readonly Harmony harmony = new(ModGUID);
        public static Plugin _self;
        #endregion

        private void Awake()
        {
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
    }
}