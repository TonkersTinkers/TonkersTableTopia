using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class Ux_TonkersTableTopiaRow : Ux_TonkersTableTopiaNodeBase
{
    [HideInInspector] public int rowNumberWhereShenanigansOccur = -1;

    public RectTransform RectTransformComponent => GetCachedRectTransform();

    public Ux_TonkersTableTopiaLayout GetTable() => GetCachedTable();

    public int RowIndex => rowNumberWhereShenanigansOccur;

    public GameObject AddContentAtColumn(int columnIndex, GameObject prefab, bool stretchToFill = true, int siblingIndex = -1)
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtColumn(columnIndex, true);
        return cell != null ? cell.AddContent(prefab, stretchToFill, siblingIndex) : null;
    }

    public T AddContentAtColumn<T>(int columnIndex, bool stretchToFill = true, int siblingIndex = -1) where T : Component
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtColumn(columnIndex, true);
        return cell != null ? cell.AddContent<T>(stretchToFill, siblingIndex) : null;
    }

    public GameObject AddContentAtFirstColumn(GameObject prefab, bool stretchToFill = true)
    {
        return AddContentAtColumn(0, prefab, stretchToFill, -1);
    }

    public GameObject AddContentAtLastColumn(GameObject prefab, bool stretchToFill = true)
    {
        Ux_TonkersTableTopiaCell cell = GetLastCell(true);
        return cell != null ? cell.AddContent(prefab, stretchToFill) : null;
    }

    public Ux_TonkersTableTopiaLayout AddNestedTableAtColumn(int columnIndex, bool ensureSnapToFill = true)
    {
        Ux_TonkersTableTopiaCell cell = GetCellAtColumn(columnIndex, true);
        return cell != null ? cell.AddNestedTable(ensureSnapToFill) : null;
    }

    private Ux_TonkersTableTopiaCell GetCellAtColumn(int columnIndex, bool createIfMissing)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        return table != null ? table.GetCellLikePizzaSlice(RowIndex, columnIndex, createIfMissing) : null;
    }

    private Ux_TonkersTableTopiaCell GetLastCell(bool createIfMissing)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null)
            return null;

        int lastColumn = Mathf.Max(0, table.totalColumnsCountHighFive - 1);
        return table.GetCellLikePizzaSlice(RowIndex, lastColumn, createIfMissing);
    }

    public void SetFixedHeightPixelsLikeTapeMeasure(float heightPixels)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowFixedHeightPixelsLikeTapeMeasure(RowIndex, heightPixels);
    }

    public void SetPercentageHeightLikeASpreadsheet(float percentage01)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowPercentageLikeASpreadsheet(RowIndex, percentage01);
    }

    public void SetFlexibleHeightLikeYogaPants()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowFlexibleLikeYogaPants(RowIndex);
    }

    public float GetLiveHeightPixelsLikeTapeMeasure()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return 0f;
        }

        return table.GetLiveRowHeightPixelsLikeTapeMeasure(RowIndex);
    }

    public float GetStoredPercentageHeightLikeASpreadsheet()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return 0f;
        }

        return table.GetStoredRowPercentageLikeASpreadsheet(RowIndex);
    }

    [Obsolete("Use AddContentAtFirstColumn instead.")]
    public GameObject AddForeignFirstColumnLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        return AddContentAtFirstColumn(prefab, snapToFill);
    }

    [Obsolete("Use AddContentAtLastColumn instead.")]
    public GameObject AddForeignLastColumnLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        return AddContentAtLastColumn(prefab, snapToFill);
    }

    [Obsolete("Use AddNestedTableAtColumn instead.")]
    public Ux_TonkersTableTopiaLayout AddNestedTableAtColumnLikeRussianDoll(int col, bool ensureSnapToFill = true)
    {
        return AddNestedTableAtColumn(col, ensureSnapToFill);
    }

    [Obsolete("Use RectTransformComponent instead.")]
    public RectTransform GetRectLikeYogaMat()
    {
        return RectTransformComponent;
    }

    [Obsolete("Use RowIndex instead.")]
    public int GetRowIndexLikeCornDog()
    {
        return RowIndex;
    }
}