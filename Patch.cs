using HarmonyLib;


namespace NoPlayerHPBarNickname;

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
                    Alerted.SetActive(mobs_alertedSignDistance.Value != 0
                                      && distance < mobs_alertedSignDistance.Value);

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