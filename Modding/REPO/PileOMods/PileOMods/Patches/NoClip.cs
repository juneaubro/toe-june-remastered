using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal class NoClip_PlayerController : MonoBehaviour
    {
        public static bool Enabled = false;
        private static readonly ModHotkey NoclipKey = new(MouseAndKeyboard.UpArrow, Toggle, false);
        private static Rigidbody _rigidbody;
        private static float _originalCustomGravity;
        private static float _originalJumpForce;
        private static AudioSource _footstepAudioSource;
        private static PlayerController _instance;

        [HarmonyPatch(typeof(PlayerController), "Start")]
        [HarmonyPostfix]
        public static void Start(PlayerController __instance)
        {
            _instance = __instance;
            _rigidbody = _instance.GetComponent<Rigidbody>();
            _footstepAudioSource = AudioManager.instance.AudioFootstep.GetComponent<AudioSource>();
            _originalCustomGravity = _instance.CustomGravity;
            _originalJumpForce = _instance.JumpForce;
        }

        [HarmonyPatch(typeof(PlayerController), "Update")]
        [HarmonyPostfix]
        public static void Update()
        {
            NoclipKey.Update();

            if (Enabled)
            {
                _instance.CollisionController.ResetFalling();
                _instance.CollisionController.StopFallLoop();
                _instance.CollisionController.Grounded = true;
                _instance.Crouching = false;
                _instance.CrouchDisable(0.2f);

                _rigidbody.useGravity = !Enabled;
                _rigidbody.isKinematic = Enabled;

                InputManager inputManager = InputManager.instance;
                Transform transform = _instance.transform;
                float rightDir = inputManager.GetMovementX();
                float forwardDir = inputManager.GetMovementY();
                float upDir = 0.0f;
                float flySpeed = 10.0f;

                if (inputManager.GetAction(InputKey.Sprint).IsPressed())
                    flySpeed *= 1.75f;
                if (inputManager.GetAction(InputKey.Jump).IsPressed())
                    upDir = 1.0f;
                if (inputManager.GetAction(InputKey.Crouch).IsPressed())
                    upDir = -1.0f;

                transform.position += ((transform.right * rightDir) + (transform.forward * forwardDir) + (transform.up * upDir)) * flySpeed * Time.deltaTime;
            }
        }

        private static void Toggle()
        {
            if (SemiFunc.IsMainMenu())
                return;

            Enabled = !Enabled;

            _rigidbody.detectCollisions = !Enabled;
            _rigidbody.velocity = Vector3.zero;

            _footstepAudioSource.mute = Enabled;

            _instance.CustomGravity = Enabled ? 0.0f : _originalCustomGravity;
            _instance.JumpForce = Enabled ? 0.0f : _originalJumpForce;
            _instance.Velocity = Vector3.zero;
            _instance.VelocityRelative = Vector3.zero;

            Console.WriteLine($"noclip {(Enabled ? "enabled" : "disabled")}");
        }
    }

    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class NoClip_PlayerAvatar
    {
        private static float _originalLandVolume;
        private static float _originalJumpVolume;
        private static float _originalSlideVolume;

        [HarmonyPatch(typeof(PlayerAvatar), "Start")]
        [HarmonyPostfix]
        public static void Start(PlayerAvatar __instance)
        {
            _originalLandVolume = __instance.landSound.Volume;
            _originalJumpVolume = __instance.jumpSound.Volume;
            _originalSlideVolume = __instance.slideSound.Volume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "StandToCrouch")]
        [HarmonyPrefix]
        public static void StandToCrouch()
        {
            if (NoClip_PlayerController.Enabled)
                return;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Land")]
        [HarmonyPrefix]
        public static void Land_Prefix(PlayerAvatar __instance)
        {
            __instance.landSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalLandVolume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Land")]
        [HarmonyPostfix]
        public static void Land_Postfix(PlayerAvatar __instance)
        {
            __instance.landSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalLandVolume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Jump")]
        [HarmonyPrefix]
        public static void Jump_Prefix(PlayerAvatar __instance)
        {
            __instance.jumpSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalJumpVolume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Jump")]
        [HarmonyPostfix]
        public static void Jump_Postfix(PlayerAvatar __instance)
        {
            __instance.jumpSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalJumpVolume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Slide")]
        [HarmonyPrefix]
        public static void Slide_Prefix(PlayerAvatar __instance)
        {
            __instance.slideSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalSlideVolume;
        }

        [HarmonyPatch(typeof(PlayerAvatar), "Slide")]
        [HarmonyPostfix]
        public static void Slide_Postfix(PlayerAvatar __instance)
        {
            __instance.slideSound.Volume = NoClip_PlayerController.Enabled ? 0.0f : _originalSlideVolume;
        }
    }
}
