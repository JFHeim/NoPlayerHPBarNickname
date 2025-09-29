using HarmonyLib;

namespace NoPlayerHPBarNickname;

[HarmonyPatch]
file static class Patch
{
    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.LateUpdate))] [HarmonyPostfix] [HarmonyWrapSafe]
    private static void UpdateHuds(EnemyHud __instance)
    {
        if (__instance == null || __instance.m_huds == null || __instance.m_huds.Count <= 0 || Player.m_localPlayer == null) return;
        var maxShowDistance = AnyHudMaxShowDistanceConfig.Value;
        if (maxShowDistance != 0) __instance.m_maxShowDistance = maxShowDistance;

        foreach (var hud in __instance.m_huds)
        {
            var character = hud.Key;
            var hudData = hud.Value;
            if (character == null || hudData == null || character.IsDead()) continue;
            
            var nickObj = hudData.m_name?.transform.gameObject;
            if (!nickObj) continue;
            
            var healthObj = hudData.m_gui?.transform.Find("Health")?.gameObject;
            if (!healthObj) continue;
            
            var distance = Utils.DistanceXZ(Player.m_localPlayer.transform.position, character.transform.position);
            int nickDistance, healthDistance;
            if (character.IsPlayer())
            {
                nickDistance = PlayersNameDistanceConfig.Value;
                healthDistance = PlayersBarDistanceConfig.Value;
            } else
            {
                nickDistance = MobsNameDistanceConfig.Value;
                healthDistance = MobsBarDistanceConfig.Value;
                hudData.m_alerted?.gameObject.SetActive(MobsAlertedSignDistanceConfig.Value != 0 && distance < MobsAlertedSignDistanceConfig.Value);

                var showStars = MobsStarsDistanceConfig.Value != 0 && distance < MobsStarsDistanceConfig.Value;
                int level = hudData.m_character.GetLevel();
                if(level == 2) hudData.m_level2?.gameObject.SetActive(showStars);
                if(level == 3) hudData.m_level3?.gameObject.SetActive(showStars);
            }

            nickObj.SetActive(nickDistance != 0 && distance < nickDistance);
            healthObj.SetActive(healthDistance != 0 && distance < healthDistance);
        }
    }
}