#nullable enable
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

        private static PlayerControllerB? _playerController;
        private static CharacterController? _controller;
        private static Rigidbody? _rigidbody;

        public static bool g_enabled = false;
        public static bool flyUp = false;
        private static float originalRadius;
        public static float originalJumpForce;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake()
        {
            //if (Player.LocalPlayer() == null)
            //{
            //    Debug.Log("Player.LocalPlayer() is null! -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-");
            //    return;
            //}

            //_controller = Player.LocalPlayer().GetComponent<CharacterController>();
            //_rigidbody = Player.LocalPlayer().GetComponent<Rigidbody>();

            //originalRadius = _controller.radius;
            //originalJumpForce = Player.LocalPlayer().jumpForce;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(PlayerControllerB __instance)
        {
            if (_playerController == null)
            {
                _playerController = __instance;
            }
            _controller = _playerController.GetComponent<CharacterController>();
            _rigidbody = _playerController.GetComponent<Rigidbody>();

            originalRadius = _controller.radius;
            originalJumpForce = _playerController.jumpForce;

            noclipKey.Update();
            if (g_enabled)
            {
                // don't know if all these are necessary but it works
                currentFrameHeight = _playerController.transform.position.y;
                _playerController.fallValue = 0.0f;
                _playerController.fallValueUncapped = 0.0f;
                _playerController.takingFallDamage = false;
                _playerController.externalForces = Vector3.zero;
                if (lastFrameHeight != 0.0f)
                {
                    Vector3 transformPosition = _playerController.transform.position;
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
                    _playerController.transform.position = transformPosition;
                }
                lastFrameHeight = _playerController.transform.position.y;
            }
            else
            {
                _controller.radius = originalRadius;
                _playerController.jumpForce = originalJumpForce;
                lastFrameHeight = 0.0f;
                currentFrameHeight = 0.0f;
            }
        }

        public static void NoClipToggle()
        {
            if (_playerController == null || _controller == null || _rigidbody == null)
                return;

            g_enabled = !g_enabled;

            Collider[] colliders = _playerController.GetComponents<Collider>();
            if(colliders.Length == 0)
                Debug.Log("no colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

            Collider[] ccolliders = _playerController.GetComponentsInChildren<Collider>();
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

            _controller.radius = Math.Abs(_controller.radius - originalRadius) > 0.1 ? originalRadius : float.PositiveInfinity; // doesn't actually work with 0
            _rigidbody.detectCollisions = !_rigidbody.detectCollisions;
            _controller.detectCollisions = !_controller.detectCollisions;
            _playerController.ResetFallGravity();
        }
    }
}
