using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoClip
    {
        public static ModHotkey noclipKey = new ModHotkey(MouseAndKeyboard.PageDown, Thing);
        private static PlayerControllerB controller;
        private static float fallValue = 0.0f;
        private static float fallValueUncapped = 0.0f;
        public static bool g_enabled = false;
        public static Vector3 originalGravity;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {
            controller = __instance;
            originalGravity = Physics.gravity;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(PlayerControllerB __instance)
        {
            noclipKey.Update();
            if (g_enabled)
            {
                __instance.fallValue = fallValue;
                __instance.fallValueUncapped = fallValueUncapped;
                Vector3 transformPosition = __instance.transform.position;
                transformPosition.y = transformPosition.y;
            }
            else
            {
                Physics.gravity = originalGravity;
            }
        }


        public static void Thing()
        {
            g_enabled = !g_enabled;

            Collider[] colliders = controller.GetComponents<Collider>();
            if(colliders.Length == 0)
                Debug.Log("no colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

            Collider[] ccolliders = controller.GetComponentsInChildren<Collider>();
            if(ccolliders.Length == 0)
                Debug.Log("no child colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");


            foreach (Collider col in colliders)
            {
                col.enabled = !col.enabled;
                if (col.enabled == false)
                {
                    fallValueUncapped = 0.0f;
                    fallValue = 0.0f;
                    Debug.Log(col.name + " is false -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }
            }

            foreach (Collider col in ccolliders)
            {
                col.enabled = !col.enabled;
                if (col.enabled == false)
                {
                    fallValueUncapped = 0.0f;
                    fallValue = 0.0f;
                    Debug.Log(col.name + " is false -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }
            }
        }
    }
}
