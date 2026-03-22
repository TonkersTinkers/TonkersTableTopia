using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static Ux_TonkersTableTopiaEditorExtensions;
using static Ux_TonkersTableTopiaLayoutSizingExtensions;

[CustomEditor(typeof(Ux_TonkersTableTopiaRow))]
public class Ux_TonkersTableTopiaRowEditor : Editor
{
    private static readonly GUIContent GC_BackToTable = new GUIContent("Back to Tonkers Table Topia");

    private static readonly GUIContent GC_DeleteRow = new GUIContent("Delete Row");

    private static readonly Queue<System.Action> _deferred = new Queue<System.Action>(4);

    private static bool _delayScheduled;

    private Ux_TonkersTableTopiaLayout _cachedTable;

    public override void OnInspectorGUI()
    {
        var row = (Ux_TonkersTableTopiaRow)target;
        var table = _cachedTable != null ? _cachedTable : (_cachedTable = row ? row.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_BackToTable, GUILayout.Height(22))) Selection.activeObject = table;
            GUI.enabled = true;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_DeleteRow, GUILayout.Height(22)))
            {
                int r = row.rowNumberWhereShenanigansOccur;
                DeferToTableAndExit(table, () =>
                {
                    Undo.RecordObject(table, "Delete Row");
                    table.SafeDeleteRowAtWithWittyConfirm(r);
                });
            }
            GUI.enabled = true;
        }

        if (table == null)
        {
            EditorGUILayout.HelpBox("No TonkersTableTopiaLayout found in parents.", MessageType.Info);
            return;
        }

        int rIdx = Mathf.Clamp(row.rowNumberWhereShenanigansOccur, 0, table.totalRowsCountLetTheShowBegin - 1);
        if (table.snazzyRowWardrobes.Count != table.totalRowsCountLetTheShowBegin) table.SyncRowWardrobes();
        var rs = table.snazzyRowWardrobes[rIdx];

        bool show = EditorPrefs.GetBool("TTT_Row_ShowStyle", true);
        show = EditorGUILayout.Foldout(show, $"Row {rIdx + 1}");
        EditorPrefs.SetBool("TTT_Row_ShowStyle", show);
        if (!show) return;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Row {rIdx + 1}", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Align", GUILayout.Width(50));
                bool leftOn = table.IsRowHorizAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                bool centerOn = table.IsRowHorizAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                bool rightOn = table.IsRowHorizAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                bool topOn = table.IsRowVertAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                bool middleOn = table.IsRowVertAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                bool bottomOn = table.IsRowVertAlignedLikeMirror(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                bool fullOn = table.IsRowFullLikeWaterbed(rIdx);
                float w7 = CalcShrinkyDinkWidthLikeDietCokeSquisher(7, 50f, 100f);

                EditorGUI.BeginDisabledGroup(leftOn);
                if (GUILayout.Button("Left", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Left");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(centerOn);
                if (GUILayout.Button("Center", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Center");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(rightOn);
                if (GUILayout.Button("Right", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Right");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(topOn);
                if (GUILayout.Button("Top", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Top");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(middleOn);
                if (GUILayout.Button("Middle", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Middle");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(bottomOn);
                if (GUILayout.Button("Bottom", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Bottom");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(fullOn);
                if (GUILayout.Button("Full", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Full");
                    table.AlignRowToFillLikeWaterbed(rIdx);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();
            }

            bool useFixedHeight = rs.requestedHeightMaybePercentIfNegative > 0f;
            bool newUseFixedHeight = useFixedHeight;
            float newRequested = rs.requestedHeightMaybePercentIfNegative;
            Sprite newBackdrop = rs.backdropPictureOnTheHouse;
            Color newTint = rs.backdropTintFlavor;
            bool newSliced = rs.backdropUseSlicedLikePizza;
            bool newFillLast = rs.lastVisibleCellEatsLeftovers;
            bool newCustom = rs.customAnchorsAndPivotBecauseWeFancy;
            Vector2 newAnchorMin = rs.customAnchorMinPointy;
            Vector2 newAnchorMax = rs.customAnchorMaxPointy;
            Vector2 newPivot = rs.customPivotSpinny;
            bool rowImgToggle = EditorPrefs.GetBool($"TTT_RowImg_{rIdx}", false);
            bool newRowImgToggle = rowImgToggle;

            EditorGUI.BeginChangeCheck();

            newUseFixedHeight = EditorGUILayout.Toggle("Use Fixed Height", useFixedHeight);
            if (newUseFixedHeight)
            {
                float currentInnerHeight = Mathf.Max(0f, table.GetComponent<RectTransform>().rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);
                float px = useFixedHeight ? table.ResolveRowSpecForCurrentInnerHeightLikeBlueprint(rs.requestedHeightMaybePercentIfNegative, currentInnerHeight) : table.GetLiveRowHeightPixelsLikeTapeMeasure(rIdx);
                px = EditorGUILayout.FloatField("Height (px)", px);
                newRequested = table.ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, px), currentInnerHeight);
            }
            else
            {
                float[] livePct = table.ComputeRowPercentagesLikeASpreadsheet();
                float pct = rs.requestedHeightMaybePercentIfNegative < 0f ? (-rs.requestedHeightMaybePercentIfNegative * 100f) : ((rIdx < livePct.Length ? livePct[rIdx] : 0f) * 100f);
                pct = Mathf.Clamp(EditorGUILayout.Slider("Height %", pct, 0f, 100f), 0f, 100f);
                newRequested = pct > 0f ? -(pct / 100f) : 0f;
            }

            newFillLast = EditorGUILayout.Toggle("Last Visible Cell Eats Leftovers", newFillLast);

            newRowImgToggle = EditorGUILayout.ToggleLeft("Image Settings", rowImgToggle);
            if (newRowImgToggle)
            {
                newBackdrop = (Sprite)EditorGUILayout.ObjectField("Background Image", newBackdrop, typeof(Sprite), false);
                if (newBackdrop != null)
                {
                    newTint = EditorGUILayout.ColorField("Tint Color", newTint);
                    newSliced = EditorGUILayout.Toggle("Sliced", newSliced);
                }
            }

            newCustom = EditorGUILayout.Toggle("Custom Anchors & Pivot", newCustom);
            if (newCustom)
            {
                newAnchorMin = EditorGUILayout.Vector2Field("Anchor Min", newAnchorMin);
                newAnchorMax = EditorGUILayout.Vector2Field("Anchor Max", newAnchorMax);
                newPivot = EditorGUILayout.Vector2Field("Pivot", newPivot);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(table, "Edit Row Style");
                EditorPrefs.SetBool($"TTT_RowImg_{rIdx}", newRowImgToggle);
                rs.requestedHeightMaybePercentIfNegative = newRequested;
                rs.backdropPictureOnTheHouse = newBackdrop;
                rs.backdropTintFlavor = newTint;
                rs.backdropUseSlicedLikePizza = newSliced;
                rs.lastVisibleCellEatsLeftovers = newFillLast;
                rs.customAnchorsAndPivotBecauseWeFancy = newCustom;
                rs.customAnchorMinPointy = newAnchorMin;
                rs.customAnchorMaxPointy = newAnchorMax;
                rs.customPivotSpinny = newPivot;
                table.shareThePieEvenlyForRows = false;
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
    }

    private static void EnqueueDeferred(System.Action a)
    {
        if (a == null) return;
        _deferred.Enqueue(a);
        if (!_delayScheduled)
        {
            _delayScheduled = true;
            EditorApplication.delayCall += DrainDeferred;
        }
    }

    private static void DrainDeferred()
    {
        while (_deferred.Count > 0)
        {
            var act = _deferred.Dequeue();
            try { act(); } catch { }
        }
        _delayScheduled = false;
    }

    private static void DeferToTableAndExit(Ux_TonkersTableTopiaLayout table, System.Action action)
    {
        if (table == null) return;

        Selection.activeObject = table;

        DeferEditorSafe(() =>
        {
            if (table == null) return;
            action?.Invoke();
            EditorGUIUtility.PingObject(table);
            RequestWysiRepaintLikeFreshCoat();
        });

        GUIUtility.ExitGUI();
    }

    private void OnEnable()
    {
        var row = (Ux_TonkersTableTopiaRow)target;
        _cachedTable = row ? row.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;
    }
}