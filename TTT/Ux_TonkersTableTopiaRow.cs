using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent()]
public class Ux_TonkersTableTopiaRow : MonoBehaviour
{
    [HideInInspector] public int rowNumberWhereShenanigansOccur = -1;

    public GameObject AddForeignKidAtColumnLikeDoorDash(int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        int r = rowNumberWhereShenanigansOccur;
        var cell = t.GetCellLikePizzaSlice(r, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash(prefab, snapToFill, atSiblingIndex);
    }

    public T AddForeignKidAtColumnLikeDoorDash<T>(int col, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        int r = rowNumberWhereShenanigansOccur;
        var cell = t.GetCellLikePizzaSlice(r, col, true);
        if (cell == null) return null;
        return cell.AddForeignKidLikeDoorDash<T>(snapToFill, atSiblingIndex);
    }

    public GameObject AddForeignFirstColumnLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        return AddForeignKidAtColumnLikeDoorDash(0, prefab, snapToFill, -1);
    }

    public GameObject AddForeignLastColumnLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        int lastCol = Mathf.Max(0, t.totalColumnsCountHighFive - 1);
        return AddForeignKidAtColumnLikeDoorDash(lastCol, prefab, snapToFill, -1);
    }

    public Ux_TonkersTableTopiaLayout AddNestedTableAtColumnLikeRussianDoll(int col, bool ensureSnapToFill = true)
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        int r = rowNumberWhereShenanigansOccur;
        var cell = t.GetCellLikePizzaSlice(r, col, true);
        if (cell == null) return null;
        return cell.AddNestedTableLikeRussianDoll(ensureSnapToFill);
    }

    public Ux_TonkersTableTopiaLayout GetTableLikeFamilyReunion()
    {
        return GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
    }

    public RectTransform GetRectLikeYogaMat()
    {
        return GetComponent<RectTransform>();
    }

    public int GetRowIndexLikeCornDog()
    {
        return rowNumberWhereShenanigansOccur;
    }

}
