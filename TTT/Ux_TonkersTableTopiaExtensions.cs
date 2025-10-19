using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class Ux_TonkersTableTopiaExtensions
{
#if UNITY_EDITOR

    public static void SelectAndPingLikeABeacon(this Object target)
    {
        if (target == null) return;
        UnityEditor.Selection.activeObject = target;
        UnityEditor.EditorGUIUtility.PingObject(target);
    }

#endif

    private static readonly Dictionary<RectTransform, Vector2> _dadBodParentMemo = new Dictionary<RectTransform, Vector2>();

    public static bool BulkDeleteColumnsLikeAChamp(this Ux_TonkersTableTopiaLayout t, int startColInclusive, int endColInclusive)
    {
        if (t == null) return false;
        if (t.totalColumnsCountHighFive <= 1) return false;
        int c0 = Mathf.Clamp(startColInclusive, 0, t.totalColumnsCountHighFive - 1);
        int c1 = Mathf.Clamp(endColInclusive, 0, t.totalColumnsCountHighFive - 1);
        if (c1 < c0)
        {
            var tmp = c0;
            c0 = c1;
            c1 = tmp;
        }
        int want = c1 - c0 + 1;
        if (want >= t.totalColumnsCountHighFive) c1 = c0 + (t.totalColumnsCountHighFive - 2);
        if (c1 < c0) return false;
        for (int c = c0; c <= c1; c++) if (IsColumnDeletionBlockedByMergers(t, c)) return false;
        for (int c = c1; c >= c0; c--) if (!t.TryPolitelyDeleteColumn(c)) return false;
        t.FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public static bool BulkDeleteRowsLikeABoss(this Ux_TonkersTableTopiaLayout t, int startRowInclusive, int endRowInclusive)
    {
        if (t == null) return false;
        if (t.totalRowsCountLetTheShowBegin <= 1) return false;
        int r0 = Mathf.Clamp(startRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        int r1 = Mathf.Clamp(endRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        if (r1 < r0)
        {
            var tmp = r0;
            r0 = r1;
            r1 = tmp;
        }
        int want = r1 - r0 + 1;
        if (want >= t.totalRowsCountLetTheShowBegin) r1 = r0 + (t.totalRowsCountLetTheShowBegin - 2);
        if (r1 < r0) return false;
        for (int r = r0; r <= r1; r++) if (IsRowDeletionBlockedByMergers(t, r)) return false;
        for (int r = r1; r >= r0; r--) if (!t.TryPolitelyDeleteRow(r)) return false;
        t.FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public static bool CanMergeThisPicnicBlanket(this Ux_TonkersTableTopiaLayout t, int startRow, int startCol, int rowCount, int colCount)
    {
        if (t == null) return false;
        if (rowCount < 1 || colCount < 1) return false;
        t.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        t.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        if (rowCount * colCount <= 1) return false;
        if (t.AreAllSeatsOwnedByOneHeadHoncho(startRow, startCol, rowCount, colCount, out var boss))
        {
            if (boss != null && boss.rowNumberWhereThePartyIs == startRow && boss.columnNumberPrimeRib == startCol && Mathf.Max(1, boss.howManyRowsAreHoggingThisSeat) == rowCount && Mathf.Max(1, boss.howManyColumnsAreSneakingIn) == colCount) return false;
        }
        return true;
    }

    public static bool CanUnmergeThisFoodFight(this Ux_TonkersTableTopiaLayout t, int startRow, int startCol, int rowCount, int colCount)
    {
        if (t == null) return false;
        if (rowCount < 1 || colCount < 1) return false;
        t.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        t.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (!t.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out var main)) continue;
                if (main != null && (main.howManyRowsAreHoggingThisSeat > 1 || main.howManyColumnsAreSneakingIn > 1)) return true;
            }
        }
        return false;
    }

    public static void ClampRectToTableLikeASensibleSeatbelt(this Ux_TonkersTableTopiaLayout t, ref int startRow, ref int startCol, ref int rowCount, ref int colCount)
    {
        if (t == null) return;
        int maxRows = Mathf.Max(1, t.totalRowsCountLetTheShowBegin);
        int maxCols = Mathf.Max(1, t.totalColumnsCountHighFive);
        startRow = Mathf.Clamp(startRow, 0, maxRows - 1);
        startCol = Mathf.Clamp(startCol, 0, maxCols - 1);
        int r1 = Mathf.Clamp(startRow + Mathf.Max(1, rowCount) - 1, startRow, maxRows - 1);
        int c1 = Mathf.Clamp(startCol + Mathf.Max(1, colCount) - 1, startCol, maxCols - 1);
        rowCount = r1 - startRow + 1;
        colCount = c1 - startCol + 1;
    }

    public static float[] ComputeRowPercentagesLikeASpreadsheet(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return System.Array.Empty<float>();
        var rt = t.GetComponent<RectTransform>();
        float innerH = Mathf.Max(0f, rt.rect.height - t.comfyPaddingTopHat - t.comfyPaddingBottomCushion);
        int n = Mathf.Max(1, t.totalRowsCountLetTheShowBegin);
        float[] px = DistributeLikeACaterer(n, i => (i < t.snazzyRowWardrobes.Count) ? t.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f, t.sociallyDistancedRowsPixels, innerH);
        float spacing = t.sociallyDistancedRowsPixels * Mathf.Max(0, n - 1);
        float avail = Mathf.Max(1f, innerH - spacing);
        var pct = new float[n];
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            pct[i] = Mathf.Clamp01(px[i] / avail);
            sum += pct[i];
        }
        if (sum > 0f)
        {
            for (int i = 0; i < n; i++) pct[i] /= sum;
        }
        return pct;
    }

    public static float[] ComputeColumnPercentagesLikeASpreadsheet(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return System.Array.Empty<float>();
        var rt = t.GetComponent<RectTransform>();
        float innerW = Mathf.Max(0f, rt.rect.width - t.comfyPaddingLeftForElbows - t.comfyPaddingRightForElbows);
        int n = Mathf.Max(1, t.totalColumnsCountHighFive);
        float[] px = DistributeLikeACaterer(n, i => (i < t.fancyColumnWardrobes.Count) ? t.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f, t.sociallyDistancedColumnsPixels, innerW);
        float spacing = t.sociallyDistancedColumnsPixels * Mathf.Max(0, n - 1);
        float avail = Mathf.Max(1f, innerW - spacing);
        var pct = new float[n];
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            pct[i] = Mathf.Clamp01(px[i] / avail);
            sum += pct[i];
        }
        if (sum > 0f) for (int i = 0; i < n; i++) pct[i] /= sum;
        return pct;
    }

    public static GameObject CreateButtonBellyFlop(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Button BellyFlop", b =>
        {
            var img = b.AddComponent<Image>();
            img.raycastTarget = true;
            var btn = b.AddComponent<Button>();
            btn.targetGraphic = img;
        });
        var rt = go.GetComponent<RectTransform>();
        var label = new GameObject("Text").AddComponent<Text>();
        label.text = "Button";
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        var lrt = label.GetComponent<RectTransform>();
        lrt.SetParent(rt, false);
        lrt.SnapCroutonToFillParentLikeGravy();
        return go;
    }

    public static GameObject CreateImageCheeseburger(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Image Cheeseburger", go =>
        {
            var img = go.AddComponent<Image>();
            img.raycastTarget = true;
        });
    }

    public static GameObject CreateRawImageNachos(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT RawImage Nachos", go =>
        {
            go.AddComponent<RawImage>();
        });
    }

    public static GameObject CreateTextDadJokes(this RectTransform parent)
    {
        return parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Text DadJokes", go =>
        {
            var txt = go.AddComponent<Text>();
            txt.text = "New Text";
            txt.alignment = TextAnchor.MiddleCenter;
            txt.raycastTarget = false;
        });
    }

    private static float[] _dlacFixed;
    private static float[] _dlacPerc;
    private static int[] _dlacFlex;

    private static void EnsureCatererCapacity(int count)
    {
        if (_dlacFixed == null || _dlacFixed.Length < count) System.Array.Resize(ref _dlacFixed, count);
        if (_dlacPerc == null || _dlacPerc.Length < count) System.Array.Resize(ref _dlacPerc, count);
        if (_dlacFlex == null || _dlacFlex.Length < count) System.Array.Resize(ref _dlacFlex, count);
    }

    public static void DistributeLikeACatererInto(int count, System.Func<int, float> getSpec, float spacing, float inner, ref float[] result)
    {
        if (result == null || result.Length != count) result = new float[count];
        EnsureCatererCapacity(count);
        for (int i = 0; i < count; i++)
        {
            _dlacFixed[i] = 0f;
            _dlacPerc[i] = 0f;
            _dlacFlex[i] = 0;
            result[i] = 0f;
        }
        float fixedSum = 0f;
        float percSum = 0f;
        int flexCount = 0;
        for (int i = 0; i < count; i++)
        {
            float v = getSpec != null ? getSpec(i) : 0f;
            if (v > 0f)
            {
                _dlacFixed[i] = v;
                fixedSum += v;
            }
            else if (v < 0f)
            {
                float p = Mathf.Clamp01(-v);
                _dlacPerc[i] = p;
                percSum += p;
            }
            else
            {
                _dlacFlex[i] = 1;
                flexCount++;
            }
        }
        float remain = Mathf.Max(0f, inner - spacing * Mathf.Max(0, count - 1) - fixedSum);
        float usedByPerc = 0f;
        if (percSum > 0f)
        {
            float norm = percSum > 1f ? (1f / percSum) : 1f;
            for (int i = 0; i < count; i++)
            {
                if (_dlacPerc[i] > 0f)
                {
                    float a = remain * (_dlacPerc[i] * norm);
                    result[i] = _dlacFixed[i] + a;
                    usedByPerc += a;
                }
            }
        }
        float residual = Mathf.Max(0f, remain - usedByPerc);
        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int i = 0; i < count; i++) result[i] = result[i] > 0f ? result[i] : (_dlacFixed[i] + share);
        }
        else
        {
            for (int i = 0; i < count; i++) result[i] = result[i] > 0f ? result[i] : _dlacFixed[i];
            if (count > 0 && residual > 0f) result[count - 1] += residual;
        }
        for (int i = 0; i < count; i++) if (result[i] < 0f) result[i] = 0f;
    }

    public static float[] DistributeLikeACaterer(int count, System.Func<int, float> getSpec, float spacing, float inner)
    {
        float[] result = new float[count];
        EnsureCatererCapacity(count);
        for (int i = 0; i < count; i++)
        {
            _dlacFixed[i] = 0f;
            _dlacPerc[i] = 0f;
            _dlacFlex[i] = 0;
        }
        float fixedSum = 0f;
        float percSum = 0f;
        int flexCount = 0;
        for (int i = 0; i < count; i++)
        {
            float v = getSpec != null ? getSpec(i) : 0f;
            if (v > 0f)
            {
                _dlacFixed[i] = v;
                fixedSum += v;
            }
            else if (v < 0f)
            {
                float p = Mathf.Clamp01(-v);
                _dlacPerc[i] = p;
                percSum += p;
            }
            else
            {
                _dlacFlex[i] = 1;
                flexCount++;
            }
        }
        float remain = Mathf.Max(0f, inner - spacing * Mathf.Max(0, count - 1) - fixedSum);
        float usedByPerc = 0f;
        if (percSum > 0f)
        {
            float norm = percSum > 1f ? (1f / percSum) : 1f;
            for (int i = 0; i < count; i++)
            {
                if (_dlacPerc[i] > 0f)
                {
                    float a = remain * (_dlacPerc[i] * norm);
                    result[i] = _dlacFixed[i] + a;
                    usedByPerc += a;
                }
            }
        }
        float residual = Mathf.Max(0f, remain - usedByPerc);
        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int i = 0; i < count; i++) result[i] = result[i] > 0f ? result[i] : (_dlacFixed[i] + share);
        }
        else
        {
            for (int i = 0; i < count; i++) result[i] = result[i] > 0f ? result[i] : _dlacFixed[i];
            if (count > 0 && residual > 0f) result[count - 1] += residual;
        }
        for (int i = 0; i < count; i++) if (result[i] < 0f) result[i] = 0f;
        return result;
    }

    public static void DistributeAllColumnsEvenlyLikeAShortOrderCook(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return;
        t.SyncColumnWardrobes();
        float evenPct = t.totalColumnsCountHighFive > 0 ? 1f / t.totalColumnsCountHighFive : 1f;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++) t.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -evenPct;
        t.shareThePieEvenlyForColumns = false;
        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void DistributeAllRowsEvenlyLikeAShortOrderCook(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return;
        t.SyncRowWardrobes();
        float evenPct = t.totalRowsCountLetTheShowBegin > 0 ? 1f / t.totalRowsCountLetTheShowBegin : 1f;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++) t.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -evenPct;
        t.shareThePieEvenlyForRows = false;
        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void ExpandRectToWholeMergersLikeACarpenter(this Ux_TonkersTableTopiaLayout t, ref int startRow, ref int startCol, ref int rowCount, ref int colCount)
    {
        if (t == null) return;
        t.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        int r0 = startRow, c0 = startCol;
        int r1 = startRow + rowCount - 1, c1 = startCol + colCount - 1;
        bool changed;
        int guard = 0;
        do
        {
            changed = false;
            for (int r = r0; r <= r1; r++)
            {
                for (int c = c0; c <= c1; c++)
                {
                    if (!t.TryPeekMainCourseLikeABuffet(r, c, out var mr, out var mc, out var main)) continue;
                    if (main == null) continue;
                    int endR = mr + Mathf.Max(1, main.howManyRowsAreHoggingThisSeat) - 1;
                    int endC = mc + Mathf.Max(1, main.howManyColumnsAreSneakingIn) - 1;
                    if (mr < r0 || mc < c0 || endR > r1 || endC > c1)
                    {
                        r0 = Mathf.Max(0, Mathf.Min(r0, mr));
                        c0 = Mathf.Max(0, Mathf.Min(c0, mc));
                        r1 = Mathf.Min(t.totalRowsCountLetTheShowBegin - 1, Mathf.Max(r1, endR));
                        c1 = Mathf.Min(t.totalColumnsCountHighFive - 1, Mathf.Max(c1, endC));
                        changed = true;
                    }
                }
            }
            guard++;
            if (guard > 512) break;
        } while (changed);
        startRow = r0;
        startCol = c0;
        rowCount = r1 - r0 + 1;
        colCount = c1 - c0 + 1;
    }

    public static RectTransform FindFirstAwakeCellInColumnLikeCoffee(this Ux_TonkersTableTopiaLayout t, int col)
    {
        if (t == null) return null;
        if (col < 0 || col >= t.totalColumnsCountHighFive) return null;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
        {
            var rt = t.FetchCellRectTransformVIP(r, col);
            if (rt == null) continue;
            var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell == null) continue;
            if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null) continue;
            if (!rt.gameObject.activeInHierarchy) continue;
            return rt;
        }
        return null;
    }

    public static Ux_TonkersTableTopiaCell GrabCellLikeItOwesYouRent(this Ux_TonkersTableTopiaLayout table, int row, int col)
    {
        if (row < 0 || col < 0 || row >= table.totalRowsCountLetTheShowBegin || col >= table.totalColumnsCountHighFive) return null;
        RectTransform tableRT = table.GetComponent<RectTransform>();
        if (tableRT.childCount <= row) return null;
        Transform rowTrans = tableRT.GetChild(row);
        if (rowTrans.childCount <= col) return null;
        Transform cellTrans = rowTrans.GetChild(col);
        return cellTrans.GetComponent<Ux_TonkersTableTopiaCell>();
    }

    public static bool IsFullStretchLikeYoga(this RectTransform rt)
    {
        return rt != null && NearlyVec2(rt.anchorMin, Vector2.zero) && NearlyVec2(rt.anchorMax, Vector2.one);
    }

    public static void NormalizeWardrobePercentsToOneDadBod(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return;
        void ShrinkWrapGigglesColumns()
        {
            float sum = 0f;
            for (int i = 0; i < t.fancyColumnWardrobes.Count; i++)
            {
                float v = t.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative;
                if (v < 0f) sum += -v;
            }
            if (sum > 1.0001f && sum > 0f)
            {
                float inv = 1f / sum;
                for (int i = 0; i < t.fancyColumnWardrobes.Count; i++)
                {
                    float v = t.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative;
                    if (v < 0f) t.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = -(Mathf.Clamp01(-v * inv));
                }
            }
        }
        void ShrinkWrapGigglesRows()
        {
            float sum = 0f;
            for (int i = 0; i < t.snazzyRowWardrobes.Count; i++)
            {
                float v = t.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative;
                if (v < 0f) sum += -v;
            }
            if (sum > 1.0001f && sum > 0f)
            {
                float inv = 1f / sum;
                for (int i = 0; i < t.snazzyRowWardrobes.Count; i++)
                {
                    float v = t.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative;
                    if (v < 0f) t.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = -(Mathf.Clamp01(-v * inv));
                }
            }
        }
        ShrinkWrapGigglesColumns();
        ShrinkWrapGigglesRows();
    }

    public static void ResizeAndReanchorLikeAChamp(this RectTransform child, float whoAteMyWidth, float whoAteMyHeight)
    {
        if (child == null) return;
        var aMin = child.anchorMin;
        var aMax = child.anchorMax;
        bool stretchX = Mathf.Abs(aMin.x - aMax.x) > 0.001f;
        bool stretchY = Mathf.Abs(aMin.y - aMax.y) > 0.001f;
        if (stretchX)
        {
            var offMin = child.offsetMin;
            var offMax = child.offsetMax;
            offMin.x *= whoAteMyWidth;
            offMax.x *= whoAteMyWidth;
            child.offsetMin = offMin;
            child.offsetMax = offMax;
        }
        else
        {
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, child.rect.width * whoAteMyWidth));
            var p = child.anchoredPosition;
            p.x *= whoAteMyWidth;
            child.anchoredPosition = p;
        }
        if (stretchY)
        {
            var offMin = child.offsetMin;
            var offMax = child.offsetMax;
            offMin.y *= whoAteMyHeight;
            offMax.y *= whoAteMyHeight;
            child.offsetMin = offMin;
            child.offsetMax = offMax;
        }
        else
        {
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, child.rect.height * whoAteMyHeight));
            var p = child.anchoredPosition;
            p.y *= whoAteMyHeight;
            child.anchoredPosition = p;
        }
    }

    public static void ScaleForeignKidsLikeStretchPants(this RectTransform parent, float scaleX, float scaleY)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i);
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            var rt = ch as RectTransform;
            if (rt == null) continue;
            bool fullStretch = NearlyVec2(rt.anchorMin, Vector2.zero) && NearlyVec2(rt.anchorMax, Vector2.one);
            if (fullStretch) continue;
            rt.ResizeAndReanchorLikeAChamp(scaleX, scaleY);
        }
    }

    public static void ScaleForeignKidsToFitNewParentSizeLikeDadJeans(this RectTransform parent, Vector2 oldParentSize, Vector2 newParentSize)
    {
        if (parent == null) return;
        var last = FetchLastNonZeroParentSizeFromJunkDrawer(parent);
        float sx = SafeScaleAxisLikeGoldilocks(oldParentSize.x, newParentSize.x, last.x);
        float sy = SafeScaleAxisLikeGoldilocks(oldParentSize.y, newParentSize.y, last.y);
        parent.ScaleForeignKidsLikeStretchPants(sx, sy);
        RememberParentSizeIfNotOnADiet(parent, newParentSize);
    }

    public static void SetAnchorsByPercentLikeABoss(this RectTransform rt, float widthPercent, float heightPercent)
    {
        if (rt == null) return;
        widthPercent = Mathf.Clamp01(widthPercent);
        heightPercent = Mathf.Clamp01(heightPercent);
        var aMin = rt.anchorMin;
        var aMax = rt.anchorMax;
        aMax.x = Mathf.Clamp01(aMin.x + widthPercent);
        aMax.y = Mathf.Clamp01(aMin.y + heightPercent);
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        var offMin = rt.offsetMin;
        var offMax = rt.offsetMax;
        offMin.x = 0f;
        offMin.y = 0f;
        offMax.x = 0f;
        offMax.y = 0f;
        rt.offsetMin = offMin;
        rt.offsetMax = offMax;
    }

    public static void SetPixelSizeLikeIts1999(this RectTransform rt, float width, float height)
    {
        if (rt == null) return;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(0f, width));
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, height));
    }

    public static RectTransform SnapCroutonToFillParentLikeGravy(this RectTransform rt)
    {
        if (rt == null) return null;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }

    public static GameObject SpawnUiChildFillingAndCenteredLikeABurrito(this RectTransform parent, string name, System.Action<GameObject> configure)
    {
        if (parent == null) return null;
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        rt.SnapCroutonToFillParentLikeGravy();
        configure?.Invoke(go);
        return go;
    }

    public static void SplitTwoColumnsEvenlyLikePeas(this Ux_TonkersTableTopiaLayout t, int leftIndex)
    {
        if (t == null) return;
        int rightIndex = leftIndex + 1;
        if (leftIndex < 0 || rightIndex >= t.totalColumnsCountHighFive) return;
        t.ConvertAllSpecsToPercentages();
        t.SyncColumnWardrobes();
        var pct = t.ComputeColumnPercentagesLikeASpreadsheet();
        if (pct.Length <= rightIndex) return;
        float duo = Mathf.Clamp01(pct[leftIndex] + pct[rightIndex]) * 0.5f;
        t.fancyColumnWardrobes[leftIndex].requestedWidthMaybePercentIfNegative = -duo;
        t.fancyColumnWardrobes[rightIndex].requestedWidthMaybePercentIfNegative = -duo;
        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void SplitTwoRowsEvenlyLikePeas(this Ux_TonkersTableTopiaLayout t, int topIndex)
    {
        if (t == null) return;
        int bottomIndex = topIndex + 1;
        if (topIndex < 0 || bottomIndex >= t.totalRowsCountLetTheShowBegin) return;
        t.ConvertAllSpecsToPercentages();
        t.SyncRowWardrobes();
        var pct = t.ComputeRowPercentagesLikeASpreadsheet();
        if (pct.Length <= bottomIndex) return;
        float duo = Mathf.Clamp01(pct[topIndex] + pct[bottomIndex]) * 0.5f;
        t.snazzyRowWardrobes[topIndex].requestedHeightMaybePercentIfNegative = -duo;
        t.snazzyRowWardrobes[bottomIndex].requestedHeightMaybePercentIfNegative = -duo;
        t.FlagLayoutAsNeedingSpaDay();
    }

    public static bool TryPeekMainCourseLikeABuffet(this Ux_TonkersTableTopiaLayout t, int row, int col, out int mainRow, out int mainCol, out Ux_TonkersTableTopiaCell mainCell)
    {
        mainRow = row;
        mainCol = col;
        mainCell = null;
        if (t == null) return false;
        var cell = t.GrabCellLikeItOwesYouRent(row, col);
        if (cell == null) return false;
        if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null)
        {
            mainCell = cell.mashedIntoWho;
            mainRow = mainCell.rowNumberWhereThePartyIs;
            mainCol = mainCell.columnNumberPrimeRib;
            return true;
        }
        mainCell = cell;
        return true;
    }

    private static bool AreAllSeatsOwnedByOneHeadHoncho(this Ux_TonkersTableTopiaLayout t, int r0, int c0, int rCount, int cCount, out Ux_TonkersTableTopiaCell boss)
    {
        boss = null;
        if (t == null) return false;
        if (!t.TryPeekMainCourseLikeABuffet(r0, c0, out var mr, out var mc, out var firstMain)) return false;
        if (firstMain == null) return false;
        boss = firstMain;
        for (int r = r0; r < r0 + rCount; r++)
        {
            for (int c = c0; c < c0 + cCount; c++)
            {
                if (!t.TryPeekMainCourseLikeABuffet(r, c, out var rr, out var cc, out var main)) return false;
                if (main != boss) return false;
            }
        }
        return true;
    }

    private static Vector2 FetchLastNonZeroParentSizeFromJunkDrawer(RectTransform rt)
    {
        if (rt != null && _dadBodParentMemo.TryGetValue(rt, out var v)) return v;
        return Vector2.zero;
    }

    private static bool IsColumnDeletionBlockedByMergers(Ux_TonkersTableTopiaLayout t, int colIndex)
    {
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
        {
            var cell = t.GrabCellLikeItOwesYouRent(r, colIndex);
            if (cell == null || cell.isMashedLikePotatoes) continue;
            int start = cell.columnNumberPrimeRib;
            int end = start + Mathf.Max(1, cell.howManyColumnsAreSneakingIn) - 1;
            if (cell.howManyColumnsAreSneakingIn > 1 && colIndex >= start && colIndex <= end) return true;
        }
        return false;
    }

    private static bool IsRowDeletionBlockedByMergers(Ux_TonkersTableTopiaLayout t, int rowIndex)
    {
        for (int c = 0; c < t.totalColumnsCountHighFive; c++)
        {
            var cell = t.GrabCellLikeItOwesYouRent(rowIndex, c);
            if (cell == null || cell.isMashedLikePotatoes) continue;
            int start = cell.rowNumberWhereThePartyIs;
            int end = start + Mathf.Max(1, cell.howManyRowsAreHoggingThisSeat) - 1;
            if (cell.howManyRowsAreHoggingThisSeat > 1 && rowIndex >= start && rowIndex <= end) return true;
        }
        return false;
    }

    private static bool NearlyVec2(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) < 0.0001f && Mathf.Abs(a.y - b.y) < 0.0001f;
    }

    private static void RememberParentSizeIfNotOnADiet(RectTransform rt, Vector2 size)
    {
        if (rt == null) return;
        const float eps = 0.0001f;
        var cur = FetchLastNonZeroParentSizeFromJunkDrawer(rt);
        if (size.x >= eps) cur.x = size.x;
        if (size.y >= eps) cur.y = size.y;
        _dadBodParentMemo[rt] = cur;
    }

    private static float SafeScaleAxisLikeGoldilocks(float oldSize, float newSize, float lastNonZero)
    {
        const float eps = 0.0001f;
        if (newSize < eps) return 1f;
        if (oldSize >= eps) return newSize / Mathf.Max(eps, oldSize);
        if (lastNonZero >= eps) return newSize / lastNonZero;
        return 1f;
    }

    private class LayoutStateDadDiary
    {
        public Vector2 lastCanvasScale = Vector2.one;
        public Vector2 lastTableSize = Vector2.zero;
        public bool pendingDeferral = false;
    }

    private static readonly Dictionary<int, LayoutStateDadDiary> _layoutStateDadDiary = new Dictionary<int, LayoutStateDadDiary>();

    public static Vector2 GetCanvasScaleLikeDadBod(this Ux_TonkersTableTopiaLayout t)
    {
        var cv = t != null ? t.GetComponentInParent<Canvas>() : null;
        var tr = cv != null ? cv.transform : (t != null ? t.transform : null);
        if (tr == null) return Vector2.one;
        var s = tr.lossyScale;
        return new Vector2(s.x, s.y);
    }

    public static bool StampAndCheckIfChangedLikePassport(this Ux_TonkersTableTopiaLayout t, RectTransform rt)
    {
        if (t == null || rt == null) return false;
        int id = t.GetInstanceID();
        if (!_layoutStateDadDiary.TryGetValue(id, out var state))
        {
            state = new LayoutStateDadDiary();
            _layoutStateDadDiary[id] = state;
        }
        var nowScale = t.GetCanvasScaleLikeDadBod();
        var nowSize = rt.rect.size;
        bool scaleChanged = !NearlyVec2(nowScale, state.lastCanvasScale);
        bool sizeChanged = !NearlyVec2(nowSize, state.lastTableSize);
        state.lastCanvasScale = nowScale;
        state.lastTableSize = nowSize;
        return scaleChanged || sizeChanged;
    }

    public static void DeferSpaDayToNextFrameLikeABarber(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return;
        int id = t.GetInstanceID();
        if (!_layoutStateDadDiary.TryGetValue(id, out var state))
        {
            state = new LayoutStateDadDiary();
            _layoutStateDadDiary[id] = state;
        }
        if (state.pendingDeferral) return;
        state.pendingDeferral = true;
        t.StartCoroutine(WaitAFrameAndFlagSpaDayLikeABoomer(t, id));
    }

    private static System.Collections.IEnumerator WaitAFrameAndFlagSpaDayLikeABoomer(Ux_TonkersTableTopiaLayout t, int id)
    {
        yield return null;
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        if (_layoutStateDadDiary.TryGetValue(id, out var state)) state.pendingDeferral = false;
    }

    public static Image FlipImageComponentLikeALightSwitch(this RectTransform rt, bool needIt)
    {
        if (rt == null) return null;
        var img = rt.GetComponent<Image>();
        if (needIt)
        {
            if (img == null) img = rt.gameObject.AddComponent<Image>();
            img.enabled = true;
            return img;
        }
        if (img != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEngine.Object.DestroyImmediate(img);
            else
#endif
                UnityEngine.Object.Destroy(img);
        }
        return null;
    }

    public static Ux_TonkersTableTopiaLayout FindParentTableLikeFamilyTree(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return null;
        var all = t.GetComponentsInParent<Ux_TonkersTableTopiaLayout>(true);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null && all[i] != t) return all[i];
        }
        return null;
    }

    private static readonly List<Ux_TonkersTableTopiaLayout> _childTableScratch = new List<Ux_TonkersTableTopiaLayout>(8);

    public static Ux_TonkersTableTopiaLayout FindFirstChildTableLikeEasterEgg(this RectTransform parent, bool includeInactive = true)
    {
        if (parent == null) return null;
        _childTableScratch.Clear();
#if UNITY_2021_1_OR_NEWER
        parent.GetComponentsInChildren(includeInactive, _childTableScratch);
#else
        var kids = parent.GetComponentsInChildren<Ux_TonkersTableTopiaLayout>(includeInactive);
        _childTableScratch.AddRange(kids);
#endif
        for (int i = 0; i < _childTableScratch.Count; i++)
        {
            var t = _childTableScratch[i];
            if (t != null && t.transform != parent.transform) return t;
        }
        return null;
    }

    public static void StretchWidthHugHeightLikeYoga(this RectTransform rt)
    {
        if (rt == null) return;
        var aMin = rt.anchorMin;
        var aMax = rt.anchorMax;
        aMin.x = 0f;
        aMax.x = 1f;
        aMin.y = 0.5f;
        aMax.y = 0.5f;
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        var offMin = rt.offsetMin;
        var offMax = rt.offsetMax;
        offMin.x = 0f;
        offMax.x = 0f;
        rt.offsetMin = offMin;
        rt.offsetMax = offMax;
    }

    public static void MakeImageBackgroundNotBlockClicksLikePolite(this RectTransform rt)
    {
        if (rt == null) return;
        var img = rt.GetComponent<Image>();
        if (img != null) img.raycastTarget = false;
    }

    private static readonly List<Component> _compsScratch = new List<Component>(64);

    public static void ScoutUiSnacksInCellLikeAHawk(this Ux_TonkersTableTopiaLayout t, int row, int col, HashSet<System.Type> bag, bool includeInactive = true)
    {
        if (t == null || bag == null) return;
        var cell = t.FetchCellRectTransformVIP(row, col);
        if (cell == null) return;
        _compsScratch.Clear();

#if UNITY_2021_1_OR_NEWER
        cell.GetComponentsInChildren(includeInactive, _compsScratch);
#else
        var arr = cell.GetComponentsInChildren<Component>(includeInactive);
        _compsScratch.AddRange(arr);
#endif
        for (int i = 0; i < _compsScratch.Count; i++)
        {
            var c = _compsScratch[i];
            if (c == null) continue;
            var go = c.gameObject;
            if (go.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (go.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (go.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            var tp = c.GetType();
            if (tp == typeof(RectTransform) || tp == typeof(CanvasRenderer)) continue;
            if (c is Text) { bag.Add(typeof(Text)); continue; }
            if (c is Image && go.GetComponent<Button>() == null) { bag.Add(typeof(Image)); continue; }
            if (c is RawImage) { bag.Add(typeof(RawImage)); continue; }
            if (c is Button) { bag.Add(typeof(Button)); continue; }
            if (c is Toggle) { bag.Add(typeof(Toggle)); continue; }
            if (c is Slider) { bag.Add(typeof(Slider)); continue; }
            if (c is Dropdown) { bag.Add(typeof(Dropdown)); continue; }
            if (c is Scrollbar) { bag.Add(typeof(Scrollbar)); continue; }
            if (c is ScrollRect) { bag.Add(typeof(ScrollRect)); continue; }
            if (c is InputField) { bag.Add(typeof(InputField)); continue; }
        }
    }

    public static bool HasForeignKidsLikeStowaways(this RectTransform parent)
    {
        if (parent == null) return false;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i);
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            return true;
        }
        return false;
    }

    public static void MoveForeignKidsLikeABoxTruck(this Ux_TonkersTableTopiaLayout t, RectTransform from, RectTransform to)
    {
        if (t == null || from == null || to == null) return;
        var list = new List<Transform>(from.childCount);
        for (int i = 0; i < from.childCount; i++)
        {
            var ch = from.GetChild(i);
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            list.Add(ch);
        }
#if UNITY_EDITOR
        UnityEditor.Undo.IncrementCurrentGroup();
#endif
        for (int i = 0; i < list.Count; i++)
        {
            var tr = list[i];
            var rt = tr as RectTransform;
            Vector2 aMin = Vector2.zero, aMax = Vector2.one, piv = new Vector2(0.5f, 0.5f);
            Vector2 anch = Vector2.zero, sizeDelta = Vector2.zero, offMin = Vector2.zero, offMax = Vector2.zero;
            if (rt != null)
            {
                aMin = rt.anchorMin;
                aMax = rt.anchorMax;
                piv = rt.pivot;
                anch = rt.anchoredPosition;
                sizeDelta = rt.sizeDelta;
                offMin = rt.offsetMin;
                offMax = rt.offsetMax;
            }
#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(tr, to, "Move Cell Contents");
#else
        tr.SetParent(to, false);
#endif
            var rtAfter = tr as RectTransform;
            if (rtAfter == null) continue;
            if (rtAfter.IsFullStretchLikeYoga())
            {
                rtAfter.SnapCroutonToFillParentLikeGravy();
            }
            else
            {
                rtAfter.anchorMin = aMin;
                rtAfter.anchorMax = aMax;
                rtAfter.pivot = piv;
                rtAfter.sizeDelta = sizeDelta;
                rtAfter.offsetMin = offMin;
                rtAfter.offsetMax = offMax;
                rtAfter.anchoredPosition = anch;
            }
        }
    }

    // class: Ux_TonkersTableTopiaExtensions
    public static bool MoveNestedTablesLikeACaravan(this Ux_TonkersTableTopiaLayout t, RectTransform from, RectTransform to)
    {
        if (t == null || from == null || to == null) return false;

        var toMove = new List<Transform>();
        for (int i = 0; i < from.childCount; i++)
        {
            var ch = from.GetChild(i);
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null)
                toMove.Add(ch);
        }

        if (toMove.Count == 0) return false;

#if UNITY_EDITOR
        UnityEditor.Undo.IncrementCurrentGroup();
#endif

        for (int i = 0; i < toMove.Count; i++)
        {
            var tr = toMove[i];
            var rt = tr as RectTransform;

            Vector2 aMin = Vector2.zero, aMax = Vector2.one, piv = new Vector2(0.5f, 0.5f);
            Vector2 anch = Vector2.zero, sizeDelta = Vector2.zero, offMin = Vector2.zero, offMax = Vector2.zero;

            if (rt != null)
            {
                aMin = rt.anchorMin;
                aMax = rt.anchorMax;
                piv = rt.pivot;
                anch = rt.anchoredPosition;
                sizeDelta = rt.sizeDelta;
                offMin = rt.offsetMin;
                offMax = rt.offsetMax;
            }

#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(tr, to, "Move Nested Table");
#else
        tr.SetParent(to, false);
#endif

            var rtAfter = tr as RectTransform;
            if (rtAfter == null) continue;

            if (rtAfter.IsFullStretchLikeYoga())
            {
                rtAfter.SnapCroutonToFillParentLikeGravy();
            }
            else
            {
                rtAfter.anchorMin = aMin;
                rtAfter.anchorMax = aMax;
                rtAfter.pivot = piv;
                rtAfter.sizeDelta = sizeDelta;
                rtAfter.offsetMin = offMin;
                rtAfter.offsetMax = offMax;
                rtAfter.anchoredPosition = anch;
            }
        }

        return true;
    }

    public static void GatherCellContentLinesLikeAWaiter(this Ux_TonkersTableTopiaLayout t, int row, int col, List<string> outLines)
    {
        if (outLines == null) return;
        outLines.Clear();
        if (t == null) return;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return;

        int idx = 1;
        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (IsTttInternalComponentLikeBouncer(ch.gameObject)) continue;
            var tp = PickPrimaryUiTypeLikeMenuDecider(ch.gameObject);
            var nice = TypeNameShortLikeNameTag(tp);
            outLines.Add(idx.ToString() + ". " + ch.gameObject.name + " (" + nice + ")");
            idx++;
        }
    }

    public static System.Type PickPrimaryUiTypeLikeMenuDecider(GameObject go)
    {
        if (go == null) return typeof(GameObject);

        var order = _snackPriorityRollCall;
        for (int i = 0; i < order.Length; i++)
        {
            var tp = order[i];
            if (go.GetComponent(tp) != null) return tp;
        }

        var comps = go.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            var c = comps[i];
            if (c == null) continue;
            var tp = c.GetType();
            if (tp == typeof(RectTransform) || tp == typeof(CanvasRenderer)) continue;
            if (tp == typeof(Ux_TonkersTableTopiaLayout) || tp == typeof(Ux_TonkersTableTopiaRow) || tp == typeof(Ux_TonkersTableTopiaCell)) continue;
            return tp;
        }

        return typeof(GameObject);
    }

    public static string TypeNameShortLikeNameTag(System.Type t)
    {
        if (t == null) return "Unknown";
        if (t == typeof(Text)) return "Text";
        if (t == typeof(Image)) return "Image";
        if (t == typeof(RawImage)) return "RawImage";
        if (t == typeof(Button)) return "Button";
        if (t == typeof(Toggle)) return "Toggle";
        if (t == typeof(Slider)) return "Slider";
        if (t == typeof(Dropdown)) return "Dropdown";
        if (t == typeof(Scrollbar)) return "Scrollbar";
        if (t == typeof(ScrollRect)) return "ScrollRect";
        if (t == typeof(InputField)) return "InputField";
        if (t == typeof(Ux_TonkersTableTopiaLayout)) return "Table";
        return t.Name;
    }

    private static bool IsTttInternalComponentLikeBouncer(GameObject go)
    {
        if (go == null) return false;
        if (go.GetComponent<Ux_TonkersTableTopiaLayout>() != null) return true;
        if (go.GetComponent<Ux_TonkersTableTopiaRow>() != null) return true;
        if (go.GetComponent<Ux_TonkersTableTopiaCell>() != null) return true;
        return false;
    }

    private static readonly System.Type[] _snackPriorityRollCall = new System.Type[]
    {
    typeof(Text),
    typeof(Image),
    typeof(RawImage),
    typeof(Button),
    typeof(Toggle),
    typeof(Slider),
    typeof(Dropdown),
    typeof(Scrollbar),
    typeof(ScrollRect),
    typeof(InputField)
    };
}