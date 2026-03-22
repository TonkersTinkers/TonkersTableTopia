using UnityEngine;
using static Ux_TonkersTableTopiaExtensionInternals;

public static class Ux_TonkersTableTopiaAlignmentExtensions
{
    public static bool TryDetectCellAlignmentLikeLieDetector(this Ux_TonkersTableTopiaLayout table, int row, int col, out bool isFullLikeBurrito, out Ux_TonkersTableTopiaLayout.HorizontalAlignment h, out Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        return Ux_TonkersTableTopiaAlignmentUtility.TryDetectCellAlignment(table, row, col, out isFullLikeBurrito, out h, out v);
    }

    public static bool IsColumnAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        int safeColumn = ClampColumn(table, col);
        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentQueryMode.Both, h, v);
    }

    public static bool IsColumnFullLikeWaterfall(this Ux_TonkersTableTopiaLayout table, int col)
    {
        if (table == null)
        {
            return false;
        }

        int safeColumn = ClampColumn(table, col);
        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentQueryMode.Full);
    }

    public static bool IsColumnHorizAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return false;
        }

        int safeColumn = ClampColumn(table, col);
        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentQueryMode.Horizontal, h);
    }

    public static bool IsColumnVertAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int col, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        int safeColumn = ClampColumn(table, col);
        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentQueryMode.Vertical, default, v);
    }

    public static bool IsRowAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int row, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        int safeRow = ClampRow(table, row);
        return EvaluateSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Both, h, v);
    }

    public static bool IsRowFullLikeWaterbed(this Ux_TonkersTableTopiaLayout table, int row)
    {
        if (table == null)
        {
            return false;
        }

        int safeRow = ClampRow(table, row);
        return EvaluateSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Full);
    }

    public static bool IsRowHorizAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int row, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return false;
        }

        int safeRow = ClampRow(table, row);
        return EvaluateSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Horizontal, h);
    }

    public static bool IsRowVertAlignedLikeMirror(this Ux_TonkersTableTopiaLayout table, int row, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        int safeRow = ClampRow(table, row);
        return EvaluateSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Vertical, default, v);
    }

    public static bool IsSelectionAlreadyAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout table, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment wantH, Ux_TonkersTableTopiaLayout.VerticalAlignment wantV)
    {
        return EvaluateSelection(table, r0, r1, c0, c1, AlignmentQueryMode.Both, wantH, wantV);
    }

    public static bool IsSelectionFullLikeBurritoWrap(this Ux_TonkersTableTopiaLayout table, int r0, int r1, int c0, int c1)
    {
        return EvaluateSelection(table, r0, r1, c0, c1, AlignmentQueryMode.Full);
    }

    public static bool IsSelectionHorizAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout table, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment wantH)
    {
        return EvaluateSelection(table, r0, r1, c0, c1, AlignmentQueryMode.Horizontal, wantH);
    }

    public static bool IsSelectionVertAlignedLikeDejaVu(this Ux_TonkersTableTopiaLayout table, int r0, int r1, int c0, int c1, Ux_TonkersTableTopiaLayout.VerticalAlignment wantV)
    {
        return EvaluateSelection(table, r0, r1, c0, c1, AlignmentQueryMode.Vertical, default, wantV);
    }

    public static bool IsTableAlignedLikeChoir(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Both, h, v);
    }

    public static bool IsTableFullLikeBalloon(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return false;
        }

        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Full);
    }

    public static bool IsTableHorizAlignedLikeChoir(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return false;
        }

        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Horizontal, h);
    }

    public static bool IsTableVertAlignedLikeChoir(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return false;
        }

        return EvaluateSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentQueryMode.Vertical, default, v);
    }

    public static void AlignCellForeignsLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int row, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        ApplyAlignmentToSelection(table, row, row, col, col, AlignmentApplyMode.Both, h, v, "Align Cell");
    }

    public static void AlignCellForeignsToFillLikeStuffedBurrito(this Ux_TonkersTableTopiaLayout table, int row, int col)
    {
        ApplyAlignmentToSelection(table, row, row, col, col, AlignmentApplyMode.Fill, default, default, "Align Cell Full");
    }

    public static void AlignCellHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int row, int col, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        ApplyAlignmentToSelection(table, row, row, col, col, AlignmentApplyMode.Horizontal, h, default, "Align Cell Horizontal");
    }

    public static void AlignCellVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int row, int col, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        ApplyAlignmentToSelection(table, row, row, col, col, AlignmentApplyMode.Vertical, default, v, "Align Cell Vertical");
    }

    public static void AlignRowHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return;
        }

        int safeRow = ClampRow(table, rowIndex);
        ApplyAlignmentToSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Horizontal, h, default, "Align Row Horizontal");
    }

    public static void AlignRowLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return;
        }

        int safeRow = ClampRow(table, rowIndex);
        ApplyAlignmentToSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Both, h, v, "Align Row");
    }

    public static void AlignRowVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return;
        }

        int safeRow = ClampRow(table, rowIndex);
        ApplyAlignmentToSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Vertical, default, v, "Align Row Vertical");
    }

    public static void AlignRowToFillLikeWaterbed(this Ux_TonkersTableTopiaLayout table, int rowIndex)
    {
        if (table == null)
        {
            return;
        }

        int safeRow = ClampRow(table, rowIndex);
        ApplyAlignmentToSelection(table, safeRow, safeRow, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Fill, default, default, "Align Row Full");
    }

    public static void AlignColumnHorizontalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return;
        }

        int safeColumn = ClampColumn(table, colIndex);
        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentApplyMode.Horizontal, h, default, "Align Column Horizontal");
    }

    public static void AlignColumnVerticalOnlyLikeLaserLevel(this Ux_TonkersTableTopiaLayout table, int colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return;
        }

        int safeColumn = ClampColumn(table, colIndex);
        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentApplyMode.Vertical, default, v, "Align Column Vertical");
    }

    public static void AlignColumnToFillLikeWaterfall(this Ux_TonkersTableTopiaLayout table, int colIndex)
    {
        if (table == null)
        {
            return;
        }

        int safeColumn = ClampColumn(table, colIndex);
        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), safeColumn, safeColumn, AlignmentApplyMode.Fill, default, default, "Align Column Full");
    }

    public static void AlignTableHorizontalOnlyLikeChoir(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null)
        {
            return;
        }

        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Horizontal, h, default, "Align Table Horizontal");
    }

    public static void AlignTableVerticalOnlyLikeChoir(this Ux_TonkersTableTopiaLayout table, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null)
        {
            return;
        }

        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Vertical, default, v, "Align Table Vertical");
    }

    public static void AlignTableToFillLikeBalloon(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        ApplyAlignmentToSelection(table, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1), 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1), AlignmentApplyMode.Fill, default, default, "Align Table Full");
    }

    public static void AlignForeignersInRectLikeEtiquette(this RectTransform parent, Ux_TonkersTableTopiaLayout.HorizontalAlignment h, Ux_TonkersTableTopiaLayout.VerticalAlignment v, bool alignTextsToo = true)
    {
        Ux_TonkersTableTopiaAlignmentUtility.AlignForeignersInRect(parent, h, v, alignTextsToo);
    }

    public static void AlignForeignersToFillLikeStuffedBurrito(this RectTransform parent, bool alignTextsToo = true)
    {
        Ux_TonkersTableTopiaAlignmentUtility.AlignForeignersToFill(parent, alignTextsToo);
    }

    public static Vector2 GuessCellForeignersAnchorLikeDart(this Ux_TonkersTableTopiaLayout table, int row, int col, out bool fullStretch)
    {
        return Ux_TonkersTableTopiaAlignmentUtility.GuessForeignAnchor(table, row, col, out fullStretch);
    }
}