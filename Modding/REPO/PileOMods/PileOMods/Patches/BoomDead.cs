using System;
using UnityEngine;
using HarmonyLib;
using TMPro;
using UnityEngine.InputSystem.EnhancedTouch;
using Object = System.Object;
using UnityEngine.Windows;

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
        }

        static void KillHimNow()
        {
            deadPressed = !deadPressed;

            Vector3 avatar = new Vector3(PlayerAvatar.instance.localCameraTransform.position.x, PlayerAvatar.instance.localCameraTransform.position.y, PlayerAvatar.instance.localCameraTransform.position.z);
            int layerMask = ~(1 << LayerMask.NameToLayer("RoomVolume"));
            Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.GetMousePosition());
            
            if (Physics.Raycast(ray, out var hit, float.MaxValue, layerMask))      
            {
                GameObject target = null;
                Transform parent = hit.transform.parent;
                var enemyHealth = parent.GetComponentInChildren<EnemyHealth>();

                if (enemyHealth)
                {
                    target = enemyHealth.gameObject;
                }
                else
                {
                    while (!parent.GetComponent<EnemyParent>() && parent.parent != null)
                    {
                        if (parent.GetComponent<EnemyHealth>())
                        {
                            target = parent.gameObject;
                            break;
                        }

                        enemyHealth = parent.GetComponentInChildren<EnemyHealth>();
                        if (enemyHealth)
                        {
                            target = enemyHealth.gameObject;
                            break;
                        }

                        if (parent.parent != null)
                            parent = parent.parent;
                        else
                            break;
                    }
                }

                if (target != null)
                    target.GetComponent<EnemyHealth>().Hurt(int.MaxValue, Vector3.forward);
                else
                    Console.WriteLine("lmao you missed");
            }
        }
    }
}
