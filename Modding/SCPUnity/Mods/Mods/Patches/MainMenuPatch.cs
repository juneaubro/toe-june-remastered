using HarmonyLib;
using Rewired.UI.ControlMapper;
using UnityEngine;
using UnityEngine.UI;

//+ MainMenu.mainMenuButtons
//     - Components
//             + Button, Rect
//     - Children (GameObjects)
//             + ButtonStack
//                     - Components
//                             + RectTransform, VecticalLayoutGroup
//                     - Children (GameObjects)
//                             + QuickCustomStack, newGameButton, loadGameButton, optionsButton, extrasButton
//             + ExitButton
//                     - Components
//                             + RectTransform, CanvasRenderer, Image, Button
//                     - Children (GameObjects)
//                             + ExitText

namespace Mods.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        private static GameObject _newText;
        private static Text _text;
        private static bool _textSet = false;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        static void Awake(MainMenu __instance)
        {
            if (!_textSet)
            {
                _textSet = true;

                _newText = new GameObject("newText");
                _text = _newText.AddComponent<Text>();
                _text.text = "Test";
                _newText.transform.SetParent(__instance.transform);
                Debug.Log("_newText parent was set\n\n\n\n\n\n\n");


                Button button = __instance.mainMenuButtons.AddComponent<Button>();
                ColorBlock colorBlock = new ColorBlock
                {
                    normalColor = Color.blue,
                    pressedColor = Color.yellow,
                    highlightedColor = Color.cyan
                };
                button.colors = colorBlock;
                button.transform.SetParent(__instance.transform);
            }

            Component[] components = __instance.mainMenuButtons.GetComponentsInChildren(typeof(Component));

            for (int i = 0; i < __instance.mainMenuButtons.transform.childCount; i++)
            {
                GameObject g = __instance.mainMenuButtons.transform.GetChild(i).gameObject;
                Debug.Log(g);

                for (int j = 0; j < g.transform.childCount; j++)
                {
                    GameObject go = g.transform.GetChild(j).gameObject;
                    Debug.Log($"\t\t{go}");
                }
            }
        }
    }
}
