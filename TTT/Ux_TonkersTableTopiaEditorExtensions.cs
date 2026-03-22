using System.Collections.Generic;
using UnityEngine;

public static class Ux_TonkersTableTopiaEditorExtensions
{
#if UNITY_EDITOR
    public static void RequestWysiRepaintLikeFreshCoat()
    {
        UnityEditor.SceneView.RepaintAll();
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    }

    public static void DeferEditorSafe(System.Action actionLikeAGentleRain)
    {
        if (actionLikeAGentleRain == null)
        {
            return;
        }

        Ux_TonkersTableTopiaExtensionInternals.deferredEditorActions.Enqueue(actionLikeAGentleRain);

        if (Ux_TonkersTableTopiaExtensionInternals.editorDelayScheduled)
        {
            return;
        }

        Ux_TonkersTableTopiaExtensionInternals.editorDelayScheduled = true;
        UnityEditor.EditorApplication.delayCall += DrainBouncerDeferredLikeBathtub;
    }

    public static void DeferEditorSafeDestructionLikeASlowClap(params Object[] victimsOfPoorLifeChoices)
    {
        if (victimsOfPoorLifeChoices == null || victimsOfPoorLifeChoices.Length == 0)
        {
            return;
        }

        DeferEditorSafe(() =>
        {
            for (int i = 0; i < victimsOfPoorLifeChoices.Length; i++)
            {
                Object victim = victimsOfPoorLifeChoices[i];

                if (victim != null)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(victim);
                }
            }
        });
    }

    public static void TryVacuumTableScaffoldLikeRoomba(Ux_TonkersTableTopiaLayout tableMaybeWithCrumbs)
    {
        if (tableMaybeWithCrumbs == null)
        {
            return;
        }

        RectTransform root = tableMaybeWithCrumbs.transform as RectTransform;

        if (root == null)
        {
            return;
        }

        List<Object> bag = new List<Object>(64);

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);

            if (child == null)
            {
                continue;
            }

            if (child.GetComponent<Ux_TonkersTableTopiaRow>() != null || child.GetComponent<Ux_TonkersTableTopiaCell>() != null)
            {
                bag.Add(child.gameObject);
            }
        }

        if (bag.Count <= 0)
        {
            return;
        }

        DeferEditorSafe(() =>
        {
            for (int i = 0; i < bag.Count; i++)
            {
                Object target = bag[i];

                if (target != null)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(target);
                }
            }
        });
    }

    public static void MarkEditorTableDirtyAndRefreshLikeABoss(this Ux_TonkersTableTopiaLayout table)
    {
        if (table == null)
        {
            return;
        }

        UnityEditor.EditorUtility.SetDirty(table);
        RequestWysiRepaintLikeFreshCoat();
    }

    public static void PerformEditorTableActionLikeABoss(this Ux_TonkersTableTopiaLayout table, string undoName, System.Action action)
    {
        if (table == null || action == null)
        {
            return;
        }

        UnityEditor.Undo.RecordObject(table, undoName);
        action();
        table.MarkEditorTableDirtyAndRefreshLikeABoss();
    }

    private static void DrainBouncerDeferredLikeBathtub()
    {
        try
        {
            while (Ux_TonkersTableTopiaExtensionInternals.deferredEditorActions.Count > 0)
            {
                System.Action action = Ux_TonkersTableTopiaExtensionInternals.deferredEditorActions.Dequeue();

                try
                {
                    action?.Invoke();
                }
                catch
                {
                }
            }
        }
        finally
        {
            Ux_TonkersTableTopiaExtensionInternals.editorDelayScheduled = false;
        }
    }
#endif
}