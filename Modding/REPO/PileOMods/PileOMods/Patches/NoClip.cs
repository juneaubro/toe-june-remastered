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

                _rigidbody.velocity = Vector3.zero;
                _rigidbody.useGravity = !Enabled;
                _rigidbody.isKinematic = Enabled;

                float x = InputManager.instance.GetMovementX();
                float y = InputManager.instance.GetMovementY();
                float upDir = 0.0f;
                float flySpeed = 10.0f;

                if (InputManager.instance.GetAction(InputKey.Sprint).IsPressed())
                    flySpeed *= 1.75f;

                if (InputManager.instance.GetAction(InputKey.Jump).IsPressed())
                {
                    upDir = 1.0f;
                }

                if (InputManager.instance.GetAction(InputKey.Crouch).IsPressed())
                {
                    upDir = -1.0f;
                }

                Vector3 value = (_instance.transform.right * x) + (_instance.transform.forward * y) + (_instance.transform.up * upDir);
                _instance.transform.position += value * flySpeed * Time.deltaTime;
            }
        }

        private static void Toggle()
        {
            if (SemiFunc.IsMainMenu())
                return;

            Enabled = !Enabled;

            _rigidbody.detectCollisions = !Enabled;

            _footstepAudioSource.mute = Enabled;

            _instance.CustomGravity = Enabled ? 0.0f : _originalCustomGravity;
            _instance.JumpForce = Enabled ? 0.0f : _originalJumpForce;
            _instance.Velocity = Vector3.zero;
            _instance.VelocityRelative = Vector3.zero;

            Console.WriteLine($"noclip: {Enabled}");
        }
    }

    [HarmonyPatch(typeof(PlayerAvatar))]
    internal class NoClip_PlayerAvatar
    {
        [HarmonyPatch(typeof(PlayerAvatar), "StandToCrouch")]
        [HarmonyPrefix]
        public static void StandToCrouch()
        {
            if (NoClip_PlayerController.Enabled)
                return;
        }
    }
}
