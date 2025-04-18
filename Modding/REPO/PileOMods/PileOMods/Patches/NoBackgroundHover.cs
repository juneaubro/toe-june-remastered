using HarmonyLib;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using UnityEngine;

namespace PileOMods.Patches
{
    [HarmonyPatch(typeof(MenuHolder))]
    internal class NoBackgroundHover
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private static bool IsGameFocused() => GetForegroundWindow() == GetActiveWindow();

        private static bool _focused = false;
        private static bool _lastFocused = true;
        private static bool _menuButtonsInitialized = false;
        private static bool _menuElementsHoverInitialized = false;

        [HarmonyPatch(typeof(MenuHolder), "Start")]
        [HarmonyPostfix]
        public static void Start(MenuHolder __instance) => HandleFocus(ref __instance);

        [HarmonyPatch(typeof(MenuHolder), "Update")]
        [HarmonyPrefix]
        public static void Update_Prefix(MenuHolder __instance) => HandleFocus(ref __instance);

        private static void HandleFocus(ref MenuHolder instance, bool forceUpdate = false)
        {
            if(!_menuButtonsInitialized)
                _menuButtonsInitialized = Initialize<MenuButton, NoBackgroundHover_Stub>(ref instance);
            if(!_menuElementsHoverInitialized)
                _menuElementsHoverInitialized = Initialize<MenuElementHover, NoBackgroundHover_Stub>(ref instance, false);

            if (!_menuButtonsInitialized || !_menuElementsHoverInitialized)
                return;

            _focused = IsGameFocused();

            if ((_focused != _lastFocused) || forceUpdate)
            {
                foreach (var child in instance.GetComponentsInChildren<MenuElementHover>())
                {
                    child.enabled = _focused;
                }
                foreach (var child in instance.GetComponentsInChildren<MenuButton>())
                {
                    child.enabled = _focused;
                }

                _lastFocused = _focused;
            }
        }

        private static bool Initialize<TFind, TTarget>(ref MenuHolder instance, bool zeroCheck = true)
            where TFind : Component 
        {
            int initializedCount = 0;
            int childCount = instance.GetComponentsInChildren<TFind>().Length;

            foreach (var child in instance.GetComponentsInChildren<TFind>())
            {
                GameObject childObject = child.gameObject;

                var stubComponent = childObject.GetComponent<NoBackgroundHover_Stub>();
                if (!stubComponent)
                    stubComponent = childObject.AddComponent<NoBackgroundHover_Stub>();

                if (stubComponent.IsInitialized())
                    initializedCount++;
            }

            return initializedCount == childCount && ((zeroCheck && childCount != 0) || !zeroCheck);
        }
    }

    [HarmonyPatch(typeof(MenuButton))]
    class NoBackgroundHover_Stub : MonoBehaviour
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private static bool IsGameFocused() => GetForegroundWindow() == GetActiveWindow();

        private static bool _initialized = false;
        public bool IsInitialized() => _initialized;

        [HarmonyPatch(typeof(MenuButton), "Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(MenuButton __instance)
        {
            __instance.enabled = IsGameFocused();
            _initialized = true;
        }
    }
}
