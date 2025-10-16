using UnityEngine;

public static class Ux_TonkersTableTopiaExtensions
{
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
}