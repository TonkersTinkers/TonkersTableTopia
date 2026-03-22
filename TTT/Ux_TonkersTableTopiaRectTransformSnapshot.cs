using UnityEngine;

internal struct Ux_TonkersTableTopiaRectTransformSnapshot
{
    public bool isValid;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 pivot;
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta;
    public Vector2 offsetMin;
    public Vector2 offsetMax;

    public static Ux_TonkersTableTopiaRectTransformSnapshot Capture(RectTransform rt)
    {
        if (rt == null)
        {
            return default;
        }

        return new Ux_TonkersTableTopiaRectTransformSnapshot
        {
            isValid = true,
            anchorMin = rt.anchorMin,
            anchorMax = rt.anchorMax,
            pivot = rt.pivot,
            anchoredPosition = rt.anchoredPosition,
            sizeDelta = rt.sizeDelta,
            offsetMin = rt.offsetMin,
            offsetMax = rt.offsetMax
        };
    }

    public void Restore(RectTransform rt)
    {
        if (!isValid || rt == null)
        {
            return;
        }

        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.sizeDelta = sizeDelta;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        rt.anchoredPosition = anchoredPosition;
    }
}