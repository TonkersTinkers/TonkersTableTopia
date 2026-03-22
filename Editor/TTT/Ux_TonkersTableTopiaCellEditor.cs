using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static Ux_TonkersTableTopiaEditorExtensions;

[CustomEditor(typeof(Ux_TonkersTableTopiaCell))]
public class Ux_TonkersTableTopiaCellEditor : Editor
{
    private static readonly GUIContent GC_BackToTable = new GUIContent("Back to Tonkers Table Topia");
    private static readonly GUIContent GC_DeleteCell = new GUIContent("Delete Cell");
    private static readonly GUIContent GC_DeleteRow = new GUIContent("Delete Row");

    private static readonly Queue<System.Action> _deferred = new Queue<System.Action>(4);

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
            bool hasState = table != null && table.TryDetectCellAlignmentLikeLieDetector(
                cell.rowNumberWhereThePartyIs,
                cell.columnNumberPrimeRib,
                out isFull,
                out hCur,
                out vCur);

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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
                    RequestWysiRepaintLikeFreshCoat();
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
            bool useFixedWidth = col.requestedWidthMaybePercentIfNegative > 0f;
            bool newUseFixedWidth = useFixedWidth;
            float newRequested = col.requestedWidthMaybePercentIfNegative;
            Sprite newBackdrop = col.backdropPictureOnTheHouse;
            Color newTint = col.backdropTintFlavor;
            bool newSliced = col.backdropUseSlicedLikePizza;
            bool newOneBig = col.useOneBigBackdropForWholeColumn;
            bool newCustom = col.customAnchorsAndPivotBecauseWeFancy;
            Vector2 newAnchorMin = col.customAnchorMinPointy;
            Vector2 newAnchorMax = col.customAnchorMaxPointy;
            Vector2 newPivot = col.customPivotSpinny;
            bool colImgToggle = EditorPrefs.GetBool($"TTT_ColImg_{cIdx}", false);
            bool newColImgToggle = colImgToggle;

            EditorGUI.BeginChangeCheck();

            newUseFixedWidth = EditorGUILayout.Toggle("Use Fixed Width", useFixedWidth);
            if (newUseFixedWidth)
            {
                float currentInnerWidth = Mathf.Max(0f, table.GetComponent<RectTransform>().rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);
                float px = useFixedWidth ? table.ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(col.requestedWidthMaybePercentIfNegative, currentInnerWidth) : table.GetLiveColumnWidthPixelsLikeTapeMeasure(cIdx);
                px = EditorGUILayout.FloatField("Width (px)", px);
                newRequested = table.ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, px), currentInnerWidth);
            }
            else
            {
                float[] livePct = table.ComputeColumnPercentagesLikeASpreadsheet();
                float pct = col.requestedWidthMaybePercentIfNegative < 0f ? (-col.requestedWidthMaybePercentIfNegative * 100f) : ((cIdx < livePct.Length ? livePct[cIdx] : 0f) * 100f);
                pct = Mathf.Clamp(EditorGUILayout.Slider("Width %", pct, 0f, 100f), 0f, 100f);
                newRequested = pct > 0f ? -(pct / 100f) : 0f;
            }

            newColImgToggle = EditorGUILayout.ToggleLeft("Image Settings", colImgToggle);
            if (newColImgToggle)
            {
                newBackdrop = (Sprite)EditorGUILayout.ObjectField("Background Image", newBackdrop, typeof(Sprite), false);
                if (newBackdrop != null)
                {
                    newTint = EditorGUILayout.ColorField("Tint Color", newTint);
                    newSliced = EditorGUILayout.Toggle("Sliced", newSliced);
                    newOneBig = EditorGUILayout.Toggle("Use One Stretched BG", newOneBig);
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
                Undo.RecordObject(table, "Edit Column Style");
                EditorPrefs.SetBool($"TTT_ColImg_{cIdx}", newColImgToggle);
                col.requestedWidthMaybePercentIfNegative = newRequested;
                col.backdropPictureOnTheHouse = newBackdrop;
                col.backdropTintFlavor = newTint;
                col.backdropUseSlicedLikePizza = newSliced;
                col.useOneBigBackdropForWholeColumn = newOneBig;
                col.customAnchorsAndPivotBecauseWeFancy = newCustom;
                col.customAnchorMinPointy = newAnchorMin;
                col.customAnchorMaxPointy = newAnchorMax;
                col.customPivotSpinny = newPivot;
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
            bool useFixedHeight = row.requestedHeightMaybePercentIfNegative > 0f;
            bool newUseFixedHeight = useFixedHeight;
            float newRequested = row.requestedHeightMaybePercentIfNegative;
            Sprite newBackdrop = row.backdropPictureOnTheHouse;
            Color newTint = row.backdropTintFlavor;
            bool newSliced = row.backdropUseSlicedLikePizza;
            bool newFillLast = row.lastVisibleCellEatsLeftovers;
            bool newCustom = row.customAnchorsAndPivotBecauseWeFancy;
            Vector2 newAnchorMin = row.customAnchorMinPointy;
            Vector2 newAnchorMax = row.customAnchorMaxPointy;
            Vector2 newPivot = row.customPivotSpinny;
            bool rowImgToggle = EditorPrefs.GetBool($"TTT_RowImg_{rIdx}", false);
            bool newRowImgToggle = rowImgToggle;

            EditorGUI.BeginChangeCheck();

            newUseFixedHeight = EditorGUILayout.Toggle("Use Fixed Height", useFixedHeight);
            if (newUseFixedHeight)
            {
                float currentInnerHeight = Mathf.Max(0f, table.GetComponent<RectTransform>().rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);
                float px = useFixedHeight ? table.ResolveRowSpecForCurrentInnerHeightLikeBlueprint(row.requestedHeightMaybePercentIfNegative, currentInnerHeight) : table.GetLiveRowHeightPixelsLikeTapeMeasure(rIdx);
                px = EditorGUILayout.FloatField("Height (px)", px);
                newRequested = table.ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, px), currentInnerHeight);
            }
            else
            {
                float[] livePct = table.ComputeRowPercentagesLikeASpreadsheet();
                float pct = row.requestedHeightMaybePercentIfNegative < 0f ? (-row.requestedHeightMaybePercentIfNegative * 100f) : ((rIdx < livePct.Length ? livePct[rIdx] : 0f) * 100f);
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
                row.requestedHeightMaybePercentIfNegative = newRequested;
                row.backdropPictureOnTheHouse = newBackdrop;
                row.backdropTintFlavor = newTint;
                row.backdropUseSlicedLikePizza = newSliced;
                row.lastVisibleCellEatsLeftovers = newFillLast;
                row.customAnchorsAndPivotBecauseWeFancy = newCustom;
                row.customAnchorMinPointy = newAnchorMin;
                row.customAnchorMaxPointy = newAnchorMax;
                row.customPivotSpinny = newPivot;
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
        _cell = (Ux_TonkersTableTopiaCell)target;
        _cachedTable = _cell ? _cell.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;
    }
}