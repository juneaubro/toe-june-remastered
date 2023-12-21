using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;


namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoClip
    {
        public static float lastFrameHeight;
        public static float currentFrameHeight;

        public static ModHotkey noclipKey = new ModHotkey(MouseAndKeyboard.PageDown, NoClipToggle);

        private static CharacterController controller = null;
        private static PlayerControllerB playerController = null;
        private static Rigidbody rigidbody = null;

        public static bool g_enabled = false;
        public static bool flyUp = false;
        private static float originalRadius;
        public static float originalJumpForce;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {
            playerController = __instance;
            controller = __instance.GetComponent<CharacterController>();
            rigidbody = __instance.GetComponent<Rigidbody>();

            originalRadius = controller.radius;
            originalJumpForce = playerController.jumpForce;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update()
        {
            noclipKey.Update();
            if (g_enabled)
            {
                // don't know if all these are necessary but it works
                currentFrameHeight = playerController.transform.position.y;
                playerController.fallValue = 0.0f;
                playerController.fallValueUncapped = 0.0f;
                playerController.takingFallDamage = false;
                playerController.externalForces = Vector3.zero;
                if (lastFrameHeight != 0.0f)
                {
                    Vector3 transformPosition = playerController.transform.position;
                    if (UnityInput.Current.GetKey(KeyCode.Space))
                    {
                        transformPosition.y += 0.03f;
                    }
                    else if (UnityInput.Current.GetKey(KeyCode.LeftControl) ||
                             UnityInput.Current.GetKey(KeyCode.LeftCommand))
                    {
                        transformPosition.y -= 0.03f;
                    }
                    else
                    {
                        transformPosition.y += Math.Abs(currentFrameHeight - lastFrameHeight); // i have no idea what other force is pushing this dude down but just do the opposite
                    }
                    playerController.transform.position = transformPosition;
                }
                lastFrameHeight = playerController.transform.position.y;
            }
            else
            {
                controller.radius = originalRadius;
                playerController.jumpForce = originalJumpForce;
                lastFrameHeight = 0.0f;
                currentFrameHeight = 0.0f;
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
            }
            foreach (Collider col in ccolliders)
            {
                col.enabled = !col.enabled;
            }

            controller.radius = Math.Abs(controller.radius - originalRadius) > 0.1 ? originalRadius : float.PositiveInfinity; // doesn't actually work with 0
            rigidbody.detectCollisions = !rigidbody.detectCollisions;
            controller.detectCollisions = !controller.detectCollisions;
            playerController.ResetFallGravity();
        }
    }
}
