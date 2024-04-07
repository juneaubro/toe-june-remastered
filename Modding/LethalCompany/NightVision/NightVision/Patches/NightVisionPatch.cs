using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class NightVisionPatch
    {
        public static ModHotkey nvKey = new ModHotkey(MouseAndKeyboard.Insert, toggleNightVision);
        static bool isNightVision;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake()
        {
            isNightVision = true;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(ref Light ___nightVision)
        {
            nvKey.Update();

            // could honestly change ___nightVision.enabled = false :/
            if (isNightVision && Player.LocalPlayer() != null)
            {
                ___nightVision.enabled = true;
                ___nightVision.intensity = .05f;
                ___nightVision.range = 200f;
                ___nightVision.color = new Color(20, 20, 19);
                ___nightVision.shadowStrength = 0f;
                ___nightVision.type = LightType.Point;
                ___nightVision.shadows = LightShadows.None;
                ___nightVision.shape = LightShape.Pyramid;
                ___nightVision.transform.position = Player.LocalPlayer().gameplayCamera.transform.position;
                ___nightVision.transform.eulerAngles = new Vector3(180f, 180f, 180f);
            }
            else
            {
                ___nightVision.enabled = false;
            }
        }

        public static void toggleNightVision()
        {
            isNightVision = !isNightVision;

            Debug.Log($"Nightvision {isNightVision}.");

            //if(isNightVision)
            //    Debug.Log("Nightvision on.");
            //else
            //    Debug.Log("Nightvision off.");
        }
    }
}
