#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// this class only exists to prevent you from adding table objects to other table objects,
/// the table system is managed, do not add tables to cells or rows,
/// and do not add rows to tables or cells or any other config like that,
/// just add gameobjects under cells only
/// </summary>
[InitializeOnLoad]
internal static class Ux_TonkersTableTopiaBouncerAtTheDoor
{
    private static readonly Type[] s_TableRoyalty = {
        typeof(Ux_TonkersTableTopiaLayout),
        typeof(Ux_TonkersTableTopiaRow),
        typeof(Ux_TonkersTableTopiaCell),
    };

    private static readonly HashSet<Type> s_WhitelistedPlusOnes = new HashSet<Type> {
        typeof(Transform),
        typeof(RectTransform),
        typeof(CanvasRenderer),
    };

    static Ux_TonkersTableTopiaBouncerAtTheDoor()
    {
        ObjectFactory.componentWasAdded -= OnComponentWasAddedPleaseAndThankYou;
        ObjectFactory.componentWasAdded += OnComponentWasAddedPleaseAndThankYou;
    }

    public static void OnComponentWasAddedPleaseAndThankYou(Component compyMcSneaky)
    {
        if (compyMcSneaky == null) return;
        var go = compyMcSneaky.gameObject;
        if (go == null) return;

        var row = go.GetComponent<Ux_TonkersTableTopiaRow>();
        var cell = go.GetComponent<Ux_TonkersTableTopiaCell>();
        var table = go.GetComponent<Ux_TonkersTableTopiaLayout>();

        int tttCount =
            (row != null ? 1 : 0) +
            (cell != null ? 1 : 0) +
            (table != null ? 1 : 0);

        if (tttCount <= 1) return;

        string other =
            (row != null && compyMcSneaky != row) ? nameof(Ux_TonkersTableTopiaRow) :
            (cell != null && compyMcSneaky != cell) ? nameof(Ux_TonkersTableTopiaCell) :
            nameof(Ux_TonkersTableTopiaLayout);

        string msg = $"Invalid operation, can't add {compyMcSneaky.GetType().Name} to {go.name} because {other} is already on this GameObject. A GameObject can only contain one TableTopia component.";
        RejectWithDialogAndYeetLikeBouncer(compyMcSneaky, msg);

        if (compyMcSneaky is Ux_TonkersTableTopiaLayout freshlyMintedTable)
        {
            Ux_TonkersTableTopiaExtensions.DeferEditorSafe(() =>
            {
                Ux_TonkersTableTopiaExtensions.TryVacuumTableScaffoldLikeRoomba(freshlyMintedTable);
            });
        }
    }

    private static void RejectWithDialogAndYeetLikeBouncer(Component partyCrasher, string message)
    {
        if (partyCrasher == null) return;
        Debug.LogError(message + "  You may have just broken the rows, double check the heirachy, i'm trying to prevent you from breaking it, so stop adding objecvts to table objects, this is a cell based system, gameobjects only belong as children of cells and THAT'S IT do not try to add components to tables/rows/cells.", partyCrasher);
        Ux_TonkersTableTopiaExtensions.DeferEditorSafe(() =>
        {
            if (partyCrasher is Ux_TonkersTableTopiaLayout ttt)
            {
                Ux_TonkersTableTopiaExtensions.TryVacuumTableScaffoldLikeRoomba(ttt);
            }
            UnityEditor.Undo.DestroyObjectImmediate(partyCrasher);
        });
    }
}

#endif
