using System;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

#nullable enable
namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class ThirdPerson : MonoBehaviour
    {
        public static ModHotkey tpKey = new ModHotkey(MouseAndKeyboard.Numpad3, ThirdPersonToggle);
        public static bool g_enabled = false;
        public static PlayerControllerB? playerController;
        private static Camera? _camera;
        public static Camera? fpCamera;
        public static Canvas? canvas;
        private static bool setOGCam = false;
        private static bool setCam = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {


        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update()
        {
            if (GameNetworkManager.Instance.localPlayerController == null)
                return;

            tpKey.Update();

            playerController = GameNetworkManager.Instance.localPlayerController;

            if (!setOGCam)
            {
                fpCamera = playerController.gameplayCamera;
                setOGCam = true;
            }

            if (!setCam)
            {
                _camera = new GameObject("3rdPersonMod").AddComponent<Camera>();
                _camera.gameObject.transform.SetParent(playerController.transform);
                _camera.nearClipPlane = 0.01f;
                _camera.cullingMask = Int32.MaxValue;
                _camera.hideFlags = HideFlags.HideAndDontSave;
                _camera.enabled = false;
                DontDestroyOnLoad(_camera);
                setCam = true;
            }
            canvas = GameObject.Find("Systems/UI/Canvas/").GetComponent<Canvas>();
        }

        public static void ThirdPersonToggle()
        {
            if (playerController == null || canvas == null)
                return;

            g_enabled = !g_enabled;
            
            if (g_enabled)
            {
                //playerController.gameplayCamera.gameObject.transform.localPosition += new Vector3(0, 1f, -1.5f);
                canvas.worldCamera = _camera;
            }
            else
            {
                //playerController.gameplayCamera.gameObject.transform.localPosition -= new Vector3(0, 1f, -1.5f);
                canvas.worldCamera = fpCamera;
            }

            playerController.thisPlayerModelArms.enabled = !playerController.thisPlayerModelArms.enabled;
        }

        
    }
}
