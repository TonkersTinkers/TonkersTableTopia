using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CustomEditor(typeof(Ux_TonkersTableTopiaLayout))]
public class Ux_TonkersTableTopiaLayoutEditor : Editor
{
    private static readonly Color gridLine = new Color(0f, 0f, 0f, 0.35f);
    private static readonly string PREF_ActionMode = "TTT_ActionMode";
    private static readonly string PREF_WYSI_HEADER = "Ux_WYSI_Header";
    private static readonly string PREF_WYSI_ROW_A = "Ux_WYSI_RowA";
    private static readonly string PREF_WYSI_ROW_B = "Ux_WYSI_RowB";
    private static readonly Color selFill = new Color(0.2f, 0.6f, 1f, 0.18f);
    private static readonly Color selOutline = new Color(0.2f, 0.6f, 1f, 0.85f);

    private static readonly GUIContent[] _toolbarLabels =
    {
        new GUIContent("Highlight"),
        new GUIContent("Resize"),
        new GUIContent("Select"),
        new GUIContent("Move")
    };

    private static int _lastInspectedTableId = 0;
    private static EditorActionMode actionMode = EditorActionMode.HighlightCells;
    private static int dragCol = -1, dragRow = -1;
    private static float dragStartMouse;
    private static bool moveDragActive; private static int moveSrcRow = -1, moveSrcCol = -1;
    private static int headerRowBigEnchilada = -1, headerColBigEnchilada = -1;
    private static bool sceneHighlight = true;
    private static int selRow = -1, selCol = -1, selRow2 = -1, selCol2 = -1;
    private static bool showDefaultInspector = false;
    private static bool showPreview = true;

    private float[] startColW, startRowH;

    private Rect _lastGridRect;
    private float _lastInnerHeight;
    private float _lastInnerWidth;

    private Ux_TonkersTableTopiaLayout table;

    private RectTransform _cachedTableRT;

    private readonly List<Ux_TonkersTableTopiaCell> _selectedCellsScratch = new List<Ux_TonkersTableTopiaCell>(64);

    private readonly HashSet<System.Type> _snackTypesScratch = new HashSet<System.Type>(16);
    private readonly List<GUIContent> _badgesScratch = new List<GUIContent>(8);
    private Object[] _objScratch = System.Array.Empty<Object>();
    private float[] _previewColBuf = System.Array.Empty<float>();
    private float[] _previewRowBuf = System.Array.Empty<float>();
    private float[] _colPctBuf = System.Array.Empty<float>();
    private float[] _rowPctBuf = System.Array.Empty<float>();
    private float[] _basePctBuf = System.Array.Empty<float>();
    private readonly Vector3[] _sceneCorners = new Vector3[4];

    private static readonly Dictionary<System.Type, GUIContent> _badgeIconCache = new Dictionary<System.Type, GUIContent>(16);

    private static readonly System.Type[] _badgeOrder =
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
        typeof(InputField)
    };

    private static GUIStyle _miniBoldCenter;

    private static GUIStyle MiniBoldCenter
    {
        get
        {
            if (_miniBoldCenter == null)
            {
                _miniBoldCenter = new GUIStyle(EditorStyles.miniBoldLabel);
                _miniBoldCenter.alignment = TextAnchor.MiddleCenter;
                _miniBoldCenter.clipping = TextClipping.Clip;
                _miniBoldCenter.wordWrap = false;
            }
            return _miniBoldCenter;
        }
    }

    private enum EditorActionMode
    { HighlightCells = 0, Resize = 1, SelectObjects = 2, MoveCells = 3, }

    public static void SetHighlightLikeAGlowStick(Ux_TonkersTableTopiaLayout table, int r0, int c0, int rCount, int cCount)
    {
        if (table == null) return;
        r0 = Mathf.Clamp(r0, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        c0 = Mathf.Clamp(c0, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        int r1 = Mathf.Clamp(r0 + Mathf.Max(1, rCount) - 1, r0, table.totalRowsCountLetTheShowBegin - 1);
        int c1 = Mathf.Clamp(c0 + Mathf.Max(1, cCount) - 1, c0, table.totalColumnsCountHighFive - 1);
        selRow = r0; selCol = c0; selRow2 = r1; selCol2 = c1;
        headerRowBigEnchilada = -1; headerColBigEnchilada = -1;
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
    }

    public static bool TryFetchSelectedColumnRange(out int c0, out int c1)
    {
        c0 = Mathf.Min(selCol, selCol2);
        c1 = Mathf.Max(selCol, selCol2);
        return selCol >= 0 && selCol2 >= 0 && c1 >= c0;
    }

    public static bool TryFetchSelectedRowRange(out int r0, out int r1)
    {
        r0 = Mathf.Min(selRow, selRow2);
        r1 = Mathf.Max(selRow, selRow2);
        return selRow >= 0 && selRow2 >= 0 && r1 >= r0;
    }

    public override void OnInspectorGUI()
    {
        if (table == null) return;
        EnsureDefaultSelection();
        serializedObject.Update();

        using (new EditorGUILayout.HorizontalScope())
        {
            var parent = table.FindParentTableLikeFamilyTree();
            GUI.enabled = parent != null;
            if (GUILayout.Button(parent != null ? "Up to Parent Tonkers Table" : "Top-level Tonkers Table", GUILayout.Height(26)))
                Selection.activeObject = parent;
            GUI.enabled = true;
        }

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                showPreview = EditorGUILayout.ToggleLeft("WYSIWYG Preview", showPreview, GUILayout.Width(140));
                sceneHighlight = EditorGUILayout.ToggleLeft("Scene Highlight", sceneHighlight, GUILayout.Width(130));
            }

            int mode = GUILayout.Toolbar((int)actionMode, _toolbarLabels, EditorStyles.miniButton);
            if (mode != (int)actionMode)
            {
                actionMode = (EditorActionMode)mode;
                EditorPrefs.SetInt(PREF_ActionMode, (int)actionMode);
                Repaint();
            }

            DrawActionModeHelpdeskWithDadJokes();

            float hZoom = EditorPrefs.GetFloat("Ux_HZoom", 1f);
            float vZoom = EditorPrefs.GetFloat("Ux_VZoom", 1f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("VZoom", GUILayout.Width(48));
                vZoom = EditorGUILayout.Slider(vZoom, 0.25f, 3f);
                GUILayout.Space(10);
                GUILayout.Label("HZoom", GUILayout.Width(48));
                hZoom = EditorGUILayout.Slider(hZoom, 0.25f, 3f);
            }

            EditorPrefs.SetFloat("Ux_HZoom", Mathf.Clamp(hZoom, 0.25f, 3f));
            EditorPrefs.SetFloat("Ux_VZoom", Mathf.Clamp(vZoom, 0.25f, 3f));
        }

        DrawTableButtonsToolbarLikeASpreadsheet();

        if (showPreview) DrawWysiwyg();
        if (showPreview) DrawSelectionSectionLikeDessert();

        EditorGUILayout.Space(6);
        DrawHeaderInspectorIfTheBossIsWatching();
        DrawCellInspectorForHighlightedSnacks();
        EditorGUILayout.Space(6);
        DrawTableSizeControls();
        EditorGUILayout.Space(6);
        DrawAlternatingColorsUI();
        DrawWysiwygPreviewColorPrefs();
        EditorGUILayout.Space(4);

        showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Advanced (Default Inspector)");
        if (showDefaultInspector) DrawDefaultInspector();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(table);
            table.FlagLayoutAsNeedingSpaDay();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void LoadPreviewColors(out Color rowA, out Color rowB, out Color header)
    {
        ThemeDefaults(out var defA, out var defB, out var defH);
        rowA = ReadPrefColor(PREF_WYSI_ROW_A, defA);
        rowB = ReadPrefColor(PREF_WYSI_ROW_B, defB);
        header = ReadPrefColor(PREF_WYSI_HEADER, defH);
    }

    private static Color ReadPrefColor(string key, Color fallback)
    {
        var hex = EditorPrefs.GetString(key, string.Empty);
        if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString(hex, out var c)) return c;
        return fallback;
    }

    private static void ThemeDefaults(out Color rowA, out Color rowB, out Color header)
    {
        bool pro = EditorGUIUtility.isProSkin;
        var baseBg = pro ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.86f, 0.86f, 0.86f, 1f);
        rowA = baseBg;
        rowB = Color.Lerp(baseBg, Color.black, pro ? 0.08f : 0.06f);
        header = Color.Lerp(baseBg, Color.black, pro ? 0.22f : 0.12f);
    }

    private static void WritePrefColor(string key, Color c)
    {
        EditorPrefs.SetString(key, "#" + ColorUtility.ToHtmlStringRGBA(c));
    }

    private void ApplyColResize(int splitIndex, float deltaContent)
    {
        EnsureColumnStylesSize();

        int left = splitIndex;
        int right = splitIndex + 1;
        if (left < 0 || right >= table.totalColumnsCountHighFive) return;
        if (startColW == null || startColW.Length != table.totalColumnsCountHighFive) return;

        float spacing = table.sociallyDistancedColumnsPixels * Mathf.Max(0, table.totalColumnsCountHighFive - 1);
        float availW = Mathf.Max(1f, _lastInnerWidth - spacing);
        float deltaPct = deltaContent / availW;

        const float floor = 0.01f;

        EnsureFloatBuffer(ref _basePctBuf, startColW.Length);
        System.Array.Copy(startColW, _basePctBuf, startColW.Length);

        float l = Mathf.Clamp01(_basePctBuf[left]);
        float r = Mathf.Clamp01(_basePctBuf[right]);

        Ux_TonkersTableTopiaExtensions.RebalanceTwoPercentsLikeSeesaw(ref l, ref r, deltaPct, floor);

        _basePctBuf[left] = l;
        _basePctBuf[right] = r;

        for (int i = 0; i < table.totalColumnsCountHighFive; i++)
            table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = -Mathf.Clamp01(_basePctBuf[i]);

        EditorUtility.SetDirty(table);
    }

    private void ApplyRowResize(int splitIndex, float deltaContent)
    {
        EnsureRowStylesSize();
        int top = splitIndex;
        int bottom = splitIndex + 1;
        float availH = Mathf.Max(1f, _lastInnerHeight - table.sociallyDistancedRowsPixels * Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        float deltaPct = deltaContent / availH;
        EnsureFloatBuffer(ref _basePctBuf, startRowH != null ? startRowH.Length : 0);
        if (startRowH != null) System.Array.Copy(startRowH, _basePctBuf, startRowH.Length);

        const float floor = 0.01f;
        _basePctBuf[top] = Mathf.Clamp(startRowH[top] + deltaPct, floor, 1f - floor);
        _basePctBuf[bottom] = Mathf.Clamp(startRowH[bottom] - deltaPct, floor, 1f - floor);

        float sum = 0f;
        for (int i = 0; i < _basePctBuf.Length; i++) sum += _basePctBuf[i];
        if (sum <= 0f)
        {
            float even = table.totalRowsCountLetTheShowBegin > 0 ? 1f / table.totalRowsCountLetTheShowBegin : 1f;
            for (int i = 0; i < _basePctBuf.Length; i++) _basePctBuf[i] = even;
        }
        else
        {
            for (int i = 0; i < _basePctBuf.Length; i++) _basePctBuf[i] /= sum;
        }

        for (int i = 0; i < table.totalRowsCountLetTheShowBegin; i++)
            table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = -Mathf.Clamp01(_basePctBuf[i]);

        EditorUtility.SetDirty(table);
    }

    private void ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment h)
    {
        if (table == null) return;
        if (!HasSelection()) return;

        Undo.RecordObject(table, "Align Selection Horizontal");
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);

        for (int r = r0; r <= r1; r++)
            for (int c = c0; c <= c1; c++)
                table.AlignCellHorizontalOnlyLikeLaserLevel(r, c, h);

        EditorUtility.SetDirty(table);
    }

    private void ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment v)
    {
        if (table == null) return;
        if (!HasSelection()) return;

        Undo.RecordObject(table, "Align Selection Vertical");
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);

        for (int r = r0; r <= r1; r++)
            for (int c = c0; c <= c1; c++)
                table.AlignCellVerticalOnlyLikeLaserLevel(r, c, v);

        EditorUtility.SetDirty(table);
    }

    private void ClampSelection()
    {
        selRow = Mathf.Clamp(selRow, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        selRow2 = Mathf.Clamp(selRow2, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        selCol = Mathf.Clamp(selCol, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        selCol2 = Mathf.Clamp(selCol2, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
    }

    private void FillCurrentColumnPercentages()
    {
        int n = Mathf.Max(1, table.totalColumnsCountHighFive);
        var rt = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());
        float innerW = Mathf.Max(0f, rt.rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);
        EnsureFloatBuffer(ref _previewColBuf, n);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            n,
            i => (i < table.fancyColumnWardrobes.Count) ? table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f,
            table.sociallyDistancedColumnsPixels,
            innerW,
            ref _previewColBuf);

        float spacing = table.sociallyDistancedColumnsPixels * Mathf.Max(0, n - 1);
        float avail = Mathf.Max(1f, innerW - spacing);
        EnsureFloatBuffer(ref _colPctBuf, n);
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            float v = Mathf.Clamp01(_previewColBuf[i] / avail);
            _colPctBuf[i] = v;
            sum += v;
        }
        if (sum > 0f)
        {
            for (int i = 0; i < n; i++) _colPctBuf[i] /= sum;
        }
    }

    private void FillCurrentRowPercentages()
    {
        int n = Mathf.Max(1, table.totalRowsCountLetTheShowBegin);
        var rt = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());
        float innerH = Mathf.Max(0f, rt.rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);
        EnsureFloatBuffer(ref _previewRowBuf, n);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            n,
            i => (i < table.snazzyRowWardrobes.Count) ? table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f,
            table.sociallyDistancedRowsPixels,
            innerH,
            ref _previewRowBuf);

        float spacing = table.sociallyDistancedRowsPixels * Mathf.Max(0, n - 1);
        float avail = Mathf.Max(1f, innerH - spacing);
        EnsureFloatBuffer(ref _rowPctBuf, n);
        float sum = 0f;
        for (int i = 0; i < n; i++)
        {
            float v = Mathf.Clamp01(_previewRowBuf[i] / avail);
            _rowPctBuf[i] = v;
            sum += v;
        }
        if (sum > 0f)
        {
            for (int i = 0; i < n; i++) _rowPctBuf[i] /= sum;
        }
    }

    private static void EnsureFloatBuffer(ref float[] buf, int len)
    {
        if (len <= 0)
        {
            buf = System.Array.Empty<float>();
            return;
        }
        if (buf == null || buf.Length != len) buf = new float[len];
    }

    private void DeleteCol()
    {
        if (!HasSelection()) return;
        int c0 = Mathf.Min(selCol, selCol2);
        int c1 = Mathf.Max(selCol, selCol2);
        Undo.RecordObject(table, c1 > c0 ? "Delete Columns" : "Delete Column");
        Selection.activeObject = table;
        if (c1 > c0)
        {
            if (!table.BulkDeleteColumnsLikeAChamp(c0, c1))
                EditorUtility.DisplayDialog("Column Removal", "Could not delete the selected columns due to merged cells or minimum column limit.", "OK");
        }
        else
        {
            table.SafeDeleteColumnAtWithWittyConfirm(c0);
        }
        ClampSelection();
    }

    private void DeleteRow()
    {
        if (!HasSelection()) return;
        int r0 = Mathf.Min(selRow, selRow2);
        int r1 = Mathf.Max(selRow, selRow2);
        Undo.RecordObject(table, r1 > r0 ? "Delete Rows" : "Delete Row");
        Selection.activeObject = table;
        if (r1 > r0)
        {
            if (!table.BulkDeleteRowsLikeABoss(r0, r1))
                EditorUtility.DisplayDialog("Row Removal", "Could not delete the selected rows due to merged cells or minimum row limit.", "OK");
        }
        else
        {
            table.SafeDeleteRowAtWithWittyConfirm(r0);
        }
        ClampSelection();
    }

    private void DistributeColumnsNow()
    {
        Undo.RecordObject(table, "Distribute Columns");
        table.DistributeAllColumnsEvenlyLikeAShortOrderCook();
        EditorUtility.SetDirty(table);
    }

    private void DistributeRowsNow()
    {
        Undo.RecordObject(table, "Distribute Rows");
        table.DistributeAllRowsEvenlyLikeAShortOrderCook();
        EditorUtility.SetDirty(table);
    }

    private void DrawAlternatingColorsUI()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUI.BeginChangeCheck();
            table.toggleZebraStripesForRows = EditorGUILayout.Toggle("Alternating Rows", table.toggleZebraStripesForRows);
            using (new EditorGUI.IndentLevelScope())
            {
                table.zebraRowColorA = EditorGUILayout.ColorField("Row A", table.zebraRowColorA);
                table.zebraRowColorB = EditorGUILayout.ColorField("Row B", table.zebraRowColorB);
            }
            table.toggleZebraStripesForColumns = EditorGUILayout.Toggle("Alternating Columns", table.toggleZebraStripesForColumns);
            using (new EditorGUI.IndentLevelScope())
            {
                table.zebraColumnColorA = EditorGUILayout.ColorField("Column A", table.zebraColumnColorA);
                table.zebraColumnColorB = EditorGUILayout.ColorField("Column B", table.zebraColumnColorB);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(table, "Edit Alternating Colors");
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
    }

    private void DrawCellInspectorForHighlightedSnacks()
    {
        if (!HasSelection()) return;
        if (headerColBigEnchilada >= 0 || headerRowBigEnchilada >= 0) return;

        var cells = GetSelectedCellsHotAndReady();
        if (cells.Count == 0) return;

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            if (cells.Count == 1)
            {
                var cell = cells[0];
                EditorGUILayout.LabelField($"Cell R{cell.rowNumberWhereThePartyIs + 1}, C{cell.columnNumberPrimeRib + 1}", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                int newRowSpan = Mathf.Max(1, EditorGUILayout.IntField("Row Span", cell.howManyRowsAreHoggingThisSeat));
                int newColSpan = Mathf.Max(1, EditorGUILayout.IntField("Col Span", cell.howManyColumnsAreSneakingIn));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cell, "Edit Cell Span");
                    cell.howManyRowsAreHoggingThisSeat = newRowSpan;
                    cell.howManyColumnsAreSneakingIn = newColSpan;

                    var t = table;
                    if (t)
                    {
                        Undo.RecordObject(t, "Edit Cell Span");
                        t.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(t);
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    var t = table;
                    bool canUnmerge = t && (cell.howManyRowsAreHoggingThisSeat > 1 || cell.howManyColumnsAreSneakingIn > 1);
                    GUI.enabled = canUnmerge;
                    if (GUILayout.Button("Unmerge", GUILayout.Height(20)))
                    {
                        Undo.RecordObject(t, "Unmerge Cell");
                        t.UnmergeCellEveryoneGetsTheirOwnChair(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib);
                        t.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(t);
                    }
                    GUI.enabled = true;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    var t = table;
                    Ux_TonkersTableTopiaLayout kidTable = null;
                    bool hasKid = t != null && t.TryFindChildTableInCellLikeSherlock(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib, out kidTable);

                    GUI.enabled = t != null && !hasKid;
                    if (GUILayout.Button("Add Nested Table Baby", GUILayout.Height(20)))
                    {
                        Undo.IncrementCurrentGroup();
                        var child = t.CreateChildTableInCellLikeABaby(cell.rowNumberWhereThePartyIs, cell.columnNumberPrimeRib);
                        if (child != null)
                        {
                            Undo.RegisterCreatedObjectUndo(child.gameObject, "Add Nested Table");
                            t.FlagLayoutAsNeedingSpaDay();
                            Selection.activeObject = child;
                            EditorGUIUtility.PingObject(child);
                        }
                    }

                    GUI.enabled = hasKid && kidTable != null;
                    if (GUILayout.Button("Select Child Table", GUILayout.Height(20)))
                    {
                        Selection.activeObject = kidTable;
                        EditorGUIUtility.PingObject(kidTable);
                    }

                    if (GUILayout.Button("Delete Child Table", GUILayout.Height(20)))
                    {
                        Undo.RecordObject(t, "Delete Nested Table");
                        if (!Application.isPlaying) Undo.DestroyObjectImmediate(kidTable.gameObject);
                        else Destroy(kidTable.gameObject);
                        t.FlagLayoutAsNeedingSpaDay();
                    }
                    GUI.enabled = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Cells selected: {cells.Count}", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Combined Inspector", EditorStyles.miniBoldLabel);

                int r0 = Mathf.Min(selRow, selRow2);
                int c0 = Mathf.Min(selCol, selCol2);
                int rCount = Mathf.Abs(selRow2 - selRow) + 1;
                int cCount = Mathf.Abs(selCol2 - selCol) + 1;

                bool canMerge = table != null && table.CanMergeThisPicnicBlanket(r0, c0, rCount, cCount);
                bool canUnmerge = table != null && table.CanUnmergeThisFoodFight(r0, c0, rCount, cCount);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = canMerge;
                    if (GUILayout.Button("Merge", GUILayout.Height(20)))
                    {
                        Undo.RecordObject(table, "Merge Cells");
                        MergeSelection();
                        table.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(table);
                    }

                    GUI.enabled = canUnmerge;
                    if (GUILayout.Button("Unmerge", GUILayout.Height(20)))
                    {
                        Undo.RecordObject(table, "Unmerge Selection");
                        UnmergeTopLeft();
                        table.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(table);
                    }
                    GUI.enabled = true;
                }
            }

            bool imgToggle = EditorPrefs.GetBool("TTT_CellImg_Wysi", false);
            bool newImgToggle = EditorGUILayout.ToggleLeft("Image Settings", imgToggle);
            if (newImgToggle != imgToggle) EditorPrefs.SetBool("TTT_CellImg_Wysi", newImgToggle);

            if (newImgToggle)
            {
                Sprite firstSprite = cells[0].backgroundPictureBecausePlainIsLame;
                bool mixedSprite = false;
                for (int i = 1; i < cells.Count; i++)
                {
                    if (cells[i].backgroundPictureBecausePlainIsLame != firstSprite) { mixedSprite = true; break; }
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = mixedSprite;
                var newSprite = (Sprite)EditorGUILayout.ObjectField("Cell Background", firstSprite, typeof(Sprite), false);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    EnsureObjectBuffer(cells.Count);
                    for (int i = 0; i < cells.Count; i++) _objScratch[i] = cells[i];
                    Undo.RecordObjects(_objScratch, "Edit Cell Background");
                    for (int i = 0; i < cells.Count; i++) cells[i].backgroundPictureBecausePlainIsLame = newSprite;
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                }

                bool anySpriteNow = (newSprite != null);
                if (!anySpriteNow)
                {
                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (cells[i].backgroundPictureBecausePlainIsLame != null) { anySpriteNow = true; break; }
                    }
                }

                if (anySpriteNow)
                {
                    Color firstColor = cells[0].backgroundColorLikeASunset;
                    bool mixedColor = false;
                    for (int i = 1; i < cells.Count; i++)
                    {
                        if (cells[i].backgroundColorLikeASunset != firstColor) { mixedColor = true; break; }
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.showMixedValue = mixedColor;
                    var newColor = EditorGUILayout.ColorField("Cell Color", firstColor);
                    EditorGUI.showMixedValue = false;
                    if (EditorGUI.EndChangeCheck())
                    {
                        EnsureObjectBuffer(cells.Count);
                        for (int i = 0; i < cells.Count; i++) _objScratch[i] = cells[i];
                        Undo.RecordObjects(_objScratch, "Edit Cell Color");
                        for (int i = 0; i < cells.Count; i++) cells[i].backgroundColorLikeASunset = newColor;
                        table.FlagLayoutAsNeedingSpaDay();
                        EditorUtility.SetDirty(table);
                    }
                }
            }
        }
    }

    private void EnsureObjectBuffer(int count)
    {
        if (_objScratch == null || _objScratch.Length != count) _objScratch = new Object[count];
    }

    private void DrawColumnStyleInspectorForHeaderHoncho(int cIdx)
    {
        table.SyncColumnWardrobes();
        var col = table.fancyColumnWardrobes[cIdx];

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Column {cIdx + 1}", EditorStyles.boldLabel);

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

            bool imgToggle = EditorPrefs.GetBool($"TTT_ColImg_{cIdx}", false);
            bool newImgToggle = EditorGUILayout.ToggleLeft("Image Settings", imgToggle);
            if (newImgToggle != imgToggle) EditorPrefs.SetBool($"TTT_ColImg_{cIdx}", newImgToggle);
            if (newImgToggle)
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
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
    }

    private void DrawGrid(Rect grid, float[] colW, float[] rowH)
    {
        bool isResizing = dragCol >= 0 || dragRow >= 0;
        var e = Event.current;
        bool hasRect = HasRectSelection();
        int selR0 = hasRect ? Mathf.Min(selRow, selRow2) : 0;
        int selC0 = hasRect ? Mathf.Min(selCol, selCol2) : 0;
        int selRCount = hasRect ? Mathf.Abs(selRow2 - selRow) + 1 : 1;
        int selCCount = hasRect ? Mathf.Abs(selCol2 - selCol) + 1 : 1;
        bool canMerge = hasRect && table != null && table.CanMergeThisPicnicBlanket(selR0, selC0, selRCount, selCCount);
        bool canUnmerge = hasRect && table != null && table.CanUnmergeThisFoodFight(selR0, selC0, selRCount, selCCount);

        float y = grid.y;
        for (int r = 0; r < table.totalRowsCountLetTheShowBegin; r++)
        {
            float x = grid.x;
            for (int c = 0; c < table.totalColumnsCountHighFive; c++)
            {
                if (!IsTopLeft(r, c, out var spanR, out var spanC))
                {
                    x += colW[c] + table.sociallyDistancedColumnsPixels;
                    continue;
                }
                float w = Sum(colW, c, spanC) + table.sociallyDistancedColumnsPixels * (spanC - 1);
                float h = Sum(rowH, r, spanR) + table.sociallyDistancedRowsPixels * (spanR - 1);
                var cellRect = new Rect(x, y, w, h);

                Handles.color = gridLine;
                Handles.DrawAAPolyLine(1.5f, new Vector3(cellRect.xMin, cellRect.yMin), new Vector3(cellRect.xMax, cellRect.yMin));
                Handles.DrawAAPolyLine(1.5f, new Vector3(cellRect.xMin, cellRect.yMax), new Vector3(cellRect.xMax, cellRect.yMax));
                Handles.DrawAAPolyLine(1.5f, new Vector3(cellRect.xMin, cellRect.yMin), new Vector3(cellRect.xMin, cellRect.yMax));
                Handles.DrawAAPolyLine(1.5f, new Vector3(cellRect.xMax, cellRect.yMin), new Vector3(cellRect.xMax, cellRect.yMax));

                if (IsInSelection(r, c, spanR, spanC))
                {
                    EditorGUI.DrawRect(cellRect, selFill);
                    Handles.color = selOutline;
                    Handles.DrawAAPolyLine(2.5f, new Vector3(cellRect.xMin, cellRect.yMin), new Vector3(cellRect.xMax, cellRect.yMin));
                    Handles.DrawDottedLine(new Vector3(cellRect.xMin, cellRect.yMin), new Vector3(cellRect.xMin, cellRect.yMax), 0.5f);
                    Handles.DrawDottedLine(new Vector3(cellRect.xMax, cellRect.yMin), new Vector3(cellRect.xMax, cellRect.yMax), 0.5f);
                    Handles.DrawAAPolyLine(2.5f, new Vector3(cellRect.xMin, cellRect.yMax), new Vector3(cellRect.xMax, cellRect.yMax));
                }

                bool hasChildTable = table.TryFindChildTableInCellLikeSherlock(r, c, out var childTbl);

                var snackCounts = new Dictionary<System.Type, int>(8);
                table.ScoutUiSnackCountsInCellLikeBeanCounter(r, c, snackCounts, true);
                BuildCellBadgeGuiLikeScoutPatches(snackCounts, hasChildTable, _badgesScratch);
                if (_badgesScratch.Count > 0) DrawCellBadgesLikeFlair(cellRect, _badgesScratch, r, c);
                _badgesScratch.Clear();

                if (!isResizing && e.type == EventType.MouseDown && e.button == 0 && cellRect.Contains(e.mousePosition))
                {
                    bool hoveringResizeHandle = actionMode == EditorActionMode.Resize && MouseIsHoveringOverResizeHandleLikeAHungrySeagull(grid, colW, rowH);
                    if (!hoveringResizeHandle)
                    {
                        bool isMoveMode = actionMode == EditorActionMode.MoveCells;
                        if (e.clickCount == 2 && hasChildTable && childTbl != null)
                        {
                            Selection.activeObject = childTbl;
                            EditorGUIUtility.PingObject(childTbl);
                            e.Use();
                            Repaint();
                            RepaintSceneViewLikeABobRoss();
                        }
                        else if (isMoveMode)
                        {
                            StartMoveDragLikeAUhaul(r, c);
                            e.Use();
                            Repaint();
                            RepaintSceneViewLikeABobRoss();
                        }
                        else
                        {
                            if (e.shift && selRow >= 0 && selCol >= 0)
                            {
                                selRow2 = r; selCol2 = c;
                            }
                            else
                            {
                                selRow = r; selCol = c; selRow2 = r; selCol2 = c;
                            }
                            headerRowBigEnchilada = -1; headerColBigEnchilada = -1;
                            if (actionMode == EditorActionMode.SelectObjects)
                            {
                                var cell = table.GrabCellLikeItOwesYouRent(r, c);
                                if (cell != null && !cell.isMashedLikePotatoes)
                                {
                                    Selection.activeTransform = cell.transform;
                                    EditorGUIUtility.PingObject(cell);
                                }
                            }
                            e.Use();
                            Repaint();
                            RepaintSceneViewLikeABobRoss();
                        }
                    }
                }

                bool overHandle = actionMode == EditorActionMode.Resize && MouseIsHoveringOverResizeHandleLikeAHungrySeagull(grid, colW, rowH);
                if (!overHandle && cellRect.Contains(e.mousePosition) && ((e.type == EventType.MouseDown && e.button == 1) || e.type == EventType.ContextClick))
                {
                    Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForCell(
                        table, r, c, spanR, spanC, canMerge, canUnmerge, selR0, selC0, selRCount, selCCount);
                    e.Use();
                    Repaint();
                }

                x += colW[c] + table.sociallyDistancedColumnsPixels;
            }
            y += rowH[r] + table.sociallyDistancedRowsPixels;
        }

        DrawResizeHandles(grid, colW, rowH);
        HandleGridInput(grid, colW, rowH);
        DrawMoveDragGhostIfAny(grid, colW, rowH);
    }

    private void DrawHeaderInspectorIfTheBossIsWatching()
    {
        if (headerColBigEnchilada >= 0 && headerColBigEnchilada < table.totalColumnsCountHighFive)
        {
            DrawColumnStyleInspectorForHeaderHoncho(headerColBigEnchilada);
            return;
        }
        if (headerRowBigEnchilada >= 0 && headerRowBigEnchilada < table.totalRowsCountLetTheShowBegin)
        {
            DrawRowStyleInspectorForHeaderHoncho(headerRowBigEnchilada);
            return;
        }
    }

    private void DrawHeaders(Rect top, Rect left, float[] colW, float[] rowH)
    {
        var e = Event.current;

        var x = top.x;
        for (int c = 0; c < colW.Length; c++)
        {
            var r = new Rect(x, top.y, colW[c], top.height);
            EditorGUI.LabelField(r, (c + 1).ToString(), EditorStyles.centeredGreyMiniLabel);

            Handles.color = gridLine;
            Handles.DrawLine(
                new Vector3(r.xMax + table.sociallyDistancedColumnsPixels * 0.5f, r.yMin),
                new Vector3(r.xMax + table.sociallyDistancedColumnsPixels * 0.5f, r.yMax));

            if (e.type == EventType.MouseDown && e.button == 0 && r.Contains(e.mousePosition))
            {
                bool additive = e.shift;
                SelectWholeColumnLikeAHungryHighlighter(c, additive);
                headerColBigEnchilada = c;
                headerRowBigEnchilada = -1;
                e.Use();
                Repaint();
                RepaintSceneViewLikeABobRoss();
            }

            if (((e.type == EventType.MouseDown && e.button == 1 && r.Contains(e.mousePosition)) || (e.type == EventType.ContextClick && r.Contains(e.mousePosition))))
            {
                Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForColumnHeader(table, c);
                e.Use();
                Repaint();
            }

            x += colW[c] + table.sociallyDistancedColumnsPixels;
        }

        var y = left.y;
        for (int rI = 0; rI < rowH.Length; rI++)
        {
            var rr = new Rect(left.x, y, left.width, rowH[rI]);
            EditorGUI.LabelField(rr, (rI + 1).ToString(), EditorStyles.centeredGreyMiniLabel);

            Handles.color = gridLine;
            Handles.DrawLine(
                new Vector3(rr.xMin, rr.yMax + table.sociallyDistancedRowsPixels * 0.5f),
                new Vector3(rr.xMax, rr.yMax + table.sociallyDistancedRowsPixels * 0.5f));

            if (e.type == EventType.MouseDown && e.button == 0 && rr.Contains(e.mousePosition))
            {
                bool additive = e.shift;
                SelectWholeRowLikeAHungryHighlighter(rI, additive);
                headerRowBigEnchilada = rI;
                headerColBigEnchilada = -1;
                e.Use();
                Repaint();
                RepaintSceneViewLikeABobRoss();
            }

            if (((e.type == EventType.MouseDown && e.button == 1 && rr.Contains(e.mousePosition)) || (e.type == EventType.ContextClick && rr.Contains(e.mousePosition))))
            {
                Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForRowHeader(table, rI);
                e.Use();
                Repaint();
            }

            y += rowH[rI] + table.sociallyDistancedRowsPixels;
        }
    }

    private void DrawResizeHandles(Rect grid, float[] colW, float[] rowH)
    {
        if (actionMode != EditorActionMode.Resize) return;

        var e = Event.current;
        float grab = 6f;

        float x = grid.x;
        for (int c = 0; c < table.totalColumnsCountHighFive - 1; c++)
        {
            float splitX = x + colW[c] + table.sociallyDistancedColumnsPixels * 0.5f;
            var handle = new Rect(splitX - grab * 0.5f, grid.y, grab, grid.height);
            EditorGUIUtility.AddCursorRect(handle, MouseCursor.ResizeHorizontal);

            if (e.type == EventType.MouseDown && handle.Contains(e.mousePosition))
            {
                dragCol = c;
                dragStartMouse = e.mousePosition.x;
                table.ConvertAllSpecsToPercentages();
                table.NormalizeWardrobePercentsToOneDadBod();
                FillCurrentColumnPercentages();
                EnsureFloatBuffer(ref startColW, _colPctBuf.Length);
                System.Array.Copy(_colPctBuf, startColW, _colPctBuf.Length);
                table.shareThePieEvenlyForColumns = false;
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Undo.RecordObject(table, "Resize Column");
                e.Use();
            }

            if (e.type == EventType.ContextClick && handle.Contains(e.mousePosition))
            {
                Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForResizeHandle(table, true, c);
                e.Use();
                Repaint();
            }

            x += colW[c] + table.sociallyDistancedColumnsPixels;
        }

        float y = grid.y;
        for (int r = 0; r < table.totalRowsCountLetTheShowBegin - 1; r++)
        {
            float splitY = y + rowH[r] + table.sociallyDistancedRowsPixels * 0.5f;
            var handle = new Rect(grid.x, splitY - grab * 0.5f, grid.width, grab);
            EditorGUIUtility.AddCursorRect(handle, MouseCursor.ResizeVertical);

            if (e.type == EventType.MouseDown && handle.Contains(e.mousePosition))
            {
                dragRow = r;
                dragStartMouse = e.mousePosition.y;
                table.ConvertAllSpecsToPercentages();
                table.NormalizeWardrobePercentsToOneDadBod();
                FillCurrentRowPercentages();
                EnsureFloatBuffer(ref startRowH, _rowPctBuf.Length);
                System.Array.Copy(_rowPctBuf, startRowH, _rowPctBuf.Length);
                table.shareThePieEvenlyForRows = false;
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Undo.RecordObject(table, "Resize Row");
                e.Use();
            }

            if (e.type == EventType.ContextClick && handle.Contains(e.mousePosition))
            {
                Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForResizeHandle(table, false, r);
                e.Use();
                Repaint();
            }

            y += rowH[r] + table.sociallyDistancedRowsPixels;
        }
    }

    private void DrawRowStyleInspectorForHeaderHoncho(int rIdx)
    {
        table.SyncRowWardrobes();
        var row = table.snazzyRowWardrobes[rIdx];

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Row {rIdx + 1}", EditorStyles.boldLabel);

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

            bool imgToggle = EditorPrefs.GetBool($"TTT_RowImg_{rIdx}", false);
            bool newImgToggle = EditorGUILayout.ToggleLeft("Image Settings", imgToggle);
            if (newImgToggle != imgToggle) EditorPrefs.SetBool($"TTT_RowImg_{rIdx}", newImgToggle);
            if (newImgToggle)
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
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
    }

    private void DrawSelectionSectionLikeDessert()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Selection", GUILayout.Width(70));
            var clipStyle = new GUIStyle(EditorStyles.miniLabel) { clipping = TextClipping.Clip, wordWrap = false, alignment = TextAnchor.MiddleLeft };
            string selText = SelectionLabel();
            EditorGUILayout.LabelField(selText, clipStyle, GUILayout.ExpandWidth(true), GUILayout.MinWidth(1));
            GUILayout.FlexibleSpace();
            float w2 = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(2, 70f, 110f);
            if (GUILayout.Button("Select", GUILayout.Width(w2))) SelectActiveCellInHierarchy();
            if (GUILayout.Button("Ping", GUILayout.Width(w2))) PingActiveCell();
        }

        if (!HasSelection() || table == null) return;

        bool singleCell = selRow == selRow2 && selCol == selCol2 && headerColBigEnchilada < 0 && headerRowBigEnchilada < 0;

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Align", GUILayout.Width(50));
            int r0 = Mathf.Min(selRow, selRow2);
            int r1 = Mathf.Max(selRow, selRow2);
            int c0 = Mathf.Min(selCol, selCol2);
            int c1 = Mathf.Max(selCol, selCol2);

            bool leftOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
            bool centerOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
            bool rightOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
            bool topOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
            bool middleOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
            bool bottomOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
            bool fullOn = table.IsSelectionFullLikeBurritoWrap(r0, r1, c0, c1);

            float w7 = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(7, 50f, 100f);

            EditorGUI.BeginDisabledGroup(leftOn);
            if (GUILayout.Button("Left", GUILayout.Width(w7))) { ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(centerOn);
            if (GUILayout.Button("Center", GUILayout.Width(w7))) { ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(rightOn);
            if (GUILayout.Button("Right", GUILayout.Width(w7))) { ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(topOn);
            if (GUILayout.Button("Top", GUILayout.Width(w7))) { ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Top); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(middleOn);
            if (GUILayout.Button("Middle", GUILayout.Width(w7))) { ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(bottomOn);
            if (GUILayout.Button("Bottom", GUILayout.Width(w7))) { ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom); Repaint(); }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(fullOn);
            if (GUILayout.Button("Full", GUILayout.Width(w7))) { ApplyFullAlignmentToSelectionLikeFittedSheet(); Repaint(); }
            EditorGUI.EndDisabledGroup();
        }

        if (!singleCell) return;

        var rt = table.FetchCellRectTransformVIP(selRow, selCol);
        if (rt == null) return;

        var kids = new List<Transform>(16);
        rt.ListForeignKidsLikeRollCall(kids);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Contents", EditorStyles.boldLabel);
            if (kids.Count == 0)
            {
                EditorGUILayout.LabelField("None", EditorStyles.miniLabel);
            }
            else
            {
                for (int i = 0; i < kids.Count; i++)
                {
                    var t = kids[i];
                    if (t == null) continue;
                    var nice = Ux_TonkersTableTopiaExtensions.TypeNameShortLikeNameTag(Ux_TonkersTableTopiaExtensions.PickPrimaryUiTypeLikeMenuDecider(t.gameObject));
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField($"{i + 1}. {t.gameObject.name} ({nice})");
                        if (GUILayout.Button("Select", GUILayout.Width(60))) t.SelectAndPingLikeABeacon();
                        if (GUILayout.Button("Delete", GUILayout.Width(60)))
                        {
                            Undo.RecordObject(table, "Delete Cell Child");
                            if (!Application.isPlaying) Undo.DestroyObjectImmediate(t.gameObject);
                            else Destroy(t.gameObject);
                            table.FlagLayoutAsNeedingSpaDay();
                            EditorUtility.SetDirty(table);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void DrawTableButtonsToolbarLikeASpreadsheet()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = HasSelection();
                float w = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(4, 0f, 140f);
                if (GUILayout.Button("Insert Row Above", GUILayout.Width(w))) InsertRowAbove();
                if (GUILayout.Button("Insert Row Below", GUILayout.Width(w))) InsertRowBelow();
                if (GUILayout.Button("Insert Col Left", GUILayout.Width(w))) InsertColLeft();
                if (GUILayout.Button("Insert Col Right", GUILayout.Width(w))) InsertColRight();
                GUI.enabled = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = HasSelection();
                float w = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(2, 0f, 160f);
                if (GUILayout.Button("Delete Row", GUILayout.Width(w))) DeleteRow();
                if (GUILayout.Button("Delete Col", GUILayout.Width(w))) DeleteCol();
                GUI.enabled = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                bool hasRect = HasRectSelection();
                int r0 = hasRect ? Mathf.Min(selRow, selRow2) : 0;
                int c0 = hasRect ? Mathf.Min(selCol, selCol2) : 0;
                int rCount = hasRect ? Mathf.Abs(selRow2 - selRow) + 1 : 1;
                int cCount = hasRect ? Mathf.Abs(selCol2 - selCol) + 1 : 1;

                bool canMerge = hasRect && table != null && table.CanMergeThisPicnicBlanket(r0, c0, rCount, cCount);
                bool canUnmerge = hasRect && table != null && table.CanUnmergeThisFoodFight(r0, c0, rCount, cCount);

                float w4 = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(4, 0f, 160f);

                GUI.enabled = canMerge;
                if (GUILayout.Button("Merge", GUILayout.Width(w4))) MergeSelection();
                GUI.enabled = canUnmerge;
                if (GUILayout.Button("Unmerge", GUILayout.Width(w4))) UnmergeTopLeft();
                GUI.enabled = true;

                if (GUILayout.Button("Distribute Columns", GUILayout.Width(w4))) DistributeColumnsNow();
                if (GUILayout.Button("Distribute Rows", GUILayout.Width(w4))) DistributeRowsNow();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                float w = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(2, 0f, 200f);
                if (GUILayout.Button("Make Last Column Flexible", GUILayout.Width(w))) { MakeLastColumnFlexyButHouseTrained(); }
                if (GUILayout.Button("Make Last Row Flexible", GUILayout.Width(w))) { MakeLastRowFlexyButHouseTrained(); }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = HasSelection();
                GUILayout.Label("Align", GUILayout.Width(50));

                int r0 = Mathf.Min(selRow, selRow2);
                int r1 = Mathf.Max(selRow, selRow2);
                int c0 = Mathf.Min(selCol, selCol2);
                int c1 = Mathf.Max(selCol, selCol2);

                bool leftOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                bool centerOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                bool rightOn = table.IsSelectionHorizAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                bool topOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                bool middleOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                bool bottomOn = table.IsSelectionVertAlignedLikeDejaVu(r0, r1, c0, c1, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                bool fullOn = table.IsSelectionFullLikeBurritoWrap(r0, r1, c0, c1);

                float w7 = Ux_TonkersTableTopiaExtensions.CalcShrinkyDinkWidthLikeDietCokeSquisher(7, 50f, 100f);

                EditorGUI.BeginDisabledGroup(leftOn);
                if (GUILayout.Button("Left", GUILayout.Width(w7))) ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(centerOn);
                if (GUILayout.Button("Center", GUILayout.Width(w7))) ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(rightOn);
                if (GUILayout.Button("Right", GUILayout.Width(w7))) ApplyHorizontalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(topOn);
                if (GUILayout.Button("Top", GUILayout.Width(w7))) ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(middleOn);
                if (GUILayout.Button("Middle", GUILayout.Width(w7))) ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(bottomOn);
                if (GUILayout.Button("Bottom", GUILayout.Width(w7))) ApplyVerticalToSelectionLikeLaserLevel(Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(fullOn);
                if (GUILayout.Button("Full", GUILayout.Width(w7))) ApplyFullAlignmentToSelectionLikeFittedSheet();
                EditorGUI.EndDisabledGroup();

                GUI.enabled = true;
            }
        }
    }

    private void DrawTableSizeControls()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            var rt = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());
            var parent = rt ? rt.transform.parent as RectTransform : null;

            bool usePercent = EditorPrefs.GetBool("Ux_TableSize_UsePercent", true);
            bool newUsePercent = EditorGUILayout.ToggleLeft("Size By Percent", usePercent);
            if (newUsePercent != usePercent)
            {
                EditorPrefs.SetBool("Ux_TableSize_UsePercent", newUsePercent);
                if (newUsePercent && parent != null)
                {
                    Undo.RecordObject(rt, "Switch To %");
                    float parentW = Mathf.Max(1f, parent.rect.width);
                    float parentH = Mathf.Max(1f, parent.rect.height);
                    float wPct = Mathf.Clamp01(rt.rect.width / parentW);
                    float hPct = Mathf.Clamp01(rt.rect.height / parentH);
                    rt.SetAnchorsByPercentLikeABoss(wPct, hPct);
                    table.ConvertAllSpecsToPercentages();
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                    EditorUtility.SetDirty(rt);
                }
            }

            if (newUsePercent)
            {
                if (parent != null)
                    EditorGUILayout.HelpBox("Table container scales with its parent. Adjust width% and height% to set the anchored span. Rows/columns use their own percent settings separately.", MessageType.None);

                float curWPct = Mathf.Clamp01(rt.anchorMax.x - rt.anchorMin.x) * 100f;
                float curHPct = Mathf.Clamp01(rt.anchorMax.y - rt.anchorMin.y) * 100f;

                EditorGUI.BeginChangeCheck();
                float wPct = EditorGUILayout.Slider("Width %", curWPct, 0f, 100f);
                float hPct = EditorGUILayout.Slider("Height %", curHPct, 0f, 100f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(rt, "Set Table Size %");
                    rt.SetAnchorsByPercentLikeABoss(wPct / 100f, hPct / 100f);
                    table.ConvertAllSpecsToPercentages();
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                    EditorUtility.SetDirty(rt);
                }
            }
            else
            {
                if (parent != null)
                    EditorGUILayout.HelpBox("Table container uses fixed pixels. Type width/height to set size. Rows/columns sizing still controlled in their own sections.", MessageType.None);

                float w = Mathf.Max(0f, rt.rect.width);
                float h = Mathf.Max(0f, rt.rect.height);

                EditorGUI.BeginChangeCheck();
                float newW = EditorGUILayout.FloatField("Width (px)", w);
                float newH = EditorGUILayout.FloatField("Height (px)", h);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(rt, "Set Table Size px");
                    rt.SetPixelSizeLikeIts1999(newW, newH);
                    table.ConvertAllSpecsToPercentages();
                    table.FlagLayoutAsNeedingSpaDay();
                    EditorUtility.SetDirty(table);
                    EditorUtility.SetDirty(rt);
                }
            }
        }
    }

    private void DrawWysiwyg()
    {
        if (table.totalRowsCountLetTheShowBegin < 1 || table.totalColumnsCountHighFive < 1) return;

        var tableRT = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());

        float innerW = Mathf.Max(1f, tableRT.rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);
        float innerH = Mathf.Max(1f, tableRT.rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);

        int cols = table.totalColumnsCountHighFive;
        int rows = table.totalRowsCountLetTheShowBegin;

        EnsureFloatBuffer(ref _previewColBuf, cols);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            cols,
            i => (i < table.fancyColumnWardrobes.Count) ? table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f,
            table.sociallyDistancedColumnsPixels,
            innerW,
            ref _previewColBuf);

        EnsureFloatBuffer(ref _previewRowBuf, rows);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            rows,
            i => (i < table.snazzyRowWardrobes.Count) ? table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f,
            table.sociallyDistancedRowsPixels,
            innerH,
            ref _previewRowBuf);

        for (int c = 0; c < _previewColBuf.Length; c++) if (_previewColBuf[c] < 1f) _previewColBuf[c] = 1f;
        for (int r = 0; r < _previewRowBuf.Length; r++) if (_previewRowBuf[r] < 1f) _previewRowBuf[r] = 1f;

        float natW = 0f;
        for (int c = 0; c < cols; c++) natW += _previewColBuf[c];
        if (cols > 1) natW += table.sociallyDistancedColumnsPixels * (cols - 1);

        float natH = 0f;
        for (int r = 0; r < rows; r++) natH += _previewRowBuf[r];
        if (rows > 1) natH += table.sociallyDistancedRowsPixels * (rows - 1);

        float headerH = 22f;
        float headerW = 34f;

        float hZoom = Mathf.Clamp(EditorPrefs.GetFloat("Ux_HZoom", 1f), 0.25f, 3f);
        float vZoom = Mathf.Clamp(EditorPrefs.GetFloat("Ux_VZoom", 1f), 0.25f, 3f);

        float requestedW = Mathf.Max(1f, natW * hZoom);
        float contentH = Mathf.Max(1f, natH * vZoom);
        float totalH = headerH + contentH;

        Rect previewRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(totalH), GUILayout.ExpandWidth(true));

        float availW = Mathf.Max(1f, previewRect.width - headerW - 6f);
        float appliedScaleX = (natW > 0f) ? Mathf.Min(hZoom, availW / natW) : hZoom;
        float contentW = Mathf.Min(natW * appliedScaleX, availW);

        Rect headRowRect = new Rect(previewRect.x + headerW, previewRect.y, contentW, headerH);
        Rect headColRect = new Rect(previewRect.x, previewRect.y + headerH, headerW, contentH);
        Rect headCornerRect = new Rect(previewRect.x, previewRect.y, headerW, headerH);
        Rect gridRect = new Rect(previewRect.x + headerW, previewRect.y + headerH, contentW, contentH);

        float[] colW = new float[cols];
        for (int c = 0; c < cols; c++) colW[c] = _previewColBuf[c] * appliedScaleX;

        float[] rowH = new float[rows];
        for (int r = 0; r < rows; r++) rowH[r] = _previewRowBuf[r] * vZoom;

        LoadPreviewColors(out var wysiRowA, out var wysiRowB, out var wysiHeader);

        EditorGUI.DrawRect(previewRect, new Color(0.97f, 0.97f, 0.97f, 1f));
        EditorGUI.DrawRect(headCornerRect, wysiHeader);
        EditorGUI.DrawRect(headRowRect, wysiHeader);
        EditorGUI.DrawRect(headColRect, wysiHeader);

        float bandY = gridRect.y;
        for (int r = 0; r < rows; r++)
        {
            float h = rowH[r];
            var bandRect = new Rect(gridRect.x, bandY, gridRect.width, h);
            EditorGUI.DrawRect(bandRect, ((r & 1) == 0) ? wysiRowA : wysiRowB);
            bandY += h + table.sociallyDistancedRowsPixels;
        }

        Handles.color = gridLine;
        Handles.DrawAAPolyLine(2f, new Vector3(previewRect.x, previewRect.y + headerH), new Vector3(previewRect.x + previewRect.width, previewRect.y + headerH));
        Handles.DrawAAPolyLine(2f, new Vector3(previewRect.x + headerW, previewRect.y), new Vector3(previewRect.x + headerW, previewRect.y + previewRect.height));

        DrawHeaders(headRowRect, headColRect, colW, rowH);
        DrawGrid(gridRect, colW, rowH);

        _lastGridRect = gridRect;
        _lastInnerWidth = innerW;
        _lastInnerHeight = innerH;

        var e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && headCornerRect.Contains(e.mousePosition))
        {
            SetHighlightLikeAGlowStick(table, 0, 0, table.totalRowsCountLetTheShowBegin, table.totalColumnsCountHighFive);
            headerRowBigEnchilada = -1;
            headerColBigEnchilada = -1;
            e.Use();
            Repaint();
        }
        if (e.type == EventType.ContextClick && headCornerRect.Contains(e.mousePosition))
        {
            Ux_TonkersTableTopiaContextMenuGravyBoat.ShowForTableHeader(table);
            e.Use();
            Repaint();
        }
    }

    private void DrawWysiwygPreviewColorPrefs()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("WYSIWYG Preview Colors (Editor-wide)", EditorStyles.boldLabel);

            LoadPreviewColors(out var rowA, out var rowB, out var header);
            EditorGUI.BeginChangeCheck();
            rowA = EditorGUILayout.ColorField("Row A", rowA);
            rowB = EditorGUILayout.ColorField("Row B", rowB);
            header = EditorGUILayout.ColorField("Headers", header);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Use Theme Defaults", GUILayout.Width(160)))
                {
                    ThemeDefaults(out rowA, out rowB, out header);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                WritePrefColor(PREF_WYSI_ROW_A, rowA);
                WritePrefColor(PREF_WYSI_ROW_B, rowB);
                WritePrefColor(PREF_WYSI_HEADER, header);
                Repaint();
            }

            EditorGUILayout.HelpBox("These colors are stored in EditorPrefs and used for every table's WYSIWYG preview.", MessageType.None);
        }
    }

    private void EnsureColumnStylesSize()
    {
        if (table.fancyColumnWardrobes == null) table.fancyColumnWardrobes = new List<Ux_TonkersTableTopiaLayout.ColumnStyle>();
        while (table.fancyColumnWardrobes.Count < table.totalColumnsCountHighFive) table.fancyColumnWardrobes.Add(new Ux_TonkersTableTopiaLayout.ColumnStyle());
        table.SyncColumnWardrobes();
    }

    private void EnsureDefaultSelection()
    {
        if (table == null) return;

        int id = table.GetInstanceID();
        if (_lastInspectedTableId != id)
        {
            _lastInspectedTableId = id;
            selRow = selRow2 = 0;
            selCol = selCol2 = 0;
            headerRowBigEnchilada = -1;
            headerColBigEnchilada = -1;
            RepaintSceneViewLikeABobRoss();
            return;
        }

        if (selRow < 0 || selCol < 0 || selRow >= table.totalRowsCountLetTheShowBegin || selCol >= table.totalColumnsCountHighFive)
        {
            selRow = selRow2 = 0;
            selCol = selCol2 = 0;
            RepaintSceneViewLikeABobRoss();
        }

        if (headerRowBigEnchilada >= table.totalRowsCountLetTheShowBegin) headerRowBigEnchilada = -1;
        if (headerColBigEnchilada >= table.totalColumnsCountHighFive) headerColBigEnchilada = -1;
    }

    private void EnsureRowStylesSize()
    {
        if (table.snazzyRowWardrobes == null) table.snazzyRowWardrobes = new List<Ux_TonkersTableTopiaLayout.RowStyle>();
        while (table.snazzyRowWardrobes.Count < table.totalRowsCountLetTheShowBegin) table.snazzyRowWardrobes.Add(new Ux_TonkersTableTopiaLayout.RowStyle());
        table.SyncRowWardrobes();
    }

    private List<Ux_TonkersTableTopiaCell> GetSelectedCellsHotAndReady()
    {
        _selectedCellsScratch.Clear();
        if (!HasSelection()) return _selectedCellsScratch;

        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);

        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                var cell = table.GrabCellLikeItOwesYouRent(r, c);
                if (cell != null) _selectedCellsScratch.Add(cell);
            }
        }
        return _selectedCellsScratch;
    }

    private void HandleGridInput(Rect grid, float[] colW, float[] rowH)
    {
        var e = Event.current;

        if (e.type == EventType.MouseUp)
        {
            if (dragCol >= 0 || dragRow >= 0)
            {
                dragCol = -1;
                dragRow = -1;
                GUIUtility.hotControl = 0;
                e.Use();
                Repaint();
            }
            else if (moveDragActive)
            {
                int tr, tc;
                Rect dummy;
                if (GetCellAtMouseLikeHawk(grid, colW, rowH, e.mousePosition, out tr, out tc, out dummy))
                {
                    if (moveSrcRow >= 0 && moveSrcCol >= 0)
                    {
                        int srcMainR, srcMainC;
                        Ux_TonkersTableTopiaCell srcMain;
                        table.TryPeekMainCourseLikeABuffet(moveSrcRow, moveSrcCol, out srcMainR, out srcMainC, out srcMain);

                        int dstMainR, dstMainC;
                        Ux_TonkersTableTopiaCell dstMain;
                        table.TryPeekMainCourseLikeABuffet(tr, tc, out dstMainR, out dstMainC, out dstMain);

                        var srcRT = table.FetchCellRectTransformVIP(srcMainR, srcMainC);
                        var dstRT = table.FetchCellRectTransformVIP(dstMainR, dstMainC);

                        if (srcRT != null && dstRT != null && srcRT != dstRT && dstRT.gameObject.activeInHierarchy)
                        {
                            bool moved = false;
                            if (srcRT.HasForeignKidsLikeStowaways())
                            {
                                Undo.RecordObject(table, "Move Cell Contents");
                                table.MoveForeignKidsLikeABoxTruck(srcRT, dstRT);
                                moved = true;
                            }
                            if (table.MoveNestedTablesLikeACaravan(srcRT, dstRT))
                            {
                                if (!moved) Undo.RecordObject(table, "Move Nested Table");
                                moved = true;
                            }
                            if (moved)
                            {
                                table.FlagLayoutAsNeedingSpaDay();
                                EditorUtility.SetDirty(table);
                            }
                        }
                    }
                }
                moveDragActive = false;
                moveSrcRow = moveSrcCol = -1;
                e.Use();
                Repaint();
            }
        }

        if (e.type == EventType.MouseDrag)
        {
            if (dragCol >= 0)
            {
                ResizeColWithMouse(grid);
                e.Use();
            }
            if (dragRow >= 0)
            {
                ResizeRowWithMouse(grid);
                e.Use();
            }
        }

        if (moveDragActive && e.type == EventType.MouseMove)
        {
            Repaint();
        }
    }

    private bool HasRectSelection()
    {
        if (!HasSelection()) return false;
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);
        return r1 >= r0 && c1 >= c0;
    }

    private bool HasSelection()
    {
        return selRow >= 0 && selCol >= 0 && selRow < table.totalRowsCountLetTheShowBegin && selCol < table.totalColumnsCountHighFive && selRow2 >= 0 && selCol2 >= 0;
    }

    private void InsertColLeft()
    {
        if (!HasSelection()) return;
        Undo.RecordObject(table, "Insert Column");
        table.InsertColumnLikeANinja(Mathf.Min(selCol, selCol2));
    }

    private void InsertColRight()
    {
        if (!HasSelection()) return;
        Undo.RecordObject(table, "Insert Column");
        table.InsertColumnLikeANinja(Mathf.Max(selCol, selCol2) + 1);
    }

    private void InsertRowAbove()
    {
        if (!HasSelection()) return;
        Undo.RecordObject(table, "Insert Row");
        table.InsertRowLikeANinja(Mathf.Min(selRow, selRow2));
    }

    private void InsertRowBelow()
    {
        if (!HasSelection()) return;
        Undo.RecordObject(table, "Insert Row");
        table.InsertRowLikeANinja(Mathf.Max(selRow, selRow2) + 1);
    }

    private bool IsInSelection(int r, int c, int spanR, int spanC)
    {
        if (!HasSelection()) return false;
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);
        int rr0 = r, rr1 = r + spanR - 1;
        int cc0 = c, cc1 = c + spanC - 1;
        return rr1 >= r0 && rr0 <= r1 && cc1 >= c0 && cc0 <= c1;
    }

    private bool IsTopLeft(int r, int c, out int spanR, out int spanC)
    {
        spanR = 1; spanC = 1;
        var cell = table.GrabCellLikeItOwesYouRent(r, c);
        if (cell == null) return true;
        if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null) return false;
        spanR = Mathf.Max(1, cell.howManyRowsAreHoggingThisSeat);
        spanC = Mathf.Max(1, cell.howManyColumnsAreSneakingIn);
        return true;
    }

    private void MakeLastColumnFlexyButHouseTrained()
    {
        if (table == null) return;
        EnsureColumnStylesSize();
        table.ConvertAllSpecsToPercentages();
        FillCurrentColumnPercentages();
        int n = Mathf.Max(0, table.totalColumnsCountHighFive);
        for (int i = 0; i < n - 1; i++)
            table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = -Mathf.Clamp01(_colPctBuf[i]);
        if (n > 0) table.fancyColumnWardrobes[n - 1].requestedWidthMaybePercentIfNegative = 0f;
        table.shareThePieEvenlyForColumns = false;
        table.FlagLayoutAsNeedingSpaDay();
        EditorUtility.SetDirty(table);
    }

    private void MakeLastRowFlexyButHouseTrained()
    {
        if (table == null) return;
        EnsureRowStylesSize();
        table.ConvertAllSpecsToPercentages();
        FillCurrentRowPercentages();
        int n = Mathf.Max(0, table.totalRowsCountLetTheShowBegin);
        for (int i = 0; i < n - 1; i++)
            table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = -Mathf.Clamp01(_rowPctBuf[i]);
        if (n > 0) table.snazzyRowWardrobes[n - 1].requestedHeightMaybePercentIfNegative = 0f;
        table.shareThePieEvenlyForRows = false;
        table.FlagLayoutAsNeedingSpaDay();
        EditorUtility.SetDirty(table);
    }

    private void MergeSelection()
    {
        if (!HasRectSelection()) return;
        var r0 = Mathf.Min(selRow, selRow2);
        var c0 = Mathf.Min(selCol, selCol2);
        var rCount = Mathf.Abs(selRow2 - selRow) + 1;
        var cCount = Mathf.Abs(selCol2 - selCol) + 1;
        Undo.RecordObject(table, "Merge Cells");
        table.MergeCellsLikeAGroupHug(r0, c0, rCount, cCount);
        table.FlagLayoutAsNeedingSpaDay();
    }

    private bool MouseIsHoveringOverResizeHandleLikeAHungrySeagull(Rect grid, float[] colW, float[] rowH)
    {
        var e = Event.current;
        float grab = 6f;

        float x = grid.x;
        for (int c = 0; c < table.totalColumnsCountHighFive - 1; c++)
        {
            float splitX = x + colW[c] + table.sociallyDistancedColumnsPixels * 0.5f;
            var handle = new Rect(splitX - grab * 0.5f, grid.y, grab, grid.height);
            if (handle.Contains(e.mousePosition)) return true;
            x += colW[c] + table.sociallyDistancedColumnsPixels;
        }

        float y = grid.y;
        for (int r = 0; r < table.totalRowsCountLetTheShowBegin - 1; r++)
        {
            float splitY = y + rowH[r] + table.sociallyDistancedRowsPixels * 0.5f;
            var handle = new Rect(grid.x, splitY - grab * 0.5f, grid.width, grab);
            if (handle.Contains(e.mousePosition)) return true;
            y += rowH[r] + table.sociallyDistancedRowsPixels;
        }

        return false;
    }

    private void OnEnable()
    {
        table = (Ux_TonkersTableTopiaLayout)target;
        _cachedTableRT = table != null ? table.GetComponent<RectTransform>() : null;
        EnsureDefaultSelection();
        actionMode = (EditorActionMode)EditorPrefs.GetInt(PREF_ActionMode, (int)EditorActionMode.HighlightCells);
    }

    private void OnSceneGUI()
    {
        if (!sceneHighlight || !HasSelection()) return;

        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);

        for (int r = r0; r <= r1; r++)
        {
            for (int c = c0; c <= c1; c++)
            {
                var cell = table.GrabCellLikeItOwesYouRent(r, c);
                if (cell == null || cell.isMashedLikePotatoes) continue;
                var rt = cell.GetComponent<RectTransform>();
                if (rt == null) continue;

                var corners = _sceneCorners;
                rt.GetWorldCorners(corners);

                Handles.DrawSolidRectangleWithOutline(
                    new Vector3[] { corners[0], corners[1], corners[2], corners[3] },
                    new Color(0.2f, 0.6f, 1f, 0.08f),
                    selOutline);
            }
        }
    }

    private void PingActiveCell()
    {
        if (!HasSelection()) return;
        var cell = table.GrabCellLikeItOwesYouRent(selRow, selCol);
        if (cell == null) return;
        EditorGUIUtility.PingObject(cell);
    }

    private void ResizeColWithMouse(Rect grid)
    {
        if (dragCol < 0) return;
        if (startColW == null || startColW.Length != table.totalColumnsCountHighFive) return;

        float dxPx = Event.current.mousePosition.x - dragStartMouse;

        float naturalW = ComputeNaturalGridWidthLikeTapeMeasure();
        float scaleXLikePeriscope = (grid.width > 0f && naturalW > 0f) ? (grid.width / naturalW) : 1f;
        if (Mathf.Abs(scaleXLikePeriscope) < 0.0001f) scaleXLikePeriscope = 1f;

        float deltaContent = dxPx / scaleXLikePeriscope;

        ApplyColResize(dragCol, deltaContent);

        table.FlagLayoutAsNeedingSpaDay();
        Repaint();
    }

    private void ResizeRowWithMouse(Rect grid)
    {
        if (dragRow < 0 || startRowH == null || startRowH.Length != table.totalRowsCountLetTheShowBegin) return;

        float dyPx = Event.current.mousePosition.y - dragStartMouse;

        float naturalH = ComputeNaturalGridHeightLikeTapeMeasure();
        float scaleYLikePeriscope = (_lastGridRect.height > 0f) ? (_lastGridRect.height / naturalH) : 1f;

        float deltaContent = dyPx / Mathf.Max(0.0001f, scaleYLikePeriscope);

        ApplyRowResize(dragRow, deltaContent);
        table.FlagLayoutAsNeedingSpaDay();
        Repaint();
    }

    private void SelectActiveCellInHierarchy()
    {
        if (!HasSelection()) return;
        var cell = table.GrabCellLikeItOwesYouRent(selRow, selCol);
        if (cell == null || cell.isMashedLikePotatoes) return;
        Selection.activeTransform = cell.transform;
    }

    private string SelectionLabel()
    {
        if (!HasSelection()) return "None";
        if (selRow == selRow2 && selCol == selCol2) return $"R{selRow + 1},C{selCol + 1}";
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);
        return $"R{r0 + 1}..{r1 + 1}, C{c0 + 1}..{c1 + 1}";
    }

    private void SelectWholeColumnLikeAHungryHighlighter(int c, bool additive)
    {
        c = Mathf.Clamp(c, 0, Mathf.Max(0, table.totalColumnsCountHighFive - 1));
        if (additive && selRow >= 0 && selCol >= 0)
        {
            selCol2 = c; selRow = 0; selRow2 = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        }
        else
        {
            selCol = c; selCol2 = c; selRow = 0; selRow2 = Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1);
        }
        RepaintSceneViewLikeABobRoss();
    }

    private void SelectWholeRowLikeAHungryHighlighter(int r, bool additive)
    {
        r = Mathf.Clamp(r, 0, Mathf.Max(0, table.totalRowsCountLetTheShowBegin - 1));
        if (additive && selRow >= 0 && selCol >= 0)
        {
            selRow2 = r; selCol = 0; selCol2 = Mathf.Max(0, table.totalColumnsCountHighFive - 1);
        }
        else
        {
            selRow = r; selRow2 = r; selCol = 0; selCol2 = Mathf.Max(0, table.totalColumnsCountHighFive - 1);
        }
        RepaintSceneViewLikeABobRoss();
    }

    private float Sum(float[] arr, int start, int count)
    {
        float s = 0f;
        int max = Mathf.Min(arr.Length, start + count);
        for (int i = start; i < max; i++) s += arr[i];
        return s;
    }

    private void UnmergeTopLeft()
    {
        if (!HasSelection()) return;
        int r0 = Mathf.Min(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2);
        int rCount = Mathf.Abs(selRow2 - selRow) + 1;
        int cCount = Mathf.Abs(selCol2 - selCol) + 1;
        Undo.RecordObject(table, "Unmerge Selection");
        table.UnmergeEverythingInRectLikeItNeverHappened(r0, c0, rCount, cCount);
        table.FlagLayoutAsNeedingSpaDay();
        EditorUtility.SetDirty(table);
    }

    private void BuildCellBadgeGuiLikeScoutPatches(Dictionary<System.Type, int> snackCounts, bool includeTableBadge, List<GUIContent> outBadges)
    {
        outBadges.Clear();
        if (includeTableBadge) outBadges.Add(new GUIContent("T"));

        if (snackCounts == null || snackCounts.Count == 0) return;

        for (int i = 0; i < _badgeOrder.Length; i++)
        {
            var tp = _badgeOrder[i];
            if (!snackCounts.TryGetValue(tp, out var count) || count <= 0) continue;

            if (!_badgeIconCache.TryGetValue(tp, out var gc))
            {
                gc = IconForSnackTypeLikeSmiley(tp);
                _badgeIconCache[tp] = gc;
            }
            for (int k = 0; k < count; k++) outBadges.Add(gc);
        }

        foreach (var kvp in snackCounts)
        {
            var tp = kvp.Key;
            int count = kvp.Value;
            if (count <= 0) continue;

            bool alreadyStandard = false;
            for (int i = 0; i < _badgeOrder.Length; i++) if (_badgeOrder[i] == tp) { alreadyStandard = true; break; }
            if (alreadyStandard) continue;

            if (!_badgeIconCache.TryGetValue(tp, out var gc))
            {
                gc = IconForSnackTypeLikeSmiley(tp);
                _badgeIconCache[tp] = gc;
            }
            for (int k = 0; k < count; k++) outBadges.Add(gc);
        }
    }

    private GUIContent IconForSnackTypeLikeSmiley(System.Type t)
    {
        if (t == null || t == typeof(UnityEngine.Object)) return new GUIContent("?");
        var gc = EditorGUIUtility.ObjectContent(null, t);
        if (gc != null && gc.image != null) return new GUIContent(gc.image);
        return new GUIContent(ShortCodeForSnackLikeAcronym(t));
    }

    private string ShortCodeForSnackLikeAcronym(System.Type t)
    {
        if (t == typeof(Text)) return "Tx";
        if (t == typeof(Image)) return "Img";
        if (t == typeof(RawImage)) return "Raw";
        if (t == typeof(Button)) return "Btn";
        if (t == typeof(Toggle)) return "Tog";
        if (t == typeof(Slider)) return "Sld";
        if (t == typeof(Dropdown)) return "Drp";
        if (t == typeof(Scrollbar)) return "Bar";
        if (t == typeof(ScrollRect)) return "Scr";
        if (t == typeof(InputField)) return "Inp";
        return "?";
    }

    private void DrawCellBadgesLikeFlair(Rect cellRect, List<GUIContent> badges, int r, int c)
    {
        const float size = 12f;
        const float pad = 2f;
        const float gap = 1f;

        bool isFull;
        var anchor = table != null ? table.GuessCellForeignersAnchorLikeDart(r, c, out isFull) : new Vector2(0.5f, 0.5f);

        float clusterW = badges.Count * (size + gap) - gap;
        float anchorX = Mathf.Lerp(cellRect.xMin + pad, cellRect.xMax - pad, Mathf.Clamp01(anchor.x));
        float topY = cellRect.yMin + pad;
        float botY = cellRect.yMax - pad - size;
        float anchorY = Mathf.Lerp(botY, topY, Mathf.Clamp01(anchor.y));

        float xStart = Mathf.Clamp(anchorX - clusterW * 0.5f, cellRect.xMin + pad, cellRect.xMax - pad - clusterW);
        float yStart = Mathf.Clamp(anchorY - size * 0.5f, cellRect.yMin + pad, cellRect.yMax - pad - size);

        for (int i = 0; i < badges.Count; i++)
        {
            var rct = new Rect(xStart + i * (size + gap), yStart, size, size);
            EditorGUI.DrawRect(rct, new Color(0.2f, 0.6f, 1f, 0.25f));
            var b = badges[i];
            if (b.image != null) GUI.DrawTexture(rct, b.image, ScaleMode.ScaleToFit, true);
            else GUI.Label(rct, b, MiniBoldCenter);
        }
    }

    private static readonly EditorApplication.CallbackFunction _repaintAction = SceneView.RepaintAll;

    private static void RepaintSceneViewLikeABobRoss()
    {
        SceneView.RepaintAll();
        if (Event.current != null)
        {
            HandleUtility.Repaint();
        }
        else
        {
            EditorApplication.delayCall += _repaintAction;
        }
        EditorApplication.QueuePlayerLoopUpdate();
    }

    private void DrawActionModeHelpdeskWithDadJokes()
    {
        string key = "TTT_ModeHelp_Foldout_" + ((int)actionMode).ToString();
        bool show = EditorPrefs.GetBool(key, true);
        show = EditorGUILayout.Foldout(show, "Mode Help");
        EditorPrefs.SetBool(key, show);
        if (!show) return;

        string msg = "";
        switch (actionMode)
        {
            case EditorActionMode.HighlightCells:
                msg = "Highlight: click to select a cell or drag-select a rectangle. Shift extends selection. Double-click opens nested table.";
                break;

            case EditorActionMode.Resize:
                msg = "Resize: drag split handles between headers to adjust sizes.";
                break;

            case EditorActionMode.SelectObjects:
                msg = "Select: click a cell to select its RectTransform in the hierarchy for detailed editing.";
                break;

            case EditorActionMode.MoveCells:
                msg = "Move: click a source cell that contains foreign children or a nested table, then release over a destination cell to move them.";
                break;
        }
        EditorGUILayout.HelpBox(msg, MessageType.Info);
    }

    private bool GetCellAtMouseLikeHawk(Rect grid, float[] colW, float[] rowH, Vector2 mouse, out int r, out int c, out Rect cellRect)
    {
        r = -1; c = -1; cellRect = new Rect();
        if (!grid.Contains(mouse)) return false;

        float y = grid.y;
        for (int ri = 0; ri < table.totalRowsCountLetTheShowBegin; ri++)
        {
            float x = grid.x;
            for (int ci = 0; ci < table.totalColumnsCountHighFive; ci++)
            {
                float w = colW[ci];
                float h = rowH[ri];
                var rct = new Rect(x, y, w, h);
                if (rct.Contains(mouse))
                {
                    r = ri;
                    c = ci;
                    cellRect = rct;
                    return true;
                }
                x += w + table.sociallyDistancedColumnsPixels;
            }
            y += rowH[ri] + table.sociallyDistancedRowsPixels;
        }
        return false;
    }

    private void DrawMoveDragGhostIfAny(Rect grid, float[] colW, float[] rowH)
    {
        if (!moveDragActive) return;
        if (moveSrcRow < 0 || moveSrcCol < 0) return;

        var snackCounts = new Dictionary<System.Type, int>(8);
        bool hasChildTable = table.TryFindChildTableInCellLikeSherlock(moveSrcRow, moveSrcCol, out _);
        table.ScoutUiSnackCountsInCellLikeBeanCounter(moveSrcRow, moveSrcCol, snackCounts, true);
        BuildCellBadgeGuiLikeScoutPatches(snackCounts, hasChildTable, _badgesScratch);
        if (_badgesScratch.Count == 0) return;

        var e = Event.current;
        Vector2 pos = e.mousePosition;
        const float size = 16f;
        const float pad = 3f;
        float width = _badgesScratch.Count * (size + 1f) + pad * 2f;
        float height = size + pad * 2f;
        var box = new Rect(pos.x + 12f, pos.y + 12f, width, height);
        EditorGUI.DrawRect(box, new Color(0f, 0f, 0f, 0.15f));
        EditorGUI.DrawRect(new Rect(box.x, box.y, box.width, 1f), new Color(0f, 0f, 0f, 0.25f));
        EditorGUI.DrawRect(new Rect(box.x, box.yMax - 1f, box.width, 1f), new Color(0f, 0f, 0f, 0.25f));

        float x = box.x + pad;
        float y = box.y + pad;
        for (int i = 0; i < _badgesScratch.Count; i++)
        {
            var r = new Rect(x, y, size, size);
            var b = _badgesScratch[i];
            if (b.image != null) GUI.DrawTexture(r, b.image, ScaleMode.ScaleToFit, true);
            else GUI.Label(r, b, MiniBoldCenter);
            x += size + 1f;
        }
        _badgesScratch.Clear();
    }

    private void StartMoveDragLikeAUhaul(int r, int c)
    {
        moveSrcRow = r;
        moveSrcCol = c;
        moveDragActive = true;
    }

    private float ComputeNaturalGridWidthLikeTapeMeasure()
    {
        if (table == null) return 1f;
        var rt = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());
        int cols = Mathf.Max(1, table.totalColumnsCountHighFive);
        float innerW = Mathf.Max(1f, rt.rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);

        EnsureFloatBuffer(ref _previewColBuf, cols);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            cols,
            i => (i < table.fancyColumnWardrobes.Count) ? table.fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f,
            table.sociallyDistancedColumnsPixels,
            innerW,
            ref _previewColBuf
        );

        float natW = 0f;
        for (int c = 0; c < cols; c++) natW += _previewColBuf[c];
        if (cols > 1) natW += table.sociallyDistancedColumnsPixels * (cols - 1);
        return Mathf.Max(1f, natW);
    }

    private float ComputeNaturalGridHeightLikeTapeMeasure()
    {
        if (table == null) return 1f;
        var rt = _cachedTableRT != null ? _cachedTableRT : (_cachedTableRT = table.GetComponent<RectTransform>());
        int rows = Mathf.Max(1, table.totalRowsCountLetTheShowBegin);
        float innerH = Mathf.Max(1f, rt.rect.height - table.comfyPaddingTopHat - table.comfyPaddingBottomCushion);

        EnsureFloatBuffer(ref _previewRowBuf, rows);
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            rows,
            i => (i < table.snazzyRowWardrobes.Count) ? table.snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f,
            table.sociallyDistancedRowsPixels,
            innerH,
            ref _previewRowBuf
        );

        float natH = 0f;
        for (int r = 0; r < rows; r++) natH += _previewRowBuf[r];
        if (rows > 1) natH += table.sociallyDistancedRowsPixels * (rows - 1);
        return Mathf.Max(1f, natH);
    }

    private void ApplyFullAlignmentToSelectionLikeFittedSheet()
    {
        if (table == null) return;
        if (!HasSelection()) return;
        Undo.RecordObject(table, "Align Selection Full");
        int r0 = Mathf.Min(selRow, selRow2), r1 = Mathf.Max(selRow, selRow2);
        int c0 = Mathf.Min(selCol, selCol2), c1 = Mathf.Max(selCol, selCol2);
        for (int r = r0; r <= r1; r++)
            for (int c = c0; c <= c1; c++)
                table.AlignCellForeignsToFillLikeStuffedBurrito(r, c);
        EditorUtility.SetDirty(table);
    }
}
