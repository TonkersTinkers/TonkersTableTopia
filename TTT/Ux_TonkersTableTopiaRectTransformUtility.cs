using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal static class Ux_TonkersTableTopiaRectTransformUtility
{
    private static readonly Dictionary<RectTransform, Vector2> _lastKnownParentSizes = new Dictionary<RectTransform, Vector2>();

    public static bool NearlyEqualVector2(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) < 0.0001f && Mathf.Abs(a.y - b.y) < 0.0001f;
    }

    public static bool IsFullStretch(RectTransform rt)
    {
        return rt != null && NearlyEqualVector2(rt.anchorMin, Vector2.zero) && NearlyEqualVector2(rt.anchorMax, Vector2.one);
    }

    public static RectTransform SnapToFillParent(RectTransform rt)
    {
        if (rt == null)
        {
            return null;
        }

        RectTransform parentRect = rt.parent as RectTransform;
        float left = 0f;
        float right = 0f;
        float top = 0f;
        float bottom = 0f;

        Ux_TonkersTableTopiaCell cell = parentRect != null ? parentRect.GetComponent<Ux_TonkersTableTopiaCell>() : null;
        if (cell != null)
        {
            cell.TryPeekInnerPaddingLikePillowFort(out left, out right, out top, out bottom);
        }

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
        rt.anchoredPosition = Vector2.zero;
        rt.anchoredPosition3D = new Vector3(0f, 0f, 0f);
        return rt;
    }

    public static void SetPixelSize(RectTransform rt, float width, float height)
    {
        if (rt == null)
        {
            return;
        }

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, width));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, height));
    }

    public static void SetAnchorsByPercent(RectTransform rt, float widthPercent, float heightPercent)
    {
        if (rt == null)
        {
            return;
        }

        widthPercent = Mathf.Clamp01(widthPercent);
        heightPercent = Mathf.Clamp01(heightPercent);

        Vector2 anchorMin = rt.anchorMin;
        Vector2 anchorMax = rt.anchorMax;

        anchorMax.x = Mathf.Clamp01(anchorMin.x + widthPercent);
        anchorMax.y = Mathf.Clamp01(anchorMin.y + heightPercent);

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static void ResizeAndReanchor(RectTransform child, float widthScale, float heightScale)
    {
        if (child == null)
        {
            return;
        }

        Vector2 anchorMin = child.anchorMin;
        Vector2 anchorMax = child.anchorMax;
        bool stretchX = Mathf.Abs(anchorMin.x - anchorMax.x) > 0.001f;
        bool stretchY = Mathf.Abs(anchorMin.y - anchorMax.y) > 0.001f;

        if (stretchX)
        {
            Vector2 offsetMin = child.offsetMin;
            Vector2 offsetMax = child.offsetMax;
            offsetMin.x *= widthScale;
            offsetMax.x *= widthScale;
            child.offsetMin = offsetMin;
            child.offsetMax = offsetMax;
        }
        else
        {
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, child.rect.width * widthScale));
            Vector2 position = child.anchoredPosition;
            position.x *= widthScale;
            child.anchoredPosition = position;
        }

        if (stretchY)
        {
            Vector2 offsetMin = child.offsetMin;
            Vector2 offsetMax = child.offsetMax;
            offsetMin.y *= heightScale;
            offsetMax.y *= heightScale;
            child.offsetMin = offsetMin;
            child.offsetMax = offsetMax;
        }
        else
        {
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, child.rect.height * heightScale));
            Vector2 position = child.anchoredPosition;
            position.y *= heightScale;
            child.anchoredPosition = position;
        }
    }

    public static void ScaleForeignChildren(RectTransform parent, float scaleX, float scaleY)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (!Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(child))
            {
                continue;
            }

            RectTransform childRect = child as RectTransform;
            if (childRect == null || IsFullStretch(childRect))
            {
                continue;
            }

            ResizeAndReanchor(childRect, scaleX, scaleY);
        }
    }

    public static void ScaleForeignChildrenToParentSize(RectTransform parent, Vector2 oldParentSize, Vector2 newParentSize)
    {
        if (parent == null)
        {
            return;
        }

        Vector2 lastNonZeroSize = GetLastNonZeroParentSize(parent);
        float scaleX = GetSafeScale(oldParentSize.x, newParentSize.x, lastNonZeroSize.x);
        float scaleY = GetSafeScale(oldParentSize.y, newParentSize.y, lastNonZeroSize.y);

        ScaleForeignChildren(parent, scaleX, scaleY);
        RememberParentSize(parent, newParentSize);
    }

    public static void EnsureReasonableSize(RectTransform rt, RectTransform parent)
    {
        if (rt == null || parent == null || IsFullStretch(rt))
        {
            return;
        }

        FindPreferredSize(rt, out float preferredWidth, out float preferredHeight);

        float maxWidth = Mathf.Max(1f, parent.rect.width);
        float maxHeight = Mathf.Max(1f, parent.rect.height);
        float width = Mathf.Clamp(preferredWidth, 1f, maxWidth);
        float height = Mathf.Clamp(preferredHeight, 1f, maxHeight);

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static Image SetImageEnabled(RectTransform rt, bool needIt)
    {
        if (rt == null)
        {
            return null;
        }

        Image image = rt.GetComponent<Image>();

        if (needIt)
        {
            if (image == null)
            {
                image = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Image>(rt.gameObject);
            }

            if (image != null)
            {
                image.enabled = true;
            }

            return image;
        }

        if (image != null)
        {
            image.enabled = false;
            image.sprite = null;
            image.raycastTarget = false;
            image.color = Color.white;
        }

        return null;
    }

    private static void FindPreferredSize(RectTransform rt, out float width, out float height)
    {
        width = 0f;
        height = 0f;

        if (rt == null)
        {
            width = 32f;
            height = 32f;
            return;
        }

        try
        {
            width = Mathf.Max(width, LayoutUtility.GetPreferredWidth(rt));
            height = Mathf.Max(height, LayoutUtility.GetPreferredHeight(rt));
        }
        catch
        {
        }

        Image image = rt.GetComponent<Image>();
        if (image != null)
        {
            if (image.sprite != null)
            {
                Vector2 spriteSize = image.sprite.rect.size;
                if (width < 1f)
                {
                    width = spriteSize.x;
                }

                if (height < 1f)
                {
                    height = spriteSize.y;
                }
            }

            if (width < 1f)
            {
                width = 64f;
            }

            if (height < 1f)
            {
                height = 64f;
            }

            return;
        }

        RawImage rawImage = rt.GetComponent<RawImage>();
        if (rawImage != null && rawImage.texture != null)
        {
            if (width < 1f)
            {
                width = rawImage.texture.width;
            }

            if (height < 1f)
            {
                height = rawImage.texture.height;
            }

            return;
        }

        Text text = rt.GetComponent<Text>();
        if (text != null)
        {
            if (width < 1f)
            {
                width = Mathf.Max(48f, text.preferredWidth);
            }

            if (height < 1f)
            {
                height = Mathf.Max(text.fontSize + 6f, text.preferredHeight);
            }

            return;
        }

        if (width < 1f)
        {
            width = Mathf.Max(32f, rt.rect.width);
        }

        if (height < 1f)
        {
            height = Mathf.Max(20f, rt.rect.height);
        }
    }

    private static Vector2 GetLastNonZeroParentSize(RectTransform rt)
    {
        if (rt != null && _lastKnownParentSizes.TryGetValue(rt, out Vector2 value))
        {
            return value;
        }

        return Vector2.zero;
    }

    private static void RememberParentSize(RectTransform rt, Vector2 size)
    {
        if (rt == null)
        {
            return;
        }

        const float epsilon = 0.0001f;
        Vector2 current = GetLastNonZeroParentSize(rt);

        if (size.x >= epsilon)
        {
            current.x = size.x;
        }

        if (size.y >= epsilon)
        {
            current.y = size.y;
        }

        _lastKnownParentSizes[rt] = current;
    }

    private static float GetSafeScale(float oldSize, float newSize, float lastNonZero)
    {
        const float epsilon = 0.0001f;

        if (newSize < epsilon)
        {
            return 1f;
        }

        if (oldSize >= epsilon)
        {
            return newSize / Mathf.Max(epsilon, oldSize);
        }

        if (lastNonZero >= epsilon)
        {
            return newSize / lastNonZero;
        }

        return 1f;
    }
}