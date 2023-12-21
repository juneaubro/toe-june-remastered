using System.CodeDom;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class ThirdPerson
    {
        public static ModHotkey tpKey = new ModHotkey(MouseAndKeyboard.Numpad3, ThirdPersonToggle);
        public static bool g_enabled = false;
        public static PlayerControllerB playerController;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {
            // don't know if this is more optimal over putting entire g_enabled check in update
            playerController = __instance;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update()
        {
            tpKey.Update();
        }

        public static void ThirdPersonToggle()
        {
            g_enabled = !g_enabled;
            if (g_enabled)
            {
                playerController.gameplayCamera.gameObject.transform.localPosition += new Vector3(0, 1f, -1.5f);
                var transformRotation = playerController.gameplayCamera.gameObject.transform.rotation;
                transformRotation.eulerAngles = new Vector3(
                    playerController.gameplayCamera.gameObject.transform.rotation.x,
                    playerController.gameplayCamera.gameObject.transform.rotation.y + 180f,
                    playerController.gameplayCamera.gameObject.transform.rotation.z);
            }
            else
            {
                playerController.gameplayCamera.gameObject.transform.localPosition -= new Vector3(0, 1f, -1.5f);
                var transformRotation = playerController.gameplayCamera.gameObject.transform.rotation;
                transformRotation.eulerAngles = new Vector3(
                    playerController.gameplayCamera.gameObject.transform.rotation.x,
                    playerController.gameplayCamera.gameObject.transform.rotation.y - 180f,
                    playerController.gameplayCamera.gameObject.transform.rotation.z);
            }
        }

        
    }
}
