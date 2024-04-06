#nullable enable
using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;


namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NoClip
    {
        public static bool g_enabled = false;
        public static ModHotkey noclipKey = new ModHotkey(MouseAndKeyboard.PageDown, NoClipToggle);

        private static float NOCLIP_SPEED = 0.0125f;
        private static PlayerControllerB? _playerController;
        private static CharacterController? _controller;
        private static Rigidbody? _rigidbody;
        private static Transform? _parent;
        private static float _originalJumpForce;
        private static float _lastFrameHeight;
        private static float _currentFrameHeight;
        private static float _originalRadius;
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance)
        {
            _playerController = __instance;
            _controller = __instance.GetComponent<CharacterController>();
            _rigidbody = __instance.GetComponent<Rigidbody>();
            //_controller = Player.LocalPlayer().GetComponent<CharacterController>();
            //_rigidbody = Player.LocalPlayer().GetComponent<Rigidbody>();

            //originalRadius = _controller.radius;
            //originalJumpForce = Player.LocalPlayer().jumpForce;
            _originalRadius = _controller.radius;
            _originalJumpForce = _playerController.jumpForce;

        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update(PlayerControllerB __instance)
        {
            if (_playerController == null)
            {
                _playerController = __instance;
            }
            _controller = _playerController.GetComponent<CharacterController>();
            _rigidbody = _playerController.GetComponent<Rigidbody>();

            noclipKey.Update();

            if (g_enabled)
            {
                // don't know if all these are necessary but it works
                _currentFrameHeight = _playerController.transform.position.y;

                _playerController.fallValue = 0.0f;
                _playerController.fallValueUncapped = 0.0f;
                _playerController.takingFallDamage = false;
                _playerController.externalForces = Vector3.zero;

                Vector3 transformPosition = _playerController.transform.position;

                if (_lastFrameHeight != 0.0f)
                {
                    if (UnityInput.Current.GetKeyDown(KeyCode.LeftShift))
                    {
                        NOCLIP_SPEED = 0.0225f;
                    }

                    if (UnityInput.Current.GetKeyUp(KeyCode.LeftShift))
                    {
                        NOCLIP_SPEED = 0.0125f;
                    }
                    if (UnityInput.Current.GetKey(KeyCode.Space))
                    {
                        transformPosition.y += NOCLIP_SPEED;
                    }
                    if (UnityInput.Current.GetKey(KeyCode.LeftControl) ||
                             UnityInput.Current.GetKey(KeyCode.LeftCommand))
                    {
                        transformPosition.y -= NOCLIP_SPEED;
                    }
                    if (UnityInput.Current.GetKey(KeyCode.W))
                    {
                        transformPosition += (_playerController.gameplayCamera.transform.forward * NOCLIP_SPEED);
                    }
                    if (UnityInput.Current.GetKey(KeyCode.S))
                    {
                        transformPosition -= (_playerController.gameplayCamera.transform.forward * NOCLIP_SPEED);
                    }
                    if (UnityInput.Current.GetKey(KeyCode.D))
                    {
                        transformPosition += (_playerController.gameplayCamera.transform.right * NOCLIP_SPEED);
                    }
                    if (UnityInput.Current.GetKey(KeyCode.A))
                    {
                        transformPosition -= (_playerController.gameplayCamera.transform.right * NOCLIP_SPEED);
                    }
                }

                _playerController.transform.position = transformPosition;
                _lastFrameHeight = _playerController.transform.position.y;
            }
            else
            {
                _lastFrameHeight = 0.0f;
                _currentFrameHeight = 0.0f;
            }
        }

        public static void NoClipToggle()
        {
            if (_playerController == null || _controller == null || _rigidbody == null)
                return;

            g_enabled = !g_enabled;

            Collider[] colliders = _playerController.GetComponents<Collider>();
            Collider[] ccolliders = _playerController.GetComponentsInChildren<Collider>();

            foreach (Collider col in colliders)
            {
                col.enabled = !col.enabled;
                Debug.Log($"{col} is {col.enabled}");
            }
            foreach (Collider col in ccolliders)
            {
                col.enabled = !col.enabled;
                Debug.Log($"{col} is {col.enabled}");
            }

            if (g_enabled)
            {
                _controller.radius = float.NegativeInfinity;
                if (_playerController.gameObject.transform.parent != null)
                {
                    _parent = _playerController.gameObject.transform.parent; // keep reference around just in case
                    _playerController.gameObject.transform.SetParent(null);
                }

            }
            else
            {
                _controller.radius = _originalRadius;
                if (_parent != null && !StartLever.pulledLever)
                {
                    _playerController.gameObject.transform.SetParent(_parent); // should reparent player to ship
                }
            }

            _rigidbody.detectCollisions = !_rigidbody.detectCollisions;
            _controller.detectCollisions = !_controller.detectCollisions;
            _playerController.ResetFallGravity();
            _playerController.jumpForce = _originalJumpForce;
        }
    }
}
