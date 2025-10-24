using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Ux_TonkersTableTopiaCell))]
public class Ux_TonkersTableTopiaCellEditor : Editor
{
    private static readonly GUIContent GC_BackToTable = new GUIContent("Back to Tonkers Table Topia");
    private static readonly GUIContent GC_DeleteCell = new GUIContent("Delete Cell");
    private static readonly GUIContent GC_DeleteRow = new GUIContent("Delete Row");

    private static readonly Queue<System.Action> _deferred = new Queue<System.Action>(4);
    private static bool _delayScheduled;

    private Ux_TonkersTableTopiaLayout _cachedTable;
    private Ux_TonkersTableTopiaCell _cell;

    public override void OnInspectorGUI()
    {
        var cell = _cell != null ? _cell : (_cell = (Ux_TonkersTableTopiaCell)target);
        var table = _cachedTable != null ? _cachedTable : (_cachedTable = cell ? cell.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_BackToTable, GUILayout.Height(22))) Selection.activeObject = table;
            GUI.enabled = true;
        }
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_DeleteCell, GUILayout.Height(22)))
            {
                int r = cell.rowNumberWhereThePartyIs;
                int c = cell.columnNumberPrimeRib;
                DeferToTableAndExit(table, () =>
                {
                    Undo.RecordObject(table, "Delete Cell");
                    table.TryKindlyDeleteCell(r, c);
                });
            }
            if (GUILayout.Button(GC_DeleteRow, GUILayout.Height(22)))
            {
                int r = cell.rowNumberWhereThePartyIs;
                DeferToTableAndExit(table, () =>
                {
                    Undo.RecordObject(table, "Delete Row");
                    table.SafeDeleteRowAtWithWittyConfirm(r);
                });
            }
            GUI.enabled = true;
        }
        serializedObject.Update();
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Row: {cell.rowNumberWhereThePartyIs}", GUILayout.Width(80));
            EditorGUILayout.LabelField($"Col: {cell.columnNumberPrimeRib}", GUILayout.Width(80));
            GUILayout.FlexibleSpace();
        }
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Align", GUILayout.Width(50));

            bool isFull = false;
            Ux_TonkersTableTopiaLayout.HorizontalAlignment hCur = Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;
            Ux_TonkersTableTopiaLayout.VerticalAlignment vCur = Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
            bool hasState = table != null &&
                            table.TryDetectCellAlignmentLikeLieDetector(
                                cell.rowNumberWhereThePartyIs,
                                cell.columnNumberPrimeRib,
                                out isFull, out hCur, out vCur);
            bool leftOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
            bool centerOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
            bool rightOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
            bool topOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top;
            bool middleOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
            bool bottomOn = hasState && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom;
            bool fullOn = hasState && isFull;

            EditorGUI.BeginDisabledGroup(leftOn);
            if (GUILayout.Button("Left"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Left");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(centerOn);
            if (GUILayout.Button("Center"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Center");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(rightOn);
            if (GUILayout.Button("Right"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Right");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(topOn);
            if (GUILayout.Button("Top"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Top");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(middleOn);
            if (GUILayout.Button("Middle"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Middle");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(bottomOn);
            if (GUILayout.Button("Bottom"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Bottom");
                    table.AlignCellForeignsLikeLaserLevel(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(fullOn);
            if (GUILayout.Button("Full"))
            {
                if (table)
                {
                    Undo.RecordObject(table, "Align Cell Full");
                    table.AlignCellForeignsToFillLikeStuffedBurrito(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib);
                    EditorUtility.SetDirty(table);
                    Ux_TonkersTableTopiaExtensions.RequestWysiRepaintLikeFreshCoat();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        EditorGUI.BeginChangeCheck();
        using (new EditorGUI.DisabledScope(true))
        {
            int newRowSpan = Mathf.Max(1, EditorGUILayout.IntField("Row Span", cell.howManyRowsAreHoggingThisSeat));
            int newColSpan = Mathf.Max(1, EditorGUILayout.IntField("Col Span", cell.howManyColumnsAreSneakingIn));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cell, "Edit Cell Span");
                cell.howManyRowsAreHoggingThisSeat = newRowSpan;
                cell.howManyColumnsAreSneakingIn = newColSpan;
                if (table)
                {
                    Undo.RecordObject(table, "Edit Cell Span");
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table && (cell.howManyRowsAreHoggingThisSeat > 1 || cell.howManyColumnsAreSneakingIn > 1);
            if (GUILayout.Button("Unmerge", GUILayout.Height(20)))
            {
                Undo.RecordObject(table, "Unmerge Cell");
                table.UnmergeCellEveryoneGetsTheirOwnChair(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib);
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
            GUI.enabled = true;
        }

        bool imgToggle = EditorPrefs.GetBool($"TTT_CellImg_{cell.GetInstanceID()}", false);
        bool newImgToggle = EditorGUILayout.ToggleLeft("Image Settings", imgToggle);
        if (newImgToggle != imgToggle) EditorPrefs.SetBool($"TTT_CellImg_{cell.GetInstanceID()}", newImgToggle);
        if (newImgToggle)
        {
            EditorGUI.BeginChangeCheck();
            var newSprite = (Sprite)EditorGUILayout.ObjectField("Background Image", cell.backgroundPictureBecausePlainIsLame, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cell, "Edit Cell Background");
                cell.backgroundPictureBecausePlainIsLame = newSprite;
                if (table)
                {
                    Undo.RecordObject(table, "Edit Cell Background");
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                }
            }
            if (cell.backgroundPictureBecausePlainIsLame != null)
            {
                EditorGUI.BeginChangeCheck();
                var newColor = EditorGUILayout.ColorField("Tint Color", cell.backgroundColorLikeASunset);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cell, "Edit Cell Color");
                    cell.backgroundColorLikeASunset = newColor;
                    if (table)
                    {
                        Undo.RecordObject(table, "Edit Cell Color");
                        table.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(table);
                    }
                }
            }
        }

        bool padToggle = EditorPrefs.GetBool($"TTT_CellPad_{cell.GetInstanceID()}", false);
        bool newPadToggle = EditorGUILayout.ToggleLeft("Padding Settings", padToggle);
        if (newPadToggle != padToggle) EditorPrefs.SetBool($"TTT_CellPad_{cell.GetInstanceID()}", newPadToggle);
        if (newPadToggle)
        {
            EditorGUI.BeginChangeCheck();
            bool usePad = EditorGUILayout.Toggle("Inner Padding Pillow Fort", cell.useInnerPaddingPillowFort);
            float left = cell.innerPaddingLeftMarshmallow;
            float right = cell.innerPaddingRightMarshmallow;
            float top = cell.innerPaddingTopMarshmallow;
            float bottom = cell.innerPaddingBottomMarshmallow;
            if (usePad)
            {
                EditorGUI.indentLevel++;
                left = EditorGUILayout.FloatField("Left Marshmallow", left);
                right = EditorGUILayout.FloatField("Right Marshmallow", right);
                top = EditorGUILayout.FloatField("Top Marshmallow", top);
                bottom = EditorGUILayout.FloatField("Bottom Marshmallow", bottom);
                EditorGUI.indentLevel--;
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cell, "Edit Cell Padding");
                cell.useInnerPaddingPillowFort = usePad;
                cell.innerPaddingLeftMarshmallow = left;
                cell.innerPaddingRightMarshmallow = right;
                cell.innerPaddingTopMarshmallow = top;
                cell.innerPaddingBottomMarshmallow = bottom;
                if (table)
                {
                    Undo.RecordObject(table, "Edit Cell Padding");
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                }
                EditorUtility.SetDirty(cell);
            }
        }

        if (table == null)
        {
            EditorGUILayout.HelpBox("No TonkersTableTopiaLayout found in parents.", MessageType.Info);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (table.fancyColumnWardrobes.Count != table.totalColumnsCountHighFive) table.SyncColumnWardrobes();
        if (table.snazzyRowWardrobes.Count != table.totalRowsCountLetTheShowBegin) table.SyncRowWardrobes();

        int rIdx = Mathf.Clamp(cell.rowNumberWhereThePartyIs, 0, table.totalRowsCountLetTheShowBegin - 1);
        int cIdx = Mathf.Clamp(cell.columnNumberPrimeRib, 0, table.totalColumnsCountHighFive - 1);
        var col = table.fancyColumnWardrobes[cIdx];
        var row = table.snazzyRowWardrobes[rIdx];

        EditorGUILayout.Space(8);
        bool showCol = EditorPrefs.GetBool("TTT_Cell_ShowCol", true);
        showCol = EditorGUILayout.Foldout(showCol, $"Column {cIdx + 1}");
        EditorPrefs.SetBool("TTT_Cell_ShowCol", showCol);
        if (showCol)
        {
            bool manualCols = EditorPrefs.GetBool("TTT_ManualCols", false);
            EditorGUI.BeginChangeCheck();
            manualCols = EditorGUILayout.Toggle("Fixed Widths", manualCols);
            EditorPrefs.SetBool("TTT_ManualCols", manualCols);
            if (manualCols)
            {
                float px = Mathf.Max(0f, col.requestedWidthMaybePercentIfNegative > 0f ? col.requestedWidthMaybePercentIfNegative : 0f);
                px = EditorGUILayout.FloatField("Width (px)", px);
                col.requestedWidthMaybePercentIfNegative = Mathf.Max(0f, px);
            }
            else
            {
                float pct = col.requestedWidthMaybePercentIfNegative < 0f ? (-col.requestedWidthMaybePercentIfNegative * 100f) : 0f;
                pct = Mathf.Clamp(EditorGUILayout.Slider("Width %", pct, 0f, 100f), 0f, 100f);
                col.requestedWidthMaybePercentIfNegative = pct > 0f ? -(pct / 100f) : 0f;
            }
            bool colImgToggle = EditorPrefs.GetBool($"TTT_ColImg_{cIdx}", false);
            bool newColImgToggle = EditorGUILayout.ToggleLeft("Image Settings", colImgToggle);
            if (newColImgToggle != colImgToggle) EditorPrefs.SetBool($"TTT_ColImg_{cIdx}", newColImgToggle);
            if (newColImgToggle)
            {
                col.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background Image", col.backdropPictureOnTheHouse, typeof(Sprite), false);
                if (col.backdropPictureOnTheHouse != null)
                {
                    col.backdropTintFlavor = EditorGUILayout.ColorField("Tint Color", col.backdropTintFlavor);
                }
            }
            col.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Custom Anchors & Pivot", col.customAnchorsAndPivotBecauseWeFancy);
            if (col.customAnchorsAndPivotBecauseWeFancy)
            {
                col.customAnchorMinPointy = EditorGUILayout.Vector2Field("Anchor Min", col.customAnchorMinPointy);
                col.customAnchorMaxPointy = EditorGUILayout.Vector2Field("Anchor Max", col.customAnchorMaxPointy);
                col.customPivotSpinny = EditorGUILayout.Vector2Field("Pivot", col.customPivotSpinny);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(table, "Edit Column Style");
                table.shareThePieEvenlyForColumns = false;
                table.SyncColumnWardrobes();
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }

        EditorGUILayout.Space(6);
        bool showRow = EditorPrefs.GetBool("TTT_Cell_ShowRow", true);
        showRow = EditorGUILayout.Foldout(showRow, $"Row {rIdx + 1}");
        EditorPrefs.SetBool("TTT_Cell_ShowRow", showRow);
        if (showRow)
        {
            bool manualRows = EditorPrefs.GetBool("TTT_ManualRows", false);
            EditorGUI.BeginChangeCheck();
            manualRows = EditorGUILayout.Toggle("Fixed Heights", manualRows);
            EditorPrefs.SetBool("TTT_ManualRows", manualRows);
            if (manualRows)
            {
                float px = Mathf.Max(0f, row.requestedHeightMaybePercentIfNegative > 0f ? row.requestedHeightMaybePercentIfNegative : 0f);
                px = EditorGUILayout.FloatField("Height (px)", px);
                row.requestedHeightMaybePercentIfNegative = Mathf.Max(0f, px);
            }
            else
            {
                float pct = row.requestedHeightMaybePercentIfNegative < 0f ? (-row.requestedHeightMaybePercentIfNegative * 100f) : 0f;
                pct = Mathf.Clamp(EditorGUILayout.Slider("Height %", pct, 0f, 100f), 0f, 100f);
                row.requestedHeightMaybePercentIfNegative = pct > 0f ? -(pct / 100f) : 0f;
            }
            bool rowImgToggle = EditorPrefs.GetBool($"TTT_RowImg_{rIdx}", false);
            bool newRowImgToggle = EditorGUILayout.ToggleLeft("Image Settings", rowImgToggle);
            if (newRowImgToggle != rowImgToggle) EditorPrefs.SetBool($"TTT_RowImg_{rIdx}", newRowImgToggle);
            if (newRowImgToggle)
            {
                row.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background Image", row.backdropPictureOnTheHouse, typeof(Sprite), false);
                if (row.backdropPictureOnTheHouse != null)
                {
                    row.backdropTintFlavor = EditorGUILayout.ColorField("Tint Color", row.backdropTintFlavor);
                }
            }
            row.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Custom Anchors & Pivot", row.customAnchorsAndPivotBecauseWeFancy);
            if (row.customAnchorsAndPivotBecauseWeFancy)
            {
                row.customAnchorMinPointy = EditorGUILayout.Vector2Field("Anchor Min", row.customAnchorMinPointy);
                row.customAnchorMaxPointy = EditorGUILayout.Vector2Field("Anchor Max", row.customAnchorMaxPointy);
                row.customPivotSpinny = EditorGUILayout.Vector2Field("Pivot", row.customPivotSpinny);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(table, "Edit Row Style");
                table.shareThePieEvenlyForRows = false;
                table.SyncRowWardrobes();
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
        serializedObject.ApplyModifiedProperties();
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
        _cell = (Ux_TonkersTableTopiaCell)target;
        _cachedTable = _cell ? _cell.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;
    }
}
