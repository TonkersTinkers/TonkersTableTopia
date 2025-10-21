using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Ux_TonkersTableTopiaExtensions

{
    private static readonly List<Ux_TonkersTableTopiaLayout> _childTableScratch = new List<Ux_TonkersTableTopiaLayout>(8);
    private static readonly Dictionary<RectTransform, Vector2> _dadBodParentMemo = new Dictionary<RectTransform, Vector2>();
    private static readonly Dictionary<int, LayoutStateDadDiary> _layoutStateDadDiary = new Dictionary<int, LayoutStateDadDiary>();
    private static float[] _dlacFixed;
    private static float[] _dlacPerc;
    private static int[] _dlacFlex;
    private static System.Collections.Generic.Queue<System.Action> _bouncerDeferred;
    private static bool _bouncerDelayScheduled;

#if UNITY_EDITOR

    public static void SelectAndPingLikeABeacon(this UnityEngine.Object target)
    {
        if (target == null) return;
        UnityEditor.Selection.activeObject = target;
        UnityEditor.EditorGUIUtility.PingObject(target);
    }

#endif

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

    public static float CalcShrinkyDinkWidthLikeDietCoke(int buttonCount, float min = 44f, float max = 120f, float gap = 4f, float padding = 32f)
    {
        if (buttonCount < 1) return min;
#if UNITY_EDITOR
        float avail = Mathf.Max(1f, UnityEditor.EditorGUIUtility.currentViewWidth - padding);
#else
    float avail = Mathf.Max(1f, Screen.width - padding);
#endif
        float w = (avail - gap * Mathf.Max(0, buttonCount - 1)) / buttonCount;
        return Mathf.Clamp(w, min, max);
    }

    public static float CalcShrinkyDinkWidthLikeDietCokeSquisher(int buttonCount, float occupiedWidth, float max = 160f, float gap = 4f, float padding = 32f)
    {
        if (buttonCount < 1) return 1f;
#if UNITY_EDITOR
        float avail = Mathf.Max(1f, UnityEditor.EditorGUIUtility.currentViewWidth - padding - Mathf.Max(0f, occupiedWidth));
#else
    float avail = Mathf.Max(1f, Screen.width - padding - Mathf.Max(0f, occupiedWidth));
#endif
        float w = (avail - gap * Mathf.Max(0, buttonCount - 1)) / buttonCount;
        return Mathf.Clamp(w, 1f, max);
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

    private static void EnsureCatererCapacity(int count)
    {
        if (_dlacFixed == null || _dlacFixed.Length < count) System.Array.Resize(ref _dlacFixed, count);
        if (_dlacPerc == null || _dlacPerc.Length < count) System.Array.Resize(ref _dlacPerc, count);
        if (_dlacFlex == null || _dlacFlex.Length < count) System.Array.Resize(ref _dlacFlex, count);
    }

    private static void EnsureReasonableSizeLikeDadPun(RectTransform rt, RectTransform parent)
    {
        if (rt == null || parent == null) return;

        if (Mathf.Abs(rt.anchorMin.x - 0f) < 0.0001f && Mathf.Abs(rt.anchorMin.y - 0f) < 0.0001f &&
            Mathf.Abs(rt.anchorMax.x - 1f) < 0.0001f && Mathf.Abs(rt.anchorMax.y - 1f) < 0.0001f)
        {
            return;
        }

        FindPreferredSizeLikeGoldilocks(rt, out float prefW, out float prefH);

        float maxW = Mathf.Max(1f, parent.rect.width);
        float maxH = Mathf.Max(1f, parent.rect.height);

        float w = Mathf.Clamp(prefW, 1f, maxW);
        float h = Mathf.Clamp(prefH, 1f, maxH);

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
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

    public static bool TryDetectCellAlignmentLikeLieDetector(this Ux_TonkersTableTopiaLayout t, int row, int col, out bool isFullLikeBurrito, out Ux_TonkersTableTopiaLayout.HorizontalAlignment h, out Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        isFullLikeBurrito = false;
        h = Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;
        v = Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
        if (t == null) return false;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return false;
        RectTransform child = null;
        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            child = ch;
            break;
        }
        if (child == null) return false;
        bool full = Mathf.Abs(child.anchorMin.x - 0f) < 0.0001f && Mathf.Abs(child.anchorMin.y - 0f) < 0.0001f && Mathf.Abs(child.anchorMax.x - 1f) < 0.0001f && Mathf.Abs(child.anchorMax.y - 1f) < 0.0001f;
        if (full)
        {
            isFullLikeBurrito = true;
            return true;
        }
        Vector2 p;
        if (Mathf.Abs(child.anchorMin.x - child.anchorMax.x) < 0.0001f && Mathf.Abs(child.anchorMin.y - child.anchorMax.y) < 0.0001f)
        {
            p = child.anchorMin;
        }
        else
        {
            p = (child.anchorMin + child.anchorMax) * 0.5f;
        }
        h = p.x < 0.3334f ? Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left : (p.x > 0.6666f ? Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right : Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
        v = p.y < 0.3334f ? Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom : (p.y > 0.6666f ? Ux_TonkersTableTopiaLayout.VerticalAlignment.Top : Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
        return true;
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

    public static bool IsColumnAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        int c0 = Mathf.Clamp(col, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
        return t.IsSelectionAlreadyAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), c0, c0, h, v);
    }

    public static bool IsColumnFullLikeWaterfall(this Ux_TonkersTableTopiaLayout t, int col)
    {
        if (t == null) return false;
        int c0 = Mathf.Clamp(col, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
        return t.IsSelectionFullLikeBurritoWrap(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), c0, c0);
    }

    public static bool IsColumnHorizAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return false;
        int c0 = Mathf.Clamp(col, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
        return t.IsSelectionHorizAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), c0, c0, h);
    }

    public static bool IsColumnVertAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int col, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        int c0 = Mathf.Clamp(col, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
        return t.IsSelectionVertAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), c0, c0, v);
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

    public static bool IsRowAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int row, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        int r0 = Mathf.Clamp(row, 0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1));
        return t.IsSelectionAlreadyAlignedLikeDejaVu(r0, r0, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), h, v);
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

    public static bool IsRowFullLikeWaterbed(this Ux_TonkersTableTopiaLayout t, int row)
    {
        if (t == null) return false;
        int r0 = Mathf.Clamp(row, 0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1));
        return t.IsSelectionFullLikeBurritoWrap(r0, r0, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
    }

    public static bool IsRowHorizAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int row, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return false;
        int r0 = Mathf.Clamp(row, 0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1));
        return t.IsSelectionHorizAlignedLikeDejaVu(r0, r0, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), h);
    }

    public static bool IsRowVertAlignedLikeMirror(this Ux_TonkersTableTopiaLayout t, int row, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        int r0 = Mathf.Clamp(row, 0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1));
        return t.IsSelectionVertAlignedLikeDejaVu(r0, r0, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), v);
    }

    public static bool IsSelectionAlreadyAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout t, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment wantH, Ux_TonkersTableTopiaLayout.VerticalAlignment wantV)
    {
        if (t == null) return false;
        bool any = false;
        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!t.TryDetectCellAlignmentLikeLieDetector(r, c, out var isFull, out var h, out var v)) return false;
                if (isFull) return false;
                if (h != wantH || v != wantV) return false;
                any = true;
            }
        }
        return any;
    }

    public static bool IsSelectionFullLikeBurritoWrap(this Ux_TonkersTableTopiaLayout t, int r0, int r1, int c0, int c1)
    {
        if (t == null) return false;
        bool any = false;
        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!t.TryDetectCellAlignmentLikeLieDetector(r, c, out var isFull, out _, out _)) return false;
                if (!isFull) return false;
                any = true;
            }
        }
        return any;
    }

    public static bool IsSelectionHorizAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout t, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment wantH)
    {
        if (t == null) return false;
        bool any = false;
        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!t.TryDetectCellAlignmentLikeLieDetector(r, c, out var isFull, out var h, out _)) return false;
                if (isFull) return false;
                if (h != wantH) return false;
                any = true;
            }
        }
        return any;
    }

    public static bool IsSelectionVertAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout t, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.VerticalAlignment wantV)
    {
        if (t == null) return false;
        bool any = false;
        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!t.TryDetectCellAlignmentLikeLieDetector(r, c, out var isFull, out _, out var v)) return false;
                if (isFull) return false;
                if (v != wantV) return false;
                any = true;
            }
        }
        return any;
    }

    public static bool IsTableAlignedLikeChoir(this Ux_TonkersTableTopiaLayout t, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        return t.IsSelectionAlreadyAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), h, v);
    }

    public static bool IsTableFullLikeBalloon(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return false;
        return t.IsSelectionFullLikeBurritoWrap(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
    }

    public static bool IsTableHorizAlignedLikeChoir(this Ux_TonkersTableTopiaLayout t, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return false;
        return t.IsSelectionHorizAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), h);
    }

    public static bool IsTableVertAlignedLikeChoir(this Ux_TonkersTableTopiaLayout t, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return false;
        return t.IsSelectionVertAlignedLikeDejaVu(0, Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1), v);
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

#if UNITY_EDITOR
        if (!Application.isPlaying || !t.isActiveAndEnabled)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (t == null) return;
                if (_layoutStateDadDiary.TryGetValue(id, out var st)) st.pendingDeferral = false;
                t.FlagLayoutAsNeedingSpaDay();
            };
            return;
        }
#endif

        if (t.isActiveAndEnabled)
        {
            t.StartCoroutine(WaitAFrameAndFlagSpaDayLikeABoomer(t, id));
        }
        else
        {
            if (_layoutStateDadDiary.TryGetValue(id, out var st)) st.pendingDeferral = false;
        }
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
            if (!Application.isPlaying) Object.DestroyImmediate(img);
            else
#endif
                Object.Destroy(img);
        }
        return null;
    }

    public static void RebalanceTwoPercentsLikeSeesaw(ref float leftPct, ref float rightPct, float deltaPct, float floor)
    {
        float sum = Mathf.Clamp01(leftPct + rightPct);
        float minLeft = floor;
        float maxLeft = Mathf.Max(floor, sum - floor);

        float newLeft = Mathf.Clamp(leftPct + deltaPct, minLeft, maxLeft);
        float newRight = sum - newLeft;

        leftPct = newLeft;
        rightPct = newRight;
    }

#if UNITY_EDITOR

    public static void RequestWysiRepaintLikeFreshCoat()
    {
        UnityEditor.SceneView.RepaintAll();
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    }

#endif

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

    private static void FindPreferredSizeLikeGoldilocks(RectTransform rt, out float w, out float h)
    {
        w = 0f; h = 0f;
        if (rt == null) { w = 32f; h = 32f; return; }

        try
        {
            w = Mathf.Max(w, LayoutUtility.GetPreferredWidth(rt));
            h = Mathf.Max(h, LayoutUtility.GetPreferredHeight(rt));
        }
        catch { }

        var img = rt.GetComponent<Image>();
        if (img != null)
        {
            if (img.sprite != null)
            {
                var sr = img.sprite.rect.size;
                if (w < 1f) w = sr.x;
                if (h < 1f) h = sr.y;
            }
            if (w < 1f) w = 64f;
            if (h < 1f) h = 64f;
            return;
        }

        var raw = rt.GetComponent<RawImage>();
        if (raw != null && raw.texture != null)
        {
            w = w < 1f ? raw.texture.width : w;
            h = h < 1f ? raw.texture.height : h;
            return;
        }

        var txt = rt.GetComponent<Text>();
        if (txt != null)
        {
            if (w < 1f) w = Mathf.Max(48f, txt.preferredWidth);
            if (h < 1f) h = Mathf.Max(txt.fontSize + 6f, txt.preferredHeight);
            return;
        }

        if (w < 1f) w = Mathf.Max(32f, rt.rect.width);
        if (h < 1f) h = Mathf.Max(20f, rt.rect.height);
    }

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

    public static void ScoutUiSnackCountsInCellLikeBeanCounter(this Ux_TonkersTableTopiaLayout t, int row, int col, Dictionary<System.Type, int> bag, bool includeInactive = true)
    {
        if (bag == null) return;
        bag.Clear();
        if (t == null) return;
        var cell = t.FetchCellRectTransformVIP(row, col);
        if (cell == null) return;

        for (int i = 0; i < cell.childCount; i++)
        {
            var ch = cell.GetChild(i) as RectTransform;
            if (ch == null) continue;
            var go = ch.gameObject;
            if (!includeInactive && !go.activeInHierarchy) continue;
            if (go.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (go.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (go.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            var snackType = DecideSnackTypeForGoLikeBuffetPlate(go);
            if (!bag.TryGetValue(snackType, out var n)) n = 0;
            bag[snackType] = n + 1;
        }
    }

    private static System.Type DecideSnackTypeForGoLikeBuffetPlate(GameObject go)
    {
        if (go == null) return typeof(UnityEngine.Object);
        if (go.GetComponent<Button>() != null) return typeof(Button);
        if (go.GetComponent<Toggle>() != null) return typeof(Toggle);
        if (go.GetComponent<Slider>() != null) return typeof(Slider);
        if (go.GetComponent<Dropdown>() != null) return typeof(Dropdown);
        if (go.GetComponent<Scrollbar>() != null) return typeof(Scrollbar);
        if (go.GetComponent<ScrollRect>() != null) return typeof(ScrollRect);
        if (go.GetComponent<InputField>() != null) return typeof(InputField);
        if (go.GetComponent<Text>() != null) return typeof(Text);
        if (go.GetComponent<RawImage>() != null) return typeof(RawImage);
        if (go.GetComponent<Image>() != null) return typeof(Image);
        return typeof(UnityEngine.Object);
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

    public static void ListForeignKidsLikeRollCall(this RectTransform parent, List<Transform> outList)
    {
        if (outList == null) return;
        outList.Clear();
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i);
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            outList.Add(ch);
        }
    }

    public static void AlignCellForeignsLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int row, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return;
        rt.AlignForeignersInRectLikeEtiquette(h, v, true);
        t.FlagLayoutAsNeedingSpaDay();
    }

    private static void ApplyRectSinglePointAnchorLikeEtiquette(RectTransform rt, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (rt == null) return;

        float ax = h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left ? 0f : (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center ? 0.5f : 1f);
        float ay = v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom ? 0f : (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle ? 0.5f : 1f);

        var aMin = rt.anchorMin;
        var aMax = rt.anchorMax;
        aMin.x = ax; aMax.x = ax;
        aMin.y = ay; aMax.y = ay;
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;

        var piv = rt.pivot;
        piv.x = ax;
        piv.y = ay;
        rt.pivot = piv;

        var pos = rt.anchoredPosition;
        pos.x = 0f;
        pos.y = 0f;
        rt.anchoredPosition = pos;

        EnsureReasonableSizeLikeDadPun(rt, rt.parent as RectTransform);
    }

    private static TextAnchor MapToTextAnchorDad(Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top)
        {
            if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left) return TextAnchor.UpperLeft;
            if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center) return TextAnchor.UpperCenter;
            return TextAnchor.UpperRight;
        }
        if (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle)
        {
            if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left) return TextAnchor.MiddleLeft;
            if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center) return TextAnchor.MiddleCenter;
            return TextAnchor.MiddleRight;
        }
        if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left) return TextAnchor.LowerLeft;
        if (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center) return TextAnchor.LowerCenter;
        return TextAnchor.LowerRight;
    }

    public static GameObject CreateToggleFlipFlop(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Toggle FlipFlop", x => { });
        var bg = new GameObject("Background").AddComponent<Image>();
        var ck = new GameObject("Checkmark").AddComponent<Image>();
        var lbl = new GameObject("Label").AddComponent<Text>();
        var rt = go.GetComponent<RectTransform>();
        var bgRT = bg.GetComponent<RectTransform>(); bgRT.SetParent(rt, false); bgRT.anchorMin = new Vector2(0f, 0.5f); bgRT.anchorMax = new Vector2(0f, 0.5f); bgRT.sizeDelta = new Vector2(20, 20); bgRT.anchoredPosition = new Vector2(10, 0);
        var ckRT = ck.GetComponent<RectTransform>(); ckRT.SetParent(bgRT, false); ckRT.anchorMin = Vector2.one * 0.5f; ckRT.anchorMax = Vector2.one * 0.5f; ckRT.sizeDelta = new Vector2(12, 12);
        var lblRT = lbl.GetComponent<RectTransform>(); lblRT.SetParent(rt, false); lblRT.anchorMin = new Vector2(0f, 0.5f); lblRT.anchorMax = new Vector2(0f, 0.5f); lblRT.anchoredPosition = new Vector2(35, 0); lblRT.sizeDelta = new Vector2(160, 20);
        lbl.text = "Toggle"; lbl.alignment = TextAnchor.MiddleLeft; lbl.raycastTarget = false;
        var img = bg; img.color = Color.white;
        var tog = go.AddComponent<Toggle>();
        tog.graphic = ck;
        tog.targetGraphic = img;
        return go;
    }

    public static GameObject CreateSliderSlipNSlide(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Slider SlipNSlide", x => { });
        var rt = go.GetComponent<RectTransform>();
        rt.StretchWidthHugHeightLikeYoga(); rt.sizeDelta = new Vector2(0, 20);
        var bg = new GameObject("Background").AddComponent<Image>();
        var fillArea = new GameObject("Fill Area").AddComponent<RectTransform>();
        var fill = new GameObject("Fill").AddComponent<Image>();
        var handleSlideArea = new GameObject("Handle Slide Area").AddComponent<RectTransform>();
        var handle = new GameObject("Handle").AddComponent<Image>();
        bg.rectTransform.SetParent(rt, false); bg.rectTransform.SnapCroutonToFillParentLikeGravy(); bg.raycastTarget = true;
        fillArea.SetParent(rt, false); fillArea.anchorMin = new Vector2(0f, 0.25f); fillArea.anchorMax = new Vector2(1f, 0.75f); fillArea.offsetMin = new Vector2(10, 0); fillArea.offsetMax = new Vector2(-10, 0);
        fill.rectTransform.SetParent(fillArea, false); fill.rectTransform.anchorMin = new Vector2(0f, 0.25f); fill.rectTransform.anchorMax = new Vector2(0.5f, 0.75f); fill.raycastTarget = false;
        handleSlideArea.SetParent(rt, false); handleSlideArea.anchorMin = new Vector2(0f, 0f); handleSlideArea.anchorMax = new Vector2(1f, 1f); handleSlideArea.offsetMin = new Vector2(10, 0); handleSlideArea.offsetMax = new Vector2(-10, 0);
        handle.rectTransform.SetParent(handleSlideArea, false); handle.rectTransform.anchorMin = new Vector2(0.5f, 0.5f); handle.rectTransform.anchorMax = new Vector2(0.5f, 0.5f); handle.rectTransform.sizeDelta = new Vector2(20, 20);
        var s = go.AddComponent<Slider>();
        s.fillRect = fill.rectTransform;
        s.handleRect = handle.rectTransform;
        s.direction = Slider.Direction.LeftToRight;
        s.targetGraphic = handle;
        return go;
    }

    public static GameObject CreateScrollbarScoot(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Scrollbar Scoot", x => { });
        var rt = go.GetComponent<RectTransform>(); rt.StretchWidthHugHeightLikeYoga(); rt.sizeDelta = new Vector2(0, 20);
        var bg = new GameObject("Background").AddComponent<Image>(); bg.rectTransform.SetParent(rt, false); bg.rectTransform.SnapCroutonToFillParentLikeGravy();
        var handle = new GameObject("Handle").AddComponent<Image>(); handle.rectTransform.SetParent(bg.rectTransform, false); handle.rectTransform.anchorMin = new Vector2(0f, 0f); handle.rectTransform.anchorMax = new Vector2(0.2f, 1f);
        var sb = go.AddComponent<Scrollbar>();
        sb.targetGraphic = handle;
        sb.handleRect = handle.rectTransform;
        sb.direction = Scrollbar.Direction.LeftToRight;
        return go;
    }

    public static GameObject CreateScrollRectScooter(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT ScrollRect Scooter", x => { });
        var rt = go.GetComponent<RectTransform>();
        var viewport = new GameObject("Viewport").AddComponent<Image>();
        var mask = viewport.gameObject.AddComponent<Mask>(); mask.showMaskGraphic = false;
        var content = new GameObject("Content").AddComponent<RectTransform>();
        viewport.rectTransform.SetParent(rt, false); viewport.rectTransform.SnapCroutonToFillParentLikeGravy();
        content.SetParent(viewport.rectTransform, false); content.anchorMin = new Vector2(0f, 1f); content.anchorMax = new Vector2(1f, 1f); content.pivot = new Vector2(0.5f, 1f);
        var sr = go.AddComponent<ScrollRect>();
        sr.viewport = viewport.rectTransform;
        sr.content = content;
        return go;
    }

    public static GameObject CreateInputFieldChattyCathy(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT InputField ChattyCathy", x => { });
        var rt = go.GetComponent<RectTransform>();
        var img = go.AddComponent<Image>(); img.raycastTarget = true;
        var placeholder = new GameObject("Placeholder").AddComponent<Text>();
        var text = new GameObject("Text").AddComponent<Text>();
        placeholder.rectTransform.SetParent(rt, false); placeholder.rectTransform.SnapCroutonToFillParentLikeGravy();
        text.rectTransform.SetParent(rt, false); text.rectTransform.SnapCroutonToFillParentLikeGravy();
        placeholder.text = "Enter text..."; placeholder.color = new Color(0.5f, 0.5f, 0.5f, 0.75f); placeholder.alignment = TextAnchor.MiddleLeft; placeholder.raycastTarget = false;
        text.text = ""; text.alignment = TextAnchor.MiddleLeft; text.raycastTarget = true;
        var input = go.AddComponent<InputField>();
        input.placeholder = placeholder;
        input.textComponent = text;
        return go;
    }

    public static GameObject CreateDropdownDropItLikeItsHot(this RectTransform parent)
    {
        var go = parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Dropdown DropIt", x => { });
        var rt = go.GetComponent<RectTransform>();
        var bg = go.AddComponent<Image>(); bg.raycastTarget = true;
        var caption = new GameObject("Label").AddComponent<Text>();
        var arrow = new GameObject("Arrow").AddComponent<Image>();
        caption.rectTransform.SetParent(rt, false); caption.rectTransform.SnapCroutonToFillParentLikeGravy(); caption.alignment = TextAnchor.MiddleLeft; caption.text = "Option A"; caption.raycastTarget = false;
        arrow.rectTransform.SetParent(rt, false); arrow.rectTransform.anchorMin = new Vector2(1f, 0.5f); arrow.rectTransform.anchorMax = new Vector2(1f, 0.5f); arrow.rectTransform.sizeDelta = new Vector2(20, 20); arrow.rectTransform.anchoredPosition = new Vector2(-10, 0);
        var template = new GameObject("Template").AddComponent<Image>();
        template.rectTransform.SetParent(rt, false); template.rectTransform.SnapCroutonToFillParentLikeGravy(); template.gameObject.SetActive(false);
        var viewport = new GameObject("Viewport").AddComponent<Image>();
        viewport.rectTransform.SetParent(template.rectTransform, false); viewport.rectTransform.SnapCroutonToFillParentLikeGravy();
        var mask = viewport.gameObject.AddComponent<Mask>(); mask.showMaskGraphic = false;
        var content = new GameObject("Content").AddComponent<RectTransform>();
        content.SetParent(viewport.rectTransform, false); content.anchorMin = new Vector2(0f, 1f); content.anchorMax = new Vector2(1f, 1f); content.pivot = new Vector2(0.5f, 1f);
        var item = new GameObject("Item").AddComponent<Toggle>();
        var itemBG = item.gameObject.AddComponent<Image>();
        var itemCheck = new GameObject("Item Checkmark").AddComponent<Image>();
        var itemLabel = new GameObject("Item Label").AddComponent<Text>();
        item.transform.SetParent(content, false);
        itemCheck.rectTransform.SetParent(item.transform, false);
        itemLabel.rectTransform.SetParent(item.transform, false);
        itemLabel.alignment = TextAnchor.MiddleLeft;
        var sr = template.gameObject.AddComponent<ScrollRect>();
        sr.viewport = viewport.rectTransform;
        sr.content = content;
        var dd = go.AddComponent<Dropdown>();
        dd.template = template.rectTransform;
        dd.captionText = caption;
        dd.itemText = itemLabel;
        dd.options = new List<Dropdown.OptionData>
    {
        new Dropdown.OptionData("Option A"),
        new Dropdown.OptionData("Option B"),
        new Dropdown.OptionData("Option C")
    };
        dd.value = 0;
        dd.RefreshShownValue();
        return go;
    }

    public static void AlignRowHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++) t.AlignCellHorizontalOnlyLikeLaserLevel(rowIndex, c, h);
    }

    public static void AlignRowLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++)
            t.AlignCellForeignsLikeLaserLevel(rowIndex, c, h, v);
    }

    public static void AlignRowVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++) t.AlignCellVerticalOnlyLikeLaserLevel(rowIndex, c, v);
    }

    public static void AlignCellForeignsToFillLikeStuffedBurrito(this Ux_TonkersTableTopiaLayout t, int row, int col)
    {
        if (t == null) return;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return;
        rt.AlignForeignersToFillLikeStuffedBurrito(true);
        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void AlignCellHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int row, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return;

        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            float ax = h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left ? 0f : (h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center ? 0.5f : 1f);
            var aMin = ch.anchorMin; var aMax = ch.anchorMax;
            aMin.x = ax; aMax.x = ax;
            ch.anchorMin = aMin; ch.anchorMax = aMax;

            var pivot = ch.pivot; pivot.x = ax; ch.pivot = pivot;

            var p = ch.anchoredPosition; p.x = 0f; ch.anchoredPosition = p;

            EnsureReasonableSizeLikeDadPun(ch, rt);
            var txt = ch.GetComponent<Text>();
            if (txt != null) txt.alignment = WithHorizontalChangedLikeBarber(txt.alignment, h);
        }

        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void AlignCellVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int row, int col, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return;

        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            float ay = v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom ? 0f : (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle ? 0.5f : 1f);
            var aMin = ch.anchorMin; var aMax = ch.anchorMax;
            aMin.y = ay; aMax.y = ay;
            ch.anchorMin = aMin; ch.anchorMax = aMax;

            var pivot = ch.pivot; pivot.y = ay; ch.pivot = pivot;

            var p = ch.anchoredPosition; p.y = 0f; ch.anchoredPosition = p;

            EnsureReasonableSizeLikeDadPun(ch, rt);
            var txt = ch.GetComponent<Text>();
            if (txt != null) txt.alignment = WithVerticalChangedLikeElevator(txt.alignment, v);
        }

        t.FlagLayoutAsNeedingSpaDay();
    }

    public static void AlignRowToFillLikeWaterbed(this Ux_TonkersTableTopiaLayout t, int rowIndex)
    {
        if (t == null) return;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++)
            t.AlignCellForeignsToFillLikeStuffedBurrito(rowIndex, c);
    }

    public static void AlignColumnToFillLikeWaterfall(this Ux_TonkersTableTopiaLayout t, int colIndex)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
            t.AlignCellForeignsToFillLikeStuffedBurrito(r, colIndex);
    }

    public static void AlignColumnHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++) t.AlignCellHorizontalOnlyLikeLaserLevel(r, colIndex, h);
    }

    public static void AlignColumnVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout t, int colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++) t.AlignCellVerticalOnlyLikeLaserLevel(r, colIndex, v);
    }

    public static void AlignTableHorizontalOnlyLikeChoir(this Ux_TonkersTableTopiaLayout t, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
            for (int c = 0; c < t.totalColumnsCountHighFive; c++)
                t.AlignCellHorizontalOnlyLikeLaserLevel(r, c, h);
    }

    public static void AlignTableVerticalOnlyLikeChoir(this Ux_TonkersTableTopiaLayout t, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
            for (int c = 0; c < t.totalColumnsCountHighFive; c++)
                t.AlignCellVerticalOnlyLikeLaserLevel(r, c, v);
    }

    public static void AlignTableToFillLikeBalloon(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return;
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
            for (int c = 0; c < t.totalColumnsCountHighFive; c++)
                t.AlignCellForeignsToFillLikeStuffedBurrito(r, c);
    }

    public static void AlignForeignersInRectLikeEtiquette(this RectTransform parent, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v, bool alignTextsToo = true)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            ApplyRectSinglePointAnchorLikeEtiquette(ch, h, v);
            if (!alignTextsToo) continue;

            var txt = ch.GetComponent<Text>();
            if (txt != null)
            {
                txt.alignment = MapToTextAnchorDad(h, v);
            }
        }
    }

    public static void AlignForeignersToFillLikeStuffedBurrito(this RectTransform parent, bool alignTextsToo = true)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            ch.anchorMin = Vector2.zero;
            ch.anchorMax = Vector2.one;
            ch.pivot = new Vector2(0.5f, 0.5f);
            ch.offsetMin = Vector2.zero;
            ch.offsetMax = Vector2.zero;
            ch.anchoredPosition = Vector2.zero;

            if (!alignTextsToo) continue;
            var txt = ch.GetComponent<Text>();
            if (txt != null) txt.alignment = TextAnchor.MiddleCenter;
        }
    }

    public static Vector2 GuessCellForeignersAnchorLikeDart(this Ux_TonkersTableTopiaLayout t, int row, int col, out bool fullStretch)
    {
        fullStretch = false;
        if (t == null) return new Vector2(0.5f, 0.5f);
        var rt = t.FetchCellRectTransformVIP(row, col);
        if (rt == null) return new Vector2(0.5f, 0.5f);

        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i) as RectTransform;
            if (ch == null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;

            if (NearlyVec2(ch.anchorMin, Vector2.zero) && NearlyVec2(ch.anchorMax, Vector2.one))
            {
                fullStretch = true;
                return new Vector2(0.5f, 0.5f);
            }

            if (NearlyVec2(ch.anchorMin, ch.anchorMax))
                return ch.anchorMin;

            return (ch.anchorMin + ch.anchorMax) * 0.5f;
        }

        return new Vector2(0.5f, 0.5f);
    }

    private static System.Collections.IEnumerator WaitAFrameAndFlagSpaDayLikeABoomer(Ux_TonkersTableTopiaLayout t, int id)
    {
        yield return null;
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        if (_layoutStateDadDiary.TryGetValue(id, out var state)) state.pendingDeferral = false;
    }

    private static TextAnchor WithHorizontalChangedLikeBarber(TextAnchor current, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        bool top = current == TextAnchor.UpperLeft || current == TextAnchor.UpperCenter || current == TextAnchor.UpperRight;
        bool mid = current == TextAnchor.MiddleLeft || current == TextAnchor.MiddleCenter || current == TextAnchor.MiddleRight;
        bool bot = current == TextAnchor.LowerLeft || current == TextAnchor.LowerCenter || current == TextAnchor.LowerRight;

        if (top)
            return h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left ? TextAnchor.UpperLeft :
                   h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center ? TextAnchor.UpperCenter : TextAnchor.UpperRight;

        if (mid)
            return h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left ? TextAnchor.MiddleLeft :
                   h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center ? TextAnchor.MiddleCenter : TextAnchor.MiddleRight;

        return h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left ? TextAnchor.LowerLeft :
               h == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center ? TextAnchor.LowerCenter : TextAnchor.LowerRight;
    }

    private static TextAnchor WithVerticalChangedLikeElevator(TextAnchor current, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        bool left = current == TextAnchor.UpperLeft || current == TextAnchor.MiddleLeft || current == TextAnchor.LowerLeft;
        bool center = current == TextAnchor.UpperCenter || current == TextAnchor.MiddleCenter || current == TextAnchor.LowerCenter;

        if (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top)
            return left ? TextAnchor.UpperLeft : (center ? TextAnchor.UpperCenter : TextAnchor.UpperRight);

        if (v == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle)
            return left ? TextAnchor.MiddleLeft : (center ? TextAnchor.MiddleCenter : TextAnchor.MiddleRight);

        return left ? TextAnchor.LowerLeft : (center ? TextAnchor.LowerCenter : TextAnchor.LowerRight);
    }

    public static float TallyLabelRowHogWidthLikeSumo(GUIStyle style, params string[] labels)
    {
        if (style == null) style = UnityEditor.EditorStyles.label;
        float w = 0f;
        if (labels != null)
        {
            for (int i = 0; i < labels.Length; i++)
            {
                var s = labels[i];
                if (string.IsNullOrEmpty(s)) continue;
                w += style.CalcSize(new GUIContent(s)).x;
            }
        }
        return w + 12f;
    }

    public static GameObject AddForeignKidToCellLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int row, int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, atSiblingIndex);
    }

    public static T AddForeignKidToCellLikeDoorDash<T>(this Ux_TonkersTableTopiaLayout t, int row, int col, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash<T>(snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstInCellLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int row, int col, GameObject prefab, bool snapToFill = true)
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastInCellLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int row, int col, GameObject prefab, bool snapToFill = true)
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, -1);
    }

    public static GameObject AddForeignKidToColumnAtRowLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int col, int row, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstRowInColumnLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int col, GameObject prefab, bool snapToFill = true)
    {
        if (t == null) return null;
        return t.AddForeignKidToColumnAtRowLikeDoorDash(col, 0, prefab, snapToFill, -1);
    }

    public static GameObject AddForeignLastRowInColumnLikeDoorDash(this Ux_TonkersTableTopiaLayout t, int col, GameObject prefab, bool snapToFill = true)
    {
        if (t == null) return null;
        int lastRow = Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1);
        return t.AddForeignKidToColumnAtRowLikeDoorDash(col, lastRow, prefab, snapToFill, -1);
    }

    public static Ux_TonkersTableTopiaLayout AddNestedTableToCellLikeRussianDoll(this Ux_TonkersTableTopiaLayout t, int row, int col, bool ensureSnapToFill = true)
    {
        if (t == null) return null;
        var kid = t.CreateChildTableInCellLikeABaby(row, col);
        if (kid != null && ensureSnapToFill)
        {
            var rt = kid.GetComponent<RectTransform>();
            if (rt != null) rt.SnapCroutonToFillParentLikeGravy();
            t.FlagLayoutAsNeedingSpaDay();
        }
        return kid;
    }

    public static GameObject AddForeignLikeOneLinerAt(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        if (row == null) return null;
        return row.AddForeignKidAtColumnLikeDoorDash(col, prefab, snapToFill, atSiblingIndex);
    }

    public static T AddForeignLikeOneLinerAt<T>(this Ux_TonkersTableTopiaRow row, int col, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        if (row == null) return null;
        return row.AddForeignKidAtColumnLikeDoorDash<T>(col, snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstLikeVIP(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true)
    {
        if (row == null) return null;
        return row.AddForeignKidAtColumnLikeDoorDash(col, prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastLikeClosingTime(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true)
    {
        if (row == null) return null;
        return row.AddForeignKidAtColumnLikeDoorDash(col, prefab, snapToFill, -1);
    }

    public static T AddForeignFirstLikeVIP<T>(this Ux_TonkersTableTopiaLayout t, int col, bool snapToFill = true) where T : Component
    {
        if (t == null) return null;
        return t.AddForeignKidToColumnAtRowLikeDoorDash<T>(col, 0, snapToFill, 0);
    }

    public static T AddForeignLastLikeClosingTime<T>(this Ux_TonkersTableTopiaLayout t, int col, bool snapToFill = true) where T : Component
    {
        if (t == null) return null;
        int lastRow = Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1);
        return t.AddForeignKidToColumnAtRowLikeDoorDash<T>(col, lastRow, snapToFill, -1);
    }

    public static T AddForeignKidToColumnAtRowLikeDoorDash<T>(this Ux_TonkersTableTopiaLayout t, int col, int row, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        if (t == null) return null;
        var cell = t.GetCellLikePizzaSlice(row, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash<T>(snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstLikeVIP(this Ux_TonkersTableTopiaCell cell, GameObject prefab, bool snapToFill = true)
    {
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastLikeClosingTime(this Ux_TonkersTableTopiaCell cell, GameObject prefab, bool snapToFill = true)
    {
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, -1);
    }

    public static T AddForeignFirstLikeVIP<T>(this Ux_TonkersTableTopiaCell cell, bool snapToFill = true) where T : Component
    {
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash<T>(snapToFill, 0);
    }

    public static T AddForeignLastLikeClosingTime<T>(this Ux_TonkersTableTopiaCell cell, bool snapToFill = true) where T : Component
    {
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash<T>(snapToFill, -1);
    }

    public static Ux_TonkersTableTopiaRow GetRowLikeBreadSlice(this Ux_TonkersTableTopiaLayout t, int row, bool createIfMissing = false)
    {
        if (t == null) return null;
        if (createIfMissing) t.GetCellLikePizzaSlice(row, 0, true);
        var rt = t.FetchRowRectTransformVIP(row);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public static bool TryGetRowLikePoliteWaiter(this Ux_TonkersTableTopiaLayout t, int row, out Ux_TonkersTableTopiaRow result)
    {
        result = t.GetRowLikeBreadSlice(row, false);
        return result != null;
    }

    public static List<Ux_TonkersTableTopiaRow> GetAllRowsLikeBakeryDozen(this Ux_TonkersTableTopiaLayout t)
    {
        var list = new List<Ux_TonkersTableTopiaRow>();
        if (t == null) return list;
        for (int r = 0; r < Mathf.Max(0, t.totalRowsCountLetTheShowBegin); r++)
        {
            var row = t.GetRowLikeBreadSlice(r, false);
            if (row != null) list.Add(row);
        }
        return list;
    }

    public static bool TryGetCellLikePoliteWaiter(this Ux_TonkersTableTopiaLayout t, int row, int col, out Ux_TonkersTableTopiaCell cell)
    {
        cell = null;
        if (t == null) return false;
        cell = t.GetCellLikePizzaSlice(row, col, false);
        return cell != null;
    }

    public static Ux_TonkersTableTopiaCell GetCellLikeMainCourseOnly(this Ux_TonkersTableTopiaLayout t, int row, int col)
    {
        if (t == null) return null;
        if (!t.TryPeekMainCourseLikeABuffet(row, col, out _, out _, out var main)) return null;
        return main;
    }

    public static List<Ux_TonkersTableTopiaCell> GetRowCellsLikeDonutFlight(this Ux_TonkersTableTopiaLayout t, int row, bool distinctMainsOnly = true)
    {
        var list = new List<Ux_TonkersTableTopiaCell>();
        if (t == null) return list;
        var seen = distinctMainsOnly ? new HashSet<Ux_TonkersTableTopiaCell>() : null;
        for (int c = 0; c < Mathf.Max(0, t.totalColumnsCountHighFive); c++)
        {
            if (!t.TryPeekMainCourseLikeABuffet(row, c, out _, out _, out var main)) continue;
            if (main == null) continue;
            if (seen != null)
            {
                if (seen.Add(main)) list.Add(main);
            }
            else list.Add(main);
        }
        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetColumnCellsLikeCornOnCob(this Ux_TonkersTableTopiaLayout t, int col, bool distinctMainsOnly = true)
    {
        var list = new List<Ux_TonkersTableTopiaCell>();
        if (t == null) return list;
        var seen = distinctMainsOnly ? new HashSet<Ux_TonkersTableTopiaCell>() : null;
        for (int r = 0; r < Mathf.Max(0, t.totalRowsCountLetTheShowBegin); r++)
        {
            if (!t.TryPeekMainCourseLikeABuffet(r, col, out _, out _, out var main)) continue;
            if (main == null) continue;
            if (seen != null)
            {
                if (seen.Add(main)) list.Add(main);
            }
            else list.Add(main);
        }
        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetAllCellsLikeBucketOfChicken(this Ux_TonkersTableTopiaLayout t, bool distinctMainsOnly = true)
    {
        var list = new List<Ux_TonkersTableTopiaCell>();
        if (t == null) return list;
        var seen = distinctMainsOnly ? new HashSet<Ux_TonkersTableTopiaCell>() : null;
        for (int r = 0; r < Mathf.Max(0, t.totalRowsCountLetTheShowBegin); r++)
        {
            for (int c = 0; c < Mathf.Max(0, t.totalColumnsCountHighFive); c++)
            {
                if (!t.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out var main)) continue;
                if (main == null) continue;
                if (seen != null)
                {
                    if (seen.Add(main)) list.Add(main);
                }
                else list.Add(main);
            }
        }
        return list;
    }

    public static List<Ux_TonkersTableTopiaRow> GetRowRangeLikeSubwaySixInch(this Ux_TonkersTableTopiaLayout t, int startRowInclusive, int endRowInclusive, bool createIfMissing = false)
    {
        var list = new List<Ux_TonkersTableTopiaRow>();
        if (t == null) return list;
        if (t.totalRowsCountLetTheShowBegin < 1) return list;
        int r0 = Mathf.Clamp(startRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        int r1 = Mathf.Clamp(endRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        if (r1 < r0) { var tmp = r0; r0 = r1; r1 = tmp; }
        for (int r = r0; r <= r1; r++)
        {
            var row = t.GetRowLikeBreadSlice(r, createIfMissing);
            if (row != null) list.Add(row);
        }
        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetColumnRangeCellsLikeCornField(this Ux_TonkersTableTopiaLayout t, int startColInclusive, int endColInclusive, bool distinctMainsOnly = true)
    {
        var list = new List<Ux_TonkersTableTopiaCell>();
        if (t == null) return list;
        if (t.totalColumnsCountHighFive < 1) return list;
        int c0 = Mathf.Clamp(startColInclusive, 0, t.totalColumnsCountHighFive - 1);
        int c1 = Mathf.Clamp(endColInclusive, 0, t.totalColumnsCountHighFive - 1);
        if (c1 < c0) { var tmp = c0; c0 = c1; c1 = tmp; }
        var seen = distinctMainsOnly ? new HashSet<Ux_TonkersTableTopiaCell>() : null;
        for (int c = c0; c <= c1; c++)
        {
            for (int r = 0; r < Mathf.Max(0, t.totalRowsCountLetTheShowBegin); r++)
            {
                if (!t.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out var main)) continue;
                if (main == null) continue;
                if (seen != null)
                {
                    if (seen.Add(main)) list.Add(main);
                }
                else list.Add(main);
            }
        }
        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetCellsRectLikePicnicBlanket(this Ux_TonkersTableTopiaLayout t, int startRow, int startCol, int rowCount, int colCount, bool expandToWholeMergers = true, bool distinctMainsOnly = true)
    {
        var list = new List<Ux_TonkersTableTopiaCell>();
        if (t == null) return list;
        if (rowCount < 1 || colCount < 1) return list;
        t.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        if (expandToWholeMergers) t.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        var seen = distinctMainsOnly ? new HashSet<Ux_TonkersTableTopiaCell>() : null;
        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (!t.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out var main)) continue;
                if (main == null) continue;
                if (seen != null)
                {
                    if (seen.Add(main)) list.Add(main);
                }
                else list.Add(main);
            }
        }
        return list;
    }

    public static Ux_TonkersTableTopiaRow GetFirstRowLikeEarlyBird(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null || t.totalRowsCountLetTheShowBegin < 1) return null;
        var rt = t.FetchRowRectTransformVIP(0);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public static Ux_TonkersTableTopiaRow GetLastRowLikeClosingTime(this Ux_TonkersTableTopiaLayout t)
    {
        if (t == null) return null;
        int last = Mathf.Max(0, t.totalRowsCountLetTheShowBegin - 1);
        return t.GetRowLikeBreadSlice(last, false);
    }

    public static Ux_TonkersTableTopiaRow GetRowLikeCornOnTheCob(this Ux_TonkersTableTopiaLayout t, int index)
    {
        if (t == null) return null;
        var rt = t.FetchRowRectTransformVIP(index);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    private static Ux_TonkersTableTopiaLayout TableFromRowLikeGPS(Ux_TonkersTableTopiaRow row)
    {
        return row != null ? row.GetTableLikeFamilyReunion() : null;
    }

    private static int ClampColumnLikeSeatbelt(Ux_TonkersTableTopiaLayout t, int col)
    {
        return Mathf.Clamp(col, 0, Mathf.Max(0, t.totalColumnsCountHighFive - 1));
    }

    public static RectTransform FetchCellRectTransformVIP(this Ux_TonkersTableTopiaRow row, int col)
    {
        var t = TableFromRowLikeGPS(row);
        if (t == null) return null;
        int r = row.rowNumberWhereShenanigansOccur;
        col = ClampColumnLikeSeatbelt(t, col);
        return t.FetchCellRectTransformVIP(r, col);
    }

    public static Ux_TonkersTableTopiaCell GetCellLikePizzaSlice(this Ux_TonkersTableTopiaRow row, int col, bool createIfMissing = false)
    {
        var t = TableFromRowLikeGPS(row);
        if (t == null) return null;
        int r = row.rowNumberWhereShenanigansOccur;
        col = ClampColumnLikeSeatbelt(t, col);
        return t.GetCellLikePizzaSlice(r, col, createIfMissing);
    }

    public static void GetAllCellRectsLikeSnackBar(this Ux_TonkersTableTopiaRow row, List<RectTransform> outList, bool includeInactive = true)
    {
        if (outList == null) return;
        outList.Clear();
        var t = TableFromRowLikeGPS(row);
        if (t == null) return;
        int r = row.rowNumberWhereShenanigansOccur;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++)
        {
            var rt = t.FetchCellRectTransformVIP(r, c);
            if (rt == null) continue;
            if (!includeInactive && !rt.gameObject.activeInHierarchy) continue;
            outList.Add(rt);
        }
    }

    public static void GetAllCellsLikeSnackBar(this Ux_TonkersTableTopiaRow row, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        if (outList == null) return;
        outList.Clear();
        var t = TableFromRowLikeGPS(row);
        if (t == null) return;
        int r = row.rowNumberWhereShenanigansOccur;
        for (int c = 0; c < t.totalColumnsCountHighFive; c++)
        {
            var rt = t.FetchCellRectTransformVIP(r, c);
            if (rt == null) continue;
            if (!includeInactive && !rt.gameObject.activeInHierarchy) continue;
            var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell != null) outList.Add(cell);
        }
    }

    public static void GetCellsInRangeLikeSamplerPlatter(this Ux_TonkersTableTopiaRow row, int startColInclusive, int endColInclusive, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        if (outList == null) return;
        outList.Clear();
        var t = TableFromRowLikeGPS(row);
        if (t == null) return;
        int r = row.rowNumberWhereShenanigansOccur;
        int c0 = Mathf.Min(startColInclusive, endColInclusive);
        int c1 = Mathf.Max(startColInclusive, endColInclusive);
        c0 = ClampColumnLikeSeatbelt(t, c0);
        c1 = ClampColumnLikeSeatbelt(t, c1);
        for (int c = c0; c <= c1; c++)
        {
            var rt = t.FetchCellRectTransformVIP(r, c);
            if (rt == null) continue;
            if (!includeInactive && !rt.gameObject.activeInHierarchy) continue;
            var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell != null) outList.Add(cell);
        }
    }

    public static void GetColumnOfCellRectsLikeSkyscraper(this Ux_TonkersTableTopiaRow row, int col, List<RectTransform> outList, bool includeInactive = true)
    {
        if (outList == null) return;
        outList.Clear();
        var t = TableFromRowLikeGPS(row);
        if (t == null) return;
        col = ClampColumnLikeSeatbelt(t, col);
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
        {
            var rt = t.FetchCellRectTransformVIP(r, col);
            if (rt == null) continue;
            if (!includeInactive && !rt.gameObject.activeInHierarchy) continue;
            outList.Add(rt);
        }
    }

    public static void GetColumnOfCellsLikeSkyscraper(this Ux_TonkersTableTopiaRow row, int col, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        if (outList == null) return;
        outList.Clear();
        var t = TableFromRowLikeGPS(row);
        if (t == null) return;
        col = ClampColumnLikeSeatbelt(t, col);
        for (int r = 0; r < t.totalRowsCountLetTheShowBegin; r++)
        {
            var rt = t.FetchCellRectTransformVIP(r, col);
            if (rt == null) continue;
            if (!includeInactive && !rt.gameObject.activeInHierarchy) continue;
            var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell != null) outList.Add(cell);
        }
    }

    public static int TotalColumnsLikeRollCall(this Ux_TonkersTableTopiaRow row)
    {
        var t = TableFromRowLikeGPS(row);
        return t != null ? Mathf.Max(0, t.totalColumnsCountHighFive) : 0;
    }

    public static bool TryPeekMainCourseLikeABuffet(this Ux_TonkersTableTopiaRow row, int col, out int mainRow, out int mainCol, out Ux_TonkersTableTopiaCell mainCell)
    {
        mainRow = -1;
        mainCol = -1;
        mainCell = null;
        var t = TableFromRowLikeGPS(row);
        if (t == null) return false;
        int r = row.rowNumberWhereShenanigansOccur;
        col = ClampColumnLikeSeatbelt(t, col);
        return t.TryPeekMainCourseLikeABuffet(r, col, out mainRow, out mainCol, out mainCell);
    }

    public static bool IsTonkersTableRoyaltyLikeVIP(this Component c)
    {
        if (c == null) return false;
        return c is Ux_TonkersTableTopiaLayout || c is Ux_TonkersTableTopiaRow || c is Ux_TonkersTableTopiaCell;
    }

    public static bool HasAnyTonkersTableRoyaltyLikeBouncer(this GameObject go)
    {
        if (go == null) return false;
        return go.GetComponent<Ux_TonkersTableTopiaLayout>() != null
            || go.GetComponent<Ux_TonkersTableTopiaRow>() != null
            || go.GetComponent<Ux_TonkersTableTopiaCell>() != null;
    }

    public static Component FindAnyTonkersTableRoyaltyLikeNeedle(this GameObject go)
    {
        if (go == null) return null;
        var a = go.GetComponent<Ux_TonkersTableTopiaLayout>();
        if (a != null) return a;
        var b = go.GetComponent<Ux_TonkersTableTopiaRow>();
        if (b != null) return b;
        var c = go.GetComponent<Ux_TonkersTableTopiaCell>();
        if (c != null) return c;
        return null;
    }

    public static bool IsAllowedSidekickForTableRoyaltyLikeChaperone(this Component c)
    {
        if (c == null) return false;
        var t = c.GetType();
        if (t == typeof(Transform)) return true;
        if (t == typeof(RectTransform)) return true;
        if (t == typeof(CanvasRenderer)) return true;
        return false;
    }

#if UNITY_EDITOR

    public static void DeferEditorSafe(System.Action actionLikeAGentleRain)
    {
        if (actionLikeAGentleRain == null) return;
        _bouncerDeferred ??= new System.Collections.Generic.Queue<System.Action>(8);
        _bouncerDeferred.Enqueue(actionLikeAGentleRain);
        if (_bouncerDelayScheduled) return;
        _bouncerDelayScheduled = true;
        UnityEditor.EditorApplication.delayCall += DrainBouncerDeferredLikeBathtub;
    }

    private static void DrainBouncerDeferredLikeBathtub()
    {
        try
        {
            while (_bouncerDeferred != null && _bouncerDeferred.Count > 0)
            {
                var a = _bouncerDeferred.Dequeue();
                try { a?.Invoke(); } catch { }
            }
        }
        finally
        {
            _bouncerDelayScheduled = false;
        }
    }

#endif

#if UNITY_EDITOR

    public static void DeferEditorSafeDestructionLikeASlowClap(params UnityEngine.Object[] victimsOfPoorLifeChoices)
    {
        if (victimsOfPoorLifeChoices == null || victimsOfPoorLifeChoices.Length == 0) return;
        DeferEditorSafe(() =>
        {
            for (int i = 0; i < victimsOfPoorLifeChoices.Length; i++)
            {
                var v = victimsOfPoorLifeChoices[i];
                if (v == null) continue;
                UnityEditor.Undo.DestroyObjectImmediate(v);
            }
        });
    }

#endif

#if UNITY_EDITOR

    public static void TryVacuumTableScaffoldLikeRoomba(Ux_TonkersTableTopiaLayout tableMaybeWithCrumbs)
    {
        if (tableMaybeWithCrumbs == null) return;
        var root = tableMaybeWithCrumbs.transform as RectTransform;
        if (root == null) return;

        var bag = new System.Collections.Generic.List<UnityEngine.Object>(64);

        for (int i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            if (child == null) continue;

            var row = child.GetComponent<Ux_TonkersTableTopiaRow>();
            var cell = child.GetComponent<Ux_TonkersTableTopiaCell>();

            if (row != null)
            {
                bag.Add(child.gameObject);
                continue;
            }

            if (cell != null)
            {
                bag.Add(child.gameObject);
                continue;
            }
        }

        if (bag.Count > 0)
        {
            DeferEditorSafe(() =>
            {
                for (int i = 0; i < bag.Count; i++)
                {
                    var o = bag[i];
                    if (o == null) continue;
                    UnityEditor.Undo.DestroyObjectImmediate(o);
                }
            });
        }
    }

#endif
}
