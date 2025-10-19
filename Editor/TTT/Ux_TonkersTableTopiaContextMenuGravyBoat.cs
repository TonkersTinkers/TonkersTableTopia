using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Ux_TonkersTableTopiaContextMenuGravyBoat
{
    private static readonly List<SnackPack> _snacks = new List<SnackPack>();

    static Ux_TonkersTableTopiaContextMenuGravyBoat()
    {
        RegisterSnack("UI/Image", parent => parent.CreateImageCheeseburger());
        RegisterSnack("UI/RawImage", parent => parent.CreateRawImageNachos());
        RegisterSnack("UI/Text", parent => parent.CreateTextDadJokes());
        RegisterSnack("UI/Button", parent => parent.CreateButtonBellyFlop());
        RegisterSnack("UI/Empty Panel", parent => parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Panel", go => go.AddComponent<Image>()));
    }

    public static void RegisterSnack(string menuPath, Func<RectTransform, GameObject> maker)
    {
        if (string.IsNullOrEmpty(menuPath) || maker == null) return;

        for (int i = _snacks.Count - 1; i >= 0; i--)
        {
            if (_snacks[i].path == menuPath) _snacks.RemoveAt(i);
        }
        _snacks.Add(new SnackPack(menuPath, maker));

        _snacks.Sort((a, b) => string.Compare(a.path, b.path, StringComparison.OrdinalIgnoreCase));
    }

    public static void ShowForCell(Ux_TonkersTableTopiaLayout table, int row, int col, int spanR, int spanC, bool canMerge, bool canUnmerge, int r0, int c0, int rCount, int cCount)
    {
        RectTransform cellRT = table.FetchCellRectTransformVIP(row, col);
        if (cellRT == null) return;

        var cellComp = cellRT.GetComponent<Ux_TonkersTableTopiaCell>();
        if (cellComp != null && cellComp.isMashedLikePotatoes && cellComp.mashedIntoWho != null)
        {
            var main = cellComp.mashedIntoWho;
            cellRT = table.FetchCellRectTransformVIP(main.rowNumberWhereThePartyIs, main.columnNumberPrimeRib);
            row = main.rowNumberWhereThePartyIs;
            col = main.columnNumberPrimeRib;
            spanR = Mathf.Max(1, main.howManyRowsAreHoggingThisSeat);
            spanC = Mathf.Max(1, main.howManyColumnsAreSneakingIn);
        }

        var m = new GenericMenu();
        System.Action hl = new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, row, col, spanR, spanC));
        AddCancelSelectAndHighlightLikeABoss(m, cellRT, hl);

        for (int i = 0; i < _snacks.Count; i++)
        {
            var s = _snacks[i];
            var path = s.path;
            var baker = s.baker;
            m.AddItem(new GUIContent($"Add/{path}"), false, () =>
            {
                Undo.IncrementCurrentGroup();
                var go = baker(cellRT);
                if (go != null)
                {
                    Undo.RegisterCreatedObjectUndo(go, "Add UI Thingamajig");
                    table.FlagLayoutAsNeedingSpaDay();
                    Selection.activeGameObject = go;
                    EditorGUIUtility.PingObject(go);
                }
            });
        }
        m.AddSeparator("Add/");

        m.AddDisabledItem(new GUIContent($"Cell Info/Row {row + 1}, Col {col + 1}, Span {spanR}x{spanC}"));

        bool hasKid = table.TryFindChildTableInCellLikeSherlock(row, col, out var kidTable);
        if (hasKid && kidTable != null) m.AddDisabledItem(new GUIContent($"Cell Info/Nested Table/{kidTable.name} (Table)"));

        var infoLines = new List<string>(32);
        table.GatherCellContentLinesLikeAWaiter(row, col, infoLines);
        if (infoLines.Count == 0) m.AddDisabledItem(new GUIContent("Cell Info/Contents/(none)"));
        else
        {
            for (int i = 0; i < infoLines.Count; i++)
                m.AddDisabledItem(new GUIContent($"Cell Info/Contents/{infoLines[i]}"));
        }

        m.AddSeparator("");

        if (canMerge) m.AddItem(new GUIContent("Cells/Merge Selection"), false, () =>
        {
            Undo.RecordObject(table, "Merge Cells");
            table.MergeCellsLikeAGroupHug(r0, c0, rCount, cCount);
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });
        else m.AddDisabledItem(new GUIContent("Cells/Merge Selection"));

        if (canUnmerge) m.AddItem(new GUIContent("Cells/Unmerge Selection"), false, () =>
        {
            Undo.RecordObject(table, "Unmerge");
            table.UnmergeEverythingInRectLikeItNeverHappened(r0, c0, rCount, cCount);
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });
        else m.AddDisabledItem(new GUIContent("Cells/Unmerge Selection"));

        m.AddSeparator("Cells/");

        m.AddItem(new GUIContent("Row/Insert Above"), false, () =>
        {
            Undo.RecordObject(table, "Insert Row");
            table.InsertRowLikeANinja(row);
        });
        m.AddItem(new GUIContent("Row/Insert Below"), false, () =>
        {
            Undo.RecordObject(table, "Insert Row");
            table.InsertRowLikeANinja(row + 1);
        });
        m.AddItem(new GUIContent("Row/Delete"), false, () =>
        {
            Undo.RecordObject(table, rCount > 1 ? "Delete Rows" : "Delete Row");
            if (rCount > 1)
            {
                table.BulkDeleteRowsLikeABoss(r0, r0 + rCount - 1);
            }
            else
            {
                table.SafeDeleteRowAtWithWittyConfirm(row);
            }
        });

        m.AddItem(new GUIContent("Column/Insert Left"), false, () =>
        {
            Undo.RecordObject(table, "Insert Column");
            table.InsertColumnLikeANinja(col);
        });
        m.AddItem(new GUIContent("Column/Insert Right"), false, () =>
        {
            Undo.RecordObject(table, "Insert Column");
            table.InsertColumnLikeANinja(col + 1);
        });
        m.AddItem(new GUIContent("Column/Delete"), false, () =>
        {
            Undo.RecordObject(table, cCount > 1 ? "Delete Columns" : "Delete Column");
            if (cCount > 1)
            {
                table.BulkDeleteColumnsLikeAChamp(c0, c0 + cCount - 1);
            }
            else
            {
                table.SafeDeleteColumnAtWithWittyConfirm(col);
            }
        });

        m.AddSeparator("");

        if (!hasKid)
        {
            m.AddItem(new GUIContent("Nested Table/Add Child Table (1 row, 3 cols, stretch width)"), false, () =>
            {
                Undo.IncrementCurrentGroup();
                var child = table.CreateChildTableInCellLikeABaby(row, col);
                if (child != null) Undo.RegisterCreatedObjectUndo(child.gameObject, "Add Nested Table");
                table.FlagLayoutAsNeedingSpaDay();
                if (child != null)
                {
                    Selection.activeObject = child;
                    EditorGUIUtility.PingObject(child);
                }
            });
        }
        else
        {
            m.AddDisabledItem(new GUIContent("Nested Table/Add Child Table (1 row, 3 cols, stretch width)"));
            m.AddItem(new GUIContent("Nested Table/Select Child Table"), false, () =>
            {
                if (kidTable != null) kidTable.SelectAndPingLikeABeacon();
            });
            m.AddItem(new GUIContent("Nested Table/Delete Child Table"), false, () =>
            {
                if (kidTable == null) return;
                Undo.RecordObject(table, "Delete Nested Table");
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(kidTable.gameObject);
                else UnityEngine.Object.Destroy(kidTable.gameObject);
                table.FlagLayoutAsNeedingSpaDay();
            });
        }

        m.ShowAsContext();
    }

    public static void ShowForColumnHeader(Ux_TonkersTableTopiaLayout table, int colIndex)
    {
        var m = new GenericMenu();
        var colTarget = (UnityEngine.Object)(table.FindFirstAwakeCellInColumnLikeCoffee(colIndex) ?? table.FetchCellRectTransformVIP(0, colIndex) ?? (table as Component));
        System.Action hl = table != null ? new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, colIndex, table.totalRowsCountLetTheShowBegin, 1)) : null;

        AddCancelSelectAndHighlightLikeABoss(m, colTarget, hl);

        m.AddItem(new GUIContent("Column/Insert Left"), false, () => { Undo.RecordObject(table, "Insert Column"); table.InsertColumnLikeANinja(colIndex); });
        m.AddItem(new GUIContent("Column/Insert Right"), false, () => { Undo.RecordObject(table, "Insert Column"); table.InsertColumnLikeANinja(colIndex + 1); });
        m.AddItem(new GUIContent("Column/Delete"), false, () =>
        {
            Undo.RecordObject(table, "Delete Column(s)");
            int c0, c1;
            if (Ux_TonkersTableTopiaLayoutEditor.TryFetchSelectedColumnRange(out c0, out c1) && c1 > c0)
            {
                if (!table.BulkDeleteColumnsLikeAChamp(c0, c1))
                    EditorUtility.DisplayDialog("Column Removal", "Could not delete the selected columns due to merged cells or minimum column limit.", "OK");
            }
            else
            {
                table.SafeDeleteColumnAtWithWittyConfirm(colIndex);
            }
        });

        m.AddSeparator("Column/");
        m.AddItem(new GUIContent("Column/Distribute All Columns Evenly"), false, () =>
        {
            Undo.RecordObject(table, "Distribute Columns");
            float even = table.totalColumnsCountHighFive > 0 ? 1f / table.totalColumnsCountHighFive : 1f;
            table.SyncColumnWardrobes();
            for (int c = 0; c < table.totalColumnsCountHighFive; c++) table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -even;
            table.shareThePieEvenlyForColumns = false;
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });

        m.ShowAsContext();
    }

    public static void ShowForResizeHandle(Ux_TonkersTableTopiaLayout table, bool isColumnHandle, int splitIndex)
    {
        var m = new GenericMenu();
        UnityEngine.Object target = null;
        System.Action hl = null;

        if (isColumnHandle)
        {
            target = (UnityEngine.Object)(table.FindFirstAwakeCellInColumnLikeCoffee(splitIndex) ?? table.FetchCellRectTransformVIP(0, splitIndex) ?? (table as Component));
            int c0 = Mathf.Clamp(splitIndex, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 2));
            hl = new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, c0, table.totalRowsCountLetTheShowBegin, Mathf.Min(2, table.totalColumnsCountHighFive - c0)));
        }
        else
        {
            target = (UnityEngine.Object)(table.FetchRowRectTransformVIP(splitIndex) ?? (table as Component));
            int r0 = Mathf.Clamp(splitIndex, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 2));
            hl = new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, r0, 0, Mathf.Min(2, table.totalRowsCountLetTheShowBegin - r0), table.totalColumnsCountHighFive));
        }

        AddCancelSelectAndHighlightLikeABoss(m, target, hl);

        if (isColumnHandle)
        {
            m.AddItem(new GUIContent("Resize/Evenly Split Left/Right"), false, () =>
            {
                Undo.RecordObject(table, "Evenly Split Columns");
                table.ConvertAllSpecsToPercentages();
                table.SplitTwoColumnsEvenlyLikePeas(splitIndex);
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            });

            AddPercentSettersForColumns(table, m, splitIndex);

            m.AddSeparator("Resize/");
            m.AddItem(new GUIContent("Resize/Distribute All Columns Evenly"), false, () =>
            {
                Undo.RecordObject(table, "Distribute Columns");
                table.DistributeAllColumnsEvenlyLikeAShortOrderCook();
                EditorUtility.SetDirty(table);
            });
        }
        else
        {
            AddPercentSettersForRows(table, m, splitIndex);

            m.AddSeparator("Resize/");
            m.AddItem(new GUIContent("Resize/Distribute All Rows Evenly"), false, () =>
            {
                Undo.RecordObject(table, "Distribute Rows");
                table.DistributeAllRowsEvenlyLikeAShortOrderCook();
                EditorUtility.SetDirty(table);
            });
        }

        m.ShowAsContext();
    }

    public static void ShowForRowHeader(Ux_TonkersTableTopiaLayout table, int rowIndex)
    {
        var m = new GenericMenu();
        var rowRt = table.FetchRowRectTransformVIP(rowIndex) as UnityEngine.Object;
        System.Action hl = table != null ? new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, rowIndex, 0, 1, table.totalColumnsCountHighFive)) : null;

        AddCancelSelectAndHighlightLikeABoss(m, rowRt ?? table, hl);

        m.AddItem(new GUIContent("Row/Insert Above"), false, () => { Undo.RecordObject(table, "Insert Row"); table.InsertRowLikeANinja(rowIndex); });
        m.AddItem(new GUIContent("Row/Insert Below"), false, () => { Undo.RecordObject(table, "Insert Row"); table.InsertRowLikeANinja(rowIndex + 1); });
        m.AddItem(new GUIContent("Row/Delete"), false, () =>
        {
            Undo.RecordObject(table, "Delete Row(s)");
            int r0, r1;
            if (Ux_TonkersTableTopiaLayoutEditor.TryFetchSelectedRowRange(out r0, out r1) && r1 > r0)
            {
                if (!table.BulkDeleteRowsLikeABoss(r0, r1))
                    EditorUtility.DisplayDialog("Row Removal", "Could not delete the selected rows due to merged cells or minimum row limit.", "OK");
            }
            else
            {
                table.SafeDeleteRowAtWithWittyConfirm(rowIndex);
            }
        });

        m.AddSeparator("Row/");
        m.AddItem(new GUIContent("Row/Distribute All Rows Evenly"), false, () =>
        {
            Undo.RecordObject(table, "Distribute Rows");
            float even = table.totalRowsCountLetTheShowBegin > 0 ? 1f / table.totalRowsCountLetTheShowBegin : 1f;
            table.SyncRowWardrobes();
            for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++) table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -even;
            table.shareThePieEvenlyForRows = false;
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });

        m.ShowAsContext();
    }

    public static void ShowForTableHeader(Ux_TonkersTableTopiaLayout table)
    {
        var m = new GenericMenu();
        System.Action hl = table != null ? new System.Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, 0, table.totalRowsCountLetTheShowBegin, table.totalColumnsCountHighFive)) : null;
        AddCancelSelectAndHighlightLikeABoss(m, table, hl);

        m.AddItem(new GUIContent("Table/Distribute Columns Evenly"), false, () =>
        {
            Undo.RecordObject(table, "Distribute Columns");
            float even = table.totalColumnsCountHighFive > 0 ? 1f / table.totalColumnsCountHighFive : 1f;
            table.SyncColumnWardrobes();
            for (int c = 0; c < table.totalColumnsCountHighFive; c++) table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -even;
            table.shareThePieEvenlyForColumns = false;
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });

        m.AddItem(new GUIContent("Table/Distribute Rows Evenly"), false, () =>
        {
            Undo.RecordObject(table, "Distribute Rows");
            float even = table.totalRowsCountLetTheShowBegin > 0 ? 1f / table.totalRowsCountLetTheShowBegin : 1f;
            table.SyncRowWardrobes();
            for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++) table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -even;
            table.shareThePieEvenlyForRows = false;
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });

        m.AddSeparator("Table/");
        m.AddItem(new GUIContent("Table/Insert Row At Top"), false, () => { Undo.RecordObject(table, "Insert Row"); table.InsertRowLikeANinja(0); });
        m.AddItem(new GUIContent("Table/Insert Column At Left"), false, () => { Undo.RecordObject(table, "Insert Column"); table.InsertColumnLikeANinja(0); });

        m.ShowAsContext();
    }

    private static void AddCancelSelectAndHighlightLikeABoss(GenericMenu m, UnityEngine.Object selectTarget, System.Action highlightAction)
    {
        m.AddItem(new GUIContent("Cancel"), false, () => { });
        if (selectTarget != null) m.AddItem(new GUIContent("Select"), false, () => { selectTarget.SelectAndPingLikeABeacon(); });
        else m.AddDisabledItem(new GUIContent("Select"));

        if (highlightAction != null) m.AddItem(new GUIContent("Highlight"), false, () => { highlightAction(); });
        else m.AddDisabledItem(new GUIContent("Highlight"));

        m.AddSeparator("");
    }

    private static void AddPercentSettersForColumns(Ux_TonkersTableTopiaLayout table, GenericMenu m, int index)
    {
        float[] picks = { 0.1f, 0.25f, 1f / 3f, 0.5f, 0.75f };
        for (int i = 0; i < picks.Length; i++)
        {
            var p = picks[i];
            m.AddItem(new GUIContent($"Resize/Set Column {index + 1} To/{Mathf.RoundToInt(p * 100f)}%"), false, () =>
            {
                Undo.RecordObject(table, "Set Column %");
                table.SyncColumnWardrobes();

                float keep = Mathf.Clamp01(-table.fancyColumnWardrobes[index].requestedWidthMaybePercentIfNegative);
                float delta = p - keep;

                float sumOthers = 0f;
                for (int c = 0; c < table.totalColumnsCountHighFive; c++) if (c != index) sumOthers += Mathf.Clamp01(-table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative);
                float scale = sumOthers > 0.0001f ? Mathf.Clamp01((sumOthers - delta) / sumOthers) : 1f;

                table.fancyColumnWardrobes[index].requestedWidthMaybePercentIfNegative = -p;
                for (int c = 0; c < table.totalColumnsCountHighFive; c++)
                    if (c != index)
                        table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -Mathf.Clamp01(Mathf.Clamp01(-table.fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative) * scale);

                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            });
        }
    }

    private static void AddPercentSettersForRows(Ux_TonkersTableTopiaLayout table, GenericMenu m, int index)
    {
        float[] picks = { 0.1f, 0.25f, 1f / 3f, 0.5f, 0.75f };
        for (int i = 0; i < picks.Length; i++)
        {
            var p = picks[i];
            m.AddItem(new GUIContent($"Resize/Set Row {index + 1} To/{Mathf.RoundToInt(p * 100f)}%"), false, () =>
            {
                Undo.RecordObject(table, "Set Row %");
                table.SyncRowWardrobes();

                float keep = Mathf.Clamp01(-table.snazzyRowWardrobes[index].requestedHeightMaybePercentIfNegative);
                float delta = p - keep;

                float sumOthers = 0f;
                for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++) if (r != index) sumOthers += Mathf.Clamp01(-table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative);
                float scale = sumOthers > 0.0001f ? Mathf.Clamp01((sumOthers - delta) / sumOthers) : 1f;

                table.snazzyRowWardrobes[index].requestedHeightMaybePercentIfNegative = -p;
                for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
                    if (r != index)
                        table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -Mathf.Clamp01(Mathf.Clamp01(-table.snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative) * scale);

                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            });
        }
    }

    private class SnackPack
    {
        public Func<RectTransform, GameObject> baker;
        public string path;

        public SnackPack(string p, Func<RectTransform, GameObject> b)
        { path = p; baker = b; }
    }
}