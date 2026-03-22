using System.Collections.Generic;
using UnityEngine;

public static class Ux_TonkersTableTopiaLayoutQueryExtensions
{
    public static Ux_TonkersTableTopiaRow GetRowLikeBreadSlice(this Ux_TonkersTableTopiaLayout table, int row, bool createIfMissing = false)
    {
        if (table == null)
        {
            return null;
        }

        if (createIfMissing)
        {
            table.GetCellLikePizzaSlice(row, 0, true);
        }

        return table.FetchRowComponentVIP(row);
    }

    public static bool TryGetRowLikePoliteWaiter(this Ux_TonkersTableTopiaLayout table, int row, out Ux_TonkersTableTopiaRow result)
    {
        result = table.GetRowLikeBreadSlice(row, false);
        return result != null;
    }

    public static List<Ux_TonkersTableTopiaRow> GetAllRowsLikeBakeryDozen(this Ux_TonkersTableTopiaLayout table)
    {
        List<Ux_TonkersTableTopiaRow> list = new List<Ux_TonkersTableTopiaRow>();
        table.GetAllRowsLikeBakeryDozen(list);
        return list;
    }

    public static void GetAllRowsLikeBakeryDozen(this Ux_TonkersTableTopiaLayout table, List<Ux_TonkersTableTopiaRow> outList)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (table == null)
        {
            return;
        }

        int rowCount = Mathf.Max(0, table.totalRowsCountLetTheShowBegin);

        for (int r = 0; r < rowCount; r++)
        {
            Ux_TonkersTableTopiaRow row = table.GetRowLikeBreadSlice(r, false);

            if (row != null)
            {
                outList.Add(row);
            }
        }
    }

    public static bool TryGetCellLikePoliteWaiter(this Ux_TonkersTableTopiaLayout table, int row, int col, out Ux_TonkersTableTopiaCell cell)
    {
        cell = null;

        if (table == null)
        {
            return false;
        }

        cell = table.GetCellLikePizzaSlice(row, col, false);
        return cell != null;
    }

    public static Ux_TonkersTableTopiaCell GetCellLikeMainCourseOnly(this Ux_TonkersTableTopiaLayout table, int row, int col)
    {
        if (table == null)
        {
            return null;
        }

        if (!table.TryPeekMainCourseLikeABuffet(row, col, out _, out _, out Ux_TonkersTableTopiaCell main))
        {
            return null;
        }

        return main;
    }

    public static List<Ux_TonkersTableTopiaCell> GetRowCellsLikeDonutFlight(this Ux_TonkersTableTopiaLayout table, int row, bool distinctMainsOnly = true)
    {
        List<Ux_TonkersTableTopiaCell> list = new List<Ux_TonkersTableTopiaCell>();
        table.GetRowCellsLikeDonutFlight(row, list, distinctMainsOnly);
        return list;
    }

    public static void GetRowCellsLikeDonutFlight(this Ux_TonkersTableTopiaLayout table, int row, List<Ux_TonkersTableTopiaCell> outList, bool distinctMainsOnly = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (table == null)
        {
            return;
        }

        CollectMainCells(table, row, row, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), outList, false, distinctMainsOnly);
    }

    public static List<Ux_TonkersTableTopiaCell> GetColumnCellsLikeCornOnCob(this Ux_TonkersTableTopiaLayout table, int col, bool distinctMainsOnly = true)
    {
        List<Ux_TonkersTableTopiaCell> list = new List<Ux_TonkersTableTopiaCell>();
        table.GetColumnCellsLikeCornOnCob(col, list, distinctMainsOnly);
        return list;
    }

    public static void GetColumnCellsLikeCornOnCob(this Ux_TonkersTableTopiaLayout table, int col, List<Ux_TonkersTableTopiaCell> outList, bool distinctMainsOnly = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (table == null)
        {
            return;
        }

        CollectMainCells(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), col, col, outList, false, distinctMainsOnly);
    }

    public static List<Ux_TonkersTableTopiaCell> GetAllCellsLikeBucketOfChicken(this Ux_TonkersTableTopiaLayout table, bool distinctMainsOnly = true)
    {
        List<Ux_TonkersTableTopiaCell> list = new List<Ux_TonkersTableTopiaCell>();
        table.GetAllCellsLikeBucketOfChicken(list, distinctMainsOnly);
        return list;
    }

    public static void GetAllCellsLikeBucketOfChicken(this Ux_TonkersTableTopiaLayout table, List<Ux_TonkersTableTopiaCell> outList, bool distinctMainsOnly = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (table == null)
        {
            return;
        }

        CollectMainCells(
            table,
            0,
            Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1),
            0,
            Mathf.Max(0, table.totalColumnsCountHighFive - 1),
            outList,
            false,
            distinctMainsOnly);
    }

    public static List<Ux_TonkersTableTopiaRow> GetRowRangeLikeSubwaySixInch(this Ux_TonkersTableTopiaLayout table, int startRowInclusive, int endRowInclusive, bool createIfMissing = false)
    {
        List<Ux_TonkersTableTopiaRow> list = new List<Ux_TonkersTableTopiaRow>();

        if (table == null || table.totalRowsCountLetTheShowBegin < 1)
        {
            return list;
        }

        if (!Ux_TonkersTableTopiaExtensionInternals.TryNormalizeInclusiveRange(startRowInclusive, endRowInclusive, table.totalRowsCountLetTheShowBegin, out int minRow, out int maxRow))
        {
            return list;
        }

        for (int r = minRow; r <= maxRow; r++)
        {
            Ux_TonkersTableTopiaRow row = table.GetRowLikeBreadSlice(r, createIfMissing);

            if (row != null)
            {
                list.Add(row);
            }
        }

        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetColumnRangeCellsLikeCornField(this Ux_TonkersTableTopiaLayout table, int startColInclusive, int endColInclusive, bool distinctMainsOnly = true)
    {
        List<Ux_TonkersTableTopiaCell> list = new List<Ux_TonkersTableTopiaCell>();

        if (table == null || table.totalColumnsCountHighFive < 1)
        {
            return list;
        }

        if (!Ux_TonkersTableTopiaExtensionInternals.TryNormalizeInclusiveRange(startColInclusive, endColInclusive, table.totalColumnsCountHighFive, out int minCol, out int maxCol))
        {
            return list;
        }

        CollectMainCells(
            table,
            0,
            Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1),
            minCol,
            maxCol,
            list,
            false,
            distinctMainsOnly);

        return list;
    }

    public static List<Ux_TonkersTableTopiaCell> GetCellsRectLikePicnicBlanket(this Ux_TonkersTableTopiaLayout table, int startRow, int startCol, int rowCount, int colCount, bool expandToWholeMergers = true, bool distinctMainsOnly = true)
    {
        List<Ux_TonkersTableTopiaCell> list = new List<Ux_TonkersTableTopiaCell>();
        table.GetCellsRectLikePicnicBlanket(startRow, startCol, rowCount, colCount, list, expandToWholeMergers, distinctMainsOnly);
        return list;
    }

    public static void GetCellsRectLikePicnicBlanket(this Ux_TonkersTableTopiaLayout table, int startRow, int startCol, int rowCount, int colCount, List<Ux_TonkersTableTopiaCell> outList, bool expandToWholeMergers = true, bool distinctMainsOnly = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (table == null || rowCount < 1 || colCount < 1)
        {
            return;
        }

        table.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);

        if (expandToWholeMergers)
        {
            table.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        }

        CollectMainCells(table, startRow, startRow + rowCount - 1, startCol, startCol + colCount - 1, outList, false, distinctMainsOnly);
    }

    public static Ux_TonkersTableTopiaRow GetFirstRowLikeEarlyBird(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null || table.totalRowsCountLetTheShowBegin < 1)
        {
            return null;
        }

        RectTransform rectTransform = table.FetchRowRectTransformVIP(0);
        return rectTransform != null ? rectTransform.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public static Ux_TonkersTableTopiaRow GetLastRowLikeClosingTime(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return null;
        }

        int last = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        return table.GetRowLikeBreadSlice(last, false);
    }

    public static Ux_TonkersTableTopiaRow GetRowLikeCornOnTheCob(this Ux_TonkersTableTopiaLayout table, int index)
    {
        if (table == null)
        {
            return null;
        }

        RectTransform rectTransform = table.FetchRowRectTransformVIP(index);
        return rectTransform != null ? rectTransform.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public static RectTransform FetchCellRectTransformVIP(this Ux_TonkersTableTopiaRow row, int col)
    {
        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return null;
        }

        int rowIndex = row.rowNumberWhereShenanigansOccur;
        int safeColumn = Ux_TonkersTableTopiaExtensionInternals.ClampColumn(table, col);
        return table.FetchCellRectTransformVIP(rowIndex, safeColumn);
    }

    public static Ux_TonkersTableTopiaCell GetCellLikePizzaSlice(this Ux_TonkersTableTopiaRow row, int col, bool createIfMissing = false)
    {
        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return null;
        }

        int rowIndex = row.rowNumberWhereShenanigansOccur;
        int safeColumn = Ux_TonkersTableTopiaExtensionInternals.ClampColumn(table, col);
        return table.GetCellLikePizzaSlice(rowIndex, safeColumn, createIfMissing);
    }

    public static void GetAllCellRectsLikeSnackBar(this Ux_TonkersTableTopiaRow row, List<RectTransform> outList, bool includeInactive = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return;
        }

        int rowIndex = row.rowNumberWhereShenanigansOccur;

        for (int c = 0; c < table.totalColumnsCountHighFive; c++)
        {
            RectTransform rectTransform = table.FetchCellRectTransformVIP(rowIndex, c);

            if (rectTransform == null)
            {
                continue;
            }

            if (!includeInactive && !rectTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            outList.Add(rectTransform);
        }
    }

    public static void GetAllCellsLikeSnackBar(this Ux_TonkersTableTopiaRow row, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        CollectRowCells(row, 0, int.MaxValue, outList, includeInactive);
    }

    public static void GetCellsInRangeLikeSamplerPlatter(this Ux_TonkersTableTopiaRow row, int startColInclusive, int endColInclusive, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        CollectRowCells(row, startColInclusive, endColInclusive, outList, includeInactive);
    }

    public static void GetColumnOfCellRectsLikeSkyscraper(this Ux_TonkersTableTopiaRow row, int col, List<RectTransform> outList, bool includeInactive = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return;
        }

        int safeColumn = Ux_TonkersTableTopiaExtensionInternals.ClampColumn(table, col);

        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            RectTransform rectTransform = table.FetchCellRectTransformVIP(r, safeColumn);

            if (rectTransform == null)
            {
                continue;
            }

            if (!includeInactive && !rectTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            outList.Add(rectTransform);
        }
    }

    public static void GetColumnOfCellsLikeSkyscraper(this Ux_TonkersTableTopiaRow row, int col, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return;
        }

        int safeColumn = Ux_TonkersTableTopiaExtensionInternals.ClampColumn(table, col);

        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            RectTransform rectTransform = table.FetchCellRectTransformVIP(r, safeColumn);

            if (rectTransform == null)
            {
                continue;
            }

            if (!includeInactive && !rectTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            Ux_TonkersTableTopiaCell cell = rectTransform.GetComponent<Ux_TonkersTableTopiaCell>();

            if (cell != null)
            {
                outList.Add(cell);
            }
        }
    }

    public static int TotalColumnsLikeRollCall(this Ux_TonkersTableTopiaRow row)
    {
        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);
        return table != null ? Mathf.Max(0, table.totalColumnsCountHighFive) : 0;
    }

    public static bool TryPeekMainCourseLikeABuffet(this Ux_TonkersTableTopiaRow row, int col, out int mainRow, out int mainCol, out Ux_TonkersTableTopiaCell mainCell)
    {
        mainRow = -1;
        mainCol = -1;
        mainCell = null;

        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return false;
        }

        int rowIndex = row.rowNumberWhereShenanigansOccur;
        int safeColumn = Ux_TonkersTableTopiaExtensionInternals.ClampColumn(table, col);
        return table.TryPeekMainCourseLikeABuffet(rowIndex, safeColumn, out mainRow, out mainCol, out mainCell);
    }

    private static void CollectMainCells(
        Ux_TonkersTableTopiaLayout table,
        int startRow,
        int endRow,
        int startCol,
        int endCol,
        List<Ux_TonkersTableTopiaCell> outList,
        bool expandToWholeMergers,
        bool distinctMainsOnly)
    {
        if (table == null || outList == null)
        {
            return;
        }

        int r0 = Mathf.Min(startRow, endRow);
        int r1 = Mathf.Max(startRow, endRow);
        int c0 = Mathf.Min(startCol, endCol);
        int c1 = Mathf.Max(startCol, endCol);

        r0 = Mathf.Clamp(r0, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        r1 = Mathf.Clamp(r1, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        c0 = Mathf.Clamp(c0, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        c1 = Mathf.Clamp(c1, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));

        int rowCount = r1 - r0 + 1;
        int colCount = c1 - c0 + 1;

        if (rowCount < 1 || colCount < 1)
        {
            return;
        }

        if (expandToWholeMergers)
        {
            table.ExpandRectToWholeMergersLikeACarpenter(ref r0, ref c0, ref rowCount, ref colCount);
            r1 = r0 + rowCount - 1;
            c1 = c0 + colCount - 1;
        }

        HashSet<Ux_TonkersTableTopiaCell> seen = Ux_TonkersTableTopiaExtensionInternals.PrepareMainCellScratch(distinctMainsOnly);

        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!table.TryPeekMainCourseLikeABuffet(r, c, out _, out _, out Ux_TonkersTableTopiaCell main) || main == null)
                {
                    continue;
                }

                if (seen != null && !seen.Add(main))
                {
                    continue;
                }

                outList.Add(main);
            }
        }
    }

    private static Ux_TonkersTableTopiaLayout TableFromRowLikeGPS(Ux_TonkersTableTopiaRow row)
    {
        return row != null ? row.GetTable() : null;
    }

    private static void CollectRowCells(Ux_TonkersTableTopiaRow row, int startColInclusive, int endColInclusive, List<Ux_TonkersTableTopiaCell> outList, bool includeInactive)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        Ux_TonkersTableTopiaLayout table = TableFromRowLikeGPS(row);

        if (table == null)
        {
            return;
        }

        int rowIndex = row.rowNumberWhereShenanigansOccur;

        if (!Ux_TonkersTableTopiaExtensionInternals.TryNormalizeInclusiveRange(startColInclusive, endColInclusive, table.totalColumnsCountHighFive, out int minCol, out int maxCol))
        {
            return;
        }

        for (int c = minCol; c <= maxCol; c++)
        {
            RectTransform rectTransform = table.FetchCellRectTransformVIP(rowIndex, c);

            if (rectTransform == null)
            {
                continue;
            }

            if (!includeInactive && !rectTransform.gameObject.activeInHierarchy)
            {
                continue;
            }

            Ux_TonkersTableTopiaCell cell = rectTransform.GetComponent<Ux_TonkersTableTopiaCell>();

            if (cell != null)
            {
                outList.Add(cell);
            }
        }
    }
}