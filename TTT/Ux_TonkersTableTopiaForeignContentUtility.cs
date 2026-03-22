using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal static class Ux_TonkersTableTopiaForeignContentUtility
{
    private static readonly List<Transform> _foreignScratch = new List<Transform>(64);
    private static readonly List<Ux_TonkersTableTopiaLayout> _childTableScratch = new List<Ux_TonkersTableTopiaLayout>(16);
    private static readonly List<Component> _componentScratch = new List<Component>(64);

    private static readonly Type[] _priorityTypes =
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
        typeof(InputField),
    };

    public static bool IsForeignTransform(Transform child, bool includeInactive = true)
    {
        if (child == null)
        {
            return false;
        }

        if (!includeInactive && !child.gameObject.activeInHierarchy)
        {
            return false;
        }

        return Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(child);
    }

    public static bool TryGetForeignRect(Transform child, bool includeInactive, out RectTransform foreignRect)
    {
        foreignRect = child as RectTransform;
        return foreignRect != null && IsForeignTransform(child, includeInactive);
    }

    public static RectTransform GetFirstForeignRect(RectTransform parent, bool includeInactive = true)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            if (TryGetForeignRect(parent.GetChild(i), includeInactive, out RectTransform foreignRect))
            {
                return foreignRect;
            }
        }

        return null;
    }

    public static void CollectForeignChildren(RectTransform parent, List<Transform> outList, bool includeInactive = true)
    {
        if (outList == null)
        {
            return;
        }

        outList.Clear();

        if (parent == null)
        {
            return;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (!IsForeignTransform(child, includeInactive))
            {
                continue;
            }

            outList.Add(child);
        }
    }

    public static bool HasForeignChildren(RectTransform parent)
    {
        if (parent == null)
        {
            return false;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            if (IsForeignTransform(parent.GetChild(i)))
            {
                return true;
            }
        }

        return false;
    }

    public static void MoveForeignChildren(Ux_TonkersTableTopiaLayout table, RectTransform from, RectTransform to)
    {
        if (table == null || from == null || to == null)
        {
            return;
        }

        CollectForeignChildren(from, _foreignScratch);
        if (_foreignScratch.Count == 0)
        {
            return;
        }

#if UNITY_EDITOR
        UnityEditor.Undo.IncrementCurrentGroup();
#endif

        for (int i = 0; i < _foreignScratch.Count; i++)
        {
            ReparentPreservingLayout(_foreignScratch[i], to, "Move Cell Contents");
        }

        _foreignScratch.Clear();
    }

    public static bool MoveNestedTables(Ux_TonkersTableTopiaLayout table, RectTransform from, RectTransform to)
    {
        if (table == null || from == null || to == null)
        {
            return false;
        }

        _childTableScratch.Clear();

        for (int i = 0; i < from.childCount; i++)
        {
            Transform child = from.GetChild(i);
            Ux_TonkersTableTopiaLayout nestedTable = child.GetComponent<Ux_TonkersTableTopiaLayout>();
            if (nestedTable != null)
            {
                _childTableScratch.Add(nestedTable);
            }
        }

        if (_childTableScratch.Count == 0)
        {
            return false;
        }

#if UNITY_EDITOR
        UnityEditor.Undo.IncrementCurrentGroup();
#endif

        for (int i = 0; i < _childTableScratch.Count; i++)
        {
            ReparentPreservingLayout(_childTableScratch[i].transform, to, "Move Nested Table");
        }

        _childTableScratch.Clear();
        return true;
    }

    public static void ScoutUiSnackCountsInCell(Ux_TonkersTableTopiaLayout table, int row, int col, Dictionary<Type, int> bag, bool includeInactive = true)
    {
        if (bag == null)
        {
            return;
        }

        bag.Clear();

        if (table == null)
        {
            return;
        }

        RectTransform cell = table.FetchCellRectTransformVIP(row, col);
        if (cell == null)
        {
            return;
        }

        for (int i = 0; i < cell.childCount; i++)
        {
            RectTransform child = cell.GetChild(i) as RectTransform;
            if (child == null)
            {
                continue;
            }

            GameObject gameObject = child.gameObject;
            if (!includeInactive && !gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Ux_TonkersTableTopiaHierarchyRules.IsManagedScaffold(child))
            {
                continue;
            }

            Type snackType = DecideSnackType(gameObject);
            if (!bag.TryGetValue(snackType, out int count))
            {
                count = 0;
            }

            bag[snackType] = count + 1;
        }
    }

    public static void GatherCellContentLines(Ux_TonkersTableTopiaLayout table, int row, int col, List<string> outLines)
    {
        if (outLines == null)
        {
            return;
        }

        outLines.Clear();

        if (table == null)
        {
            return;
        }

        RectTransform cell = table.FetchCellRectTransformVIP(row, col);
        if (cell == null)
        {
            return;
        }

        int index = 1;
        for (int i = 0; i < cell.childCount; i++)
        {
            RectTransform child = cell.GetChild(i) as RectTransform;
            if (child == null || !IsForeignTransform(child))
            {
                continue;
            }

            Type type = PickPrimaryUiType(child.gameObject);
            outLines.Add(index.ToString() + ". " + child.gameObject.name + " (" + GetShortTypeName(type) + ")");
            index++;
        }
    }

    public static Type PickPrimaryUiType(GameObject go)
    {
        if (go == null)
        {
            return typeof(GameObject);
        }

        if (Ux_TonkersTableTopiaHierarchyRules.IsManagedScaffold(go.transform))
        {
            return typeof(GameObject);
        }

        for (int i = 0; i < _priorityTypes.Length; i++)
        {
            Type type = _priorityTypes[i];
            if (go.GetComponent(type) != null)
            {
                return type;
            }
        }

        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            Component component = components[i];
            if (component == null)
            {
                continue;
            }

            Type type = component.GetType();
            if (type == typeof(RectTransform) || type == typeof(CanvasRenderer))
            {
                continue;
            }

            return type;
        }

        return typeof(GameObject);
    }

    public static string GetShortTypeName(Type type)
    {
        if (type == null)
        {
            return "Unknown";
        }

        if (type == typeof(Text))
        {
            return "Text";
        }

        if (type == typeof(Image))
        {
            return "Image";
        }

        if (type == typeof(RawImage))
        {
            return "RawImage";
        }

        if (type == typeof(Button))
        {
            return "Button";
        }

        if (type == typeof(Toggle))
        {
            return "Toggle";
        }

        if (type == typeof(Slider))
        {
            return "Slider";
        }

        if (type == typeof(Dropdown))
        {
            return "Dropdown";
        }

        if (type == typeof(Scrollbar))
        {
            return "Scrollbar";
        }

        if (type == typeof(ScrollRect))
        {
            return "ScrollRect";
        }

        if (type == typeof(InputField))
        {
            return "InputField";
        }

        if (type == typeof(Ux_TonkersTableTopiaLayout))
        {
            return "Table";
        }

        return type.Name;
    }

    private static Type DecideSnackType(GameObject go)
    {
        if (go == null)
        {
            return typeof(UnityEngine.Object);
        }

        if (go.GetComponent<Button>() != null)
        {
            return typeof(Button);
        }

        if (go.GetComponent<Toggle>() != null)
        {
            return typeof(Toggle);
        }

        if (go.GetComponent<Slider>() != null)
        {
            return typeof(Slider);
        }

        if (go.GetComponent<Dropdown>() != null)
        {
            return typeof(Dropdown);
        }

        if (go.GetComponent<Scrollbar>() != null)
        {
            return typeof(Scrollbar);
        }

        if (go.GetComponent<ScrollRect>() != null)
        {
            return typeof(ScrollRect);
        }

        if (go.GetComponent<InputField>() != null)
        {
            return typeof(InputField);
        }

        if (go.GetComponent<Text>() != null)
        {
            return typeof(Text);
        }

        if (go.GetComponent<RawImage>() != null)
        {
            return typeof(RawImage);
        }

        if (go.GetComponent<Image>() != null)
        {
            return typeof(Image);
        }

        return typeof(UnityEngine.Object);
    }

    private static void ReparentPreservingLayout(Transform child, Transform newParent, string undoName)
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

        ApplyLayoutAfterReparent(childRect, snapshot);
    }

    private static void ApplyLayoutAfterReparent(RectTransform childRect, Ux_TonkersTableTopiaRectTransformSnapshot snapshot)
    {
        if (childRect == null)
        {
            return;
        }

        if (Ux_TonkersTableTopiaRectTransformUtility.IsFullStretch(childRect))
        {
            Ux_TonkersTableTopiaRectTransformUtility.SnapToFillParent(childRect);
            return;
        }

        snapshot.Restore(childRect);
    }
}