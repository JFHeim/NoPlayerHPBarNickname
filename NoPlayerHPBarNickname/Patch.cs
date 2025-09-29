using HarmonyLib;

namespace NoPlayerHPBarNickname;

[HarmonyPatch]
public static class Patch
{
    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.LateUpdate))] [HarmonyPostfix] [HarmonyWrapSafe]
    private static void UpdateHuds(EnemyHud __instance)
    {
        if (__instance == null || __instance.m_huds == null || __instance.m_huds.Count <= 0 || m_localPlayer == null) return;
        var maxShowDistance = AnyHudMaxShowDistanceConfig.Value;
        if (maxShowDistance != 0) __instance.m_maxShowDistance = maxShowDistance;

        foreach (var hud in __instance.m_huds)
        {
            var character = hud.Key;
            var data = hud.Value;
            if (character == null || data == null || character.IsDead()) continue;
            
            var nickObj = data.m_name?.transform.gameObject;
            if (!nickObj) continue;
            
            var healthObj = data.m_gui?.transform.Find("Health")?.gameObject;
            if (!healthObj) continue;
            
            var distance = Utils.DistanceXZ(m_localPlayer.transform.position, character.transform.position);
            int nickDistance, healthDistance;
            if (character.IsPlayer())
            {
                nickDistance = PlayersNameDistanceConfig.Value;
                healthDistance = PlayersBarDistanceConfig.Value;
            } else
            {
                nickDistance = MobsNameDistanceConfig.Value;
                healthDistance = MobsBarDistanceConfig.Value;
                var guiTransform = data.m_gui?.transform;
                var alerted = guiTransform?.Find("Alerted")?.gameObject;
                alerted?.SetActive(MobsAlertedSignDistanceConfig.Value != 0 && distance < MobsAlertedSignDistanceConfig.Value);

                var showStars = MobsStarsDistanceConfig.Value != 0 && distance < MobsStarsDistanceConfig.Value;
                if (guiTransform != null)
                {
                    var level_2 = guiTransform.Find("level_2").gameObject;
                    var level_3 = guiTransform.Find("level_3").gameObject;
                    level_2.SetActive(showStars);
                    level_3.SetActive(showStars);
                }
            }

            nickObj.SetActive(nickDistance != 0 && distance < nickDistance);
            healthObj.SetActive(healthDistance != 0 && distance < healthDistance);
        }
    }
}