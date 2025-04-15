using UnityEngine;
using HarmonyLib;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class BoomDead
    {
        public static ModHotkey dead = new ModHotkey(MouseAndKeyboard.MouseMiddle, KillHimNow, true);
        public static bool deadPressed = false;

        [HarmonyPatch(typeof(PlayerAvatar), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerAvatar __instance)
        {
            dead.Update();
            if (deadPressed)
            {
                Vector3 avatar = new Vector3(__instance.localCameraTransform.position.x, __instance.localCameraTransform.position.y, __instance.localCameraTransform.position.z);
                if (Physics.Raycast(avatar + __instance.transform.forward * 1.1f, __instance.localCameraTransform.transform.forward, out var hit, float.MaxValue)) // max float not that big?
                {
                    //UnityEngine.Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.tag.Equals("Enemy"))
                    {
                        //Debug.Log("Enemy SPOTTED.");
                        GameObject enemy = hit.transform.parent.Find("Controller").gameObject;
                        enemy.GetComponent<EnemyHealth>().Hurt(int.MaxValue, Vector3.forward);
                    }
                }
            }
        }

        static void KillHimNow()
        {
            deadPressed = !deadPressed;
        }
    }
}
