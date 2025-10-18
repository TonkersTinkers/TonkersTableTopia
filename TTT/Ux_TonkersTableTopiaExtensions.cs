using UnityEngine;
using UnityEngine.UI;

public static class Ux_TonkersTableTopiaExtensions
{
#if UNITY_EDITOR

    public static void SelectAndPingLikeABeacon(this UnityEngine.Object target)
    {
        if (target == null) return;
        UnityEditor.Selection.activeObject = target;
        UnityEditor.EditorGUIUtility.PingObject(target);
    }

#endif

    private static readonly System.Collections.Generic.Dictionary<RectTransform, Vector2> _dadBodParentMemo = new System.Collections.Generic.Dictionary<RectTransform, Vector2>();

    public static bool BulkDeleteColumnsLikeAChamp(this Ux_TonkersTableTopiaLayout t, int startColInclusive, int endColInclusive)
    {
        if (t == null) return false;
        if (t.totalColumnsCountHighFive <= 1) return false;

        int c0 = Mathf.Clamp(startColInclusive, 0, t.totalColumnsCountHighFive - 1);
        int c1 = Mathf.Clamp(endColInclusive, 0, t.totalColumnsCountHighFive - 1);
        if (c1 < c0) { var tmp = c0; c0 = c1; c1 = tmp; }

        int want = c1 - c0 + 1;
        if (want >= t.totalColumnsCountHighFive) c1 = c0 + (t.totalColumnsCountHighFive - 2);
        if (c1 < c0) return false;

        for (int c = c0; c <= c1; c++)
            if (IsColumnDeletionBlockedByMergers(t, c)) return false;

        for (int c = c1; c >= c0; c--)
            if (!t.TryPolitelyDeleteColumn(c)) return false;

        t.FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public static bool BulkDeleteRowsLikeABoss(this Ux_TonkersTableTopiaLayout t, int startRowInclusive, int endRowInclusive)
    {
        if (t == null) return false;
        if (t.totalRowsCountLetTheShowBegin <= 1) return false;

        int r0 = Mathf.Clamp(startRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        int r1 = Mathf.Clamp(endRowInclusive, 0, t.totalRowsCountLetTheShowBegin - 1);
        if (r1 < r0) { var tmp = r0; r0 = r1; r1 = tmp; }

        int want = r1 - r0 + 1;
        if (want >= t.totalRowsCountLetTheShowBegin) r1 = r0 + (t.totalRowsCountLetTheShowBegin - 2);
        if (r1 < r0) return false;

        for (int r = r0; r <= r1; r++)
            if (IsRowDeletionBlockedByMergers(t, r)) return false;

        for (int r = r1; r >= r0; r--)
            if (!t.TryPolitelyDeleteRow(r)) return false;

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
            if (boss != null &&
                boss.rowNumberWhereThePartyIs == startRow &&
                boss.columnNumberPrimeRib == startCol &&
                Mathf.Max(1, boss.howManyRowsAreHoggingThisSeat) == rowCount &&
                Mathf.Max(1, boss.howManyColumnsAreSneakingIn) == colCount)
                return false;
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
                if (main != null && (main.howManyRowsAreHoggingThisSeat > 1 || main.howManyColumnsAreSneakingIn > 1))
                    return true;
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

    public static float[] DistributeLikeACaterer(int count, System.Func<int, float> getSpec, float spacing, float inner)
    {
        var result = new float[count];
        var fixedPx = new float[count];
        var perc = new float[count];
        var flexShares = new int[count];
        float fixedSum = 0f;
        float percSum = 0f;
        int flexCount = 0;

        for (int i = 0; i < count; i++)
        {
            float v = getSpec != null ? getSpec(i) : 0f;
            if (v > 0f)
            {
                fixedPx[i] = v;
                fixedSum += v;
            }
            else if (v < 0f)
            {
                float p = Mathf.Clamp01(-v);
                perc[i] = p;
                percSum += p;
            }
            else
            {
                flexShares[i] = 1;
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
                if (perc[i] > 0f)
                {
                    float a = remain * (perc[i] * norm);
                    result[i] = fixedPx[i] + a;
                    usedByPerc += a;
                }
            }
        }

        float residual = Mathf.Max(0f, remain - usedByPerc);

        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int i = 0; i < count; i++)
                result[i] = result[i] > 0f ? result[i] : (fixedPx[i] + share);
        }
        else
        {
            for (int i = 0; i < count; i++)
                result[i] = result[i] > 0f ? result[i] : fixedPx[i];
            if (count > 0 && residual > 0f)
                result[count - 1] += residual;
        }

        for (int i = 0; i < count; i++)
            if (result[i] < 0f) result[i] = 0f;

        return result;
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
        offMin.x = 0f; offMin.y = 0f;
        offMax.x = 0f; offMax.y = 0f;
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
}
