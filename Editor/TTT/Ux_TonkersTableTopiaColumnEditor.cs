using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static Ux_TonkersTableTopiaEditorExtensions;
using static Ux_TonkersTableTopiaLayoutSizingExtensions;

[CustomEditor(typeof(Ux_TonkersTableTopiaColumn))]
public class Ux_TonkersTableTopiaColumnEditor : Editor
{
    private static readonly GUIContent GC_BackToTable = new GUIContent("Back to Tonkers Table Topia");
    private static readonly GUIContent GC_DeleteColumn = new GUIContent("Delete Column");

    private Ux_TonkersTableTopiaLayout _cachedTable;

    public override void OnInspectorGUI()
    {
        var column = (Ux_TonkersTableTopiaColumn)target;
        var table = _cachedTable != null ? _cachedTable : (_cachedTable = column ? column.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_BackToTable, GUILayout.Height(22)))
            {
                Selection.activeObject = table;
            }
            GUI.enabled = true;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUI.enabled = table != null;
            if (GUILayout.Button(GC_DeleteColumn, GUILayout.Height(22)))
            {
                int c = column.columnNumberPrimeRib;
                DeferToTableAndExit(table, () =>
                {
                    Undo.RecordObject(table, "Delete Column");
                    table.SafeDeleteColumnAtWithWittyConfirm(c);
                });
            }
            GUI.enabled = true;
        }

        if (table == null)
        {
            EditorGUILayout.HelpBox("No TonkersTableTopiaLayout found in parents.", MessageType.Info);
            return;
        }

        int cIdx = Mathf.Clamp(column.columnNumberPrimeRib, 0, table.totalColumnsCountHighFive - 1);
        if (table.fancyColumnWardrobes.Count != table.totalColumnsCountHighFive)
        {
            table.SyncColumnWardrobes();
        }

        var cs = table.fancyColumnWardrobes[cIdx];

        bool show = EditorPrefs.GetBool("TTT_Column_ShowStyle", true);
        show = EditorGUILayout.Foldout(show, $"Column {cIdx + 1}");
        EditorPrefs.SetBool("TTT_Column_ShowStyle", show);

        if (!show)
        {
            return;
        }

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Column {cIdx + 1}", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Align", GUILayout.Width(50));
                bool leftOn = table.IsColumnHorizAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                bool centerOn = table.IsColumnHorizAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                bool rightOn = table.IsColumnHorizAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                bool topOn = table.IsColumnVertAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                bool middleOn = table.IsColumnVertAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                bool bottomOn = table.IsColumnVertAlignedLikeMirror(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                bool fullOn = table.IsColumnFullLikeWaterfall(cIdx);
                float w7 = CalcShrinkyDinkWidthLikeDietCokeSquisher(7, 50f, 100f);

                EditorGUI.BeginDisabledGroup(leftOn);
                if (GUILayout.Button("Left", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Left");
                    table.AlignColumnHorizontalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Left);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(centerOn);
                if (GUILayout.Button("Center", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Center");
                    table.AlignColumnHorizontalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Center);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(rightOn);
                if (GUILayout.Button("Right", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Right");
                    table.AlignColumnHorizontalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.HorizontalAlignment.Right);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(topOn);
                if (GUILayout.Button("Top", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Top");
                    table.AlignColumnVerticalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Top);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(middleOn);
                if (GUILayout.Button("Middle", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Middle");
                    table.AlignColumnVerticalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Middle);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(bottomOn);
                if (GUILayout.Button("Bottom", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Bottom");
                    table.AlignColumnVerticalOnlyLikeLaserLevel(cIdx, Ux_TonkersTableTopiaLayout.VerticalAlignment.Bottom);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(fullOn);
                if (GUILayout.Button("Full", GUILayout.Width(w7)))
                {
                    Undo.RecordObject(table, "Align Column Full");
                    table.AlignColumnToFillLikeWaterfall(cIdx);
                    EditorUtility.SetDirty(table);
                    RequestWysiRepaintLikeFreshCoat();
                }
                EditorGUI.EndDisabledGroup();
            }

            bool useFixedWidth = cs.requestedWidthMaybePercentIfNegative > 0f;
            bool newUseFixedWidth = useFixedWidth;
            float newRequested = cs.requestedWidthMaybePercentIfNegative;
            Sprite newBackdrop = cs.backdropPictureOnTheHouse;
            Color newTint = cs.backdropTintFlavor;
            bool newSliced = cs.backdropUseSlicedLikePizza;
            bool newOneBig = cs.useOneBigBackdropForWholeColumn;
            bool newCustom = cs.customAnchorsAndPivotBecauseWeFancy;
            Vector2 newAnchorMin = cs.customAnchorMinPointy;
            Vector2 newAnchorMax = cs.customAnchorMaxPointy;
            Vector2 newPivot = cs.customPivotSpinny;
            bool colImgToggle = EditorPrefs.GetBool($"TTT_ColImg_{cIdx}", false);
            bool newColImgToggle = colImgToggle;

            EditorGUI.BeginChangeCheck();
            newUseFixedWidth = EditorGUILayout.Toggle("Use Fixed Width", useFixedWidth);
            if (newUseFixedWidth)
            {
                float currentInnerWidth = Mathf.Max(0f, table.GetComponent<RectTransform>().rect.width - table.comfyPaddingLeftForElbows - table.comfyPaddingRightForElbows);
                float px = useFixedWidth ? table.ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(cs.requestedWidthMaybePercentIfNegative, currentInnerWidth) : table.GetLiveColumnWidthPixelsLikeTapeMeasure(cIdx);
                px = EditorGUILayout.FloatField("Width (px)", px);
                newRequested = table.ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, px), currentInnerWidth);
            }
            else
            {
                float[] livePct = table.ComputeColumnPercentagesLikeASpreadsheet();
                float pct = cs.requestedWidthMaybePercentIfNegative < 0f ? -cs.requestedWidthMaybePercentIfNegative * 100f : (cIdx < livePct.Length ? livePct[cIdx] : 0f) * 100f;
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
                cs.requestedWidthMaybePercentIfNegative = newRequested;
                cs.backdropPictureOnTheHouse = newBackdrop;
                cs.backdropTintFlavor = newTint;
                cs.backdropUseSlicedLikePizza = newSliced;
                cs.useOneBigBackdropForWholeColumn = newOneBig;
                cs.customAnchorsAndPivotBecauseWeFancy = newCustom;
                cs.customAnchorMinPointy = newAnchorMin;
                cs.customAnchorMaxPointy = newAnchorMax;
                cs.customPivotSpinny = newPivot;
                table.shareThePieEvenlyForColumns = false;
                table.FlagLayoutAsNeedingSpaDay();
                EditorUtility.SetDirty(table);
            }
        }
    }

    private static void DeferToTableAndExit(Ux_TonkersTableTopiaLayout table, System.Action action)
    {
        if (table == null)
        {
            return;
        }

        Selection.activeObject = table;
        DeferEditorSafe(() =>
        {
            if (table == null)
            {
                return;
            }

            action?.Invoke();
            EditorGUIUtility.PingObject(table);
            RequestWysiRepaintLikeFreshCoat();
        });
        GUIUtility.ExitGUI();
    }

    private void OnEnable()
    {
        var column = (Ux_TonkersTableTopiaColumn)target;
        _cachedTable = column ? column.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;
    }
}