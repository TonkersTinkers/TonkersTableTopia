using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ux_TonkersTableTopiaCell))]
public class Ux_TonkersTableTopiaCellEditor : Editor
{
    private SerializedProperty bgImageProp, bgColorProp;
    private SerializedProperty rowIndexProp, colIndexProp, rowSpanProp, colSpanProp, isMergedProp, mergedIntoProp;

    public override void OnInspectorGUI()
    {
        var cell = (Ux_TonkersTableTopiaCell)target;
        var table = cell ? cell.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table != null;
            if (GUILayout.Button("Back to Tonkers Table Topia", GUILayout.Height(22))) Selection.activeObject = table;
            GUI.enabled = true;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUI.enabled = table != null;
            if (GUILayout.Button("Delete Cell", GUILayout.Height(22)))
            {
                int r = cell.rowNumberWhereThePartyIs;
                int c = cell.columnNumberPrimeRib;
                DeferToTableAndExit(table, () =>
                {
                    Undo.RecordObject(table, "Delete Cell");
                    table.TryKindlyDeleteCell(r, c);
                });
            }
            if (GUILayout.Button("Delete Row", GUILayout.Height(22)))
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

        EditorGUI.BeginChangeCheck();
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
            var newSprite = (Sprite)EditorGUILayout.ObjectField("Cell Background", cell.backgroundPictureBecausePlainIsLame, typeof(Sprite), false);
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
                var newColor = EditorGUILayout.ColorField("Cell Color", cell.backgroundColorLikeASunset);
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
            manualCols = EditorGUILayout.Toggle("Manual Widths", manualCols);
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
                col.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background", col.backdropPictureOnTheHouse, typeof(Sprite), false);
                if (col.backdropPictureOnTheHouse != null)
                {
                    col.backdropTintFlavor = EditorGUILayout.ColorField("Color", col.backdropTintFlavor);
                }
            }

            col.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Override Anchors/Pivot", col.customAnchorsAndPivotBecauseWeFancy);
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
            manualRows = EditorGUILayout.Toggle("Manual Heights", manualRows);
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
                row.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background", row.backdropPictureOnTheHouse, typeof(Sprite), false);
                if (row.backdropPictureOnTheHouse != null)
                {
                    row.backdropTintFlavor = EditorGUILayout.ColorField("Color", row.backdropTintFlavor);
                }
            }

            row.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Override Anchors/Pivot", row.customAnchorsAndPivotBecauseWeFancy);
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

    private static void DeferToTableAndExit(Ux_TonkersTableTopiaLayout table, System.Action action)
    {
        if (table == null) return;
        Selection.activeObject = table;
        EditorApplication.delayCall += () =>
        {
            if (table != null) action?.Invoke();
            EditorGUIUtility.PingObject(table);
        };
        EditorGUIUtility.ExitGUI();
    }

    private void OnEnable()
    {
        rowIndexProp = serializedObject.FindProperty("rowNumberWhereThePartyIs");
        colIndexProp = serializedObject.FindProperty("columnNumberPrimeRib");
        rowSpanProp = serializedObject.FindProperty("howManyRowsAreHoggingThisSeat");
        colSpanProp = serializedObject.FindProperty("howManyColumnsAreSneakingIn");
        isMergedProp = serializedObject.FindProperty("isMashedLikePotatoes");
        mergedIntoProp = serializedObject.FindProperty("mashedIntoWho");
        bgImageProp = serializedObject.FindProperty("backgroundPictureBecausePlainIsLame");
        bgColorProp = serializedObject.FindProperty("backgroundColorLikeASunset");
    }
}
