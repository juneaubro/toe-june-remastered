using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Tayx.Graphy.Utils.NumString;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Mods.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class MainMenuPatch
    {
        private static GameObject buttonStack = null;
        private static bool _buttonCreated = false;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void Start(MainMenu __instance)
        {
            if (!_buttonCreated)
            {
                _buttonCreated = true;

                // Get ButtonStack GameObject
                buttonStack = __instance.mainMenuButtons.transform.GetChild(0).gameObject;

                // Make new GameObject to add to ButtonStack
                GameObject newObject = new GameObject("NewButton");
                GameObject copyObject = buttonStack.transform.GetChild(1).gameObject; // newGameButton
                newObject.transform.localScale = copyObject.transform.localScale;

                // Ensure new GameObject is integrated into the Main Menu buttons
                newObject.transform.SetParent(buttonStack.transform);

                // Copy button style from another button to new GameObject's button
                Button button = newObject.AddComponent<Button>();
                button.transform.SetParent(newObject.transform);
                Button copyButton = copyObject.GetComponent<Button>();
                Button.ButtonClickedEvent newEvent = new Button.ButtonClickedEvent();
                // TODO: insert method to call when clicking button
                newEvent.AddListener(__instance.OnExit);
                button.onClick = newEvent;
                button.animationTriggers = copyButton.animationTriggers;
                button.colors = copyButton.colors;
                button.transition = copyButton.transition;
                button.transform.localScale = copyButton.transform.localScale;
                button.transform.position = new Vector3(copyButton.transform.position.x,
                    button.transform.position.y, copyButton.transform.position.z);

                // Copy image style from another image to new GameObject's image
                Image image = newObject.AddComponent<Image>();
                image.transform.SetParent(newObject.transform);
                Image copyImage = copyObject.GetComponent<Image>();
                image.color = copyImage.color;
                image.alphaHitTestMinimumThreshold = copyImage.alphaHitTestMinimumThreshold;
                image.fillAmount = copyImage.fillAmount;
                image.fillCenter = copyImage.fillCenter;
                image.fillClockwise = copyImage.fillClockwise;
                image.fillMethod = copyImage.fillMethod;
                image.fillOrigin = copyImage.fillOrigin;
                image.material = copyImage.material;
                image.overrideSprite = copyImage.overrideSprite;
                image.pixelsPerUnitMultiplier = copyImage.pixelsPerUnitMultiplier;
                image.preserveAspect = copyImage.preserveAspect;
                image.type = copyImage.type;
                image.sprite = copyImage.sprite;
                image.useSpriteMesh = copyImage.useSpriteMesh;

                // Create sub-object to hold text
                GameObject textObject = new GameObject
                {
                    transform =
                    {
                        localScale = newObject.transform.localScale
                    }
                };
                textObject.transform.SetParent(newObject.transform);

                // Copy text style another other text to new GameObject's text
                TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
                TextMeshProUGUI copyText = copyObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); // newGameText's TextMeshProUGUI
                text.transform.SetParent(textObject.transform);
                text.alignment = copyText.alignment;
                text.font = copyText.font;
                text.fontStyle = copyText.fontStyle;
                text.fontSize = copyText.fontSize;
                text.fontMaterial = copyText.fontMaterial;
                text.fontMaterials = copyText.fontMaterials;
                text.fontSharedMaterial = copyText.fontSharedMaterial;
                text.fontSharedMaterials = copyText.fontSharedMaterials;
                text.fontSizeMax = copyText.fontSizeMax;
                text.fontSizeMin = copyText.fontSizeMin;
                text.fontWeight = copyText.fontWeight;
                text.autoSizeTextContainer = copyText.autoSizeTextContainer;
                text.maskOffset = copyText.maskOffset;
                text.alpha = copyText.alpha;
                text.color = copyText.color;
                text.faceColor = copyText.faceColor;
                text.colorGradient = copyText.colorGradient;
                text.colorGradientPreset = copyText.colorGradientPreset;
                text.margin = copyText.margin;
                text.material = copyText.material;
                text.enableAutoSizing = copyText.enableAutoSizing;
                text.enableCulling = copyText.enableCulling;
                text.enableKerning = copyText.enableKerning;
                text.enableVertexGradient = copyText.enableVertexGradient;
                text.enableWordWrapping = false;
                text.enabled = copyText.enabled;
                text.characterSpacing = copyText.characterSpacing;
                text.extraPadding = copyText.extraPadding;
                text.geometrySortingOrder = copyText.geometrySortingOrder;
                text.havePropertiesChanged = copyText.havePropertiesChanged;
                text.horizontalMapping = copyText.horizontalMapping;
                text.outlineColor = copyText.outlineColor;
                text.transform.localScale = copyText.transform.localScale;
                text.text = "MULTIPLAYER";

                //Helpers.PrintGameObjectInfo(buttonStack, true);
            }
        }
    }
}