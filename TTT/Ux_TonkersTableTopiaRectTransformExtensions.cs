using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Ux_TonkersTableTopiaExtensionInternals;

public static class Ux_TonkersTableTopiaRectTransformExtensions
{
    public static bool IsFullStretchLikeYoga(this RectTransform rectTransform)
    {
        return Ux_TonkersTableTopiaRectTransformUtility.IsFullStretch(rectTransform);
    }

    public static void ResizeAndReanchorLikeAChamp(this RectTransform child, float whoAteMyWidth, float whoAteMyHeight)
    {
        Ux_TonkersTableTopiaRectTransformUtility.ResizeAndReanchor(child, whoAteMyWidth, whoAteMyHeight);
    }

    public static void ScaleForeignKidsLikeStretchPants(this RectTransform parent, float scaleX, float scaleY)
    {
        Ux_TonkersTableTopiaRectTransformUtility.ScaleForeignChildren(parent, scaleX, scaleY);
    }

    public static void ScaleForeignKidsToFitNewParentSizeLikeDadJeans(this RectTransform parent, Vector2 oldParentSize, Vector2 newParentSize)
    {
        Ux_TonkersTableTopiaRectTransformUtility.ScaleForeignChildrenToParentSize(parent, oldParentSize, newParentSize);
    }

    public static void SetAnchorsByPercentLikeABoss(this RectTransform rectTransform, float widthPercent, float heightPercent)
    {
        Ux_TonkersTableTopiaRectTransformUtility.SetAnchorsByPercent(rectTransform, widthPercent, heightPercent);
    }

    public static void SetPixelSizeLikeIts1999(this RectTransform rectTransform, float width, float height)
    {
        Ux_TonkersTableTopiaRectTransformUtility.SetPixelSize(rectTransform, width, height);
    }

    public static RectTransform SnapCroutonToFillParentLikeGravy(this RectTransform rectTransform)
    {
        return Ux_TonkersTableTopiaRectTransformUtility.SnapToFillParent(rectTransform);
    }

    public static Image FlipImageComponentLikeALightSwitch(this RectTransform rectTransform, bool needIt)
    {
        return Ux_TonkersTableTopiaRectTransformUtility.SetImageEnabled(rectTransform, needIt);
    }

    public static GameObject SpawnUiChildFillingAndCenteredLikeABurrito(this RectTransform parent, string name, System.Action<GameObject> configure)
    {
        if (parent == null)
        {
            return null;
        }

        GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(name);
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        Ux_TonkersTableTopiaObjectUtility.SetParent(rectTransform, parent, "Add UI");
        rectTransform.SnapCroutonToFillParentLikeGravy();
        configure?.Invoke(go);
        Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(go, "Add UI");

        return go;
    }

    public static void StretchWidthHugHeightLikeYoga(this RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;

        anchorMin.x = 0f;
        anchorMax.x = 1f;
        anchorMin.y = 0.5f;
        anchorMax.y = 0.5f;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        Vector2 offsetMin = rectTransform.offsetMin;
        Vector2 offsetMax = rectTransform.offsetMax;

        offsetMin.x = 0f;
        offsetMax.x = 0f;

        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;
    }

    public static void MakeImageBackgroundNotBlockClicksLikePolite(this RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        Image image = rectTransform.GetComponent<Image>();

        if (image != null)
        {
            image.raycastTarget = false;
        }
    }

    public static GameObject CreateButtonBellyFlop(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Button BellyFlop", child =>
        {
            Image image = child.AddComponent<Image>();
            image.raycastTarget = true;

            Button button = child.AddComponent<Button>();
            button.targetGraphic = image;
        });

        if (go == null)
        {
            return null;
        }

        Text label = CreateTextChild(go.GetComponent<RectTransform>(), "Text", "Button", TextAnchor.MiddleCenter, false);
        label.rectTransform.SnapCroutonToFillParentLikeGravy();
        return go;
    }

    public static GameObject CreateImageCheeseburger(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Image Cheeseburger", child =>
        {
            Image image = child.AddComponent<Image>();
            image.raycastTarget = true;
        });
    }

    public static GameObject CreateRawImageNachos(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT RawImage Nachos", child =>
        {
            child.AddComponent<RawImage>();
        });
    }

    public static GameObject CreateTextDadJokes(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Text DadJokes", child =>
        {
            Text text = child.AddComponent<Text>();
            text.text = "New Text";
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
        });
    }

    public static GameObject CreateToggleFlipFlop(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Toggle FlipFlop", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();
        Image background = CreateImageChild(root, "Background");
        Image checkmark = CreateImageChild(background.rectTransform, "Checkmark");
        Text label = CreateTextChild(root, "Label", "Toggle", TextAnchor.MiddleLeft, false);

        RectTransform backgroundRect = background.rectTransform;
        backgroundRect.anchorMin = new Vector2(0f, 0.5f);
        backgroundRect.anchorMax = new Vector2(0f, 0.5f);
        backgroundRect.sizeDelta = new Vector2(20f, 20f);
        backgroundRect.anchoredPosition = new Vector2(10f, 0f);

        RectTransform checkmarkRect = checkmark.rectTransform;
        checkmarkRect.anchorMin = Vector2.one * 0.5f;
        checkmarkRect.anchorMax = Vector2.one * 0.5f;
        checkmarkRect.sizeDelta = new Vector2(12f, 12f);

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = new Vector2(0f, 0.5f);
        labelRect.anchorMax = new Vector2(0f, 0.5f);
        labelRect.anchoredPosition = new Vector2(35f, 0f);
        labelRect.sizeDelta = new Vector2(160f, 20f);

        background.color = Color.white;

        Toggle toggle = go.AddComponent<Toggle>();
        toggle.graphic = checkmark;
        toggle.targetGraphic = background;

        return go;
    }

    public static GameObject CreateSliderSlipNSlide(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Slider SlipNSlide", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();
        root.StretchWidthHugHeightLikeYoga();
        root.sizeDelta = new Vector2(0f, 20f);

        Image background = CreateImageChild(root, "Background");
        background.rectTransform.SnapCroutonToFillParentLikeGravy();
        background.raycastTarget = true;

        RectTransform fillArea = CreateRectChild(root, "Fill Area");
        fillArea.anchorMin = new Vector2(0f, 0.25f);
        fillArea.anchorMax = new Vector2(1f, 0.75f);
        fillArea.offsetMin = new Vector2(10f, 0f);
        fillArea.offsetMax = new Vector2(-10f, 0f);

        Image fill = CreateImageChild(fillArea, "Fill");
        fill.rectTransform.anchorMin = new Vector2(0f, 0.25f);
        fill.rectTransform.anchorMax = new Vector2(0.5f, 0.75f);
        fill.raycastTarget = false;

        RectTransform handleSlideArea = CreateRectChild(root, "Handle Slide Area");
        handleSlideArea.anchorMin = Vector2.zero;
        handleSlideArea.anchorMax = Vector2.one;
        handleSlideArea.offsetMin = new Vector2(10f, 0f);
        handleSlideArea.offsetMax = new Vector2(-10f, 0f);

        Image handle = CreateImageChild(handleSlideArea, "Handle");
        handle.rectTransform.anchorMin = Vector2.one * 0.5f;
        handle.rectTransform.anchorMax = Vector2.one * 0.5f;
        handle.rectTransform.sizeDelta = new Vector2(20f, 20f);

        Slider slider = go.AddComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.direction = Slider.Direction.LeftToRight;
        slider.targetGraphic = handle;

        return go;
    }

    public static GameObject CreateScrollbarScoot(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Scrollbar Scoot", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();
        root.StretchWidthHugHeightLikeYoga();
        root.sizeDelta = new Vector2(0f, 20f);

        Image background = CreateImageChild(root, "Background");
        background.rectTransform.SnapCroutonToFillParentLikeGravy();

        Image handle = CreateImageChild(background.rectTransform, "Handle");
        handle.rectTransform.anchorMin = new Vector2(0f, 0f);
        handle.rectTransform.anchorMax = new Vector2(0.2f, 1f);

        Scrollbar scrollbar = go.AddComponent<Scrollbar>();
        scrollbar.targetGraphic = handle;
        scrollbar.handleRect = handle.rectTransform;
        scrollbar.direction = Scrollbar.Direction.LeftToRight;

        return go;
    }

    public static GameObject CreateScrollRectScooter(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT ScrollRect Scooter", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();

        Image viewport = CreateImageChild(root, "Viewport");
        viewport.rectTransform.SnapCroutonToFillParentLikeGravy();

        Mask mask = viewport.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        RectTransform content = CreateRectChild(viewport.rectTransform, "Content");
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);

        ScrollRect scrollRect = go.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport.rectTransform;
        scrollRect.content = content;

        return go;
    }

    public static GameObject CreateInputFieldChattyCathy(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT InputField ChattyCathy", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();
        Image image = go.AddComponent<Image>();
        image.raycastTarget = true;

        Text placeholder = CreateTextChild(root, "Placeholder", "Enter text...", TextAnchor.MiddleLeft, false);
        placeholder.rectTransform.SnapCroutonToFillParentLikeGravy();
        placeholder.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);

        Text text = CreateTextChild(root, "Text", string.Empty, TextAnchor.MiddleLeft, true);
        text.rectTransform.SnapCroutonToFillParentLikeGravy();

        InputField inputField = go.AddComponent<InputField>();
        inputField.placeholder = placeholder;
        inputField.textComponent = text;

        return go;
    }

    public static GameObject CreateDropdownDropItLikeItsHot(this RectTransform parent)
    {
        GameObject go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Dropdown DropIt", null);

        if (go == null)
        {
            return null;
        }

        RectTransform root = go.GetComponent<RectTransform>();
        Image background = go.AddComponent<Image>();
        background.raycastTarget = true;

        Text caption = CreateTextChild(root, "Label", "Option A", TextAnchor.MiddleLeft, false);
        caption.rectTransform.SnapCroutonToFillParentLikeGravy();

        Image arrow = CreateImageChild(root, "Arrow");
        arrow.rectTransform.anchorMin = new Vector2(1f, 0.5f);
        arrow.rectTransform.anchorMax = new Vector2(1f, 0.5f);
        arrow.rectTransform.sizeDelta = new Vector2(20f, 20f);
        arrow.rectTransform.anchoredPosition = new Vector2(-10f, 0f);

        Image template = CreateImageChild(root, "Template");
        template.rectTransform.SnapCroutonToFillParentLikeGravy();
        template.gameObject.SetActive(false);

        Image viewport = CreateImageChild(template.rectTransform, "Viewport");
        viewport.rectTransform.SnapCroutonToFillParentLikeGravy();

        Mask mask = viewport.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        RectTransform content = CreateRectChild(viewport.rectTransform, "Content");
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);

        Toggle item = new GameObject("Item").AddComponent<Toggle>();
        item.transform.SetParent(content, false);

        Image itemBackground = item.gameObject.AddComponent<Image>();
        item.targetGraphic = itemBackground;

        Image itemCheckmark = CreateImageChild(item.transform as RectTransform, "Item Checkmark");
        Text itemLabel = CreateTextChild(item.transform as RectTransform, "Item Label", string.Empty, TextAnchor.MiddleLeft, false);

        ScrollRect scrollRect = template.gameObject.AddComponent<ScrollRect>();
        scrollRect.viewport = viewport.rectTransform;
        scrollRect.content = content;

        Dropdown dropdown = go.AddComponent<Dropdown>();
        dropdown.template = template.rectTransform;
        dropdown.captionText = caption;
        dropdown.itemText = itemLabel;
        dropdown.options = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Option A"),
            new Dropdown.OptionData("Option B"),
            new Dropdown.OptionData("Option C")
        };
        dropdown.value = 0;
        dropdown.RefreshShownValue();

        item.graphic = itemCheckmark;

        return go;
    }

    public static float TallyLabelRowHogWidthLikeSumo(GUIStyle style, params string[] labels)
    {
        style = GetSafeLabelStyle(style);

        float width = 0f;

        if (labels != null)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                string label = labels[i];

                if (string.IsNullOrEmpty(label))
                {
                    continue;
                }

                width += style.CalcSize(new GUIContent(label)).x;
            }
        }

        return width + 12f;
    }

    private static RectTransform CreateRectChild(RectTransform parent, string name)
    {
        RectTransform rectTransform = new GameObject(name).AddComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        return rectTransform;
    }

    private static Image CreateImageChild(RectTransform parent, string name)
    {
        Image image = new GameObject(name).AddComponent<Image>();
        image.rectTransform.SetParent(parent, false);
        return image;
    }

    private static Text CreateTextChild(RectTransform parent, string name, string text, TextAnchor alignment, bool raycastTarget)
    {
        Text textComponent = new GameObject(name).AddComponent<Text>();
        textComponent.rectTransform.SetParent(parent, false);
        textComponent.text = text;
        textComponent.alignment = alignment;
        textComponent.raycastTarget = raycastTarget;
        return textComponent;
    }
}