#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ux_TonkersTableTopiaRow))]
public class Ux_TonkersTableTopiaRowEditor : Editor
{
    // TonkersTableTopiaRowEditor
    public override void OnInspectorGUI()
    {
        var row = (Ux_TonkersTableTopiaRow)target;
        var table = row ? row.GetComponentInParent<Ux_TonkersTableTopiaLayout>(true) : null;

        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = table != null;
            if (GUILayout.Button("Back to Tonkers Table Topia", GUILayout.Height(22))) Selection.activeObject = table;
            GUI.enabled = true;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUI.enabled = table != null;
            if (GUILayout.Button("Delete Row", GUILayout.Height(22)))
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

            bool manualRows = EditorPrefs.GetBool("TTT_ManualRows", false);
            EditorGUI.BeginChangeCheck();
            manualRows = EditorGUILayout.Toggle("Manual Heights", manualRows);
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

            rs.backdropPictureOnTheHouse = (Sprite)EditorGUILayout.ObjectField("Background", rs.backdropPictureOnTheHouse, typeof(Sprite), false);
            rs.backdropTintFlavor = EditorGUILayout.ColorField("Color", rs.backdropTintFlavor);
            rs.customAnchorsAndPivotBecauseWeFancy = EditorGUILayout.Toggle("Override Anchors/Pivot", rs.customAnchorsAndPivotBecauseWeFancy);
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
}

#endif