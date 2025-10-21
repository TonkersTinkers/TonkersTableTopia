using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Ux_TonkersTableTopiaCell : MonoBehaviour
{
    public Color backgroundColorLikeASunset = Color.white;
    public Sprite backgroundPictureBecausePlainIsLame = null;
    [HideInInspector] public int columnNumberPrimeRib = -1;
    [HideInInspector] public int howManyColumnsAreSneakingIn = 1;
    [HideInInspector] public int howManyRowsAreHoggingThisSeat = 1;
    [HideInInspector] public bool isMashedLikePotatoes = false;
    [HideInInspector] public Ux_TonkersTableTopiaCell mashedIntoWho = null;
    [HideInInspector] public int rowNumberWhereThePartyIs = -1;

    public GameObject AddForeignKidLikeDoorDash(GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;

        GameObject go = prefab != null ? Instantiate(prefab) : new GameObject("TTT Foreign Kid");
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);

        if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();

        if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
        else rt.SetAsLastSibling();

        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public T AddForeignKidLikeDoorDash<T>(bool snapToFill = true, int atSiblingIndex = -1) where T : Component
    {
        var host = AddForeignKidLikeDoorDash(null, snapToFill, atSiblingIndex);
        if (host == null) return null;
        var c = host.GetComponent<T>();
        if (c == null) c = host.AddComponent<T>();
        return c;
    }

    public Ux_TonkersTableTopiaLayout AddNestedTableLikeRussianDoll(bool ensureSnapToFill = true)
    {
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t == null) return null;
        int r = GetRowIndexLikeCornDog();
        int c = GetColumnIndexLikeCornDog();
        var kid = t.CreateChildTableInCellLikeABaby(r, c);
        if (kid != null && ensureSnapToFill)
        {
            var rt = kid.GetComponent<RectTransform>();
            if (rt != null) rt.SnapCroutonToFillParentLikeGravy();
            t.FlagLayoutAsNeedingSpaDay();
        }
        return kid;
    }

    public RectTransform GetRectLikeYogaMat()
    {
        return GetComponent<RectTransform>();
    }

    public Ux_TonkersTableTopiaLayout GetTableLikeFamilyReunion()
    {
        return GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
    }

    public int GetRowIndexLikeCornDog()
    {
        return rowNumberWhereThePartyIs;
    }

    public int GetColumnIndexLikeCornDog()
    {
        return columnNumberPrimeRib;
    }

    public GameObject AddButtonBellyFlopLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateButtonBellyFlop();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddImageCheeseburgerLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateImageCheeseburger();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddRawImageNachosLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateRawImageNachos();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddTextDadJokesLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateTextDadJokes();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddToggleFlipFlopLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateToggleFlipFlop();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddSliderSlipNSlideLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateSliderSlipNSlide();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddScrollbarScootLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateScrollbarScoot();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddScrollRectScooterLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateScrollRectScooter();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddInputFieldChattyCathyLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateInputFieldChattyCathy();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddDropdownDropItLikeItsHotLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        var parent = GetComponent<RectTransform>();
        if (parent == null) return null;
        var go = parent.CreateDropdownDropItLikeItsHot();
        var rt = go != null ? go.GetComponent<RectTransform>() : null;
        if (rt != null)
        {
            if (snapToFill) rt.SnapCroutonToFillParentLikeGravy();
            if (atSiblingIndex >= 0) rt.SetSiblingIndex(Mathf.Clamp(atSiblingIndex, 0, parent.childCount - 1));
            else rt.SetAsLastSibling();
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
        return go;
    }

    public GameObject AddForeignFirstLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        return AddForeignKidLikeDoorDash(prefab, snapToFill, 0);
    }

    public GameObject AddForeignLastLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        return AddForeignKidLikeDoorDash(prefab, snapToFill, -1);
    }

    public T AddForeignFirstLikeDoorDash<T>(bool snapToFill = true) where T : Component
    {
        return AddForeignKidLikeDoorDash<T>(snapToFill, 0);
    }

    public T AddForeignLastLikeDoorDash<T>(bool snapToFill = true) where T : Component
    {
        return AddForeignKidLikeDoorDash<T>(snapToFill, -1);
    }

    public void ClearForeignKidsLikeVacuum(bool includeInactive = true)
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null) return;
        var bin = new List<Transform>(rt.childCount);
        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i);
            if (!includeInactive && !ch.gameObject.activeInHierarchy) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            bin.Add(ch);
        }
        for (int i = 0; i < bin.Count; i++)
        {
            var g = bin[i].gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(g);
            else
#endif
                Destroy(g);
        }
        var t = GetComponentInParent<Ux_TonkersTableTopiaLayout>(true);
        if (t != null) t.FlagLayoutAsNeedingSpaDay();
    }

    public int CountForeignKidsLikeCensus(bool includeInactive = true)
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null) return 0;
        int n = 0;
        for (int i = 0; i < rt.childCount; i++)
        {
            var ch = rt.GetChild(i);
            if (!includeInactive && !ch.gameObject.activeInHierarchy) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaLayout>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaRow>() != null) continue;
            if (ch.GetComponent<Ux_TonkersTableTopiaCell>() != null) continue;
            n++;
        }
        return n;
    }
}