﻿using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class IgnoreTwoHands
    {
        public static int slot;

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        static void Update(PlayerControllerB __instance)
        {
            //__instance.hoveringOverTrigger.twoHandedItemAllowed = true;
            //__instance.twoHanded = false;
            //__instance.twoHandedAnimation = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("BeginGrabObject")]
        static void BeginGrabObject(PlayerControllerB __instance)
        {
            __instance.twoHanded = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GrabObject")]
        static void GrabObject(PlayerControllerB __instance)
        {
            //__instance.twoHanded = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("SwitchToItemSlot")]
        static void SwitchToItemSlot(object[] __args)
        {
            slot = (int) __args[0];
        }

        [HarmonyPrefix]
        [HarmonyPatch("ScrollMouse_performed")]
        static void ScrollMouse_performed(PlayerControllerB __instance)
        {
            __instance.twoHanded = false;
        }
    }
}
