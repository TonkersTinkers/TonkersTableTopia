using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class Ux_TonkersTableTopiaColumn : Ux_TonkersTableTopiaNodeBase
{
    [HideInInspector] public int columnNumberPrimeRib = -1;

    public RectTransform RectTransformComponent => GetCachedRectTransform();
    public Ux_TonkersTableTopiaLayout GetTable() => GetCachedTable();
    public int ColumnIndex => columnNumberPrimeRib;

    public GameObject AddContentAtRow(int rowIndex, GameObject prefab, bool stretchToFill = true, int siblingIndex = -1)
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtRow(rowIndex, true);
        return cell != null ? cell.AddContent(prefab, stretchToFill, siblingIndex) : null;
    }

    public T AddContentAtRow<T>(int rowIndex, bool stretchToFill = true, int siblingIndex = -1) where T : Component
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtRow(rowIndex, true);
        return cell != null ? cell.AddContent<T>(stretchToFill, siblingIndex) : null;
    }

    public GameObject AddContentAtFirstRow(GameObject prefab, bool stretchToFill = true)
    {
        return AddContentAtRow(0, prefab, stretchToFill, -1);
    }

    public GameObject AddContentAtLastRow(GameObject prefab, bool stretchToFill = true)
    {
        Ux_TonkersTableTopiaCell cell = GetLastCell(true);
        return cell != null ? cell.AddContent(prefab, stretchToFill) : null;
    }

    public Ux_TonkersTableTopiaLayout AddNestedTableAtRow(int rowIndex, bool ensureSnapToFill = true)
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtRow(rowIndex, true);
        return cell != null ? cell.AddNestedTable(ensureSnapToFill) : null;
    }

    public void SetFixedWidthPixelsLikeTapeMeasure(float widthPixels)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnFixedWidthPixelsLikeTapeMeasure(ColumnIndex, widthPixels);
    }

    public void SetPercentageWidthLikeASpreadsheet(float percentage01)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnPercentageLikeASpreadsheet(ColumnIndex, percentage01);
    }

    public void SetFlexibleWidthLikeYogaPants()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnFlexibleLikeYogaPants(ColumnIndex);
    }

    public float GetLiveWidthPixelsLikeTapeMeasure()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return 0f;
        }

        return table.GetLiveColumnWidthPixelsLikeTapeMeasure(ColumnIndex);
    }

    public float GetStoredPercentageWidthLikeASpreadsheet()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return 0f;
        }

        return table.GetStoredColumnPercentageLikeASpreadsheet(ColumnIndex);
    }

    private Ux_TonkersTableTopiaCell GetCellAtRow(int rowIndex, bool createIfMissing)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        return table != null ? table.GetCellLikePizzaSlice(rowIndex, ColumnIndex, createIfMissing) : null;
    }

    private Ux_TonkersTableTopiaCell GetLastCell(bool createIfMissing)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null)
        {
            return null;
        }

        int lastRow = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        return table.GetCellLikePizzaSlice(lastRow, ColumnIndex, createIfMissing);
    }

    [Obsolete("Use RectTransformComponent instead.")]
    public RectTransform GetRectLikeYogaMat()
    {
        return RectTransformComponent;
    }

    [Obsolete("Use ColumnIndex instead.")]
    public int GetColumnIndexLikeCornDog()
    {
        return ColumnIndex;
    }
}