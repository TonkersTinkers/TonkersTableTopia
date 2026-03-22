using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Ux_TonkersTableTopiaExtensionInternals;

public static class Ux_TonkersTableTopiaContentExtensions
{
    public static void ScoutUiSnacksInCellLikeAHawk(this Ux_TonkersTableTopiaLayout table, int row, int col, HashSet<Type> bag, bool includeInactive = true)
    {
        if (table == null || bag == null)
        {
            return;
        }

        RectTransform cell = table.FetchCellRectTransformVIP(row, col);

        if (cell == null)
        {
            return;
        }

        componentScratch.Clear();

#if UNITY_2021_1_OR_NEWER
        cell.GetComponentsInChildren(includeInactive, componentScratch);
#else
        Component[] components = cell.GetComponentsInChildren<Component>(includeInactive);
        componentScratch.AddRange(components);
#endif

        for (int i = 0; i < componentScratch.Count; i++)
        {
            Component component = componentScratch[i];

            if (component == null)
            {
                continue;
            }

            GameObject gameObject = component.gameObject;

            if (IsTttInternalComponentLikeBouncer(gameObject))
            {
                continue;
            }

            if (TryMapSnackType(component, gameObject, out Type snackType))
            {
                bag.Add(snackType);
            }
        }
    }

    public static void ScoutUiSnackCountsInCellLikeBeanCounter(this Ux_TonkersTableTopiaLayout table, int row, int col, Dictionary<Type, int> bag, bool includeInactive = true)
    {
        Ux_TonkersTableTopiaForeignContentUtility.ScoutUiSnackCountsInCell(table, row, col, bag, includeInactive);
    }

    public static bool HasForeignKidsLikeStowaways(this RectTransform parent)
    {
        return Ux_TonkersTableTopiaForeignContentUtility.HasForeignChildren(parent);
    }

    public static void MoveForeignKidsLikeABoxTruck(this Ux_TonkersTableTopiaLayout table, RectTransform from, RectTransform to)
    {
        Ux_TonkersTableTopiaForeignContentUtility.MoveForeignChildren(table, from, to);
    }

    public static bool MoveNestedTablesLikeACaravan(this Ux_TonkersTableTopiaLayout table, RectTransform from, RectTransform to)
    {
        return Ux_TonkersTableTopiaForeignContentUtility.MoveNestedTables(table, from, to);
    }

    public static void GatherCellContentLinesLikeAWaiter(this Ux_TonkersTableTopiaLayout table, int row, int col, List<string> outLines)
    {
        Ux_TonkersTableTopiaForeignContentUtility.GatherCellContentLines(table, row, col, outLines);
    }

    public static Type PickPrimaryUiTypeLikeMenuDecider(GameObject go)
    {
        return Ux_TonkersTableTopiaForeignContentUtility.PickPrimaryUiType(go);
    }

    public static string TypeNameShortLikeNameTag(Type type)
    {
        return Ux_TonkersTableTopiaForeignContentUtility.GetShortTypeName(type);
    }

    public static void ListForeignKidsLikeRollCall(this RectTransform parent, List<Transform> outList)
    {
        Ux_TonkersTableTopiaForeignContentUtility.CollectForeignChildren(parent, outList);
    }

    public static GameObject AddForeignKidToCellLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int row, int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        Ux_TonkersTableTopiaCell cell = GetTargetCell(table, row, col);

        if (cell == null)
        {
            return null;
        }

        return cell.AddContent(prefab, snapToFill, atSiblingIndex);
    }

    public static T AddForeignKidToCellLikeDoorDash<T>(this Ux_TonkersTableTopiaLayout table, int row, int col, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        Ux_TonkersTableTopiaCell cell = GetTargetCell(table, row, col);

        if (cell == null)
        {
            return null;
        }

        return cell.AddContent<T>(snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstInCellLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int row, int col, GameObject prefab, bool snapToFill = true)
    {
        return table.AddForeignKidToCellLikeDoorDash(row, col, prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastInCellLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int row, int col, GameObject prefab, bool snapToFill = true)
    {
        return table.AddForeignKidToCellLikeDoorDash(row, col, prefab, snapToFill, -1);
    }

    public static GameObject AddForeignKidToColumnAtRowLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int col, int row, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        return table.AddForeignKidToCellLikeDoorDash(row, col, prefab, snapToFill, atSiblingIndex);
    }

    public static T AddForeignKidToColumnAtRowLikeDoorDash<T>(this Ux_TonkersTableTopiaLayout table, int col, int row, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        return table.AddForeignKidToCellLikeDoorDash<T>(row, col, snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstRowInColumnLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int col, GameObject prefab, bool snapToFill = true)
    {
        return table.AddForeignKidToColumnAtRowLikeDoorDash(col, 0, prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastRowInColumnLikeDoorDash(this Ux_TonkersTableTopiaLayout table, int col, GameObject prefab, bool snapToFill = true)
    {
        if (table == null)
        {
            return null;
        }

        int lastRow = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        return table.AddForeignKidToColumnAtRowLikeDoorDash(col, lastRow, prefab, snapToFill, -1);
    }

    public static T AddForeignFirstLikeVIP<T>(this Ux_TonkersTableTopiaLayout table, int col, bool snapToFill = true) where T : Component
    {
        return table.AddForeignKidToColumnAtRowLikeDoorDash<T>(col, 0, snapToFill, 0);
    }

    public static T AddForeignLastLikeClosingTime<T>(this Ux_TonkersTableTopiaLayout table, int col, bool snapToFill = true) where T : Component
    {
        if (table == null)
        {
            return null;
        }

        int lastRow = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        return table.AddForeignKidToColumnAtRowLikeDoorDash<T>(col, lastRow, snapToFill, -1);
    }

    public static Ux_TonkersTableTopiaLayout AddNestedTableToCellLikeRussianDoll(this Ux_TonkersTableTopiaLayout table, int row, int col, bool ensureSnapToFill = true)
    {
        if (table == null)
        {
            return null;
        }

        Ux_TonkersTableTopiaLayout childTable = table.CreateChildTableInCellLikeABaby(row, col);

        if (childTable != null && ensureSnapToFill)
        {
            RectTransform rectTransform = childTable.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                rectTransform.SnapCroutonToFillParentLikeGravy();
            }

            table.FlagLayoutAsNeedingSpaDay();
        }

        return childTable;
    }

    public static GameObject AddForeignLikeOneLinerAt(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        if (row == null)
        {
            return null;
        }

        return row.AddContentAtColumn(col, prefab, snapToFill, atSiblingIndex);
    }

    public static T AddForeignLikeOneLinerAt<T>(this Ux_TonkersTableTopiaRow row, int col, bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        if (row == null)
        {
            return null;
        }

        return row.AddContentAtColumn<T>(col, snapToFill, atSiblingIndex);
    }

    public static GameObject AddForeignFirstLikeVIP(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true)
    {
        return row.AddForeignLikeOneLinerAt(col, prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastLikeClosingTime(this Ux_TonkersTableTopiaRow row, int col, GameObject prefab, bool snapToFill = true)
    {
        return row.AddForeignLikeOneLinerAt(col, prefab, snapToFill, -1);
    }

    public static GameObject AddForeignFirstLikeVIP(this Ux_TonkersTableTopiaCell cell, GameObject prefab, bool snapToFill = true)
    {
        if (cell == null)
        {
            return null;
        }

        return cell.AddContent(prefab, snapToFill, 0);
    }

    public static GameObject AddForeignLastLikeClosingTime(this Ux_TonkersTableTopiaCell cell, GameObject prefab, bool snapToFill = true)
    {
        if (cell == null)
        {
            return null;
        }

        return cell.AddContent(prefab, snapToFill, -1);
    }

    public static T AddForeignFirstLikeVIP<T>(this Ux_TonkersTableTopiaCell cell, bool snapToFill = true) where T : Component
    {
        if (cell == null)
        {
            return null;
        }

        return cell.AddContent<T>(snapToFill, 0);
    }

    public static T AddForeignLastLikeClosingTime<T>(this Ux_TonkersTableTopiaCell cell, bool snapToFill = true) where T : Component
    {
        if (cell == null)
        {
            return null;
        }

        return cell.AddContent<T>(snapToFill, -1);
    }

    public static bool IsTonkersTableRoyaltyLikeVIP(this Component component)
    {
        return component is Ux_TonkersTableTopiaLayout || component is Ux_TonkersTableTopiaRow || component is Ux_TonkersTableTopiaCell;
    }

    public static bool HasAnyTonkersTableRoyaltyLikeBouncer(this GameObject go)
    {
        if (go == null)
        {
            return false;
        }

        return go.GetComponent<Ux_TonkersTableTopiaLayout>() != null
            || go.GetComponent<Ux_TonkersTableTopiaRow>() != null
            || go.GetComponent<Ux_TonkersTableTopiaCell>() != null;
    }

    public static Component FindAnyTonkersTableRoyaltyLikeNeedle(this GameObject go)
    {
        if (go == null)
        {
            return null;
        }

        Ux_TonkersTableTopiaLayout layout = go.GetComponent<Ux_TonkersTableTopiaLayout>();

        if (layout != null)
        {
            return layout;
        }

        Ux_TonkersTableTopiaRow row = go.GetComponent<Ux_TonkersTableTopiaRow>();

        if (row != null)
        {
            return row;
        }

        Ux_TonkersTableTopiaCell cell = go.GetComponent<Ux_TonkersTableTopiaCell>();
        return cell;
    }

    public static bool IsAllowedSidekickForTableRoyaltyLikeChaperone(this Component component)
    {
        if (component == null)
        {
            return false;
        }

        Type type = component.GetType();
        return type == typeof(Transform) || type == typeof(RectTransform) || type == typeof(CanvasRenderer);
    }

    public static bool TryPeekInnerPaddingLikePillowFort(this Ux_TonkersTableTopiaCell cell, out float leftWaffle, out float rightWaffle, out float topWaffle, out float bottomWaffle)
    {
        if (cell != null && cell.useInnerPaddingPillowFort)
        {
            leftWaffle = cell.innerPaddingLeftMarshmallow;
            rightWaffle = cell.innerPaddingRightMarshmallow;
            topWaffle = cell.innerPaddingTopMarshmallow;
            bottomWaffle = cell.innerPaddingBottomMarshmallow;
            return true;
        }

        leftWaffle = 0f;
        rightWaffle = 0f;
        topWaffle = 0f;
        bottomWaffle = 0f;
        return false;
    }

    public static void AdoptExternalIntoCellLikeStork(this Ux_TonkersTableTopiaLayout table, RectTransform destination, GameObject go)
    {
        if (table == null || destination == null || go == null)
        {
            return;
        }

        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            return;
        }

        if (!Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(rectTransform))
        {
            return;
        }

        ReparentPreservingLayoutLikeSeatbelt(rectTransform, destination, "Add To Cell");
    }

    private static Ux_TonkersTableTopiaCell GetTargetCell(Ux_TonkersTableTopiaLayout table, int row, int col)
    {
        if (table == null)
        {
            return null;
        }

        return table.GetCellLikePizzaSlice(row, col, true);
    }

    private static bool IsTttInternalComponentLikeBouncer(GameObject go)
    {
        return go != null && Ux_TonkersTableTopiaHierarchyRules.IsManagedScaffold(go.transform);
    }

    private static bool TryMapSnackType(Component component, GameObject go, out Type snackType)
    {
        snackType = null;

        if (component is RectTransform || component is CanvasRenderer)
        {
            return false;
        }

        if (component is Button)
        {
            snackType = typeof(Button);
            return true;
        }

        if (component is Toggle)
        {
            snackType = typeof(Toggle);
            return true;
        }

        if (component is Slider)
        {
            snackType = typeof(Slider);
            return true;
        }

        if (component is Dropdown)
        {
            snackType = typeof(Dropdown);
            return true;
        }

        if (component is Scrollbar)
        {
            snackType = typeof(Scrollbar);
            return true;
        }

        if (component is ScrollRect)
        {
            snackType = typeof(ScrollRect);
            return true;
        }

        if (component is InputField)
        {
            snackType = typeof(InputField);
            return true;
        }

        if (component is Text)
        {
            snackType = typeof(Text);
            return true;
        }

        if (component is RawImage)
        {
            snackType = typeof(RawImage);
            return true;
        }

        if (component is Image && go.GetComponent<Button>() == null)
        {
            snackType = typeof(Image);
            return true;
        }

        return false;
    }

    private static void ReparentPreservingLayoutLikeSeatbelt(Transform child, Transform newParent, string undoName)
    {
        if (child == null || newParent == null)
        {
            return;
        }

        RectTransform childRect = child as RectTransform;
        Ux_TonkersTableTopiaRectTransformSnapshot snapshot = Ux_TonkersTableTopiaRectTransformSnapshot.Capture(childRect);

        Ux_TonkersTableTopiaObjectUtility.SetParent(child, newParent, undoName);

#if UNITY_EDITOR
        if (!Application.isPlaying && childRect != null)
        {
            UnityEditor.Undo.RecordObject(childRect, undoName);
        }
#endif

        if (childRect == null)
        {
            return;
        }

        if (childRect.IsFullStretchLikeYoga())
        {
            childRect.SnapCroutonToFillParentLikeGravy();
            return;
        }

        snapshot.Restore(childRect);
    }
}