using UnityEngine;
using UnityEngine.UI;

public static class Ux_TonkersTableTopiaLayoutEditingExtensions
{
#if UNITY_EDITOR

    public static void SelectAndPingLikeABeacon(this Object target)
    {
        if (target == null)
        {
            return;
        }

        UnityEditor.Selection.activeObject = target;
        UnityEditor.EditorGUIUtility.PingObject(target);
    }

#endif

    public static bool BulkDeleteColumnsLikeAChamp(this Ux_TonkersTableTopiaLayout table, int startColInclusive, int endColInclusive)
    {
        return BulkDeleteColumnsLikeAChamp(table, startColInclusive, endColInclusive, table != null && table.preserveExistingColumnWidthsWhenAddingOrDeleting);
    }

    public static bool BulkDeleteColumnsLikeAChamp(this Ux_TonkersTableTopiaLayout table, int startColInclusive, int endColInclusive, bool preserveExistingColumnWidths)
    {
        if (table == null || table.totalColumnsCountHighFive <= 1)
        {
            return false;
        }

        return TryBulkDeleteRange(
            table.totalColumnsCountHighFive,
            startColInclusive,
            endColInclusive,
            col => IsColumnDeletionBlockedByMergers(table, col),
            col => table.TryPolitelyDeleteColumn(col, preserveExistingColumnWidths),
            table);
    }

    public static bool BulkDeleteRowsLikeABoss(this Ux_TonkersTableTopiaLayout table, int startRowInclusive, int endRowInclusive)
    {
        return BulkDeleteRowsLikeABoss(table, startRowInclusive, endRowInclusive, table != null && table.preserveExistingRowHeightsWhenAddingOrDeleting);
    }

    public static bool BulkDeleteRowsLikeABoss(this Ux_TonkersTableTopiaLayout table, int startRowInclusive, int endRowInclusive, bool preserveExistingRowHeights)
    {
        if (table == null || table.totalRowsCountLetTheShowBegin <= 1)
        {
            return false;
        }

        return TryBulkDeleteRange(
            table.totalRowsCountLetTheShowBegin,
            startRowInclusive,
            endRowInclusive,
            row => IsRowDeletionBlockedByMergers(table, row),
            row => table.TryPolitelyDeleteRow(row, preserveExistingRowHeights),
            table);
    }

    public static bool CanMergeThisPicnicBlanket(this Ux_TonkersTableTopiaLayout table, int startRow, int startCol, int rowCount, int colCount)
    {
        if (table == null || rowCount < 1 || colCount < 1)
        {
            return false;
        }

        table.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        table.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);

        if (rowCount * colCount <= 1)
        {
            return false;
        }

        if (AreAllSeatsOwnedByOneHeadHoncho(table, startRow, startCol, rowCount, colCount, out Ux_TonkersTableTopiaCell boss))
        {
            if (boss != null
                && boss.rowNumberWhereThePartyIs == startRow
                && boss.columnNumberPrimeRib == startCol
                && Mathf.Max(1, boss.howManyRowsAreHoggingThisSeat) == rowCount
                && Mathf.Max(1, boss.howManyColumnsAreSneakingIn) == colCount)
            {
                return false;
            }
        }

        return true;
    }

    public static bool CanUnmergeThisFoodFight(this Ux_TonkersTableTopiaLayout table, int startRow, int startCol, int rowCount, int colCount)
    {
        if (table == null || rowCount < 1 || colCount < 1)
        {
            return false;
        }

        table.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        table.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);

        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (!table.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out Ux_TonkersTableTopiaCell main))
                {
                    continue;
                }

                if (main != null && (main.howManyRowsAreHoggingThisSeat > 1 || main.howManyColumnsAreSneakingIn > 1))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void ClampRectToTableLikeASensibleSeatbelt(this Ux_TonkersTableTopiaLayout table, ref int startRow, ref int startCol, ref int rowCount, ref int colCount)
    {
        if (table == null)
        {
            return;
        }

        int maxRows = Mathf.Max(1, table.totalRowsCountLetTheShowBegin);
        int maxCols = Mathf.Max(1, table.totalColumnsCountHighFive);

        startRow = Mathf.Clamp(startRow, 0, maxRows - 1);
        startCol = Mathf.Clamp(startCol, 0, maxCols - 1);

        int lastRow = Mathf.Clamp(startRow + Mathf.Max(1, rowCount) - 1, startRow, maxRows - 1);
        int lastCol = Mathf.Clamp(startCol + Mathf.Max(1, colCount) - 1, startCol, maxCols - 1);

        rowCount = lastRow - startRow + 1;
        colCount = lastCol - startCol + 1;
    }

    public static void ExpandRectToWholeMergersLikeACarpenter(this Ux_TonkersTableTopiaLayout table, ref int startRow, ref int startCol, ref int rowCount, ref int colCount)
    {
        if (table == null)
        {
            return;
        }

        table.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);

        int r0 = startRow;
        int c0 = startCol;
        int r1 = startRow + rowCount - 1;
        int c1 = startCol + colCount - 1;

        bool changed;
        int guard = 0;

        do
        {
            changed = false;

            for (int r = r0; r <= r1; r++)
            {
                for (int c = c0; c <= c1; c++)
                {
                    if (!table.TryPeekMainCourseLikeABuffet(r, c, out int mainRow, out int mainCol, out Ux_TonkersTableTopiaCell main))
                    {
                        continue;
                    }

                    if (main == null)
                    {
                        continue;
                    }

                    int endRow = mainRow + Mathf.Max(1, main.howManyRowsAreHoggingThisSeat) - 1;
                    int endCol = mainCol + Mathf.Max(1, main.howManyColumnsAreSneakingIn) - 1;

                    if (mainRow < r0 || mainCol < c0 || endRow > r1 || endCol > c1)
                    {
                        r0 = Mathf.Max(0, Mathf.Min(r0, mainRow));
                        c0 = Mathf.Max(0, Mathf.Min(c0, mainCol));
                        r1 = Mathf.Min(table.totalRowsCountLetTheShowBegin - 1, Mathf.Max(r1, endRow));
                        c1 = Mathf.Min(table.totalColumnsCountHighFive - 1, Mathf.Max(c1, endCol));
                        changed = true;
                    }
                }
            }

            guard++;
        }
        while (changed && guard <= 512);

        startRow = r0;
        startCol = c0;
        rowCount = r1 - r0 + 1;
        colCount = c1 - c0 + 1;
    }

    public static RectTransform FindFirstAwakeCellInColumnLikeCoffee(this Ux_TonkersTableTopiaLayout table, int col)
    {
        if (table == null || col < 0 || col >= table.totalColumnsCountHighFive)
        {
            return null;
        }

        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            RectTransform rectTransform = table.FetchCellRectTransformVIP(r, col);
            Ux_TonkersTableTopiaCell cell = table.FetchCellComponentVIP(r, col);

            if (rectTransform == null || cell == null)
            {
                continue;
            }

            if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null)
            {
                continue;
            }

            if (!rectTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            return rectTransform;
        }

        return null;
    }

    public static Ux_TonkersTableTopiaCell GrabCellLikeItOwesYouRent(this Ux_TonkersTableTopiaLayout table, int row, int col)
    {
        if (table == null)
        {
            return null;
        }

        return table.FetchCellComponentVIP(row, col);
    }

    public static bool TryPeekMainCourseLikeABuffet(this Ux_TonkersTableTopiaLayout table, int row, int col, out int mainRow, out int mainCol, out Ux_TonkersTableTopiaCell mainCell)
    {
        mainRow = row;
        mainCol = col;
        mainCell = null;

        if (table == null)
        {
            return false;
        }

        Ux_TonkersTableTopiaCell cell = table.GrabCellLikeItOwesYouRent(row, col);

        if (cell == null)
        {
            return false;
        }

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

    public static Ux_TonkersTableTopiaLayout FindParentTableLikeFamilyTree(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return null;
        }

        Ux_TonkersTableTopiaExtensionInternals.childTableScratch.Clear();
        table.GetComponentsInParent(true, Ux_TonkersTableTopiaExtensionInternals.childTableScratch);

        for (int i = 0; i < Ux_TonkersTableTopiaExtensionInternals.childTableScratch.Count; i++)
        {
            Ux_TonkersTableTopiaLayout parentTable = Ux_TonkersTableTopiaExtensionInternals.childTableScratch[i];

            if (parentTable != null && parentTable != table)
            {
                return parentTable;
            }
        }

        return null;
    }

    public static Ux_TonkersTableTopiaLayout FindFirstChildTableLikeEasterEgg(this RectTransform parent, bool includeInactive = true)
    {
        if (parent == null)
        {
            return null;
        }

        Ux_TonkersTableTopiaExtensionInternals.childTableScratch.Clear();

#if UNITY_2021_1_OR_NEWER
        parent.GetComponentsInChildren(includeInactive, Ux_TonkersTableTopiaExtensionInternals.childTableScratch);
#else
        Ux_TonkersTableTopiaLayout[] children = parent.GetComponentsInChildren<Ux_TonkersTableTopiaLayout>(includeInactive);
        Ux_TonkersTableTopiaExtensionInternals.childTableScratch.AddRange(children);
#endif

        for (int i = 0; i < Ux_TonkersTableTopiaExtensionInternals.childTableScratch.Count; i++)
        {
            Ux_TonkersTableTopiaLayout childTable = Ux_TonkersTableTopiaExtensionInternals.childTableScratch[i];

            if (childTable != null && childTable.transform != parent.transform)
            {
                return childTable;
            }
        }

        return null;
    }

    public static void SetTableBackgroundImage(
        this Ux_TonkersTableTopiaLayout table,
        Sprite sprite,
        Color? tint = null,
        bool useSliced = true,
        bool raycastTarget = false,
        bool disableRowAndColumnBackdrops = true,
        bool disableZebraStripes = true)
    {
        if (table == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(table.gameObject, sprite != null ? "Set Table Background Image" : "Clear Table Background Image");
        }
#endif

        RectTransform rectTransform = table.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            return;
        }

        Image image = rectTransform.FlipImageComponentLikeALightSwitch(sprite != null);

        if (image != null)
        {
            image.sprite = sprite;
            image.type = useSliced ? Image.Type.Sliced : Image.Type.Simple;
            image.raycastTarget = raycastTarget;

            Color color = tint ?? Color.white;

            if (color.a <= 0f)
            {
                color.a = 1f;
            }

            image.color = color;
        }

        if (disableZebraStripes)
        {
            table.toggleZebraStripesForRows = false;
            table.toggleZebraStripesForColumns = false;
        }

        if (disableRowAndColumnBackdrops)
        {
            table.SyncRowWardrobes();
            table.SyncColumnWardrobes();

            for (int i = 0; i < table.snazzyRowWardrobes.Count; i++)
            {
                table.snazzyRowWardrobes[i].backdropPictureOnTheHouse = null;
            }

            for (int i = 0; i < table.fancyColumnWardrobes.Count; i++)
            {
                table.fancyColumnWardrobes[i].backdropPictureOnTheHouse = null;
            }
        }

        table.FlagLayoutAsNeedingSpaDay();
    }

    public static void ClearTableBackgroundImage(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RegisterFullObjectHierarchyUndo(table.gameObject, "Clear Table Background Image");
        }
#endif

        RectTransform rectTransform = table.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            return;
        }

        rectTransform.FlipImageComponentLikeALightSwitch(false);
        table.FlagLayoutAsNeedingSpaDay();
    }

    public static bool TryResolveCellBackgroundVisualLikeSherlock(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaCell cell, out Sprite sprite, out Color tint, out bool useSliced)
    {
        sprite = null;
        tint = Color.clear;
        useSliced = true;

        if (table == null || cell == null)
        {
            return false;
        }

        cell = Ux_TonkersTableTopiaExtensionInternals.ResolveMainCell(cell);

        if (cell.backgroundPictureBecausePlainIsLame != null)
        {
            sprite = cell.backgroundPictureBecausePlainIsLame;
            tint = cell.backgroundColorLikeASunset;
            useSliced = cell.backgroundPictureUseSlicedLikePizza;
            return true;
        }

        if (table.IsUsingOneBigColumnBackdropLikeWallpaper(cell))
        {
            return false;
        }

        if (table.TryResolveColumnBackdropAcrossSpanLikeSherlock(cell, out sprite, out tint, out useSliced))
        {
            return true;
        }

        bool useRowZebra = table.toggleZebraStripesForRows;
        bool useColumnZebra = table.toggleZebraStripesForColumns;

        if (!useRowZebra && !useColumnZebra)
        {
            return false;
        }

        int safeRow = Mathf.Max(0, cell.rowNumberWhereThePartyIs);
        int safeCol = Mathf.Max(0, cell.columnNumberPrimeRib);

        Color rowColor = ((safeRow & 1) == 0) ? table.zebraRowColorA : table.zebraRowColorB;
        Color columnColor = ((safeCol & 1) == 0) ? table.zebraColumnColorA : table.zebraColumnColorB;

        tint = useRowZebra && useColumnZebra
            ? new Color((rowColor.r + columnColor.r) * 0.5f, (rowColor.g + columnColor.g) * 0.5f, (rowColor.b + columnColor.b) * 0.5f, Mathf.Max(rowColor.a, columnColor.a))
            : (useRowZebra ? rowColor : columnColor);

        useSliced = true;
        return true;
    }

    public static bool TryResolveColumnBackdropAcrossSpanLikeSherlock(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaCell cell, out Sprite sprite, out Color tint, out bool useSliced)
    {
        sprite = null;
        tint = Color.clear;
        useSliced = true;

        if (table == null || cell == null)
        {
            return false;
        }

        cell = Ux_TonkersTableTopiaExtensionInternals.ResolveMainCell(cell);
        table.SyncColumnWardrobes();

        int startCol = Mathf.Clamp(cell.columnNumberPrimeRib, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        int spanCols = Mathf.Clamp(Mathf.Max(1, cell.howManyColumnsAreSneakingIn), 1, Mathf.Max(1, table.totalColumnsCountHighFive - startCol));

        for (int c = startCol; c < startCol + spanCols; c++)
        {
            if (c < 0 || c >= table.fancyColumnWardrobes.Count)
            {
                continue;
            }

            var style = table.fancyColumnWardrobes[c];

            if (style == null || style.backdropPictureOnTheHouse == null || style.useOneBigBackdropForWholeColumn)
            {
                continue;
            }

            sprite = style.backdropPictureOnTheHouse;
            tint = style.backdropTintFlavor;
            useSliced = style.backdropUseSlicedLikePizza;
            return true;
        }

        return false;
    }

    public static bool IsUsingOneBigColumnBackdropLikeWallpaper(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaCell cell)
    {
        if (table == null || cell == null)
        {
            return false;
        }

        cell = Ux_TonkersTableTopiaExtensionInternals.ResolveMainCell(cell);
        table.SyncColumnWardrobes();

        int startCol = Mathf.Clamp(cell.columnNumberPrimeRib, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        int spanCols = Mathf.Clamp(Mathf.Max(1, cell.howManyColumnsAreSneakingIn), 1, Mathf.Max(1, table.totalColumnsCountHighFive - startCol));

        for (int c = startCol; c < startCol + spanCols; c++)
        {
            if (c < 0 || c >= table.fancyColumnWardrobes.Count)
            {
                continue;
            }

            var style = table.fancyColumnWardrobes[c];

            if (style != null && style.backdropPictureOnTheHouse != null && style.useOneBigBackdropForWholeColumn)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryBulkDeleteRange(
        int totalCount,
        int startInclusive,
        int endInclusive,
        System.Func<int, bool> isBlocked,
        System.Func<int, bool> deleteAt,
        Ux_TonkersTableTopiaLayout table)
    {
        int min = Mathf.Clamp(startInclusive, 0, totalCount - 1);
        int max = Mathf.Clamp(endInclusive, 0, totalCount - 1);

        if (max < min)
        {
            int temp = min;
            min = max;
            max = temp;
        }

        int wanted = max - min + 1;

        if (wanted >= totalCount)
        {
            max = min + (totalCount - 2);
        }

        if (max < min)
        {
            return false;
        }

        for (int i = min; i <= max; i++)
        {
            if (isBlocked(i))
            {
                return false;
            }
        }

        for (int i = max; i >= min; i--)
        {
            if (!deleteAt(i))
            {
                return false;
            }
        }

        table.FlagLayoutAsNeedingSpaDay();
        return true;
    }

    private static bool AreAllSeatsOwnedByOneHeadHoncho(Ux_TonkersTableTopiaLayout table, int r0, int c0, int rowCount, int colCount, out Ux_TonkersTableTopiaCell boss)
    {
        boss = null;

        if (table == null)
        {
            return false;
        }

        if (!table.TryPeekMainCourseLikeABuffet(r0, c0, out _, out _, out Ux_TonkersTableTopiaCell firstMain) || firstMain == null)
        {
            return false;
        }

        boss = firstMain;

        for (int r = r0; r < r0 + rowCount; r++)
        {
            for (int c = c0; c < c0 + colCount; c++)
            {
                if (!table.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out Ux_TonkersTableTopiaCell main) || main != boss)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool IsColumnDeletionBlockedByMergers(Ux_TonkersTableTopiaLayout table, int colIndex)
    {
        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            Ux_TonkersTableTopiaCell cell = table.GrabCellLikeItOwesYouRent(r, colIndex);

            if (cell == null || cell.isMashedLikePotatoes)
            {
                continue;
            }

            int start = cell.columnNumberPrimeRib;
            int end = start + Mathf.Max(1, cell.howManyColumnsAreSneakingIn) - 1;

            if (cell.howManyColumnsAreSneakingIn > 1 && colIndex >= start && colIndex <= end)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsRowDeletionBlockedByMergers(Ux_TonkersTableTopiaLayout table, int rowIndex)
    {
        for (int c = 0; c < table.totalColumnsCountHighFive; c++)
        {
            Ux_TonkersTableTopiaCell cell = table.GrabCellLikeItOwesYouRent(rowIndex, c);

            if (cell == null || cell.isMashedLikePotatoes)
            {
                continue;
            }

            int start = cell.rowNumberWhereThePartyIs;
            int end = start + Mathf.Max(1, cell.howManyRowsAreHoggingThisSeat) - 1;

            if (cell.howManyRowsAreHoggingThisSeat > 1 && rowIndex >= start && rowIndex <= end)
            {
                return true;
            }
        }

        return false;
    }
}