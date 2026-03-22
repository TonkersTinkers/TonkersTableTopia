using UnityEngine;

internal static class Ux_TonkersTableTopiaHierarchyRules
{
    public static bool IsManagedLayout(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaLayout>() != null;
    }

    public static bool IsManagedRow(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaRow>() != null;
    }

    public static bool IsManagedCell(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaCell>() != null;
    }

    public static bool IsColumnBackdropHost(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaColumnBackdropHost>() != null;
    }

    public static bool IsColumnBackdrop(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>() != null;
    }

    public static bool IsManagedScaffold(Transform t)
    {
        return IsManagedLayout(t)
            || IsManagedRow(t)
            || IsManagedCell(t)
            || IsColumnBackdropHost(t)
            || IsColumnBackdrop(t);
    }

    public static bool IsForeignContent(Transform t)
    {
        return t != null && !IsManagedScaffold(t);
    }
}