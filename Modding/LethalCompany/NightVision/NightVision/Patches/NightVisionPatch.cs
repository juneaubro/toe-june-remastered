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
        static void Awake(ref Light ___nightVision)
        {
            isNightVision = true;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Update(ref Light ___nightVision,PlayerControllerB __instance)
        {
            nvKey.Update();
            // could honestly change ___nightVision.enabled = false :/
            if (isNightVision)
            {
                ___nightVision.enabled = true;
                ___nightVision.intensity = .05f;
                ___nightVision.range = 200f;
                ___nightVision.color = new Color(209, 208, 194);
                ___nightVision.shadowStrength = 0f;
                ___nightVision.type = LightType.Point;
                ___nightVision.shadows = LightShadows.None;
                ___nightVision.shape = LightShape.Cone;
                ___nightVision.transform.position = __instance.playerEye.transform.position;
                ___nightVision.transform.rotation = __instance.playerEye.transform.rotation;
            }
            else
            {
                ___nightVision.enabled = false;
            }
        }

        public static void toggleNightVision()
        {
            isNightVision = !isNightVision;
            if(isNightVision)
                Debug.Log("Nightvision on.");
            else
                Debug.Log("Nightvision off.");
        }
    }
}
