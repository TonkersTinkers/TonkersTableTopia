using UnityEngine;
using UnityEngine.UI;

internal static class Ux_TonkersTableTopiaAlignmentUtility
{
    public static bool TryDetectCellAlignment(
        Ux_TonkersTableTopiaLayout table,
        int row,
        int col,
        out bool isFullStretch,
        out Ux_TonkersTableTopiaLayout.HorizontalAlignment horizontal,
        out Ux_TonkersTableTopiaLayout.VerticalAlignment vertical)
    {
        isFullStretch = false;
        horizontal = Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;
        vertical = Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;

        if (table == null)
        {
            return false;
        }

        RectTransform cell = table.FetchCellRectTransformVIP(row, col);
        RectTransform child = Ux_TonkersTableTopiaForeignContentUtility.GetFirstForeignRect(cell);
        if (child == null)
        {
            return false;
        }

        if (Ux_TonkersTableTopiaRectTransformUtility.IsFullStretch(child))
        {
            isFullStretch = true;
            return true;
        }

        Vector2 point = Ux_TonkersTableTopiaRectTransformUtility.NearlyEqualVector2(child.anchorMin, child.anchorMax)
            ? child.anchorMin
            : (child.anchorMin + child.anchorMax) * 0.5f;

        horizontal = point.x < 0.3334f
            ? Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left
            : point.x > 0.6666f
                ? Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right
                : Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;

        vertical = point.y < 0.3334f
            ? Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom
            : point.y > 0.6666f
                ? Ux_TonkersTableTopiaLayout.VerticalAlignment.Top
                : Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;

        return true;
    }

    public static void AlignForeignersInRect(
        RectTransform parent,
        Ux_TonkersTableTopiaLayout.HorizontalAlignment horizontal,
        Ux_TonkersTableTopiaLayout.VerticalAlignment vertical,
        bool alignTextsToo = true)
    {
        if (parent == null)
        {
            return;
        }

        Rect parentRect = parent.rect;
        float leftPadding = 0f;
        float rightPadding = 0f;
        float topPadding = 0f;
        float bottomPadding = 0f;

        Ux_TonkersTableTopiaCell cell = parent.GetComponent<Ux_TonkersTableTopiaCell>();
        if (cell != null)
        {
            cell.TryPeekInnerPaddingLikePillowFort(out leftPadding, out rightPadding, out topPadding, out bottomPadding);
        }

        float xMin = leftPadding;
        float xMax = parentRect.width - rightPadding;
        float yMin = bottomPadding;
        float yMax = parentRect.height - topPadding;
        float anchorX = GetHorizontalAnchor(horizontal);
        float anchorY = GetVerticalAnchor(vertical);

        for (int i = 0; i < parent.childCount; i++)
        {
            if (!Ux_TonkersTableTopiaForeignContentUtility.TryGetForeignRect(parent.GetChild(i), true, out RectTransform child))
            {
                continue;
            }

            child.anchorMin = new Vector2(anchorX, anchorY);
            child.anchorMax = new Vector2(anchorX, anchorY);

            Vector2 pivot = child.pivot;
            pivot.x = anchorX;
            pivot.y = anchorY;
            child.pivot = pivot;

            Ux_TonkersTableTopiaRectTransformUtility.EnsureReasonableSize(child, parent);

            Rect childRect = child.rect;
            float targetX = anchorX == 0f
                ? xMin + childRect.width * child.pivot.x
                : anchorX == 0.5f
                    ? (xMin + xMax) * 0.5f
                    : xMax - childRect.width * (1f - child.pivot.x);

            float targetY = anchorY == 0f
                ? yMin + childRect.height * child.pivot.y
                : anchorY == 0.5f
                    ? (yMin + yMax) * 0.5f
                    : yMax - childRect.height * (1f - child.pivot.y);

            child.anchoredPosition = new Vector2(
                targetX - anchorX * parentRect.width,
                targetY - anchorY * parentRect.height);
            child.anchoredPosition3D = new Vector3(child.anchoredPosition.x, child.anchoredPosition.y, 0f);

            if (!alignTextsToo)
            {
                continue;
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                text.alignment = MapToTextAnchor(horizontal, vertical);
            }
        }
    }

    public static void AlignForeignersToFill(RectTransform parent, bool alignTextsToo = true)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            if (!Ux_TonkersTableTopiaForeignContentUtility.TryGetForeignRect(parent.GetChild(i), true, out RectTransform child))
            {
                continue;
            }

            Ux_TonkersTableTopiaRectTransformUtility.SnapToFillParent(child);

            if (!alignTextsToo)
            {
                continue;
            }

            Text text = child.GetComponent<Text>();
            if (text != null)
            {
                text.alignment = TextAnchor.MiddleCenter;
            }
        }
    }

    public static Vector2 GuessForeignAnchor(Ux_TonkersTableTopiaLayout table, int row, int col, out bool fullStretch)
    {
        fullStretch = false;

        if (table == null)
        {
            return new Vector2(0.5f, 0.5f);
        }

        RectTransform cell = table.FetchCellRectTransformVIP(row, col);
        RectTransform child = Ux_TonkersTableTopiaForeignContentUtility.GetFirstForeignRect(cell);
        if (child == null)
        {
            return new Vector2(0.5f, 0.5f);
        }

        if (Ux_TonkersTableTopiaRectTransformUtility.IsFullStretch(child))
        {
            fullStretch = true;
            return new Vector2(0.5f, 0.5f);
        }

        if (Ux_TonkersTableTopiaRectTransformUtility.NearlyEqualVector2(child.anchorMin, child.anchorMax))
        {
            return child.anchorMin;
        }

        return (child.anchorMin + child.anchorMax) * 0.5f;
    }

    public static float GetHorizontalAnchor(Ux_TonkersTableTopiaLayout.HorizontalAlignment horizontal)
    {
        switch (horizontal)
        {
            case Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left:
                return 0f;

            case Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right:
                return 1f;

            default:
                return 0.5f;
        }
    }

    public static float GetVerticalAnchor(Ux_TonkersTableTopiaLayout.VerticalAlignment vertical)
    {
        switch (vertical)
        {
            case Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom:
                return 0f;

            case Ux_TonkersTableTopiaLayout.VerticalAlignment.Top:
                return 1f;

            default:
                return 0.5f;
        }
    }

    public static void ApplyHorizontalAnchor(RectTransform child, float anchorX)
    {
        if (child == null)
        {
            return;
        }

        Vector2 anchorMin = child.anchorMin;
        Vector2 anchorMax = child.anchorMax;
        anchorMin.x = anchorX;
        anchorMax.x = anchorX;
        child.anchorMin = anchorMin;
        child.anchorMax = anchorMax;

        Vector2 pivot = child.pivot;
        pivot.x = anchorX;
        child.pivot = pivot;

        Vector2 position = child.anchoredPosition;
        position.x = 0f;
        child.anchoredPosition = position;
    }

    public static void ApplyVerticalAnchor(RectTransform child, float anchorY)
    {
        if (child == null)
        {
            return;
        }

        Vector2 anchorMin = child.anchorMin;
        Vector2 anchorMax = child.anchorMax;
        anchorMin.y = anchorY;
        anchorMax.y = anchorY;
        child.anchorMin = anchorMin;
        child.anchorMax = anchorMax;

        Vector2 pivot = child.pivot;
        pivot.y = anchorY;
        child.pivot = pivot;

        Vector2 position = child.anchoredPosition;
        position.y = 0f;
        child.anchoredPosition = position;
    }

    public static TextAnchor WithHorizontalChanged(TextAnchor current, Ux_TonkersTableTopiaLayout.HorizontalAlignment horizontal)
    {
        bool isTop = current == TextAnchor.UpperLeft || current == TextAnchor.UpperCenter || current == TextAnchor.UpperRight;
        bool isMiddle = current == TextAnchor.MiddleLeft || current == TextAnchor.MiddleCenter || current == TextAnchor.MiddleRight;

        if (isTop)
        {
            return horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left
                ? TextAnchor.UpperLeft
                : horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center
                    ? TextAnchor.UpperCenter
                    : TextAnchor.UpperRight;
        }

        if (isMiddle)
        {
            return horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left
                ? TextAnchor.MiddleLeft
                : horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center
                    ? TextAnchor.MiddleCenter
                    : TextAnchor.MiddleRight;
        }

        return horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left
            ? TextAnchor.LowerLeft
            : horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center
                ? TextAnchor.LowerCenter
                : TextAnchor.LowerRight;
    }

    public static TextAnchor WithVerticalChanged(TextAnchor current, Ux_TonkersTableTopiaLayout.VerticalAlignment vertical)
    {
        bool isLeft = current == TextAnchor.UpperLeft || current == TextAnchor.MiddleLeft || current == TextAnchor.LowerLeft;
        bool isCenter = current == TextAnchor.UpperCenter || current == TextAnchor.MiddleCenter || current == TextAnchor.LowerCenter;

        if (vertical == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top)
        {
            return isLeft ? TextAnchor.UpperLeft : isCenter ? TextAnchor.UpperCenter : TextAnchor.UpperRight;
        }

        if (vertical == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle)
        {
            return isLeft ? TextAnchor.MiddleLeft : isCenter ? TextAnchor.MiddleCenter : TextAnchor.MiddleRight;
        }

        return isLeft ? TextAnchor.LowerLeft : isCenter ? TextAnchor.LowerCenter : TextAnchor.LowerRight;
    }

    private static TextAnchor MapToTextAnchor(
        Ux_TonkersTableTopiaLayout.HorizontalAlignment horizontal,
        Ux_TonkersTableTopiaLayout.VerticalAlignment vertical)
    {
        if (vertical == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top)
        {
            if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left)
            {
                return TextAnchor.UpperLeft;
            }

            if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center)
            {
                return TextAnchor.UpperCenter;
            }

            return TextAnchor.UpperRight;
        }

        if (vertical == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle)
        {
            if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left)
            {
                return TextAnchor.MiddleLeft;
            }

            if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center)
            {
                return TextAnchor.MiddleCenter;
            }

            return TextAnchor.MiddleRight;
        }

        if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left)
        {
            return TextAnchor.LowerLeft;
        }

        if (horizontal == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center)
        {
            return TextAnchor.LowerCenter;
        }

        return TextAnchor.LowerRight;
    }
}