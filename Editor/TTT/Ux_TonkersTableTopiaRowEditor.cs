using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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

                float w7 = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(7, 50f, 100f);

                EditorGUI.BeginDisabledGroup(leftOn);
                if (GUILayout.Button("Left", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Left");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(centerOn);
                if (GUILayout.Button("Center", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Center");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(rightOn);
                if (GUILayout.Button("Right", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Right");
                    table.AlignRowHorizontalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(topOn);
                if (GUILayout.Button("Top", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Top");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(middleOn);
                if (GUILayout.Button("Middle", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Middle");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(bottomOn);
                if (GUILayout.Button("Bottom", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Bottom");
                    table.AlignRowVerticalOnlyLikeLaserLevel(rIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(fullOn);
                if (GUILayout.Button("Full", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Row Full");
                    table.AlignRowToFillLikeWaterbed(rIdx);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();
            }

            bool manualRows = EditorPrefs.GetBool("TTT_ManualRows", false);
            EditorGUI.BeginChangeCheck();
            manualRows = EditorGUILayout.Toggle("Fixed Heights", manualRows);
            EditorPrefs.SetBool("TTT_ManualRows", manualRows);

            if (manualRows)
            {
                float px = Mathf.Max(0f, rs.requestedHeightMaybePercentIfNegative > 0f ? rs.requestedHeightMaybePercentIfNegative : 0f);
                px = EditorGUILayout.FloatField("Height (px)", px);
                rs.requestedHeightMaybePercentIfNegative = Mathf.Max(0f, px);
            }
            else
            {
                float pct = rs.requestedHeightMaybePercentIfNegative < 0f ? (-rs.requestedHeightMaybePercentIfNegative * 100f) : 0f;
                pct = Mathf.Clamp(EditorGUILayout.Slider("Height %", pct, 0f, 100f), 0f, 100f);
                rs.requestedHeightMaybePercentIfNegative = pct > 0f ? -(pct / 100f) : 0f;
            }

            rs.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background Image", rs.backdropPictureOnTheHouse, typeof(Sprite), false);
            rs.backdropTintFlavor = EditorGUILayout.ColorField("Tint Color", rs.backdropTintFlavor);
            rs.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Custom Anchors & Pivot", rs.customAnchorsAndPivotBecauseWeFancy);
            if (rs.customAnchorsAndPivotBecauseWeFancy)
            {
                rs.customAnchorMinPointy = EditorGUILayout.Vector2Field("Anchor Min", rs.customAnchorMinPointy);
                rs.customAnchorMaxPointy = EditorGUILayout.Vector2Field("Anchor Max", rs.customAnchorMaxPointy);
                rs.customPivotSpinny = EditorGUILayout.Vector2Field("Pivot", rs.customPivotSpinny);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(table, "Edit Row Style");
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
        EnqueueDeferred(() =>
        {
            if (table != null) action?.Invoke();
            EditorGUIUtility.PingObject(table);
        });
        GUIUtility.ExitGUI();
    }

    private void OnEnable()
    {
        var row = (Ux_TonkersTableTopiaRow)target;
        _cachedTable = row ? row.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;
    }
}