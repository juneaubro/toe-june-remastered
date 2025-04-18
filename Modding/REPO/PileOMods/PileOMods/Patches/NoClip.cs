using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(PlayerController))]
    internal class NoClip
    {
        public static bool Enabled = false;
        private static readonly ModHotkey NoclipKey = new(MouseAndKeyboard.UpArrow, Toggle, false);
        private static Rigidbody _rigidbody;
        private static CharacterController _characterController;

        [HarmonyPatch(typeof(PlayerController), "Start")]
        [HarmonyPostfix]
        public static void Start(PlayerController __instance)
        {
            _rigidbody = __instance.GetComponent<Rigidbody>();
            _characterController = __instance.GetComponent<CharacterController>();
        }

        [HarmonyPatch(typeof(PlayerController), "Update")]
        [HarmonyPostfix]
        public static void Update(PlayerAvatar __instance)
        {
            NoclipKey.Update();

            if (Enabled)
            {
            }
        }

        private static void Toggle()
        {
            Enabled = !Enabled;

            //_characterController.radius = Single.NegativeInfinity;


            //_collider.enabled = !Enabled;
            //_avatarCollision.enabled = !Enabled;

            //foreach (var collider in _player.GetComponentsInChildren<Collider>())
            //{
            //    collider.enabled = !Enabled;
            //    Console.WriteLine($"{collider.ToString()}\t{collider.enabled}");
            //}

            //_rigidbody.detectCollisions = !Enabled;
            //_rigidbody.useGravity = !Enabled;
            //_rigidbody.mass = _rigidbody.mass == 0.0f ? _originalMass : 0.0f;

            Console.WriteLine($"noclip: {Enabled}");
        }
    }
}
