using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

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

            //Component[] components = __instance.mainMenuButtons.GetComponentsInChildren(typeof(Component));

            //for (int i = 0; i < __instance.mainMenuButtons.transform.childCount; i++) // ButtonStack, ExitButton
            //{
            //    GameObject g = __instance.mainMenuButtons.transform.GetChild(i).gameObject;
            //    Debug.Log(g);

            //    Debug.Log("\tChildren: ");
            //    for (int j = 0; j < g.transform.childCount; j++) // QuickCustomStack, newGameButton, loadGameButton, optionsButton, extrasButton, ExitText
            //    {
            //        GameObject go = g.transform.GetChild(j).gameObject;
            //        Debug.Log($"\t\t{go}");

            //        components = go.GetComponents<Component>();
            //        Debug.Log("\t\t\tComponents: ");
            //        foreach (Component c in components)
            //        {
            //            Debug.Log($"\t\t\t\t{c}");
            //        }

            //        Debug.Log("\t\t\tChildren: ");
            //        for (int k = 0; k < go.transform.childCount; k++)
            //        {
            //            GameObject gob = go.transform.GetChild(k).gameObject;
            //            Debug.Log($"\t\t\t\t{gob}");
            //        }

            //        Debug.Log("-----------------------------------------------------");
            //    }
            //}
        }
    }
}
