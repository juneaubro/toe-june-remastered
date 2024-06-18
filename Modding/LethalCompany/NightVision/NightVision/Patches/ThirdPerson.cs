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
        public static Camera? fpCamera;
        public static Canvas? canvas;
        public static GameObject? go;

        private static Camera? _camera;
        private static PlayerControllerB? _playerController;
        private static bool setOGCam = false;
        private static bool setCam = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {


        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update()
        {
            if (Player.LocalPlayer() == null)
                return;

            if (_playerController == null)
                _playerController = Player.LocalPlayer();

            tpKey.Update();

            //if (!setOGCam)
            //{
            //    fpCamera = _playerController.gameplayCamera;
            //    setOGCam = true;
            //}

            //if (!setCam)
            //{
            //    _camera = new GameObject("3rdPersonMod").AddComponent<Camera>();
            //    _camera.gameObject.transform.SetParent(_playerController.transform);
            //    _camera.nearClipPlane = 0.01f;
            //    _camera.cullingMask = Int32.MaxValue;
            //    _camera.hideFlags = HideFlags.HideAndDontSave;
            //    _camera.enabled = false;
            //    DontDestroyOnLoad(_camera);
            //    setCam = true;
            //}
            if(canvas == null)
                canvas = GameObject.Find("Systems/UI/Canvas/").GetComponent<Canvas>();

            if(go == null)
                go = GameObject.Find("Systems/UI/Canvas/Panel/");

            if (g_enabled && _camera != null)
            {
                Vector3 vector3_1 = _playerController.gameplayCamera.transform.forward * -1f;
                Vector3 vector3_2 = _playerController.gameplayCamera.transform.TransformDirection(Vector3.right) * 0.5f;
                Vector3 vector3_3 = Vector3.up * 0.5f;
                const float num = 0.5f;
                _camera.transform.position = _playerController.gameplayCamera.transform.position + vector3_1 * num + vector3_2 + vector3_3;
                _camera.transform.rotation = Quaternion.LookRotation(_playerController.gameplayCamera.transform.forward);
            }
        }

        public static void ThirdPersonToggle()
        {
            if (_playerController == null || canvas == null)
                return;

            if (!setCam)
            {
                if (_camera == null)
                {
                    _camera = new GameObject("3rdPersonCamera").AddComponent<Camera>();
                    _camera.nearClipPlane = 0.01f;
                    _camera.cullingMask = Int32.MaxValue;
                    _camera.hideFlags = HideFlags.HideAndDontSave;
                    _camera.enabled = false;
                    DontDestroyOnLoad(_camera);
                }

                _camera.gameObject.transform.SetParent(_playerController.transform);
                setCam = true;
            }

            if (go == null)
                return;

            g_enabled = !g_enabled;
            
            if (g_enabled)
            {
                if (_camera == null)
                    return;

                //playerController.gameplayCamera.gameObject.transform.localPosition += new Vector3(0, 1f, -1.5f);
                go.SetActive(false);
                canvas.worldCamera = _camera;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/").SetActive(false);
                _camera.enabled = true;
            }
            else
            {
                //playerController.gameplayCamera.gameObject.transform.localPosition -= new Vector3(0, 1f, -1.5f);
                go.SetActive(true);
                canvas.worldCamera = GameObject.Find("UICamera").GetComponent<Camera>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/").SetActive(true);
            }

            _playerController.thisPlayerModelArms.enabled = !_playerController.thisPlayerModelArms.enabled;
            _playerController.gameplayCamera.enabled = !_playerController.gameplayCamera.enabled;
        }
    }
}
