using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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

            Helpers.PrintGameObjectInfo(__instance.mainMenuButtons, true);
            Helpers.Print("OPAISJDFOIASJFGIOAWERTOIUEWAT", true);
        }
    }
}

// this hierarchy was taking too long to list out and
// is why i made the best damn logging tool i have made yet in Helpers.Print()
/* MAIN MENU HIERARCHY
MainMenu
    + mainMenuButtons
        - Components
            + Button, Rect
        - Children (GameObjects)
            + ButtonStack
                - Components
                    + RectTransform, VecticalLayoutGroup
                - Children (GameObjects)
                    + QuickCustomStack
                        - Components
                            + RectTransform, HorizontalLayerGroup, LayoutElement
                        - Children (GameObjects)
                            + quickGameButton
                    + newGameButton
                        - Components
                            + RectTransform, CanvasRenderer, Image, Button
                        - Children (GameObjects)
                            + newGameText
                    + loadGameButton
                        - Components
                            + RectTransform, CanvasRenderer, Image, Button
                        - Children (GameObjects)
                            + loadGameText
                    + optionsButton
                        - Components
                            + RectTransform, CanvasRenderer, Image, Button
                        - Children (GameObjects)
                            + optionsText
                    + extrasButton
                        - Components
                            + RectTransform, CanvasRenderer, Image, Button
                        - Children (GameObjects)
                            + extrasText
            + ExitButton
                - Components
                    + RectTransform, CanvasRenderer, Image, Button
                - Children (GameObjects)
                    + ExitText
                        - Components
                            + RectTransform, CanvasRenderer, TextMeshProUGUI, I2.Loc.Localize
                        - Children (GameObjects)
                            + TMP SubMeshUI [TitilliumWeb-Regular SDF Material + LiberationSans-Regular SDF Atlas]
*/