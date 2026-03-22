using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Ux_TonkersTableTopiaContentExtensions;

public static class Ux_TonkersTableTopiaContextMenuGravyBoat
{
    private static readonly List<SnackPack> _snacks = new List<SnackPack>();

    static Ux_TonkersTableTopiaContextMenuGravyBoat()
    {
        RegisterSnack("Image", parent => parent.CreateImageCheeseburger());
        RegisterSnack("RawImage", parent => parent.CreateRawImageNachos());
        RegisterSnack("Text", parent => parent.CreateTextDadJokes());
        RegisterSnack("Button", parent => parent.CreateButtonBellyFlop());
        RegisterSnack("Empty Panel", parent => parent.SpawnUiChildFillingAndCenteredLikeABurrito("TTT Panel", go => go.AddComponent<Image>()));
        RegisterSnack("Toggle", parent => parent.CreateToggleFlipFlop());
        RegisterSnack("Slider", parent => parent.CreateSliderSlipNSlide());
        RegisterSnack("Scrollbar", parent => parent.CreateScrollbarScoot());
        RegisterSnack("Scroll View", parent => parent.CreateScrollRectScooter());
        RegisterSnack("InputField", parent => parent.CreateInputFieldChattyCathy());
        RegisterSnack("Dropdown", parent => parent.CreateDropdownDropItLikeItsHot());
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

    public static void ShowForCell(
     Ux_TonkersTableTopiaLayout table,
     int r,
     int c,
     int spanR,
     int spanC,
     bool canMerge,
     bool canUnmerge,
     int selR0,
     int selC0,
     int selRCount,
     int selCCount)
    {
        if (table == null) return;
        var cellRT = table.FetchCellRectTransformVIP(r, c);
        if (cellRT == null) return;

        var gm = new GenericMenu();
        var selectTarget = (UnityEngine.Object)(cellRT.GetComponent<Ux_TonkersTableTopiaCell>() ?? (UnityEngine.Object)cellRT);
        Action hl = new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, r, c, spanR, spanC));
        AddCancelSelectAndHighlightLikeABoss(gm, selectTarget, hl);

        gm.AddDisabledItem(new GUIContent($"Row {r + 1}, Col {c + 1}"));
        gm.AddDisabledItem(new GUIContent($"Span Rows {spanR}, Cols {spanC}"));

        if (canMerge) gm.AddItem(new GUIContent("Merge Selection"), false, () =>
        {
            Undo.RecordObject(table, "Merge Cells");
            table.MergeCellsLikeAGroupHug(selR0, selC0, selRCount, selCCount);
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });
        else gm.AddDisabledItem(new GUIContent("Merge Selection"));

        if (canUnmerge) gm.AddItem(new GUIContent("Unmerge Selection"), false, () =>
        {
            Undo.RecordObject(table, "Unmerge Selection");
            table.UnmergeEverythingInRectLikeItNeverHappened(selR0, selC0, selRCount, selCCount);
            table.FlagLayoutAsNeedingSpaDay();
            EditorUtility.SetDirty(table);
        });
        else gm.AddDisabledItem(new GUIContent("Unmerge Selection"));

        gm.AddSeparator("");

        gm.AddItem(new GUIContent("Row/Insert Above"), false, () =>
        {
            Undo.RecordObject(table, "Insert Row");
            table.InsertRowLikeANinja(selR0);
        });
        gm.AddItem(new GUIContent("Row/Insert Below"), false, () =>
        {
            Undo.RecordObject(table, "Insert Row");
            table.InsertRowLikeANinja(selR0 + selRCount);
        });
        gm.AddItem(new GUIContent("Row/Delete"), false, () =>
        {
            Undo.RecordObject(table, "Delete Row(s)");
            if (selRCount > 1)
            {
                if (!table.BulkDeleteRowsLikeABoss(selR0, selR0 + selRCount - 1))
                    EditorUtility.DisplayDialog("Row Removal", "Could not delete the selected rows due to merged cells or minimum row limit.", "OK");
            }
            else
            {
                table.SafeDeleteRowAtWithWittyConfirm(r);
            }
        });

        gm.AddSeparator("Row/");

        gm.AddItem(new GUIContent("Column/Insert Left"), false, () =>
        {
            Undo.RecordObject(table, "Insert Column");
            table.InsertColumnLikeANinja(selC0);
        });
        gm.AddItem(new GUIContent("Column/Insert Right"), false, () =>
        {
            Undo.RecordObject(table, "Insert Column");
            table.InsertColumnLikeANinja(selC0 + selCCount);
        });
        gm.AddItem(new GUIContent("Column/Delete"), false, () =>
        {
            Undo.RecordObject(table, "Delete Column(s)");
            if (selCCount > 1)
            {
                if (!table.BulkDeleteColumnsLikeAChamp(selC0, selC0 + selCCount - 1))
                    EditorUtility.DisplayDialog("Column Removal", "Could not delete the selected columns due to merged cells or minimum column limit.", "OK");
            }
            else
            {
                table.SafeDeleteColumnAtWithWittyConfirm(c);
            }
        });

        gm.AddSeparator("");

        for (int i = 0; i < _snacks.Count; i++)
        {
            var s = _snacks[i];
            gm.AddItem(new GUIContent("Add/" + s.path), false, () =>
            {
                var go = s.baker.Invoke(cellRT);
                if (go != null) Undo.RegisterCreatedObjectUndo(go, "Add UI");
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            });
        }

        gm.AddSeparator("");

        var foreignKids = new List<Transform>(16);
        cellRT.ListForeignKidsLikeRollCall(foreignKids);
        if (foreignKids.Count == 0)
        {
            gm.AddDisabledItem(new GUIContent("Contents/None"));
        }
        else
        {
            for (int i = 0; i < foreignKids.Count; i++)
            {
                var kid = foreignKids[i];
                if (kid == null) continue;
                var prettyType = TypeNameShortLikeNameTag(PickPrimaryUiTypeLikeMenuDecider(kid.gameObject));
                string label = $"{i + 1}. {kid.gameObject.name} ({prettyType})";
                gm.AddItem(new GUIContent($"Contents/{label}/Select"), false, () => { kid.SelectAndPingLikeABeacon(); });
                gm.AddItem(new GUIContent($"Contents/{label}/Delete"), false, () =>
                {
                    Undo.RecordObject(table, "Delete Cell Child");
                    if (!Application.isPlaying) Undo.DestroyObjectImmediate(kid.gameObject);
                    else UnityEngine.Object.Destroy(kid.gameObject);
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                });
            }
        }

        gm.AddSeparator("");

        Ux_TonkersTableTopiaLayout kidTable = null;
        bool hasKid = table.TryFindChildTableInCellLikeSherlock(r, c, out kidTable);
        if (!hasKid)
        {
            gm.AddItem(new GUIContent("Nested Table/Add Baby Table"), false, () =>
            {
                Undo.IncrementCurrentGroup();
                var child = table.CreateChildTableInCellLikeABaby(r, c);
                if (child != null)
                {
                    Undo.RegisterCreatedObjectUndo(child.gameObject, "Add Nested Table");
                    table.FlagLayoutAsNeedingSpaDay();
                    Selection.activeObject = child;
                    EditorGUIUtility.PingObject(child);
                }
            });
        }
        else
        {
            gm.AddItem(new GUIContent("Nested Table/Select Child Table"), false, () =>
            {
                Selection.activeObject = kidTable;
                EditorGUIUtility.PingObject(kidTable);
            });
            gm.AddItem(new GUIContent("Nested Table/Delete Child Table"), false, () =>
            {
                Undo.RecordObject(table, "Delete Nested Table");
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(kidTable.gameObject);
                else UnityEngine.Object.Destroy(kidTable.gameObject);
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            });
        }

        gm.AddSeparator("");
        AddAlignSubmenuForCell(gm, table, r, c);
        gm.ShowAsContext();
    }

    public static void ShowForColumnHeader(Ux_TonkersTableTopiaLayout table, int colIndex)
    {
        var m = new GenericMenu();
        var colTarget = (UnityEngine.Object)(table.FindFirstAwakeCellInColumnLikeCoffee(colIndex) ?? table.FetchCellRectTransformVIP(0, colIndex) ?? (table as Component));
        Action hl = table != null ? new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, colIndex, table.totalRowsCountLetTheShowBegin, 1)) : null;

        AddCancelSelectAndHighlightLikeABoss(m, colTarget, hl);

        m.AddItem(new GUIContent("Column/Insert Left"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Insert Column", () => table.InsertColumnLikeANinja(colIndex));
        });

        m.AddItem(new GUIContent("Column/Insert Right"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Insert Column", () => table.InsertColumnLikeANinja(colIndex + 1));
        });

        m.AddItem(new GUIContent("Column/Delete"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Delete Column(s)", () =>
            {
                int c0;
                int c1;

                if (Ux_TonkersTableTopiaLayoutEditor.TryFetchSelectedColumnRange(out c0, out c1) && c1 > c0)
                {
                    if (!table.BulkDeleteColumnsLikeAChamp(c0, c1))
                    {
                        EditorUtility.DisplayDialog("Column Removal", "Could not delete the selected columns due to merged cells or minimum column limit.", "OK");
                    }
                }
                else
                {
                    table.SafeDeleteColumnAtWithWittyConfirm(colIndex);
                }
            });
        });

        m.AddSeparator("Column/");

        m.AddItem(new GUIContent("Column/Distribute All Columns Evenly"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Distribute Columns", () => table.DistributeAllColumnsEvenlyLikeAShortOrderCook());
        });

        m.AddSeparator("");
        AddAlignSubmenuForColumn(m, table, colIndex);
        m.ShowAsContext();
    }

    public static void ShowForResizeHandle(Ux_TonkersTableTopiaLayout table, bool isColumnHandle, int splitIndex)
    {
        var m = new GenericMenu();
        UnityEngine.Object target = null;
        Action hl = null;

        if (isColumnHandle)
        {
            target = (UnityEngine.Object)(table.FindFirstAwakeCellInColumnLikeCoffee(splitIndex) ?? table.FetchCellRectTransformVIP(0, splitIndex) ?? (table as Component));
            int c0 = Mathf.Clamp(splitIndex, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 2));
            hl = new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, c0, table.totalRowsCountLetTheShowBegin, Mathf.Min(2, table.totalColumnsCountHighFive - c0)));
        }
        else
        {
            target = (UnityEngine.Object)(table.FetchRowRectTransformVIP(splitIndex) ?? (table as Component));
            int r0 = Mathf.Clamp(splitIndex, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 2));
            hl = new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, r0, 0, Mathf.Min(2, table.totalRowsCountLetTheShowBegin - r0), table.totalColumnsCountHighFive));
        }

        AddCancelSelectAndHighlightLikeABoss(m, target, hl);

        if (isColumnHandle)
        {
            m.AddItem(new GUIContent("Resize/Evenly Split Left/Right"), false, () =>
            {
                table.PerformEditorTableActionLikeABoss("Evenly Split Columns", () =>
                {
                    table.ConvertAllSpecsToPercentages();
                    table.SplitTwoColumnsEvenlyLikePeas(splitIndex);
                });
            });

            AddPercentSettersForColumns(table, m, splitIndex);
            m.AddSeparator("Resize/");

            m.AddItem(new GUIContent("Resize/Distribute All Columns Evenly"), false, () =>
            {
                table.PerformEditorTableActionLikeABoss("Distribute Columns", () => table.DistributeAllColumnsEvenlyLikeAShortOrderCook());
            });
        }
        else
        {
            AddPercentSettersForRows(table, m, splitIndex);
            m.AddSeparator("Resize/");

            m.AddItem(new GUIContent("Resize/Distribute All Rows Evenly"), false, () =>
            {
                table.PerformEditorTableActionLikeABoss("Distribute Rows", () => table.DistributeAllRowsEvenlyLikeAShortOrderCook());
            });
        }

        m.ShowAsContext();
    }

    public static void ShowForRowHeader(Ux_TonkersTableTopiaLayout table, int rowIndex)
    {
        var m = new GenericMenu();
        var rowRt = table.FetchRowRectTransformVIP(rowIndex) as UnityEngine.Object;
        Action hl = table != null ? new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, rowIndex, 0, 1, table.totalColumnsCountHighFive)) : null;

        AddCancelSelectAndHighlightLikeABoss(m, rowRt ?? table, hl);

        m.AddItem(new GUIContent("Row/Insert Above"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Insert Row", () => table.InsertRowLikeANinja(rowIndex));
        });

        m.AddItem(new GUIContent("Row/Insert Below"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Insert Row", () => table.InsertRowLikeANinja(rowIndex + 1));
        });

        m.AddItem(new GUIContent("Row/Delete"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Delete Row(s)", () =>
            {
                int r0;
                int r1;

                if (Ux_TonkersTableTopiaLayoutEditor.TryFetchSelectedRowRange(out r0, out r1) && r1 > r0)
                {
                    if (!table.BulkDeleteRowsLikeABoss(r0, r1))
                    {
                        EditorUtility.DisplayDialog("Row Removal", "Could not delete the selected rows due to merged cells or minimum row limit.", "OK");
                    }
                }
                else
                {
                    table.SafeDeleteRowAtWithWittyConfirm(rowIndex);
                }
            });
        });

        m.AddSeparator("Row/");

        m.AddItem(new GUIContent("Row/Distribute All Rows Evenly"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Distribute Rows", () => table.DistributeAllRowsEvenlyLikeAShortOrderCook());
        });

        m.AddSeparator("");
        AddAlignSubmenuForRow(m, table, rowIndex);
        m.ShowAsContext();
    }

    public static void ShowForTableHeader(Ux_TonkersTableTopiaLayout table)
    {
        var m = new GenericMenu();
        Action hl = table != null ? new Action(() => Ux_TonkersTableTopiaLayoutEditor.SetHighlightLikeAGlowStick(table, 0, 0, table.totalRowsCountLetTheShowBegin, table.totalColumnsCountHighFive)) : null;

        AddCancelSelectAndHighlightLikeABoss(m, table, hl);

        m.AddItem(new GUIContent("Table/Distribute Columns Evenly"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Distribute Columns", () => table.DistributeAllColumnsEvenlyLikeAShortOrderCook());
        });

        m.AddItem(new GUIContent("Table/Distribute Rows Evenly"), false, () =>
        {
            table.PerformEditorTableActionLikeABoss("Distribute Rows", () => table.DistributeAllRowsEvenlyLikeAShortOrderCook());
        });

        m.AddSeparator("");
        AddAlignSubmenuForTable(m, table);
        m.ShowAsContext();
    }

    private static void AddCancelSelectAndHighlightLikeABoss(GenericMenu m, UnityEngine.Object selectTarget, Action highlightAction)
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
                table.PerformEditorTableActionLikeABoss("Set Column %", () =>
                {
                    table.SetColumnPercentageAndRebalanceOthersLikeASpreadsheet(index, p);
                });
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
                table.PerformEditorTableActionLikeABoss("Set Row %", () =>
                {
                    table.SetRowPercentageAndRebalanceOthersLikeASpreadsheet(index, p);
                });
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

    private static void AddAlignSubmenuForCell(GenericMenu gm, Ux_TonkersTableTopiaLayout table, int r, int c)
    {
        bool isFull = false;
        Ux_TonkersTableTopiaLayout.HorizontalAlignment hCur = Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;
        Ux_TonkersTableTopiaLayout.VerticalAlignment vCur = Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;

        bool has = table != null && table.TryDetectCellAlignmentLikeLieDetector(r, c, out isFull, out hCur, out vCur);
        bool onLeft = has && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left;
        bool onCenter = has && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center;
        bool onRight = has && !isFull && hCur == Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right;
        bool onTop = has && !isFull && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Top;
        bool onMiddle = has && !isFull && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle;
        bool onBottom = has && !isFull && vCur == Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom;
        bool onFull = has && isFull;

        gm.AddItem(new GUIContent("Align/Left"), onLeft, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Left", () =>
            {
                table.AlignCellHorizontalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
            });
        });

        gm.AddItem(new GUIContent("Align/Center"), onCenter, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Center", () =>
            {
                table.AlignCellHorizontalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
            });
        });

        gm.AddItem(new GUIContent("Align/Right"), onRight, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Right", () =>
            {
                table.AlignCellHorizontalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
            });
        });

        gm.AddSeparator("Align/");

        gm.AddItem(new GUIContent("Align/Top"), onTop, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Top", () =>
            {
                table.AlignCellVerticalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
            });
        });

        gm.AddItem(new GUIContent("Align/Middle"), onMiddle, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Middle", () =>
            {
                table.AlignCellVerticalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
            });
        });

        gm.AddItem(new GUIContent("Align/Bottom"), onBottom, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Bottom", () =>
            {
                table.AlignCellVerticalOnlyLikeLaserLevel(r, c, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
            });
        });

        gm.AddItem(new GUIContent("Align/Full"), onFull, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Full", () =>
            {
                table.AlignCellForeignsToFillLikeStuffedBurrito(r, c);
            });
        });
    }

    private static void AddAlignSubmenuForRow(GenericMenu gm, Ux_TonkersTableTopiaLayout table, int rowIndex)
    {
        bool onLeft = table.IsRowHorizAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
        bool onCenter = table.IsRowHorizAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
        bool onRight = table.IsRowHorizAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
        bool onTop = table.IsRowVertAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
        bool onMiddle = table.IsRowVertAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
        bool onBottom = table.IsRowVertAlignedLikeMirror(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
        bool onFull = table.IsRowFullLikeWaterbed(rowIndex);

        gm.AddItem(new GUIContent("Align/Left"), onLeft, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Left", () =>
            {
                table.AlignRowHorizontalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
            });
        });

        gm.AddItem(new GUIContent("Align/Center"), onCenter, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Center", () =>
            {
                table.AlignRowHorizontalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
            });
        });

        gm.AddItem(new GUIContent("Align/Right"), onRight, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Right", () =>
            {
                table.AlignRowHorizontalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
            });
        });

        gm.AddSeparator("Align/");

        gm.AddItem(new GUIContent("Align/Top"), onTop, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Top", () =>
            {
                table.AlignRowVerticalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
            });
        });

        gm.AddItem(new GUIContent("Align/Middle"), onMiddle, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Middle", () =>
            {
                table.AlignRowVerticalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
            });
        });

        gm.AddItem(new GUIContent("Align/Bottom"), onBottom, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Bottom", () =>
            {
                table.AlignRowVerticalOnlyLikeLaserLevel(rowIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
            });
        });

        gm.AddItem(new GUIContent("Align/Full"), onFull, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Row Full", () =>
            {
                table.AlignRowToFillLikeWaterbed(rowIndex);
            });
        });
    }

    private static void AddAlignSubmenuForColumn(GenericMenu gm, Ux_TonkersTableTopiaLayout table, int colIndex)
    {
        bool onLeft = table.IsColumnHorizAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
        bool onCenter = table.IsColumnHorizAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
        bool onRight = table.IsColumnHorizAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
        bool onTop = table.IsColumnVertAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
        bool onMiddle = table.IsColumnVertAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
        bool onBottom = table.IsColumnVertAlignedLikeMirror(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
        bool onFull = table.IsColumnFullLikeWaterfall(colIndex);

        gm.AddItem(new GUIContent("Align/Left"), onLeft, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Left", () =>
            {
                table.AlignColumnHorizontalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
            });
        });

        gm.AddItem(new GUIContent("Align/Center"), onCenter, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Center", () =>
            {
                table.AlignColumnHorizontalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
            });
        });

        gm.AddItem(new GUIContent("Align/Right"), onRight, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Right", () =>
            {
                table.AlignColumnHorizontalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
            });
        });

        gm.AddSeparator("Align/");

        gm.AddItem(new GUIContent("Align/Top"), onTop, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Top", () =>
            {
                table.AlignColumnVerticalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
            });
        });

        gm.AddItem(new GUIContent("Align/Middle"), onMiddle, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Middle", () =>
            {
                table.AlignColumnVerticalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
            });
        });

        gm.AddItem(new GUIContent("Align/Bottom"), onBottom, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Bottom", () =>
            {
                table.AlignColumnVerticalOnlyLikeLaserLevel(colIndex, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
            });
        });

        gm.AddItem(new GUIContent("Align/Full"), onFull, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Column Full", () =>
            {
                table.AlignColumnToFillLikeWaterfall(colIndex);
            });
        });
    }

    private static void AddAlignSubmenuForTable(GenericMenu gm, Ux_TonkersTableTopiaLayout table)
    {
        bool onLeft = table.IsTableHorizAlignedLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
        bool onCenter = table.IsTableHorizAlignedLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
        bool onRight = table.IsTableHorizAlignedLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
        bool onTop = table.IsTableVertAlignedLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
        bool onMiddle = table.IsTableVertAlignedLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
        bool onBottom = table.IsTableVertAlignedLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
        bool onFull = table.IsTableFullLikeBalloon();

        gm.AddItem(new GUIContent("Align/Left"), onLeft, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Left", () =>
            {
                table.AlignTableHorizontalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
            });
        });

        gm.AddItem(new GUIContent("Align/Center"), onCenter, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Center", () =>
            {
                table.AlignTableHorizontalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
            });
        });

        gm.AddItem(new GUIContent("Align/Right"), onRight, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Right", () =>
            {
                table.AlignTableHorizontalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
            });
        });

        gm.AddSeparator("Align/");

        gm.AddItem(new GUIContent("Align/Top"), onTop, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Top", () =>
            {
                table.AlignTableVerticalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
            });
        });

        gm.AddItem(new GUIContent("Align/Middle"), onMiddle, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Middle", () =>
            {
                table.AlignTableVerticalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
            });
        });

        gm.AddItem(new GUIContent("Align/Bottom"), onBottom, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Bottom", () =>
            {
                table.AlignTableVerticalOnlyLikeChoir(Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
            });
        });

        gm.AddItem(new GUIContent("Align/Full"), onFull, () =>
        {
            table.PerformEditorTableActionLikeABoss("Align Table Full", () =>
            {
                table.AlignTableToFillLikeBalloon();
            });
        });
    }
}