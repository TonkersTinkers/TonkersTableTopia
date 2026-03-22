using System;
using UnityEngine;
using static Ux_TonkersTableTopiaEditorExtensions;
using static Ux_TonkersTableTopiaExtensionInternals;

public static class Ux_TonkersTableTopiaLayoutSizingExtensions
{
    public static float CalcShrinkyDinkWidthLikeDietCoke(int buttonCount, float min = 44f, float max = 120f, float gap = 4f, float padding = 32f)
    {
        if (buttonCount < 1)
        {
            return min;
        }

        float availableWidth = GetAvailableWidth(padding, 0f);
        float width = (availableWidth - gap * Mathf.Max(0, buttonCount - 1)) / buttonCount;
        return Mathf.Clamp(width, min, max);
    }

    public static float CalcShrinkyDinkWidthLikeDietCokeSquisher(int buttonCount, float occupiedWidth, float max = 160f, float gap = 4f, float padding = 32f)
    {
        if (buttonCount < 1)
        {
            return 1f;
        }

        float availableWidth = GetAvailableWidth(padding, Mathf.Max(0f, occupiedWidth));
        float width = (availableWidth - gap * Mathf.Max(0, buttonCount - 1)) / buttonCount;
        return Mathf.Clamp(width, 1f, max);
    }

    public static float[] ComputeRowPercentagesLikeASpreadsheet(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return Array.Empty<float>();
        }

        RectTransform rectTransform = table.GetComponent<RectTransform>();
        float innerHeight = Mathf.Max(0f, rectTransform.rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);
        int rowCount = Mathf.Max(1, table.totalRowsCountLetTheShowBegin);

        return Ux_TonkersTableTopiaDistributionUtility.ComputeNormalizedPercentages(
            rowCount,
            i =>
            {
                float raw = i < table.snazzyRowWardrobes.Count ? table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f;
                return table.ResolveRowSpecForCurrentInnerHeightLikeBlueprint(raw, innerHeight);
            },
            table.sociallyDistancedRowsPixels,
            innerHeight);
    }

    public static float[] ComputeColumnPercentagesLikeASpreadsheet(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return Array.Empty<float>();
        }

        RectTransform rectTransform = table.GetComponent<RectTransform>();
        float innerWidth = Mathf.Max(0f, rectTransform.rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);
        int columnCount = Mathf.Max(1, table.totalColumnsCountHighFive);

        return Ux_TonkersTableTopiaDistributionUtility.ComputeNormalizedPercentages(
            columnCount,
            i =>
            {
                float raw = i < table.fancyColumnWardrobes.Count ? table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f;
                return table.ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(raw, innerWidth);
            },
            table.sociallyDistancedColumnsPixels,
            innerWidth);
    }

    public static void DistributeLikeACatererInto(int count, Func<int, float> getSpec, float spacing, float inner, ref float[] result)
    {
        Ux_TonkersTableTopiaDistributionUtility.DistributeInto(count, getSpec, spacing, inner, ref result);
    }

    public static float[] DistributeLikeACaterer(int count, Func<int, float> getSpec, float spacing, float inner)
    {
        return Ux_TonkersTableTopiaDistributionUtility.Distribute(count, getSpec, spacing, inner);
    }

    public static void DistributeAllColumnsEvenlyLikeAShortOrderCook(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        table.SyncColumnWardrobes();
        float evenPercent = table.totalColumnsCountHighFive > 0 ? 1f / table.totalColumnsCountHighFive : 1f;

        for (int c = 0; c < table.totalColumnsCountHighFive; c++)
        {
            table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -evenPercent;
        }

        table.shareThePieEvenlyForColumns = false;
        table.FlagLayoutAsNeedingSpaDay();
    }

    public static void DistributeAllRowsEvenlyLikeAShortOrderCook(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        table.SyncRowWardrobes();
        float evenPercent = table.totalRowsCountLetTheShowBegin > 0 ? 1f / table.totalRowsCountLetTheShowBegin : 1f;

        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -evenPercent;
        }

        table.shareThePieEvenlyForRows = false;
        table.FlagLayoutAsNeedingSpaDay();
    }

    public static void SplitTwoColumnsEvenlyLikePeas(this Ux_TonkersTableTopiaLayout table, int leftIndex)
    {
        if (table == null)
        {
            return;
        }

        int rightIndex = leftIndex + 1;

        if (leftIndex < 0 || rightIndex >= table.totalColumnsCountHighFive)
        {
            return;
        }

        table.ConvertAllSpecsToPercentages();
        table.SyncColumnWardrobes();

        float[] percentages = table.ComputeColumnPercentagesLikeASpreadsheet();

        if (percentages.Length <= rightIndex)
        {
            return;
        }

        float half = Mathf.Clamp01(percentages[leftIndex] + percentages[rightIndex]) * 0.5f;
        table.fancyColumnWardrobes[leftIndex].requestedWidthMaybePercentIfNegative = -half;
        table.fancyColumnWardrobes[rightIndex].requestedWidthMaybePercentIfNegative = -half;
        table.FlagLayoutAsNeedingSpaDay();
    }

    public static void SplitTwoRowsEvenlyLikePeas(this Ux_TonkersTableTopiaLayout table, int topIndex)
    {
        if (table == null)
        {
            return;
        }

        int bottomIndex = topIndex + 1;

        if (topIndex < 0 || bottomIndex >= table.totalRowsCountLetTheShowBegin)
        {
            return;
        }

        table.ConvertAllSpecsToPercentages();
        table.SyncRowWardrobes();

        float[] percentages = table.ComputeRowPercentagesLikeASpreadsheet();

        if (percentages.Length <= bottomIndex)
        {
            return;
        }

        float half = Mathf.Clamp01(percentages[topIndex] + percentages[bottomIndex]) * 0.5f;
        table.snazzyRowWardrobes[topIndex].requestedHeightMaybePercentIfNegative = -half;
        table.snazzyRowWardrobes[bottomIndex].requestedHeightMaybePercentIfNegative = -half;
        table.FlagLayoutAsNeedingSpaDay();
    }

    public static void NormalizeWardrobePercentsToOneDadBod(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        Ux_TonkersTableTopiaDistributionUtility.NormalizeNegativePercentageSpecs(
            table.fancyColumnWardrobes.Count,
            i => table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative,
            (i, value) => table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = value);

        Ux_TonkersTableTopiaDistributionUtility.NormalizeNegativePercentageSpecs(
            table.snazzyRowWardrobes.Count,
            i => table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative,
            (i, value) => table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = value);
    }

    public static void RebalanceTwoPercentsLikeSeesaw(ref float leftPct, ref float rightPct, float deltaPct, float floor)
    {
        Ux_TonkersTableTopiaDistributionUtility.RebalancePair(ref leftPct, ref rightPct, deltaPct, floor);
    }

    public static Vector2 GetCanvasScaleLikeDadBod(this Ux_TonkersTableTopiaLayout table)
    {
        Canvas canvas = table != null ? table.GetComponentInParent<Canvas>() : null;
        Transform transform = canvas != null ? canvas.transform : (table != null ? table.transform : null);

        if (transform == null)
        {
            return Vector2.one;
        }

        Vector3 scale = transform.lossyScale;
        return new Vector2(scale.x, scale.y);
    }

    public static bool StampAndCheckIfChangedLikePassport(this Ux_TonkersTableTopiaLayout table, RectTransform rectTransform)
    {
        if (table == null || rectTransform == null)
        {
            return false;
        }

        LayoutState state = GetLayoutState(table);
        Vector2 currentScale = table.GetCanvasScaleLikeDadBod();
        Vector2 currentSize = rectTransform.rect.size;

        bool scaleChanged = !NearlyEqual(currentScale, state.lastCanvasScale);
        bool sizeChanged = !NearlyEqual(currentSize, state.lastTableSize);

        state.lastCanvasScale = currentScale;
        state.lastTableSize = currentSize;

        return scaleChanged || sizeChanged;
    }

    public static void DeferSpaDayToNextFrameLikeABarber(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        LayoutState state = GetLayoutState(table);

        if (state.pendingDeferral)
        {
            return;
        }

        state.pendingDeferral = true;

#if UNITY_EDITOR
        if (!Application.isPlaying || !table.isActiveAndEnabled)
        {
            DeferEditorSafe(() =>
            {
                if (table == null)
                {
                    return;
                }

                LayoutState safeState = GetLayoutState(table);
                safeState.pendingDeferral = false;
                table.FlagLayoutAsNeedingSpaDay();
            });

            return;
        }
#endif

        if (!table.isActiveAndEnabled)
        {
            state.pendingDeferral = false;
            return;
        }

        if (!deferredSpaDayLayouts.Contains(table))
        {
            deferredSpaDayLayouts.Add(table);
        }

        if (deferredSpaDayHooked)
        {
            return;
        }

        deferredSpaDayHooked = true;
        Canvas.willRenderCanvases -= FlushDeferredSpaDaysLikeBarbershop;
        Canvas.willRenderCanvases += FlushDeferredSpaDaysLikeBarbershop;
    }

    public static void SetColumnPercentageAndRebalanceOthersLikeASpreadsheet(this Ux_TonkersTableTopiaLayout table, int index, float percent)
    {
        if (table == null)
        {
            return;
        }

        table.SyncColumnWardrobes();

        if (table.totalColumnsCountHighFive < 1 || table.fancyColumnWardrobes.Count < 1)
        {
            return;
        }

        SetPercentageAndRebalance(
            table,
            table.totalColumnsCountHighFive,
            index,
            percent,
            i => table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative,
            (i, value) => table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = value,
            t => t.shareThePieEvenlyForColumns = false);
    }

    public static void SetRowPercentageAndRebalanceOthersLikeASpreadsheet(this Ux_TonkersTableTopiaLayout table, int index, float percent)
    {
        if (table == null)
        {
            return;
        }

        table.SyncRowWardrobes();

        if (table.totalRowsCountLetTheShowBegin < 1 || table.snazzyRowWardrobes.Count < 1)
        {
            return;
        }

        SetPercentageAndRebalance(
            table,
            table.totalRowsCountLetTheShowBegin,
            index,
            percent,
            i => table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative,
            (i, value) => table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = value,
            t => t.shareThePieEvenlyForRows = false);
    }

    private static float GetAvailableWidth(float padding, float occupiedWidth)
    {
#if UNITY_EDITOR
        float totalWidth = UnityEditor.EditorGUIUtility.currentViewWidth;
#else
        float totalWidth = Screen.width;
#endif
        return Mathf.Max(1f, totalWidth - padding - occupiedWidth);
    }

    private static void SetPercentageAndRebalance(
        Ux_TonkersTableTopiaLayout table,
        int count,
        int index,
        float percent,
        Func<int, float> getRequested,
        Action<int, float> setRequested,
        Action<Ux_TonkersTableTopiaLayout> disableEvenDistribution)
    {
        index = Mathf.Clamp(index, 0, Mathf.Max(0, count - 1));
        percent = Mathf.Clamp01(percent);

        float current = getRequested(index);
        current = current < 0f ? Mathf.Clamp01(-current) : 0f;

        float delta = percent - current;
        float sumOthers = 0f;

        for (int i = 0; i < count; i++)
        {
            if (i == index)
            {
                continue;
            }

            float other = getRequested(i);

            if (other < 0f)
            {
                sumOthers += Mathf.Clamp01(-other);
            }
        }

        float scale = sumOthers > 0.0001f ? Mathf.Clamp01((sumOthers - delta) / sumOthers) : 1f;

        setRequested(index, percent > 0f ? -percent : 0f);

        for (int i = 0; i < count; i++)
        {
            if (i == index)
            {
                continue;
            }

            float other = getRequested(i);

            if (other >= 0f)
            {
                continue;
            }

            setRequested(i, -Mathf.Clamp01((-other) * scale));
        }

        disableEvenDistribution(table);
        table.FlagLayoutAsNeedingSpaDay();
    }

    private static void FlushDeferredSpaDaysLikeBarbershop()
    {
        for (int i = 0; i < deferredSpaDayLayouts.Count; i++)
        {
            Ux_TonkersTableTopiaLayout layout = deferredSpaDayLayouts[i];

            if (layout == null)
            {
                continue;
            }

            LayoutState state = GetLayoutState(layout);
            state.pendingDeferral = false;

            if (layout.isActiveAndEnabled)
            {
                layout.FlagLayoutAsNeedingSpaDay();
            }
        }

        deferredSpaDayLayouts.Clear();
        Canvas.willRenderCanvases -= FlushDeferredSpaDaysLikeBarbershop;
        deferredSpaDayHooked = false;
    }
}