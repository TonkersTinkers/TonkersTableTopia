using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class Ux_TonkersTableTopiaCell : Ux_TonkersTableTopiaNodeBase
{
    public Color backgroundColorLikeASunset = Color.white;
    public Sprite backgroundPictureBecausePlainIsLame = null;
    public bool backgroundPictureUseSlicedLikePizza = true;
    [HideInInspector] public int columnNumberPrimeRib = -1;
    [HideInInspector] public int howManyColumnsAreSneakingIn = 1;
    [HideInInspector] public int howManyRowsAreHoggingThisSeat = 1;
    [HideInInspector] public bool isMashedLikePotatoes = false;
    [HideInInspector] public Ux_TonkersTableTopiaCell mashedIntoWho = null;
    [HideInInspector] public int rowNumberWhereThePartyIs = -1;

    public bool useInnerPaddingPillowFort = false;
    public float innerPaddingLeftMarshmallow = 0f;
    public float innerPaddingRightMarshmallow = 0f;
    public float innerPaddingTopMarshmallow = 0f;
    public float innerPaddingBottomMarshmallow = 0f;

    private ContentSizeFitter _cachedContentSizeFitter;
    private readonly List<Transform> _foreignScratch = new List<Transform>(16);

    public RectTransform RectTransformComponent => GetCachedRectTransform();
    public Ux_TonkersTableTopiaLayout Table => GetCachedTable();
    public int RowIndex => rowNumberWhereThePartyIs;
    public int ColumnIndex => columnNumberPrimeRib;

    public void EnsureCachedContentSizeFitter(bool needIt)
    {
        RectTransform rect = GetCachedRectTransform();
        if (rect == null)
        {
            return;
        }

        if (_cachedContentSizeFitter == null)
        {
            rect.TryGetComponent(out _cachedContentSizeFitter);
        }

        if (!needIt)
        {
            if (_cachedContentSizeFitter != null)
            {
                _cachedContentSizeFitter.enabled = false;
            }

            return;
        }

        if (_cachedContentSizeFitter == null)
        {
            _cachedContentSizeFitter = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<ContentSizeFitter>(rect.gameObject);
        }

        if (_cachedContentSizeFitter == null)
        {
            return;
        }

        _cachedContentSizeFitter.enabled = true;
        _cachedContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        _cachedContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public GameObject AddContent(GameObject prefab, bool stretchToFill = true, int siblingIndex = -1)
    {
        return CreateAndAttachContent(
            () => Ux_TonkersTableTopiaObjectUtility.InstantiatePrefabOrCreate(prefab, "Content"),
            stretchToFill,
            siblingIndex,
            true);
    }

    public T AddContent<T>(bool stretchToFill = true, int siblingIndex = -1) where T : Component
    {
        GameObject content = CreateAndAttachContent(
            () =>
            {
                GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(typeof(T).Name);
                return Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<T>(go) != null ? go : null;
            },
            stretchToFill,
            siblingIndex,
            true);

        return content != null ? content.GetComponent<T>() : null;
    }

    public GameObject AddContentFirst(GameObject prefab, bool stretchToFill = true)
    {
        return AddContent(prefab, stretchToFill, 0);
    }

    public GameObject AddContentLast(GameObject prefab, bool stretchToFill = true)
    {
        return AddContent(prefab, stretchToFill, -1);
    }

    public T AddContentFirst<T>(bool stretchToFill = true) where T : Component
    {
        return AddContent<T>(stretchToFill, 0);
    }

    public T AddContentLast<T>(bool stretchToFill = true) where T : Component
    {
        return AddContent<T>(stretchToFill, -1);
    }

    public GameObject AddStandardContent(Ux_TonkersTableTopiaStandardContentType contentType, bool stretchToFill = true, int siblingIndex = -1)
    {
        RectTransform parent = GetCachedRectTransform();
        if (parent == null)
            return null;

        return CreateAndAttachContent(
            () => Ux_TonkersTableTopiaContentFactory.Create(parent, contentType),
            stretchToFill,
            siblingIndex,
            false);
    }

    private GameObject CreateAndAttachContent(Func<GameObject> factory, bool stretchToFill, int siblingIndex, bool registerUndo)
    {
        GameObject content = factory?.Invoke();
        if (content == null)
            return null;

        if (!AttachContent(content, stretchToFill, siblingIndex))
            return null;

        if (registerUndo)
            Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(content, "Add Cell Content");

        NotifyTableLayoutChanged();
        return content;
    }

    public Ux_TonkersTableTopiaLayout AddNestedTable(bool ensureSnapToFill = true)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null)
        {
            return null;
        }

        Ux_TonkersTableTopiaLayout child = table.CreateChildTableInCellLikeABaby(RowIndex, ColumnIndex);
        if (child == null || !ensureSnapToFill)
        {
            return child;
        }

        RectTransform childRect = child.GetComponent<RectTransform>();
        if (childRect != null)
        {
            childRect.SnapCroutonToFillParentLikeGravy();
        }

        table.FlagLayoutAsNeedingSpaDay();
        return child;
    }

    public void ClearHostedContent(bool includeInactive = true)
    {
        if (GetCachedRectTransform() == null)
            return;

        CollectHostedContent(includeInactive);

        for (int i = 0; i < _foreignScratch.Count; i++)
            Ux_TonkersTableTopiaObjectUtility.DestroyObject(_foreignScratch[i].gameObject, "Clear Cell Content");

        NotifyTableLayoutChanged();
    }

    public int CountHostedContent(bool includeInactive = true)
    {
        if (GetCachedRectTransform() == null)
            return 0;

        CollectHostedContent(includeInactive);
        return _foreignScratch.Count;
    }

    private void CollectHostedContent(bool includeInactive)
    {
        RectTransform rect = GetCachedRectTransform();
        _foreignScratch.Clear();

        if (rect == null)
            return;

        for (int i = 0; i < rect.childCount; i++)
        {
            Transform child = rect.GetChild(i);

            if (!includeInactive && !child.gameObject.activeInHierarchy)
                continue;

            if (!Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(child))
                continue;

            _foreignScratch.Add(child);
        }
    }

    private bool AttachContent(GameObject content, bool stretchToFill, int siblingIndex)
    {
        if (content == null)
        {
            return false;
        }

        RectTransform parent = GetCachedRectTransform();
        if (parent == null)
        {
            return false;
        }

        RectTransform childRect = content.GetComponent<RectTransform>();
        if (childRect == null)
        {
            childRect = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<RectTransform>(content);
        }

        if (childRect == null)
        {
            return false;
        }

        if (childRect.parent != parent)
        {
            Ux_TonkersTableTopiaObjectUtility.SetParent(childRect, parent, "Add Cell Content");
        }

        if (stretchToFill)
        {
            childRect.SnapCroutonToFillParentLikeGravy();
        }

        ApplySiblingIndex(childRect, siblingIndex, parent);
        return true;
    }

    private static void ApplySiblingIndex(RectTransform childRect, int siblingIndex, RectTransform parent)
    {
        if (childRect == null || parent == null)
        {
            return;
        }

        if (siblingIndex >= 0)
        {
            childRect.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, parent.childCount - 1));
            return;
        }

        childRect.SetAsLastSibling();
    }

    private void NotifyTableLayoutChanged()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table != null)
        {
            table.FlagLayoutAsNeedingSpaDay();
        }
    }

    [Obsolete("Use AddNestedTable instead.")]
    public Ux_TonkersTableTopiaLayout AddNestedTableLikeRussianDoll(bool ensureSnapToFill = true)
    {
        return AddNestedTable(ensureSnapToFill);
    }

    [Obsolete("Use RectTransformComponent instead.")]
    public RectTransform GetRectLikeYogaMat()
    {
        return RectTransformComponent;
    }

    [Obsolete("Use Table instead.")]
    public Ux_TonkersTableTopiaLayout GetTableLikeFamilyReunion()
    {
        return Table;
    }

    [Obsolete("Use RowIndex instead.")]
    public int GetRowIndexLikeCornDog()
    {
        return RowIndex;
    }

    [Obsolete("Use ColumnIndex instead.")]
    public int GetColumnIndexLikeCornDog()
    {
        return ColumnIndex;
    }

    [Obsolete("Use AddStandardContent(Button) instead.")]
    public GameObject AddButtonBellyFlopLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Button, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Image) instead.")]
    public GameObject AddImageCheeseburgerLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Image, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(RawImage) instead.")]
    public GameObject AddRawImageNachosLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.RawImage, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Text) instead.")]
    public GameObject AddTextDadJokesLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Text, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Toggle) instead.")]
    public GameObject AddToggleFlipFlopLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Toggle, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Slider) instead.")]
    public GameObject AddSliderSlipNSlideLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Slider, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Scrollbar) instead.")]
    public GameObject AddScrollbarScootLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Scrollbar, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(ScrollRect) instead.")]
    public GameObject AddScrollRectScooterLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.ScrollRect, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(InputField) instead.")]
    public GameObject AddInputFieldChattyCathyLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.InputField, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddStandardContent(Dropdown) instead.")]
    public GameObject AddDropdownDropItLikeItsHotLikeEasyButton(bool snapToFill = true, int atSiblingIndex = -1)
    {
        return AddStandardContent(Ux_TonkersTableTopiaStandardContentType.Dropdown, snapToFill, atSiblingIndex);
    }

    [Obsolete("Use AddContentLast instead.")]
    public GameObject AddForeignLastLikeDoorDash(GameObject prefab, bool snapToFill = true)
    {
        return AddContentLast(prefab, snapToFill);
    }

    [Obsolete("Use AddContentLast<T> instead.")]
    public T AddForeignLastLikeDoorDash<T>(bool snapToFill = true) where T : Component
    {
        return AddContentLast<T>(snapToFill);
    }

    public void SetRowFixedHeightPixelsLikeTapeMeasure(float heightPixels)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowFixedHeightPixelsLikeTapeMeasure(RowIndex, heightPixels);
    }

    public void SetRowPercentageHeightLikeASpreadsheet(float percentage01)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowPercentageLikeASpreadsheet(RowIndex, percentage01);
    }

    public void SetRowFlexibleHeightLikeYogaPants()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return;
        }

        table.SetRowFlexibleLikeYogaPants(RowIndex);
    }

    public void SetColumnFixedWidthPixelsLikeTapeMeasure(float widthPixels)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnFixedWidthPixelsLikeTapeMeasure(ColumnIndex, widthPixels);
    }

    public void SetColumnPercentageWidthLikeASpreadsheet(float percentage01)
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnPercentageLikeASpreadsheet(ColumnIndex, percentage01);
    }

    public void SetColumnFlexibleWidthLikeYogaPants()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return;
        }

        table.SetColumnFlexibleLikeYogaPants(ColumnIndex);
    }

    public float GetLiveRowHeightPixelsLikeTapeMeasure()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return 0f;
        }

        return table.GetLiveRowHeightPixelsLikeTapeMeasure(RowIndex);
    }

    public float GetLiveColumnWidthPixelsLikeTapeMeasure()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return 0f;
        }

        return table.GetLiveColumnWidthPixelsLikeTapeMeasure(ColumnIndex);
    }

    public float GetStoredRowPercentageHeightLikeASpreadsheet()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || RowIndex < 0)
        {
            return 0f;
        }

        return table.GetStoredRowPercentageLikeASpreadsheet(RowIndex);
    }

    public float GetStoredColumnPercentageWidthLikeASpreadsheet()
    {
        Ux_TonkersTableTopiaLayout table = GetCachedTable();
        if (table == null || ColumnIndex < 0)
        {
            return 0f;
        }

        return table.GetStoredColumnPercentageLikeASpreadsheet(ColumnIndex);
    }
}