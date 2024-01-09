#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
    internal class NoClip2
    {
        public static float lastFrameHeight;
        public static float currentFrameHeight;

        public static ModHotkey noclipKey = new ModHotkey(MouseAndKeyboard.MouseMiddle, NoClipToggle, true);

        private static PlayerControllerB playerController = null;
        private static CharacterController controller = null;
        private static Rigidbody rigidbody = null;
        public static Vector3 wf;
        public static bool isFallingFromJump;
        public static bool isFallingNoJump;
        public static float timeSinceCrouching;

        public static bool g_enabled = false;
        public static bool flyUp = false;
        private static float originalRadius;
        public static float originalJumpForce;
        public static bool moving = false;
        public static float totalRun = 1f;
        public static float shiftAdd = 3; // acceleration noclip speed
        public static float maxShift = 30; // max noclip sprint speed
        public static float ForceMulti = 2000f; // noclip speed

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake(PlayerControllerB __instance, Vector3 ___walkForce, bool ___isFallingFromJump, bool ___isFallingNoJump, float ___timeSinceCrouching)
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
            playerController = __instance;
            controller = __instance.GetComponent<CharacterController>();
            rigidbody = __instance.GetComponent<Rigidbody>();
            wf = ___walkForce;
            isFallingFromJump = ___isFallingFromJump;
            isFallingNoJump = ___isFallingNoJump;
            timeSinceCrouching = ___timeSinceCrouching;

            originalRadius = controller.radius;
            originalJumpForce = playerController.jumpForce;
        }

        [HarmonyPatch("Update")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {// update: line 3158 end 3770
         // gravity: line 3200
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int count = 0;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i] == null)
                {
                    continue;
                }
                else
                {
                    if (AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.fallValueUncapped)) != null)
                    {
                        if (codes[i].StoresField(AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.fallValueUncapped))))
                        {
                            count++;
                            if (count == 2)
                            {   // [3203 15 - 3203 61]
                                // IL_0249: stfld        float32 GameNetcodeStuff.PlayerControllerB::fallValueUncapped
                                codes.Insert(i - 19, new CodeInstruction(OpCodes.Ldarg_0)); // brtrue.s in IL Code is if
                                codes.Insert(i - 18, new CodeInstruction(OpCodes.Ldfld, g_enabled));
                                codes.Insert(i - 17, new CodeInstruction(OpCodes.Brfalse_S, codes[i + 19].labels)); // codes[i+38] is the else line 3222
                                codes.Insert(i - 16, new CodeInstruction(OpCodes.Ldarg_0));
                                codes.Insert(i - 15, new CodeInstruction(OpCodes.Ldc_R4, 0f));
                                codes.Insert(i - 14, new CodeInstruction(OpCodes.Stfld, playerController.fallValue));
                                codes.Insert(i - 13, new CodeInstruction(OpCodes.Ldarg_0));
                                codes.Insert(i - 12, new CodeInstruction(OpCodes.Ldc_R4, 0f));
                                codes.Insert(i - 11, new CodeInstruction(OpCodes.Stfld, playerController.fallValueUncapped));
                                codes.Insert(i - 10, new CodeInstruction(OpCodes.Br_S, codes[i + 1].labels));
                            }
                        } // sometin is null so it dont work.
                    }
                }
            }
            return codes.AsEnumerable();
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update(PlayerControllerB __instance)
        {
            noclipKey.Update();
            Debug.Log($"fallValue: {playerController.fallValue} fallValueUncapped: {playerController.fallValueUncapped} isGrouned: {playerController.thisController.isGrounded}");
            if (g_enabled)
            {
                playerController.drunknessSpeed = 0f;
                playerController.thisController.isGrounded.Equals(true);
                // don't know if all these are necessary but it works
                currentFrameHeight = playerController.transform.position.y;
                playerController.takingFallDamage = false;
                //playerController.fallValue = 0f;
                //playerController.fallValueUncapped = 0f;
                //playerController.externalForces = Vector3.zero;
                if (lastFrameHeight != 0.0f)
                {
                    Vector3 transformPosition = playerController.transform.position;
                    if (UnityInput.Current.GetKey(KeyCode.Space))
                    {
                        transformPosition.y += 0.045f;
                    }
                    else if (UnityInput.Current.GetKey(KeyCode.LeftControl) ||
                             UnityInput.Current.GetKey(KeyCode.LeftCommand))
                    {
                        transformPosition.y -= 0.045f;
                    }
                    else
                    {
                        transformPosition.y += Math.Abs(currentFrameHeight - lastFrameHeight); // i have no idea what other force is pushing this dude down but just do the opposite
                    }
                    playerController.transform.position = transformPosition;
                }
                FlyNoClip();
                lastFrameHeight = playerController.transform.position.y;
            }
            else
            {
                controller.radius = originalRadius;
                playerController.jumpForce = originalJumpForce;
                lastFrameHeight = 0.0f;
                currentFrameHeight = 0.0f;
                playerController.drunknessSpeed = 1f;
            }
        }

        public static float SetZero()
        {
            return Mathf.Clamp(0, 0, 0);
        }

        public static void FlyNoClip()
        {
            Vector3 p = GetBaseInput();
            if (p.sqrMagnitude > 0)
            { // only move while a direction key is pressed
                if (UnityInput.Current.GetKey(KeyCode.LeftShift))
                {
                    totalRun += Time.deltaTime;
                    p = p * totalRun * shiftAdd;
                    p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                    p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                    p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
                }
                else
                {
                    totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                    p = p * playerController.movementSpeed * ForceMulti;
                }
                p = p * Time.deltaTime;
                //Vector3 newPosition = playerController.transform.position;
                //if (UnityInput.Current.GetKey(KeyCode.Space))
                //{ //If player wants to move on X and Z axis only
                //    transform.Translate(p);
                //    newPosition.x = transform.position.x;
                //    newPosition.z = transform.position.z;
                //    transform.position = newPosition;
                //}
                //else
                //{
                //    transform.Translate(p);
                //}
                //wf = Vector3.MoveTowards(wf, playerController.transform.right * playerController.moveInputVector.x + playerController.transform.forward * playerController.moveInputVector.y, (isFallingFromJump || isFallingNoJump ? 1.33f : ((double)playerController.drunkness <= 0.300000011920929 ? (playerController.isCrouching || (double)timeSinceCrouching >= 0.200000002980232 ? (!playerController.isSprinting ? 10f / playerController.carryWeight : (float)(5.0 / ((double)playerController.carryWeight * 1.5))) : 15f) : Mathf.Clamp(Mathf.Abs(playerController.drunkness - 2.25f), 0.3f, 2.5f))) * Time.deltaTime);
                wf = Vector3.MoveTowards(wf, playerController.transform.right * playerController.moveInputVector.x + playerController.transform.forward * playerController.moveInputVector.y, playerController.movementSpeed * ForceMulti * Time.deltaTime);
                //playerController.transform.Translate(p);
            }
        }

        public static Vector3 GetBaseInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (UnityInput.Current.GetKey(KeyCode.W))
            {
                moving = true;
                p_Velocity += new Vector3(0, 0, 1);
            }
            else
                moving = false;
            if (UnityInput.Current.GetKey(KeyCode.S))
            {
                moving = true;
                p_Velocity += new Vector3(0, 0, -1);
            }
            else
                moving = false;
            if (UnityInput.Current.GetKey(KeyCode.A))
            {
                moving = true;
                p_Velocity += new Vector3(-1, 0, 0);
            }
            else
                moving = false;
            if (UnityInput.Current.GetKey(KeyCode.D))
            {
                moving = true;
                p_Velocity += new Vector3(1, 0, 0);
            }
            else
                moving = false;
            return p_Velocity;
        }

        public static void NoClipToggle()
        {
            g_enabled = !g_enabled;
            Collider[] colliders = playerController.GetComponents<Collider>();
            if (colliders.Length == 0)
                Debug.Log("no colliders -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

            Collider[] ccolliders = playerController.GetComponentsInChildren<Collider>();
            if (ccolliders.Length == 0)
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
            //playerController.ResetFallGravity();
        }
    }
}
