using System;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoClip
    {
        public static ModHotkey noclipKey = new ModHotkey(MouseAndKeyboard.PageDown, NoClipToggle);
        private static CharacterController controller = null;
        private static PlayerControllerB playerController = null;
        private static Rigidbody rigidbody = null;
        private static float fallValue = 0.0f;
        private static float fallValueUncapped = 0.0f;
        private static float originalRadius = 0.0f;
        public static bool g_enabled = false;
        public static bool setRadius = false;
        public static Vector3 originalGravity;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {
            playerController = __instance;
            originalGravity = Physics.gravity;
            controller = __instance.GetComponent<CharacterController>();
            rigidbody = __instance.GetComponent<Rigidbody>();
            setRadius = false;
            originalRadius = controller.radius;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(PlayerControllerB __instance)
        {
            noclipKey.Update();
            if (g_enabled)
            {
                __instance.fallValue = 0.0f;
                __instance.fallValueUncapped = 0.0f;
                __instance.takingFallDamage = false;
                //Vector3 transformPosition = __instance.transform.position;
                //transformPosition.y = transformPosition.y;
                //__instance.transform.position = transformPosition;
            }
            else
            {
                if(controller != null)
                    controller.radius = originalRadius;
            }
        }


        public static void NoClipToggle()
        {
            g_enabled = !g_enabled;

            Collider[] colliders = playerController.GetComponents<Collider>();
            if(colliders.Length == 0)
                Debug.Log("no colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

            Collider[] ccolliders = playerController.GetComponentsInChildren<Collider>();
            if(ccolliders.Length == 0)
                Debug.Log("no child colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");


            foreach (Collider col in colliders)
            {
                col.enabled = !col.enabled;
                if (col.enabled == false)
                {
                    fallValueUncapped = 0.0f;
                    fallValue = 0.0f;
                    //Debug.Log(col.name + " is false -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }
            }
            foreach (Collider col in ccolliders)
            {
                col.enabled = !col.enabled;
                if (col.enabled == false)
                {
                    fallValueUncapped = 0.0f;
                    fallValue = 0.0f;
                    //Debug.Log(col.name + " is false -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
                }
            }

            controller.radius = Math.Abs(controller.radius - originalRadius) > 0.1 ? originalRadius : 0.0f;
            Debug.Log($"controller radius: {controller.radius} -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            Debug.Log($"setRadius: {setRadius} ==================================================================");
            rigidbody.isKinematic = !rigidbody.isKinematic;
            //setRadius = true;
        }
    }
}
