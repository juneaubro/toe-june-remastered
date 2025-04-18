using System;
using UnityEngine;
using HarmonyLib;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class BoomDead
    {
        public static ModHotkey dead = new ModHotkey(MouseAndKeyboard.MouseMiddle, KillHimNow, false);
        public static bool deadPressed = false;

        [HarmonyPatch(typeof(PlayerAvatar), "Update")]
        [HarmonyPrefix]
        static void Update(PlayerAvatar __instance)
        {
            dead.Update();

            if (deadPressed)
            {
                //Vector3 avatar = new Vector3(__instance.localCameraTransform.position.x, __instance.localCameraTransform.position.y, __instance.localCameraTransform.position.z);
                //if (Physics.Raycast(avatar + __instance.transform.forward * 1.1f, __instance.localCameraTransform.transform.forward, out var hit, float.MaxValue)) // max float not that big?
                //{
                //    UnityEngine.Debug.Log(hit.transform.gameObject.name);

                //    GameObject target = new();

                //    //if (hit.transform.tag.Equals("Enemy"))
                //    //{
                //    //    target = hit.transform.parent.Find("Controller").gameObject;
                //    //}

                //    Transform parent = hit.transform.parent;
                //    while (parent != null)
                //    {
                //        if (parent.GetComponent<EnemyHealth>())
                //        {
                //            target = parent.gameObject;
                //            break;
                //        }

                //        parent = hit.transform.parent;
                //    }

                //    if(target)
                //        target.GetComponent<EnemyHealth>().Hurt(int.MaxValue, Vector3.forward);
                //    else
                //    {
                //        Console.WriteLine("lmao you missed");
                //    }
                //}
            }
        }

        static void KillHimNow()
        {
            deadPressed = !deadPressed;

            Vector3 avatar = new Vector3(PlayerAvatar.instance.localCameraTransform.position.x, PlayerAvatar.instance.localCameraTransform.position.y, PlayerAvatar.instance.localCameraTransform.position.z);
            if (Physics.Raycast(avatar + PlayerAvatar.instance.transform.forward * 1.1f, PlayerAvatar.instance.localCameraTransform.transform.forward, out var hit, float.MaxValue)) // max float not that big?
            {
                Debug.Log("1");
                GameObject target = null;
                Transform parent = hit.transform.parent;
                Debug.Log("11");

                var enemyHealth = parent.GetComponentInChildren<EnemyHealth>();

                if (enemyHealth)
                {
                    target = enemyHealth.gameObject;
                    Debug.Log("11.5");
                }
                else
                {
                    Debug.Log("12");
                    int count = 0;

                    while (!parent.GetComponent<EnemyParent>() && parent.parent != null)
                    {
                        Debug.Log("13");
                        count--;
                        if (parent.GetComponent<EnemyHealth>())
                        {
                            target = parent.gameObject;
                            break;
                        }
                        Debug.Log("14");

                        enemyHealth = parent.GetComponentInChildren<EnemyHealth>();
                        if (enemyHealth)
                            break;
                        
                        Debug.Log("15");
                        Console.WriteLine($"[{Math.Abs(count)}]\t{parent.ToString()}");

                        if (parent.parent != null)
                            parent = parent.parent;
                        else
                            break;
                    }
                    Debug.Log("16");
                }

                if (target != null)
                {
                    Debug.Log("17");
                    target.GetComponent<EnemyHealth>().Hurt(int.MaxValue, Vector3.forward);
                    Debug.Log("18");

                }
                else
                {
                    Debug.Log("19");
                    Console.WriteLine("lmao you missed");
                }
            }
        }
    }
}
