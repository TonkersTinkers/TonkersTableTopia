using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal static class Ux_TonkersTableTopiaExtensionInternals
{
    internal enum AlignmentQueryMode
    {
        Full,
        Both,
        Horizontal,
        Vertical
    }

    internal enum AlignmentApplyMode
    {
        Fill,
        Both,
        Horizontal,
        Vertical
    }

    internal sealed class LayoutState
    {
        public Vector2 lastCanvasScale = Vector2.one;
        public Vector2 lastTableSize = Vector2.zero;
        public bool pendingDeferral;
    }

    internal static readonly List<Ux_TonkersTableTopiaLayout> childTableScratch = new List<Ux_TonkersTableTopiaLayout>(8);
    internal static readonly HashSet<Ux_TonkersTableTopiaCell> mainCellScratch = new HashSet<Ux_TonkersTableTopiaCell>(64);
    internal static readonly List<Ux_TonkersTableTopiaLayout> deferredSpaDayLayouts = new List<Ux_TonkersTableTopiaLayout>(16);
    internal static readonly List<Component> componentScratch = new List<Component>(64);
    internal static readonly Dictionary<int, LayoutState> layoutStateById = new Dictionary<int, LayoutState>();
    internal static readonly Queue<Action> deferredEditorActions = new Queue<Action>(8);

    internal static bool deferredSpaDayHooked;
    internal static bool editorDelayScheduled;

    internal static LayoutState GetLayoutState(Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return null;
        }

        int id = table.GetInstanceID();

        if (!layoutStateById.TryGetValue(id, out LayoutState state))
        {
            state = new LayoutState();
            layoutStateById[id] = state;
        }

        return state;
    }

    internal static bool NearlyEqual(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) < 0.0001f && Mathf.Abs(a.y - b.y) < 0.0001f;
    }

    internal static int ClampRow(Ux_TonkersTableTopiaLayout table, int row)
    {
        return Mathf.Clamp(row, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
    }

    internal static int ClampColumn(Ux_TonkersTableTopiaLayout table, int col)
    {
        return Mathf.Clamp(col, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
    }

    internal static bool TryNormalizeInclusiveRange(int startInclusive, int endInclusive, int count, out int minInclusive, out int maxInclusive)
    {
        minInclusive = 0;
        maxInclusive = -1;

        if (count <= 0)
        {
            return false;
        }

        minInclusive = Mathf.Clamp(startInclusive, 0, count - 1);
        maxInclusive = Mathf.Clamp(endInclusive, 0, count - 1);

        if (maxInclusive < minInclusive)
        {
            int temp = minInclusive;
            minInclusive = maxInclusive;
            maxInclusive = temp;
        }

        return true;
    }

    internal static HashSet<Ux_TonkersTableTopiaCell> PrepareMainCellScratch(bool distinctMainsOnly)
    {
        if (!distinctMainsOnly)
        {
            return null;
        }

        mainCellScratch.Clear();
        return mainCellScratch;
    }

    internal static Ux_TonkersTableTopiaCell ResolveMainCell(Ux_TonkersTableTopiaCell cell)
    {
        if (cell != null && cell.isMashedLikePotatoes && cell.mashedIntoWho != null)
        {
            return cell.mashedIntoWho;
        }

        return cell;
    }

    internal static bool EvaluateSelection(
        Ux_TonkersTableTopiaLayout table,
        int r0,
        int r1,
        int c0,
        int c1,
        AlignmentQueryMode mode,
        Ux_TonkersTableTopiaLayout.HorizontalAlignment wantH = default,
        Ux_TonkersTableTopiaLayout.VerticalAlignment wantV = default)
    {
        if (table == null)
        {
            return false;
        }

        bool any = false;

        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                if (!Ux_TonkersTableTopiaAlignmentUtility.TryDetectCellAlignment(table, r, c, out bool isFull, out var h, out var v))
                {
                    return false;
                }

                switch (mode)
                {
                    case AlignmentQueryMode.Full:
                        if (!isFull)
                        {
                            return false;
                        }
                        break;

                    case AlignmentQueryMode.Both:
                        if (isFull || h != wantH || v != wantV)
                        {
                            return false;
                        }
                        break;

                    case AlignmentQueryMode.Horizontal:
                        if (isFull || h != wantH)
                        {
                            return false;
                        }
                        break;

                    case AlignmentQueryMode.Vertical:
                        if (isFull || v != wantV)
                        {
                            return false;
                        }
                        break;
                }

                any = true;
            }
        }

        return any;
    }

    internal static void ApplyAlignmentToSelection(
        Ux_TonkersTableTopiaLayout table,
        int r0,
        int r1,
        int c0,
        int c1,
        AlignmentApplyMode mode,
        Ux_TonkersTableTopiaLayout.HorizontalAlignment h = default,
        Ux_TonkersTableTopiaLayout.VerticalAlignment v = default,
        string undoName = "Align Cell")
    {
        if (table == null)
        {
            return;
        }

        bool changed = false;

        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                RectTransform cellRect = table.FetchCellRectTransformVIP(r, c);

                if (cellRect == null)
                {
                    continue;
                }

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RegisterFullObjectHierarchyUndo(cellRect.gameObject, undoName);
                }
#endif

                ApplyAlignmentToCell(cellRect, mode, h, v);
                changed = true;
            }
        }

        if (changed)
        {
            table.FlagLayoutAsNeedingSpaDay();
        }
    }

    internal static void ApplyAlignmentToCell(
        RectTransform parent,
        AlignmentApplyMode mode,
        Ux_TonkersTableTopiaLayout.HorizontalAlignment h,
        Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (parent == null)
        {
            return;
        }

        switch (mode)
        {
            case AlignmentApplyMode.Fill:
                Ux_TonkersTableTopiaAlignmentUtility.AlignForeignersToFill(parent, true);
                return;

            case AlignmentApplyMode.Both:
                Ux_TonkersTableTopiaAlignmentUtility.AlignForeignersInRect(parent, h, v, true);
                return;

            case AlignmentApplyMode.Horizontal:
                ApplyHorizontalAlignment(parent, h);
                return;

            case AlignmentApplyMode.Vertical:
                ApplyVerticalAlignment(parent, v);
                return;
        }
    }

    internal static void ApplyHorizontalAlignment(RectTransform parent, Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        float anchorX = Ux_TonkersTableTopiaAlignmentUtility.GetHorizontalAnchor(h);

        for (int i = 0; i < parent.childCount; i++)
        {
            if (!Ux_TonkersTableTopiaForeignContentUtility.TryGetForeignRect(parent.GetChild(i), true, out RectTransform child))
            {
                continue;
            }

            Ux_TonkersTableTopiaAlignmentUtility.ApplyHorizontalAnchor(child, anchorX);
            Ux_TonkersTableTopiaRectTransformUtility.EnsureReasonableSize(child, parent);

            Text text = child.GetComponent<Text>();

            if (text != null)
            {
                text.alignment = Ux_TonkersTableTopiaAlignmentUtility.WithHorizontalChanged(text.alignment, h);
            }
        }
    }

    internal static void ApplyVerticalAlignment(RectTransform parent, Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        float anchorY = Ux_TonkersTableTopiaAlignmentUtility.GetVerticalAnchor(v);

        for (int i = 0; i < parent.childCount; i++)
        {
            if (!Ux_TonkersTableTopiaForeignContentUtility.TryGetForeignRect(parent.GetChild(i), true, out RectTransform child))
            {
                continue;
            }

            Ux_TonkersTableTopiaAlignmentUtility.ApplyVerticalAnchor(child, anchorY);
            Ux_TonkersTableTopiaRectTransformUtility.EnsureReasonableSize(child, parent);

            Text text = child.GetComponent<Text>();

            if (text != null)
            {
                text.alignment = Ux_TonkersTableTopiaAlignmentUtility.WithVerticalChanged(text.alignment, v);
            }
        }
    }

    internal static GUIStyle GetSafeLabelStyle(GUIStyle style)
    {
        if (style != null)
        {
            return style;
        }

#if UNITY_EDITOR
        if (UnityEditor.EditorStyles.label != null)
        {
            return UnityEditor.EditorStyles.label;
        }
#endif

        return GUI.skin != null ? GUI.skin.label : new GUIStyle();
    }
}