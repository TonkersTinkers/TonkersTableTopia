using UnityEngine;

public static class Ux_TonkersTableTopiaObjectUtility
{
    public static GameObject CreateUiObject(string name)
    {
        return new GameObject(string.IsNullOrEmpty(name) ? "UI" : name, typeof(RectTransform));
    }

    public static GameObject InstantiatePrefabOrCreate(GameObject prefab, string fallbackName)
    {
        if (prefab == null)
        {
            return CreateUiObject(fallbackName);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab)
                ? (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab)
                : Object.Instantiate(prefab);
        }
#endif

        return Object.Instantiate(prefab);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        if (go == null)
        {
            return null;
        }

        T existing = go.GetComponent<T>();
        if (existing != null)
        {
            return existing;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return UnityEditor.Undo.AddComponent<T>(go);
        }
#endif

        return go.AddComponent<T>();
    }

    public static void SetParent(Transform child, Transform parent, string undoName)
    {
        if (child == null || parent == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.SetTransformParent(child, parent, undoName);
            return;
        }
#endif

        child.SetParent(parent, false);
    }

    public static void RegisterCreatedObject(GameObject go, string undoName)
    {
        if (go == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, undoName);
        }
#endif
    }

    public static void DestroyObject(Object obj, string undoName)
    {
        if (obj == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.DestroyObjectImmediate(obj);
            return;
        }
#endif

        Object.Destroy(obj);
    }
}