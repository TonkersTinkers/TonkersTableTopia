using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;
using UnityEngine.UI;
using static Ux_TonkersTableTopiaLayoutSizingExtensions;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Layout/Tonkers Table Topia")]
[DisallowMultipleComponent]
public class Ux_TonkersTableTopiaLayout : MonoBehaviour
{
    [Tooltip("Scales positive fixed row and column pixel values against this table's saved design size.")]
    public bool scaleFixedSizesWithResolutionLikeBlueprint = true;

    [Tooltip("Saved authoring size for this table. This is table-local, not necessarily the full screen resolution.")]
    public Vector2 designSizeForThisTableLikeBlueprint = Vector2.zero;

    [Tooltip("How many rows are strutting on stage")]
    [Min(1)]
    public int totalRowsCountLetTheShowBegin = 1;

    [Tooltip("How many columns are bringing snacks")]
    [Min(1)]
    public int totalColumnsCountHighFive = 1;

    public float comfyPaddingLeftForElbows = 0f;
    public float comfyPaddingRightForElbows = 0f;
    public float comfyPaddingTopHat = 0f;
    public float comfyPaddingBottomCushion = 0f;
    public bool toggleZebraStripesForRows = true;
    public bool toggleZebraStripesForColumns = false;
    public Color zebraRowColorA = Color.white;
    public Color zebraRowColorB = new Color(0.96f, 0.96f, 0.96f, 1f);
    public Color zebraColumnColorA = Color.white;
    public Color zebraColumnColorB = new Color(0.96f, 0.96f, 0.96f, 1f);

    [Tooltip("Horizontal space in pixels so columns do not talk too close")]
    public float sociallyDistancedColumnsPixels = 0f;

    [Tooltip("Vertical space in pixels to keep rows from stepping on toes")]
    public float sociallyDistancedRowsPixels = 0f;

    [Tooltip("If true, all columns split the pie evenly")]
    public bool shareThePieEvenlyForColumns = false;

    [Tooltip("If true, all rows split the pie evenly")]
    public bool shareThePieEvenlyForRows = false;

    [Tooltip("Auto-resize width to hug content like a good friend")]
    public bool autoHugWidthLikeAGoodFriend = true;

    [Tooltip("Auto-resize height because why not")]
    public bool autoHugHeightBecauseWhyNot = true;

    [Tooltip("When inserting or deleting rows, keep the other rows at their current resolved heights by converting them to fixed pixel values.")]
    public bool preserveExistingRowHeightsWhenAddingOrDeleting = false;

    [Tooltip("When inserting or deleting columns, keep the other columns at their current resolved widths by converting them to fixed pixel values.")]
    public bool preserveExistingColumnWidthsWhenAddingOrDeleting = false;

    public enum HorizontalAlignment
    { Left, Center, Right }

    public enum VerticalAlignment
    { Top, Middle, Bottom }

    [Tooltip("Horizontal schmoozing preference when there is extra width")]
    public HorizontalAlignment horizontalSchmoozingPreference = HorizontalAlignment.Left;

    [Tooltip("Vertical schmoozing preference when there is extra height")]
    public VerticalAlignment verticalSchmoozingPreference = VerticalAlignment.Top;

    [Tooltip("Auto-add a ContentSizeFitter to each cell so it dresses to its content")]
    public bool autoHireContentSizerBecauseLazy = false;

    private static readonly Vector2 s_DefaultDesignSizeLikeBlueprint = new Vector2(1920f, 1080f);
    private static readonly Color s_DefaultZebraRowColorA = new Color(0.38f, 0.23f, 0.23f, 1f);
    private static readonly Color s_DefaultZebraRowColorB = new Color(0.24f, 0.25f, 0.34f, 1f);
    private static readonly Color s_DefaultZebraColumnColorA = new Color(0.31f, 0.31f, 0.31f, 1f);
    private static readonly Color s_DefaultZebraColumnColorB = new Color(0.16f, 0.16f, 0.16f, 1f);

    private float[] _originalColumnSpecsScratch = Array.Empty<float>();
    private float[] _originalRowSpecsScratch = Array.Empty<float>();
    private bool[] _wasFixedColumnScratch = Array.Empty<bool>();
    private bool[] _wasFlexColumnScratch = Array.Empty<bool>();
    private bool[] _wasFixedRowScratch = Array.Empty<bool>();
    private bool[] _wasFlexRowScratch = Array.Empty<bool>();

#if UNITY_EDITOR

    // upgrade for prefabs due to new column objects
    [SerializeField, HideInInspector] private int _editorUpgradeVersion;

    private const int CurrentEditorUpgradeVersion = 1;
    private static readonly HashSet<string> _loggedOutdatedPrefabPaths = new HashSet<string>();
#endif

    [Serializable]
    public class ColumnStyle
    {
        public float requestedWidthMaybePercentIfNegative = 0f;
        public Sprite backdropPictureOnTheHouse = null;
        public Color backdropTintFlavor = Color.white;
        public bool customAnchorsAndPivotBecauseWeFancy = false;
        public Vector2 customAnchorMinPointy = new Vector2(0f, 1f);
        public Vector2 customAnchorMaxPointy = new Vector2(0f, 1f);
        public Vector2 customPivotSpinny = new Vector2(0f, 1f);
        public bool useOneBigBackdropForWholeColumn = true;
        public bool backdropUseSlicedLikePizza = true;
    }

    [Serializable]
    public class RowStyle
    {
        public float requestedHeightMaybePercentIfNegative = 0f;
        public Sprite backdropPictureOnTheHouse = null;
        public Color backdropTintFlavor = Color.white;
        public bool backdropUseSlicedLikePizza = true;
        public bool customAnchorsAndPivotBecauseWeFancy = false;
        public Vector2 customAnchorMinPointy = new Vector2(0f, 1f);
        public Vector2 customAnchorMaxPointy = new Vector2(0f, 1f);
        public Vector2 customPivotSpinny = new Vector2(0f, 1f);

        [Tooltip("If true, the last visible cell eats leftovers so the row looks full")]
        public bool lastVisibleCellEatsLeftovers = true;
    }

    public List<ColumnStyle> fancyColumnWardrobes = new List<ColumnStyle>();
    public List<RowStyle> snazzyRowWardrobes = new List<RowStyle>();
    private RectTransform _columnBackdropHostRT;
    private readonly List<RectTransform> _columnBackdropScratch = new List<RectTransform>(32);

    private List<RectTransform> backstageRowRectsVIP = new List<RectTransform>();
    private List<List<RectTransform>> backstageCellRectsVIP = new List<List<RectTransform>>();

    private bool layoutNeedsAFreshCoatOfPaint = true;

#if UNITY_EDITOR
    private bool _insideValidationPass;
#endif

    private RectTransform _rt;
    private readonly List<RectTransform> _rootChildrenRoundup = new List<RectTransform>(64);
    private readonly List<RectTransform> _managedRowsLine = new List<RectTransform>(64);
    private readonly List<RectTransform> _nonManagedRootBackpack = new List<RectTransform>(32);
    private readonly List<RectTransform> _rowChildrenRoundup = new List<RectTransform>(64);
    private readonly List<RectTransform> _managedCellsLine = new List<RectTransform>(64);
    private readonly List<RectTransform> _nonManagedRowBackpack = new List<RectTransform>(32);
    private readonly List<RectTransform> _thisRowCellsRects = new List<RectTransform>(64);
    private readonly List<Transform> _stowawaysOnThisCrazyTrain = new List<Transform>(64);
    private float[] _rowHeightsBuf = Array.Empty<float>();
    private float[] _colWidthsBuf = Array.Empty<float>();

    private static readonly Comparison<RectTransform> _siblingIndexComparison = (left, right) => left.GetSiblingIndex().CompareTo(right.GetSiblingIndex());

    private Ux_TablePool _pool;
    private static readonly List<Ux_TonkersTableTopiaLayout> _runtimeDirtyLayouts = new List<Ux_TonkersTableTopiaLayout>(64);
    private static bool _runtimeDirtyLayoutsHooked;
    private readonly List<Ux_TonkersTableTopiaRow> _backstageRowCompsVIP = new List<Ux_TonkersTableTopiaRow>(64);
    private readonly List<List<Ux_TonkersTableTopiaCell>> _backstageCellCompsVIP = new List<List<Ux_TonkersTableTopiaCell>>(64);
    private bool _structureDirty = true;
    private bool _runtimeLayoutQueued;
    private Canvas _cachedCanvas;
    private Vector2 _lastAppliedRectSize = new Vector2(-1f, -1f);
    private Vector2 _lastAppliedCanvasScale = new Vector2(-1f, -1f);
    private readonly List<RectTransform> _backstageColumnRectsVIP = new List<RectTransform>(64);
    private readonly List<Ux_TonkersTableTopiaColumn> _backstageColumnCompsVIP = new List<Ux_TonkersTableTopiaColumn>(64);

    private RectTransform RentRowRT()
    {
        InitializeCachedDependencies();
        return _pool?.RentRow();
    }

    private RectTransform RentCellRT()
    {
        InitializeCachedDependencies();
        return _pool?.RentCell();
    }

    private void ReturnRowRT(RectTransform rt)
    {
        InitializeCachedDependencies();
        _pool?.ReturnRow(rt);
    }

    private void ReturnCellRT(RectTransform rt)
    {
        InitializeCachedDependencies();
        _pool?.ReturnCell(rt);
    }

    private void Awake()
    {
        InitializeCachedDependencies();
        EnsureDesignSizeInitialized();
        MarkStructureDirty();
    }

    private void Reset()
    {
        InitializeCachedDependencies();
        ApplyDefaultTonkersTablePresetLikeReferenceSpec();
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnEnable()
    {
        InitializeCachedDependencies();
        MarkStructureDirty();
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnDisable()
    {
        _runtimeLayoutQueued = false;
    }

    private void OnTransformParentChanged()
    {
        InitializeCachedDependencies();
        MarkStructureDirty();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnTransformChildrenChanged()
    {
        MarkStructureDirty();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnRectTransformDimensionsChange()
    {
        MarkLayoutDirty();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ScheduleEditorLayoutTouchUp();
            return;
        }
#endif

        QueueRuntimeLayout();
    }

    public void ConvertAllSpecsToPercentages()
    {
        EnsureWardrobeListsMatchHeadcount();

        RectTransform rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        if (rt == null)
            return;

        float innerWidth = Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerHeight = Mathf.Max(0f, rt.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);

        int columnCount = Mathf.Max(1, totalColumnsCountHighFive);
        int rowCount = Mathf.Max(1, totalRowsCountLetTheShowBegin);

        EnsurePercentConversionScratchCapacityLikeReceipt(columnCount, rowCount);

        CacheColumnSpecState(columnCount);
        CacheRowSpecState(rowCount);

        DistributeLikeACatererInto(
            columnCount,
            i => ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(_originalColumnSpecsScratch[i], innerWidth),
            sociallyDistancedColumnsPixels,
            innerWidth,
            ref _colWidthsBuf);

        DistributeLikeACatererInto(
            rowCount,
            i => ResolveRowSpecForCurrentInnerHeightLikeBlueprint(_originalRowSpecsScratch[i], innerHeight),
            sociallyDistancedRowsPixels,
            innerHeight,
            ref _rowHeightsBuf);

        ApplyConvertedColumnSpecs(columnCount, innerWidth);
        ApplyConvertedRowSpecs(rowCount, innerHeight);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _insideValidationPass = true;
#endif

        InitializeCachedDependencies();
        EnsureWardrobeListsMatchHeadcount();
        MarkStructureDirty();
        FlagLayoutAsNeedingSpaDay();

#if UNITY_EDITOR
        if (EditorNeedsUpgrade())
        {
            ReportOutdatedPrefabUpgradeRequirement();
        }
        else
        {
            ClearOutdatedPrefabLogState();
        }

        _insideValidationPass = false;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        ApplyEditorLayoutIfNeeded();
#endif
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying || _rt == null)
        {
            return;
        }

        if (HasRuntimeLayoutChange())
        {
            MarkLayoutDirty();
        }

        if (_structureDirty || layoutNeedsAFreshCoatOfPaint)
        {
            QueueRuntimeLayout();
        }
    }

    public void FlagLayoutAsNeedingSpaDay()
    {
        MarkLayoutDirty();
        RequestLayoutRefresh();
    }

#if UNITY_EDITOR
    private static readonly HashSet<int> _scheduledEditorUpdates = new HashSet<int>();

    private void ScheduleEditorLayoutTouchUp()
    {
        int instanceIdentificationBadge = GetInstanceID();
        if (_scheduledEditorUpdates.Contains(instanceIdentificationBadge)) return;
        _scheduledEditorUpdates.Add(instanceIdentificationBadge);
        EditorApplication.delayCall += () =>
        {
            if (this == null)
            {
                _scheduledEditorUpdates.Remove(instanceIdentificationBadge);
                return;
            }
            if (!Application.isPlaying && layoutNeedsAFreshCoatOfPaint)
            {
                UpdateSeatingLikeAProUsher();
                layoutNeedsAFreshCoatOfPaint = false;
            }
            _scheduledEditorUpdates.Remove(instanceIdentificationBadge);
        };
    }

#endif

    private void EnsureWardrobeListsMatchHeadcount()
    {
        SyncColumnWardrobes();
        SyncRowWardrobes();
    }

    public void RebuildComedyClubSeatingChart()
    {
#if UNITY_EDITOR
        if (IsDirectPrefabAssetLikeMuseumPiece())
        {
            _structureDirty = false;
            return;
        }
#endif

        EnsureWardrobeListsMatchHeadcount();

        RectTransform tableRectTransformMainStage = _rt;
        if (tableRectTransformMainStage == null)
        {
            _structureDirty = false;
            return;
        }

        PrepareStructureCachesForRebuild();
        CollectRootChildren(tableRectTransformMainStage);
        SplitRootChildrenIntoManagedAndForeign();
        EnsureManagedRowsExist(tableRectTransformMainStage);

        for (int rowIndex = 0; rowIndex < totalRowsCountLetTheShowBegin; rowIndex++)
        {
            RebuildManagedRow(rowIndex);
        }

        TrimExtraManagedRows();
        RebuildManagedColumns();
        MoveRootForeignChildrenToFirstCell(tableRectTransformMainStage);
        _structureDirty = false;
    }

    private void ParkRowInItsAssignedSpot(int rowNumberIndex, float rowHeightThisRound, RectTransform tableRect, float innerWidthPlayable, float offsetXLeft, float yFromTop, float totalW, float totalH)
    {
        var rowRect = backstageRowRectsVIP[rowNumberIndex];
        Vector2 aMin, aMax, piv;
        var rowStyle = (rowNumberIndex < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowNumberIndex] : null;
        if (rowStyle != null && rowStyle.customAnchorsAndPivotBecauseWeFancy)
        {
            aMin = rowStyle.customAnchorMinPointy;
            aMax = rowStyle.customAnchorMaxPointy;
            piv = rowStyle.customPivotSpinny;
        }
        else
        {
            aMin = new Vector2(0f, 1f);
            aMax = new Vector2(0f, 1f);
            piv = new Vector2(0f, 1f);
        }
        ApplyRectPlacementWithPanache(rowRect, aMin, aMax, piv, offsetXLeft, yFromTop, innerWidthPlayable, rowHeightThisRound, totalW, totalH);
        DressRowInItsSundayBest(rowNumberIndex, rowRect);
    }

    private void DressRowInItsSundayBest(int rowNumberIndex, RectTransform rowRect)
    {
        if (rowRect == null)
        {
            return;
        }

        Ux_TonkersTableTopiaRow rowComponent = rowNumberIndex >= 0 && rowNumberIndex < _backstageRowCompsVIP.Count
            ? _backstageRowCompsVIP[rowNumberIndex]
            : null;

        if (rowComponent == null)
        {
            rowComponent = rowRect.GetComponent<Ux_TonkersTableTopiaRow>();
        }

        RowStyle rowStyle = rowNumberIndex < snazzyRowWardrobes.Count ? snazzyRowWardrobes[rowNumberIndex] : null;
        bool needImage = rowStyle != null && rowStyle.backdropPictureOnTheHouse != null;

        if (rowComponent == null)
        {
            var img = rowRect.FlipImageComponentLikeALightSwitch(needImage);
            if (img != null)
            {
                img.sprite = rowStyle.backdropPictureOnTheHouse;
                img.color = rowStyle.backdropTintFlavor;
                img.type = rowStyle.backdropUseSlicedLikePizza ? Image.Type.Sliced : Image.Type.Simple;
                img.raycastTarget = false;
            }

            return;
        }

        rowComponent.SetCachedBackgroundVisual(
            needImage,
            needImage ? rowStyle.backdropPictureOnTheHouse : null,
            needImage ? rowStyle.backdropTintFlavor : Color.white,
            needImage && rowStyle.backdropUseSlicedLikePizza);
    }

    private float CalculateCellHeightForTheStretch(int startRow, int rowSpan, float[] rowHeightsBuffet)
    {
        if (rowSpan <= 1) return rowHeightsBuffet[startRow];
        float totalHeight = 0f;
        int endRowExclusive = Mathf.Min(totalRowsCountLetTheShowBegin, startRow + rowSpan);
        for (int rowWalker = startRow; rowWalker < endRowExclusive; rowWalker++)
        {
            totalHeight += rowHeightsBuffet[rowWalker];
            if (rowWalker < endRowExclusive - 1) totalHeight += sociallyDistancedRowsPixels;
        }
        return totalHeight;
    }

    public void UpdateSeatingLikeAProUsher()
    {
#if UNITY_EDITOR
        if (IsDirectPrefabAssetLikeMuseumPiece())
        {
            layoutNeedsAFreshCoatOfPaint = false;
            _structureDirty = false;
            return;
        }
#endif

        RectTransform tableRectTransformMainStage = _rt;
        if (tableRectTransformMainStage == null)
        {
            return;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        if (totalColumnsCountHighFive < 1 || totalRowsCountLetTheShowBegin < 1)
        {
            return;
        }

        float[] calculatedRowHeightsBuffet = CalculateRowHeightsLikeAShortOrderCook(tableRectTransformMainStage);
        float totalContentHeightCalories = CalculateTotalContentHeightCalories(calculatedRowHeightsBuffet);

        if (autoHugHeightBecauseWhyNot && !AreWeStretchingVertically(tableRectTransformMainStage))
        {
            tableRectTransformMainStage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentHeightCalories);
        }

        float innerWidthPlayable = Mathf.Max(0f, tableRectTransformMainStage.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerHeightPlayable = Mathf.Max(0f, tableRectTransformMainStage.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
        float[] baseColumnWidths = CalculateBaseColumnWidths(innerWidthPlayable);
        float totalInnerContentWidth = CalculateInnerContentWidthLikeSnackLine(baseColumnWidths);
        float totalContentWidthCalories = CalculateTotalContentWidthCalories(baseColumnWidths);

        if (autoHugWidthLikeAGoodFriend && !AreWeStretchingHorizontally(tableRectTransformMainStage))
        {
            tableRectTransformMainStage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalContentWidthCalories);
            innerWidthPlayable = Mathf.Max(0f, tableRectTransformMainStage.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
            innerHeightPlayable = Mathf.Max(0f, tableRectTransformMainStage.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
            baseColumnWidths = CalculateBaseColumnWidths(innerWidthPlayable);
            totalInnerContentWidth = CalculateInnerContentWidthLikeSnackLine(baseColumnWidths);
            totalContentWidthCalories = CalculateTotalContentWidthCalories(baseColumnWidths);
        }

        RefreshColumnBackdropLayerLikeWallpaperCrew(baseColumnWidths, innerHeightPlayable);

        float leftOffsetStart = comfyPaddingLeftForElbows;
        float rowLeadOffset = CalculateHorizontalGrandEntrance(innerWidthPlayable, totalInnerContentWidth);
        float rowLeftoverWidth = Mathf.Max(0f, innerWidthPlayable - totalInnerContentWidth);
        float currentYTopDown = CalculateVerticalGrandEntrance(tableRectTransformMainStage, totalContentHeightCalories) - comfyPaddingTopHat;

        for (int rowStepper = 0; rowStepper < totalRowsCountLetTheShowBegin; rowStepper++)
        {
            float rowHeightThisRound = calculatedRowHeightsBuffet[rowStepper];
            RectTransform rowRT = backstageRowRectsVIP[rowStepper];
            Vector2 oldRowSizeJeans = rowRT != null ? rowRT.rect.size : Vector2.zero;

            ParkRowInItsAssignedSpot(
                rowStepper,
                rowHeightThisRound,
                tableRectTransformMainStage,
                innerWidthPlayable,
                leftOffsetStart,
                currentYTopDown,
                tableRectTransformMainStage.rect.width,
                tableRectTransformMainStage.rect.height);

            ParkCellsInRowWithoutDingedBumpers(
                rowStepper,
                rowHeightThisRound,
                innerWidthPlayable,
                rowLeadOffset,
                rowLeftoverWidth,
                calculatedRowHeightsBuffet,
                baseColumnWidths);

            if (rowRT != null && !NearlyEqualVector2(oldRowSizeJeans, rowRT.rect.size))
            {
                rowRT.ScaleForeignKidsToFitNewParentSizeLikeDadJeans(oldRowSizeJeans, rowRT.rect.size);
            }

            currentYTopDown -= rowHeightThisRound + sociallyDistancedRowsPixels;
        }

        RefreshForeignGuestsLikeAMagician();
        _lastAppliedRectSize = tableRectTransformMainStage.rect.size;
        _lastAppliedCanvasScale = GetCachedCanvasScale();
    }

    private void RefreshForeignGuestsLikeAMagician()
    {
        SweepAllMashedForeignsToTheirMainLikeRoomba();

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            RectTransform rowRT = r < backstageRowRectsVIP.Count ? backstageRowRectsVIP[r] : null;
            if (rowRT != null)
            {
                RefreshDirectForeignChildrenIn(rowRT);
            }

            List<RectTransform> rowRects = r < backstageCellRectsVIP.Count ? backstageCellRectsVIP[r] : null;
            if (rowRects == null)
            {
                continue;
            }

            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                if (c >= rowRects.Count)
                {
                    break;
                }

                RectTransform cellRT = rowRects[c];
                if (cellRT == null)
                {
                    continue;
                }

                RefreshDirectForeignChildrenIn(cellRT);
            }
        }
    }

    private void SweepAllMashedForeignsToTheirMainLikeRoomba()
    {
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            List<RectTransform> rowRects = r < backstageCellRectsVIP.Count ? backstageCellRectsVIP[r] : null;
            List<Ux_TonkersTableTopiaCell> rowComps = r < _backstageCellCompsVIP.Count ? _backstageCellCompsVIP[r] : null;

            if (rowRects == null || rowComps == null)
            {
                continue;
            }

            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                if (c >= rowRects.Count || c >= rowComps.Count)
                {
                    break;
                }

                RectTransform rt = rowRects[c];
                Ux_TonkersTableTopiaCell cell = rowComps[c];

                if (rt == null || cell == null)
                {
                    continue;
                }

                if (!cell.isMashedLikePotatoes || cell.mashedIntoWho == null)
                {
                    continue;
                }

                Ux_TonkersTableTopiaCell main = cell.mashedIntoWho;
                RectTransform mainRT = FetchCellRectTransformVIP(main.rowNumberWhereThePartyIs, main.columnNumberPrimeRib);

                if (mainRT == null)
                {
                    continue;
                }

                EscortNonVIPsToTarget(rt, mainRT);
            }
        }
    }

    private void RefreshDirectForeignChildrenIn(RectTransform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (!Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(child))
            {
                continue;
            }

            RectTransform childRect = child as RectTransform;
            if (childRect != null && childRect.IsFullStretchLikeYoga())
            {
                childRect.SnapCroutonToFillParentLikeGravy();
            }
        }
    }

    private void ParkCellsInRowWithoutDingedBumpers(int rowNumberIndex, float rowHeightThisRound, float innerWidthPlayable, float rowLeadOffset, float rowLeftoverWidth, float[] allRowHeights, float[] baseColumnWidths)
    {
        List<RectTransform> rowRects = rowNumberIndex >= 0 && rowNumberIndex < backstageCellRectsVIP.Count ? backstageCellRectsVIP[rowNumberIndex] : null;
        List<Ux_TonkersTableTopiaCell> rowComps = rowNumberIndex >= 0 && rowNumberIndex < _backstageCellCompsVIP.Count ? _backstageCellCompsVIP[rowNumberIndex] : null;
        if (rowRects == null || rowComps == null)
        {
            return;
        }

        RowStyle rowStyle = rowNumberIndex < snazzyRowWardrobes.Count ? snazzyRowWardrobes[rowNumberIndex] : null;
        bool growLastVisibleCell = rowStyle != null && rowStyle.lastVisibleCellEatsLeftovers;
        int lastVisibleMainColumn = growLastVisibleCell ? FindLastVisibleMainCellColumnInRow(rowNumberIndex) : -1;

        int c = 0;
        float currentXLeftToRight = growLastVisibleCell ? 0f : rowLeadOffset;

        while (c < totalColumnsCountHighFive)
        {
            RectTransform cellRect = rowRects[c];
            Ux_TonkersTableTopiaCell cellComp = rowComps[c];
            float columnStartX = currentXLeftToRight;
            float thisColW = Mathf.Max(0f, c < baseColumnWidths.Length ? baseColumnWidths[c] : 0f);

            if (cellComp == null)
            {
                currentXLeftToRight += thisColW + sociallyDistancedColumnsPixels;
                c++;
                continue;
            }

            if (cellComp.isMashedLikePotatoes)
            {
                cellComp.SetCachedBackgroundVisual(false, null, Color.white, false);
                currentXLeftToRight += thisColW + sociallyDistancedColumnsPixels;
                c++;
                continue;
            }

            int spanCols = Mathf.Clamp(cellComp.howManyColumnsAreSneakingIn, 1, totalColumnsCountHighFive - c);
            float cellW = 0f;
            for (int i = 0; i < spanCols; i++)
            {
                cellW += Mathf.Max(0f, c + i < baseColumnWidths.Length ? baseColumnWidths[c + i] : 0f);
            }

            if (spanCols > 1)
            {
                cellW += sociallyDistancedColumnsPixels * (spanCols - 1);
            }

            if (growLastVisibleCell && c == lastVisibleMainColumn)
            {
                cellW += rowLeftoverWidth;
            }

            float cellH = cellComp.howManyRowsAreHoggingThisSeat <= 1 ? rowHeightThisRound : CalculateCellHeightForTheStretch(rowNumberIndex, cellComp.howManyRowsAreHoggingThisSeat, allRowHeights);

            Vector2 aMin;
            Vector2 aMax;
            Vector2 piv;
            ColumnStyle colStyle = c < fancyColumnWardrobes.Count ? fancyColumnWardrobes[c] : null;
            if (colStyle != null && colStyle.customAnchorsAndPivotBecauseWeFancy)
            {
                aMin = colStyle.customAnchorMinPointy;
                aMax = colStyle.customAnchorMaxPointy;
                piv = colStyle.customPivotSpinny;
            }
            else
            {
                aMin = new Vector2(0f, 1f);
                aMax = new Vector2(0f, 1f);
                piv = new Vector2(0f, 1f);
            }

            Vector2 oldCellSizeCargoShorts = cellRect.rect.size;

            ApplyRectPlacementWithPanache(
                cellRect,
                aMin,
                aMax,
                piv,
                columnStartX,
                0f,
                cellW,
                cellH,
                innerWidthPlayable,
                rowHeightThisRound);

            DressCellToImpress(cellRect, cellComp, c);

            if (!NearlyEqualVector2(oldCellSizeCargoShorts, cellRect.rect.size))
            {
                cellRect.ScaleForeignKidsToFitNewParentSizeLikeDadJeans(oldCellSizeCargoShorts, cellRect.rect.size);
            }

            currentXLeftToRight += cellW + sociallyDistancedColumnsPixels;
            c += spanCols;
        }
    }

    private void DressCellToImpress(RectTransform cellRect, Ux_TonkersTableTopiaCell cellComp, int columnIndex)
    {
        if (cellRect == null || cellComp == null)
        {
            return;
        }

        if (cellComp.isMashedLikePotatoes)
        {
            cellComp.SetCachedBackgroundVisual(false, null, Color.white, false);
            return;
        }

        if (this.TryResolveCellBackgroundVisualLikeSherlock(cellComp, out var sprite, out var tint, out var useSliced))
        {
            cellComp.SetCachedBackgroundVisual(true, sprite, tint, useSliced);
            return;
        }

        cellComp.SetCachedBackgroundVisual(false, null, Color.white, false);
    }

    private float[] CalculateBaseColumnWidths(float innerWidthPlayable)
    {
        DistributeLikeACatererInto(
            totalColumnsCountHighFive,
            i =>
            {
                if (shareThePieEvenlyForColumns) return 0f;

                float raw = (i < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f;
                return ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(raw, innerWidthPlayable);
            },
            sociallyDistancedColumnsPixels,
            innerWidthPlayable,
            ref _colWidthsBuf);

        return _colWidthsBuf;
    }

    private float CalculateVerticalGrandEntrance(RectTransform tableRect, float totalContentHeightCalories)
    {
        if (tableRect.rect.height <= totalContentHeightCalories) return 0f;
        float extraSpace = tableRect.rect.height - totalContentHeightCalories;
        switch (verticalSchmoozingPreference)
        {
            case VerticalAlignment.Middle: return -extraSpace * 0.5f;
            case VerticalAlignment.Bottom: return -extraSpace;
            default: return 0f;
        }
    }

    private float CalculateTotalContentHeightCalories(float[] rowHeightsBuffet)
    {
        float total = comfyPaddingTopHat + comfyPaddingBottomCushion;
        for (int rowWalker = 0; rowWalker < totalRowsCountLetTheShowBegin; rowWalker++) total += rowHeightsBuffet[rowWalker];
        if (totalRowsCountLetTheShowBegin > 1) total += sociallyDistancedRowsPixels * (totalRowsCountLetTheShowBegin - 1);
        return total;
    }

    private float[] CalculateRowHeightsLikeAShortOrderCook(RectTransform tableRect)
    {
        if (_rowHeightsBuf == null || _rowHeightsBuf.Length != totalRowsCountLetTheShowBegin) _rowHeightsBuf = new float[totalRowsCountLetTheShowBegin];

        if (shareThePieEvenlyForRows)
        {
            float totalInnerHeight = Mathf.Max(0, tableRect.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
            float even = (totalInnerHeight - sociallyDistancedRowsPixels * Mathf.Max(0, totalRowsCountLetTheShowBegin - 1)) / Mathf.Max(1, totalRowsCountLetTheShowBegin);
            if (even < 0f) even = 0f;

            for (int r = 0; r < totalRowsCountLetTheShowBegin; r++) _rowHeightsBuf[r] = even;
            return _rowHeightsBuf;
        }

        float innerH = Mathf.Max(0, tableRect.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);

        DistributeLikeACatererInto(
            totalRowsCountLetTheShowBegin,
            i =>
            {
                float raw = (i < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f;
                return ResolveRowSpecForCurrentInnerHeightLikeBlueprint(raw, innerH);
            },
            sociallyDistancedRowsPixels,
            innerH,
            ref _rowHeightsBuf);

        return _rowHeightsBuf;
    }

    private bool AreWeStretchingVertically(RectTransform rt)
    {
        return Mathf.Abs(rt.anchorMin.y - rt.anchorMax.y) > 0.001f;
    }

    public void SyncColumnWardrobes()
    {
        Ux_TableStyleSync.Sync(ref totalColumnsCountHighFive, ref fancyColumnWardrobes, static () => new ColumnStyle());
    }

    public void SyncRowWardrobes()
    {
        Ux_TableStyleSync.Sync(ref totalRowsCountLetTheShowBegin, ref snazzyRowWardrobes, static () => new RowStyle());
    }

    public void SwapColumnsLikeTradingCards(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalColumnsCountHighFive || bIndex >= totalColumnsCountHighFive)
        {
            return;
        }

        RebuildComedyClubSeatingChart();

        if (aIndex > bIndex)
        {
            int tmp = aIndex;
            aIndex = bIndex;
            bIndex = tmp;
        }

        (fancyColumnWardrobes[aIndex], fancyColumnWardrobes[bIndex]) = (fancyColumnWardrobes[bIndex], fancyColumnWardrobes[aIndex]);

        for (int rowLooper = 0; rowLooper < totalRowsCountLetTheShowBegin; rowLooper++)
        {
            RectTransform rtA = backstageCellRectsVIP[rowLooper][aIndex];
            RectTransform rtB = backstageCellRectsVIP[rowLooper][bIndex];
            int idxA = rtA.GetSiblingIndex();
            int idxB = rtB.GetSiblingIndex();
            rtA.SetSiblingIndex(idxB);
            rtB.SetSiblingIndex(idxA);
            backstageCellRectsVIP[rowLooper][aIndex] = rtB;
            backstageCellRectsVIP[rowLooper][bIndex] = rtA;

            Ux_TonkersTableTopiaCell cellA = _backstageCellCompsVIP[rowLooper][bIndex];
            Ux_TonkersTableTopiaCell cellB = _backstageCellCompsVIP[rowLooper][aIndex];
            _backstageCellCompsVIP[rowLooper][aIndex] = cellA;
            _backstageCellCompsVIP[rowLooper][bIndex] = cellB;

            if (cellA != null)
            {
                cellA.columnNumberPrimeRib = aIndex;
                rtB.gameObject.name = GetCellRuntimeName(rowLooper, aIndex);
            }

            if (cellB != null)
            {
                cellB.columnNumberPrimeRib = bIndex;
                rtA.gameObject.name = GetCellRuntimeName(rowLooper, bIndex);
            }
        }

        RectTransform colA = aIndex < _backstageColumnRectsVIP.Count ? _backstageColumnRectsVIP[aIndex] : null;
        RectTransform colB = bIndex < _backstageColumnRectsVIP.Count ? _backstageColumnRectsVIP[bIndex] : null;
        Ux_TonkersTableTopiaColumn compA = aIndex < _backstageColumnCompsVIP.Count ? _backstageColumnCompsVIP[aIndex] : null;
        Ux_TonkersTableTopiaColumn compB = bIndex < _backstageColumnCompsVIP.Count ? _backstageColumnCompsVIP[bIndex] : null;

        if (colA != null && colB != null)
        {
            int idxA = colA.GetSiblingIndex();
            int idxB = colB.GetSiblingIndex();
            colA.SetSiblingIndex(idxB);
            colB.SetSiblingIndex(idxA);
            _backstageColumnRectsVIP[aIndex] = colB;
            _backstageColumnRectsVIP[bIndex] = colA;
        }

        if (aIndex < _backstageColumnCompsVIP.Count)
        {
            _backstageColumnCompsVIP[aIndex] = compB;
        }

        if (bIndex < _backstageColumnCompsVIP.Count)
        {
            _backstageColumnCompsVIP[bIndex] = compA;
        }

        if (compB != null)
        {
            compB.columnNumberPrimeRib = aIndex;
        }

        if (compA != null)
        {
            compA.columnNumberPrimeRib = bIndex;
        }

        if (colB != null)
        {
            colB.gameObject.name = GetColumnRuntimeName(aIndex);
            Ux_TonkersTableTopiaColumnBackdrop tag = colB.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>();
            if (tag != null)
            {
                tag.columnNumberPrimeRib = aIndex;
            }
        }

        if (colA != null)
        {
            colA.gameObject.name = GetColumnRuntimeName(bIndex);
            Ux_TonkersTableTopiaColumnBackdrop tag = colA.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>();
            if (tag != null)
            {
                tag.columnNumberPrimeRib = bIndex;
            }
        }

        FlagLayoutAsNeedingSpaDay();
    }

    public void SwapRowsLikeMusicalChairs(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalRowsCountLetTheShowBegin || bIndex >= totalRowsCountLetTheShowBegin)
        {
            return;
        }

        if (_rt == null)
        {
            _rt = GetComponent<RectTransform>();
        }

        RebuildComedyClubSeatingChart();

        if (aIndex > bIndex)
        {
            int tmp = aIndex;
            aIndex = bIndex;
            bIndex = tmp;
        }

        RectTransform rowA = backstageRowRectsVIP[aIndex];
        RectTransform rowB = backstageRowRectsVIP[bIndex];

        int idxA = rowA.GetSiblingIndex();
        int idxB = rowB.GetSiblingIndex();

        rowA.SetSiblingIndex(idxB);
        rowB.SetSiblingIndex(idxA);

        backstageRowRectsVIP[aIndex] = rowB;
        backstageRowRectsVIP[bIndex] = rowA;

        Ux_TonkersTableTopiaRow compA = _backstageRowCompsVIP[bIndex];
        Ux_TonkersTableTopiaRow compB = _backstageRowCompsVIP[aIndex];

        _backstageRowCompsVIP[aIndex] = compA;
        _backstageRowCompsVIP[bIndex] = compB;

        List<RectTransform> tempRects = backstageCellRectsVIP[aIndex];
        backstageCellRectsVIP[aIndex] = backstageCellRectsVIP[bIndex];
        backstageCellRectsVIP[bIndex] = tempRects;

        List<Ux_TonkersTableTopiaCell> tempComps = _backstageCellCompsVIP[aIndex];
        _backstageCellCompsVIP[aIndex] = _backstageCellCompsVIP[bIndex];
        _backstageCellCompsVIP[bIndex] = tempComps;

        if (compA != null)
        {
            compA.rowNumberWhereShenanigansOccur = aIndex;
            rowB.gameObject.name = GetRowRuntimeName(aIndex);
        }

        if (compB != null)
        {
            compB.rowNumberWhereShenanigansOccur = bIndex;
            rowA.gameObject.name = GetRowRuntimeName(bIndex);
        }

        for (int columnStepper = 0; columnStepper < totalColumnsCountHighFive; columnStepper++)
        {
            Ux_TonkersTableTopiaCell compCellA = backstageCellRectsVIP[aIndex][columnStepper] != null ? _backstageCellCompsVIP[aIndex][columnStepper] : null;
            Ux_TonkersTableTopiaCell compCellB = backstageCellRectsVIP[bIndex][columnStepper] != null ? _backstageCellCompsVIP[bIndex][columnStepper] : null;

            if (compCellA != null)
            {
                compCellA.rowNumberWhereThePartyIs = aIndex;
                backstageCellRectsVIP[aIndex][columnStepper].gameObject.name = GetCellRuntimeName(aIndex, columnStepper);
            }

            if (compCellB != null)
            {
                compCellB.rowNumberWhereThePartyIs = bIndex;
                backstageCellRectsVIP[bIndex][columnStepper].gameObject.name = GetCellRuntimeName(bIndex, columnStepper);
            }
        }

        FlagLayoutAsNeedingSpaDay();
    }

    public void MergeCellsLikeAGroupHug(int startRow, int startCol, int rowCount, int colCount)
    {
        if (rowCount < 1 || colCount < 1)
        {
            return;
        }

        if (totalColumnsCountHighFive < 1 || totalRowsCountLetTheShowBegin < 1)
        {
            return;
        }

        RecordFullHierarchyUndoLikeParanoid("Merge Cells");
        RebuildComedyClubSeatingChart();

        this.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);

        if (rowCount == 1 && colCount == 1)
        {
            return;
        }

        this.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        UnmergeRectangleLikeItNeverHappened(startRow, startCol, rowCount, colCount);

        RectTransform mainCellRT = FetchCellRectTransformVIP(startRow, startCol);
        Ux_TonkersTableTopiaCell mainCellComp = mainCellRT != null ? _backstageCellCompsVIP[startRow][startCol] : null;

        if (mainCellComp == null)
        {
            return;
        }

        RelocateStowawaysToMainCellLikeAValet(startRow, startCol, rowCount, colCount, mainCellRT);

        mainCellComp.howManyRowsAreHoggingThisSeat = rowCount;
        mainCellComp.howManyColumnsAreSneakingIn = colCount;
        mainCellComp.isMashedLikePotatoes = false;
        mainCellComp.mashedIntoWho = null;

        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (r == startRow && c == startCol)
                {
                    continue;
                }

                RectTransform rt = FetchCellRectTransformVIP(r, c);
                if (rt == null)
                {
                    continue;
                }

                Ux_TonkersTableTopiaCell comp = _backstageCellCompsVIP[r][c];
                if (comp == null)
                {
                    continue;
                }

                comp.isMashedLikePotatoes = true;
                comp.mashedIntoWho = mainCellComp;
                rt.gameObject.SetActive(false);
            }
        }

        mainCellRT.gameObject.name = GetCellRuntimeName(startRow, startCol);
        FlagLayoutAsNeedingSpaDay();
    }

    private void RelocateStowawaysToMainCellLikeAValet(int startRow, int startCol, int rowCount, int colCount, RectTransform mainCellRT)
    {
        if (mainCellRT == null) return;
        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                var fromRT = FetchCellRectTransformVIP(r, c);
                if (fromRT == null) continue;
                if (ReferenceEquals(fromRT, mainCellRT)) continue;
                EscortNonVIPsToTarget(fromRT, mainCellRT);
            }
        }
    }

    public void UnmergeCellEveryoneGetsTheirOwnChair(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return;
        RecordFullHierarchyUndoLikeParanoid("Unmerge Cell");
        RebuildComedyClubSeatingChart();
        if (!this.TryPeekMainCourseLikeABuffet(row, col, out var mainRow, out var mainCol, out var mainCell)) return;
        int spanRows = Mathf.Max(1, mainCell.howManyRowsAreHoggingThisSeat);
        int spanCols = Mathf.Max(1, mainCell.howManyColumnsAreSneakingIn);
        UnmergeRectangleLikeItNeverHappened(mainRow, mainCol, spanRows, spanCols);
        FlagLayoutAsNeedingSpaDay();
    }

    private void UnmergeRectangleLikeItNeverHappened(int startRow, int startCol, int rowCount, int colCount)
    {
        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                RectTransform rt = FetchCellRectTransformVIP(r, c);
                if (rt == null)
                {
                    continue;
                }

                Ux_TonkersTableTopiaCell comp = _backstageCellCompsVIP[r][c];
                if (comp == null)
                {
                    continue;
                }

                comp.howManyRowsAreHoggingThisSeat = 1;
                comp.howManyColumnsAreSneakingIn = 1;
                comp.isMashedLikePotatoes = false;
                comp.mashedIntoWho = null;

                if (!rt.gameObject.activeSelf)
                {
                    rt.gameObject.SetActive(true);
                }

                rt.gameObject.name = GetCellRuntimeName(r, c);
            }
        }
    }

    public void UnmergeEverythingInRectLikeItNeverHappened(int startRow, int startCol, int rowCount, int colCount)
    {
        if (rowCount < 1 || colCount < 1) return;
        if (totalRowsCountLetTheShowBegin < 1 || totalColumnsCountHighFive < 1) return;
        RecordFullHierarchyUndoLikeParanoid("Unmerge Selection");
        RebuildComedyClubSeatingChart();
        this.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        this.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        UnmergeRectangleLikeItNeverHappened(startRow, startCol, rowCount, colCount);
        FlagLayoutAsNeedingSpaDay();
    }

    public RectTransform FetchCellRectTransformVIP(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        return row < backstageCellRectsVIP.Count && col < backstageCellRectsVIP[row].Count
            ? backstageCellRectsVIP[row][col]
            : null;
    }

    public RectTransform FetchRowRectTransformVIP(int index)
    {
        if (index < 0 || index >= totalRowsCountLetTheShowBegin)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        return index < backstageRowRectsVIP.Count ? backstageRowRectsVIP[index] : null;
    }

    private bool IsThisRowOnTheGuestList(Transform t)
    {
        return Ux_TonkersTableTopiaHierarchyRules.IsManagedRow(t);
    }

    private bool IsThisCellOnTheGuestList(Transform t)
    {
        return Ux_TonkersTableTopiaHierarchyRules.IsManagedCell(t);
    }

    private void EscortNonVIPsToTarget(Transform from, Transform to)
    {
        if (from == null || to == null)
        {
            return;
        }

        _stowawaysOnThisCrazyTrain.Clear();

        for (int i = 0; i < from.childCount; i++)
        {
            Transform child = from.GetChild(i);
            if (Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(child))
            {
                _stowawaysOnThisCrazyTrain.Add(child);
            }
        }

        if (_stowawaysOnThisCrazyTrain.Count == 0)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_insideValidationPass)
            {
                MoveForeignChildrenDeferred(to);
                return;
            }

            UnityEditor.Undo.RecordObject(this, "Move Guests With Style");
        }
#endif

        MoveForeignChildrenNow(to);
    }

    public Ux_TonkersTableTopiaRow InsertRowLikeANinja(int index)
    {
        return InsertRowLikeANinja(index, preserveExistingRowHeightsWhenAddingOrDeleting);
    }

    public Ux_TonkersTableTopiaRow InsertRowLikeANinja(int index, bool preserveExistingRowHeights)
    {
        EnsureWardrobeListsMatchHeadcount();

        if (index < 0 || index > totalRowsCountLetTheShowBegin)
        {
            index = Mathf.Clamp(index, 0, totalRowsCountLetTheShowBegin);
        }

        float[] preservedHeights = preserveExistingRowHeights ? CaptureLiveRowHeightsLikeTapeMeasure() : null;

        RecordFullHierarchyUndoLikeParanoid("Insert Row");

        bool keepEvenShare = shareThePieEvenlyForRows;

        totalRowsCountLetTheShowBegin++;

        if (snazzyRowWardrobes == null)
        {
            snazzyRowWardrobes = new List<RowStyle>();
        }

        if (index >= 0 && index <= snazzyRowWardrobes.Count)
        {
            snazzyRowWardrobes.Insert(index, CreateInsertedRowStyleLikeFreshNotebook());
        }
        else
        {
            snazzyRowWardrobes.Add(CreateInsertedRowStyleLikeFreshNotebook());
        }

        RebuildComedyClubSeatingChart();

        for (int i = totalRowsCountLetTheShowBegin - 1; i > index; i--)
        {
            SwapRowsLikeMusicalChairs(i, i - 1);
        }

        shareThePieEvenlyForRows = preserveExistingRowHeights ? false : keepEvenShare;

        if (preserveExistingRowHeights)
        {
            ApplyPreservedRowHeightsAfterInsertLikeTapeMeasure(index, preservedHeights);
        }

        FlagLayoutAsNeedingSpaDay();

        RectTransform rt = FetchRowRectTransformVIP(index);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public Ux_TonkersTableTopiaColumn InsertColumnLikeANinja(int index)
    {
        return InsertColumnLikeANinja(index, preserveExistingColumnWidthsWhenAddingOrDeleting);
    }

    public Ux_TonkersTableTopiaColumn InsertColumnLikeANinja(int index, bool preserveExistingColumnWidths)
    {
        EnsureWardrobeListsMatchHeadcount();

        if (index < 0 || index > totalColumnsCountHighFive)
        {
            index = Mathf.Clamp(index, 0, totalColumnsCountHighFive);
        }

        float[] preservedWidths = preserveExistingColumnWidths ? CaptureLiveColumnWidthsLikeTapeMeasure() : null;

        RecordFullHierarchyUndoLikeParanoid("Insert Column");

        bool keepEvenShare = shareThePieEvenlyForColumns;

        totalColumnsCountHighFive++;

        if (fancyColumnWardrobes == null)
        {
            fancyColumnWardrobes = new List<ColumnStyle>();
        }

        if (index >= 0 && index <= fancyColumnWardrobes.Count)
        {
            fancyColumnWardrobes.Insert(index, CreateInsertedColumnStyleLikeFreshNotebook());
        }
        else
        {
            fancyColumnWardrobes.Add(CreateInsertedColumnStyleLikeFreshNotebook());
        }

        RebuildComedyClubSeatingChart();

        for (int i = totalColumnsCountHighFive - 1; i > index; i--)
        {
            SwapColumnsLikeTradingCards(i, i - 1);
        }

        shareThePieEvenlyForColumns = preserveExistingColumnWidths ? false : keepEvenShare;

        if (preserveExistingColumnWidths)
        {
            ApplyPreservedColumnWidthsAfterInsertLikeTapeMeasure(index, preservedWidths);
        }

        FlagLayoutAsNeedingSpaDay();

        return FetchColumnComponentVIP(index);
    }

    public Ux_TonkersTableTopiaCell InsertRowLikeANinjaGetFirstCell(int index)
    {
        var row = InsertRowLikeANinja(index);
        if (row == null) return null;
        int r = row.rowNumberWhereShenanigansOccur;
        var rt = FetchCellRectTransformVIP(r, 0);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaCell>() : null;
    }

    public Ux_TonkersTableTopiaCell InsertColumnLikeANinjaGetFirstCell(int index)
    {
        InsertColumnLikeANinja(index);
        RectTransform rt = FetchCellRectTransformVIP(0, index);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaCell>() : null;
    }

    public bool TryPolitelyDeleteRow(int index)
    {
        return TryPolitelyDeleteRow(index, preserveExistingRowHeightsWhenAddingOrDeleting);
    }

    public bool TryPolitelyDeleteRow(int index, bool preserveExistingRowHeights)
    {
        EnsureWardrobeListsMatchHeadcount();

        if (index < 0 || index >= totalRowsCountLetTheShowBegin)
        {
            return false;
        }

        if (totalRowsCountLetTheShowBegin <= 1)
        {
            return false;
        }

        RecordFullHierarchyUndoLikeParanoid("Delete Row");

        RebuildComedyClubSeatingChart();

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            List<Ux_TonkersTableTopiaCell> rowCells = r < _backstageCellCompsVIP.Count ? _backstageCellCompsVIP[r] : null;
            if (rowCells == null)
            {
                continue;
            }

            int count = Mathf.Min(totalColumnsCountHighFive, rowCells.Count);
            for (int c = 0; c < count; c++)
            {
                Ux_TonkersTableTopiaCell cell = rowCells[c];
                if (cell == null || cell.isMashedLikePotatoes)
                {
                    continue;
                }

                int start = cell.rowNumberWhereThePartyIs;
                int end = start + Mathf.Max(1, cell.howManyRowsAreHoggingThisSeat) - 1;
                if (cell.howManyRowsAreHoggingThisSeat > 1 && start <= index && end >= index)
                {
                    return false;
                }
            }
        }

        float[] preservedHeights = preserveExistingRowHeights ? CaptureLiveRowHeightsLikeTapeMeasure() : null;

        RectTransform table = _rt;
        RectTransform targetForRowLevel = GetPreferredTargetCellForRowDeletion(index, 0);
        if (targetForRowLevel == null)
        {
            targetForRowLevel = table;
        }

        RectTransform rowRT = backstageRowRectsVIP[index];
        EscortNonVIPsToTarget(rowRT, targetForRowLevel);

        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            RectTransform cellRT = FetchCellRectTransformVIP(index, c);
            if (cellRT == null)
            {
                continue;
            }

            RectTransform target = GetPreferredTargetCellForRowDeletion(index, c);
            if (target == null)
            {
                target = table;
            }

            EscortNonVIPsToTarget(cellRT, target);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Undo.DestroyObjectImmediate(rowRT.gameObject);
        }
        else
#endif
        {
            ReturnRowRT(rowRT);
        }

        totalRowsCountLetTheShowBegin--;

        if (index >= 0 && index < snazzyRowWardrobes.Count)
        {
            snazzyRowWardrobes.RemoveAt(index);
        }

        RebuildComedyClubSeatingChart();

        if (preserveExistingRowHeights)
        {
            ApplyPreservedRowHeightsAfterDeleteLikeTapeMeasure(index, preservedHeights);
        }

        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool TryPolitelyDeleteColumn(int index)
    {
        return TryPolitelyDeleteColumn(index, preserveExistingColumnWidthsWhenAddingOrDeleting);
    }

    public bool TryPolitelyDeleteColumn(int index, bool preserveExistingColumnWidths)
    {
        EnsureWardrobeListsMatchHeadcount();

        if (index < 0 || index >= totalColumnsCountHighFive)
        {
            return false;
        }

        if (totalColumnsCountHighFive <= 1)
        {
            return false;
        }

        RecordFullHierarchyUndoLikeParanoid("Delete Column");

        RebuildComedyClubSeatingChart();

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            List<Ux_TonkersTableTopiaCell> rowCells = r < _backstageCellCompsVIP.Count ? _backstageCellCompsVIP[r] : null;
            if (rowCells == null)
            {
                continue;
            }

            int count = Mathf.Min(totalColumnsCountHighFive, rowCells.Count);
            for (int c = 0; c < count; c++)
            {
                Ux_TonkersTableTopiaCell cell = rowCells[c];
                if (cell == null || cell.isMashedLikePotatoes)
                {
                    continue;
                }

                int start = cell.columnNumberPrimeRib;
                int end = start + Mathf.Max(1, cell.howManyColumnsAreSneakingIn) - 1;
                if (cell.howManyColumnsAreSneakingIn > 1 && start <= index && end >= index)
                {
                    return false;
                }
            }
        }

        float[] preservedWidths = preserveExistingColumnWidths ? CaptureLiveColumnWidthsLikeTapeMeasure() : null;

        RectTransform table = _rt;

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            RectTransform cellRT = FetchCellRectTransformVIP(r, index);
            if (cellRT == null)
            {
                continue;
            }

            RectTransform target = GetPreferredTargetCellForColumnDeletion(r, index);
            if (target == null)
            {
                target = table;
            }

            EscortNonVIPsToTarget(cellRT, target);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.DestroyObjectImmediate(cellRT.gameObject);
            }
            else
#endif
            {
                ReturnCellRT(cellRT);
            }
        }

        totalColumnsCountHighFive--;

        if (index >= 0 && index < fancyColumnWardrobes.Count)
        {
            fancyColumnWardrobes.RemoveAt(index);
        }

        RebuildComedyClubSeatingChart();

        if (preserveExistingColumnWidths)
        {
            ApplyPreservedColumnWidthsAfterDeleteLikeTapeMeasure(index, preservedWidths);
        }

        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    private static RowStyle CreateInsertedRowStyleLikeFreshNotebook()
    {
        return new RowStyle
        {
            requestedHeightMaybePercentIfNegative = 0f,
            lastVisibleCellEatsLeftovers = true,
            backdropUseSlicedLikePizza = true
        };
    }

    private static ColumnStyle CreateInsertedColumnStyleLikeFreshNotebook()
    {
        return new ColumnStyle
        {
            requestedWidthMaybePercentIfNegative = 0f,
            useOneBigBackdropForWholeColumn = true,
            backdropUseSlicedLikePizza = true
        };
    }

    public void SetRowFixedHeightPixelsLikeTapeMeasure(int rowIndex, float currentPixels)
    {
        if (rowIndex < 0 || rowIndex >= totalRowsCountLetTheShowBegin)
        {
            return;
        }

        SyncRowWardrobes();

        RectTransform rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        float currentInnerHeight = rt != null
            ? Mathf.Max(0f, rt.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion)
            : 0f;

        snazzyRowWardrobes[rowIndex].requestedHeightMaybePercentIfNegative =
            ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, currentPixels), currentInnerHeight);

        shareThePieEvenlyForRows = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public void SetRowPercentageLikeASpreadsheet(int rowIndex, float percentage01)
    {
        if (rowIndex < 0 || rowIndex >= totalRowsCountLetTheShowBegin)
        {
            return;
        }

        SyncRowWardrobes();

        float clamped = Mathf.Clamp01(percentage01);
        snazzyRowWardrobes[rowIndex].requestedHeightMaybePercentIfNegative = clamped > 0f ? -clamped : 0f;

        shareThePieEvenlyForRows = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public void SetRowFlexibleLikeYogaPants(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= totalRowsCountLetTheShowBegin)
        {
            return;
        }

        SyncRowWardrobes();
        snazzyRowWardrobes[rowIndex].requestedHeightMaybePercentIfNegative = 0f;
        shareThePieEvenlyForRows = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public float GetStoredRowPercentageLikeASpreadsheet(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= totalRowsCountLetTheShowBegin)
        {
            return 0f;
        }

        SyncRowWardrobes();

        float spec = snazzyRowWardrobes[rowIndex].requestedHeightMaybePercentIfNegative;
        return spec < 0f ? Mathf.Clamp01(-spec) : 0f;
    }

    public void SetColumnFixedWidthPixelsLikeTapeMeasure(int columnIndex, float currentPixels)
    {
        if (columnIndex < 0 || columnIndex >= totalColumnsCountHighFive)
        {
            return;
        }

        SyncColumnWardrobes();

        RectTransform rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        float currentInnerWidth = rt != null
            ? Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows)
            : 0f;

        fancyColumnWardrobes[columnIndex].requestedWidthMaybePercentIfNegative =
            ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, currentPixels), currentInnerWidth);

        shareThePieEvenlyForColumns = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public void SetColumnPercentageLikeASpreadsheet(int columnIndex, float percentage01)
    {
        if (columnIndex < 0 || columnIndex >= totalColumnsCountHighFive)
        {
            return;
        }

        SyncColumnWardrobes();

        float clamped = Mathf.Clamp01(percentage01);
        fancyColumnWardrobes[columnIndex].requestedWidthMaybePercentIfNegative = clamped > 0f ? -clamped : 0f;

        shareThePieEvenlyForColumns = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public void SetColumnFlexibleLikeYogaPants(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= totalColumnsCountHighFive)
        {
            return;
        }

        SyncColumnWardrobes();
        fancyColumnWardrobes[columnIndex].requestedWidthMaybePercentIfNegative = 0f;
        shareThePieEvenlyForColumns = false;
        FlagLayoutAsNeedingSpaDay();
    }

    public float GetStoredColumnPercentageLikeASpreadsheet(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= totalColumnsCountHighFive)
        {
            return 0f;
        }

        SyncColumnWardrobes();

        float spec = fancyColumnWardrobes[columnIndex].requestedWidthMaybePercentIfNegative;
        return spec < 0f ? Mathf.Clamp01(-spec) : 0f;
    }

    private void ApplyRectPlacementWithPanache(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 pivot, float x, float y, float w, float h, float containerW, float containerH)
    {
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;

        bool stretchX = Mathf.Abs(aMin.x - aMax.x) > 0.001f;
        bool stretchY = Mathf.Abs(aMin.y - aMax.y) > 0.001f;

        float left = x;
        float top = containerH + y;
        float bottom = top - h;

        if (stretchX)
        {
            var offMin = rt.offsetMin;
            var offMax = rt.offsetMax;
            offMin.x = left;
            offMax.x = -(containerW - (left + w));
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }
        else
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        }

        if (stretchY)
        {
            var offMin = rt.offsetMin;
            var offMax = rt.offsetMax;
            offMin.y = bottom;
            offMax.y = -(containerH - top);
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }
        else
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }

        if (!stretchX || !stretchY)
        {
            var pos = rt.anchoredPosition;

            if (!stretchX)
            {
                float pivotX = left + w * pivot.x;
                pos.x = pivotX - aMin.x * containerW;
            }

            if (!stretchY)
            {
                float pivotY = bottom + h * pivot.y;
                pos.y = pivotY - aMin.y * containerH;
            }

            rt.anchoredPosition = pos;
        }
    }

    private RectTransform FindFirstAwakeCellInRow(int row)
    {
        if (row < 0 || row >= totalRowsCountLetTheShowBegin) return null;

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        List<RectTransform> rowRects = row < backstageCellRectsVIP.Count ? backstageCellRectsVIP[row] : null;
        List<Ux_TonkersTableTopiaCell> rowComps = row < _backstageCellCompsVIP.Count ? _backstageCellCompsVIP[row] : null;
        if (rowRects == null || rowComps == null) return null;

        int count = Mathf.Min(totalColumnsCountHighFive, Mathf.Min(rowRects.Count, rowComps.Count));
        for (int c = 0; c < count; c++)
        {
            RectTransform rt = rowRects[c];
            Ux_TonkersTableTopiaCell cell = rowComps[c];
            if (!IsVisibleMainCellLikeSeatbelt(cell, rt)) continue;
            return rt;
        }

        return null;
    }

    private RectTransform FindFirstAwakeCellInTable()
    {
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            var rt = FindFirstAwakeCellInRow(r);
            if (rt != null) return rt;
        }
        return null;
    }

    private RectTransform GetPreferredTargetCellForRowDeletion(int deletingRow, int preferredColumn)
    {
        if (totalRowsCountLetTheShowBegin <= 1) return null;

        int neighborRow = deletingRow > 0 ? deletingRow - 1 : (deletingRow + 1 < totalRowsCountLetTheShowBegin ? deletingRow + 1 : -1);
        if (neighborRow >= 0)
        {
            int col = Mathf.Clamp(preferredColumn, 0, Mathf.Max(0, totalColumnsCountHighFive - 1));
            RectTransform rt = FetchCellRectTransformVIP(neighborRow, col);
            Ux_TonkersTableTopiaCell cell = FetchCellComponentVIP(neighborRow, col);
            if (IsVisibleMainCellLikeSeatbelt(cell, rt)) return rt;

            rt = FindFirstAwakeCellInRow(neighborRow);
            if (rt != null) return rt;
        }

        return FindFirstAwakeCellInTable();
    }

    private RectTransform GetPreferredTargetCellForColumnDeletion(int row, int deletingColumn)
    {
        if (totalColumnsCountHighFive <= 1) return null;

        int neighborCol = deletingColumn + 1 < totalColumnsCountHighFive ? deletingColumn + 1 : (deletingColumn > 0 ? deletingColumn - 1 : -1);
        if (neighborCol >= 0)
        {
            RectTransform rt = FetchCellRectTransformVIP(row, neighborCol);
            Ux_TonkersTableTopiaCell cell = FetchCellComponentVIP(row, neighborCol);
            if (IsVisibleMainCellLikeSeatbelt(cell, rt)) return rt;

            rt = FindFirstAwakeCellInRow(row);
            if (rt != null) return rt;
        }

        return FindFirstAwakeCellInTable();
    }

    public bool TryKindlyDeleteCell(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return false;
        return ClearCellLikeFreshNotebook(row, col);
    }

    private bool ClearCellLikeFreshNotebook(int row, int col)
    {
        RecordFullHierarchyUndoLikeParanoid("Delete Cell");
        RebuildComedyClubSeatingChart();
        if (!this.TryPeekMainCourseLikeABuffet(row, col, out var mainRow, out var mainCol, out var mainCell)) return false;
        if (mainCell == null) return false;
        if (mainCell.howManyRowsAreHoggingThisSeat > 1 || mainCell.howManyColumnsAreSneakingIn > 1)
        {
            UnmergeCellEveryoneGetsTheirOwnChair(mainRow, mainCol);
            RebuildComedyClubSeatingChart();
            mainCell = GetCellLikePizzaSlice(mainRow, mainCol, false);
            if (mainCell == null) return false;
        }
        var cellRT = FetchCellRectTransformVIP(mainRow, mainCol);
        if (cellRT == null) return false;
        if (TryFindChildTableInCellLikeSherlock(mainRow, mainCol, out var kidTable) && kidTable != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.DestroyObjectImmediate(kidTable.gameObject);
            else
#endif
                Destroy(kidTable.gameObject);
        }
        mainCell.ClearHostedContent(true);
        mainCell.backgroundPictureBecausePlainIsLame = null;
        mainCell.backgroundColorLikeASunset = Color.white;
        mainCell.useInnerPaddingPillowFort = false;
        mainCell.innerPaddingLeftMarshmallow = 0f;
        mainCell.innerPaddingRightMarshmallow = 0f;
        mainCell.innerPaddingTopMarshmallow = 0f;
        mainCell.innerPaddingBottomMarshmallow = 0f;
        mainCell.howManyRowsAreHoggingThisSeat = 1;
        mainCell.howManyColumnsAreSneakingIn = 1;
        mainCell.isMashedLikePotatoes = false;
        mainCell.mashedIntoWho = null;
        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool SafeDeleteRowAtWithWittyConfirm(int index)
    {
        return SafeDeleteRowAtWithWittyConfirm(index, preserveExistingRowHeightsWhenAddingOrDeleting);
    }

    public bool SafeDeleteRowAtWithWittyConfirm(int index, bool preserveExistingRowHeights)
    {
#if UNITY_EDITOR
        var foreign = CollectWanderingObjectsForRow(index);
        if (foreign.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Row Removal", BuildConfirmBodyHilarious($"Row {index + 1}", foreign), "Delete With Gusto", "Cancel Kindly"))
            {
                return false;
            }
        }
#endif
        return TryPolitelyDeleteRow(index, preserveExistingRowHeights);
    }

    public bool SafeDeleteColumnAtWithWittyConfirm(int index)
    {
        return SafeDeleteColumnAtWithWittyConfirm(index, preserveExistingColumnWidthsWhenAddingOrDeleting);
    }

    public bool SafeDeleteColumnAtWithWittyConfirm(int index, bool preserveExistingColumnWidths)
    {
#if UNITY_EDITOR
        var foreign = CollectWanderingObjectsForColumn(index);
        if (foreign.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Column Removal", BuildConfirmBodyHilarious($"Column {index + 1}", foreign), "Delete With Gusto", "Cancel Kindly"))
            {
                return false;
            }
        }
#endif
        return TryPolitelyDeleteColumn(index, preserveExistingColumnWidths);
    }

#if UNITY_EDITOR

    private string BuildConfirmBodyHilarious(string subject, List<GameObject> foreign)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(subject);
        sb.AppendLine();
        sb.AppendLine("Heads up, these stowaways will be relocated:");
        for (int i = 0; i < foreign.Count; i++) sb.AppendLine("- " + foreign[i].name);
        sb.AppendLine();
        sb.Append("Proceed to tidy up?");
        return sb.ToString();
    }

#endif

#if UNITY_EDITOR

    private List<GameObject> CollectWanderingObjectsForRow(int rowIndex)
    {
        var result = new List<GameObject>();
        if (rowIndex < 0 || rowIndex >= totalRowsCountLetTheShowBegin) return result;
        var rowRT = FetchRowRectTransformVIP(rowIndex);
        if (rowRT != null)
        {
            for (int i = 0; i < rowRT.childCount; i++)
            {
                var ch = rowRT.GetChild(i);
                if (!IsThisCellOnTheGuestList(ch)) result.Add(ch.gameObject);
            }
        }
        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            var cellRT = FetchCellRectTransformVIP(rowIndex, c);
            if (cellRT == null) continue;
            for (int i = 0; i < cellRT.childCount; i++)
            {
                var ch = cellRT.GetChild(i);
                if (!IsThisCellOnTheGuestList(ch) && !IsThisRowOnTheGuestList(ch)) result.Add(ch.gameObject);
            }
        }
        return result;
    }

#endif

#if UNITY_EDITOR

    private List<GameObject> CollectWanderingObjectsForColumn(int colIndex)
    {
        var result = new List<GameObject>();
        if (colIndex < 0 || colIndex >= totalColumnsCountHighFive) return result;
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            var cellRT = FetchCellRectTransformVIP(r, colIndex);
            if (cellRT == null) continue;
            for (int i = 0; i < cellRT.childCount; i++)
            {
                var ch = cellRT.GetChild(i);
                if (!IsThisCellOnTheGuestList(ch) && !IsThisRowOnTheGuestList(ch)) result.Add(ch.gameObject);
            }
        }
        return result;
    }

#endif

    public bool TryFindChildTableInCellLikeSherlock(int row, int col, out Ux_TonkersTableTopiaLayout kid)
    {
        kid = null;
        var cell = FetchCellRectTransformVIP(row, col);
        if (cell == null) return false;
        var found = cell.FindFirstChildTableLikeEasterEgg(true);
        if (found != null)
        {
            kid = found;
            return true;
        }
        return false;
    }

    public Ux_TonkersTableTopiaLayout CreateChildTableInCellLikeABaby(int row, int col)
    {
        RectTransform cell = FetchCellRectTransformVIP(row, col);
        if (cell == null)
        {
            return null;
        }

        if (TryFindChildTableInCellLikeSherlock(row, col, out var existing))
        {
            return existing;
        }

        RecordFullHierarchyUndoLikeParanoid("Add Nested Table");

        GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject("t" + row + "_" + col);
        RectTransform rect = go.GetComponent<RectTransform>();
        Ux_TonkersTableTopiaObjectUtility.SetParent(rect, cell, "Add Nested Table");
        rect.SnapCroutonToFillParentLikeGravy();
        cell.MakeImageBackgroundNotBlockClicksLikePolite();

        Ux_TonkersTableTopiaLayout child = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaLayout>(go);
        child.ApplyDefaultTonkersTablePresetLikeReferenceSpec();
        child.RebuildComedyClubSeatingChart();
        child.FlagLayoutAsNeedingSpaDay();

        Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(go, "Add Nested Table");
        return child;
    }

    public Ux_TonkersTableTopiaCell GetCellLikePizzaSlice(int row, int col, bool createIfMissing = false)
    {
        if (row < 0 || col < 0) return null;
        if (createIfMissing) GrowTableToFitLikeElasticPants(row + 1, col + 1);
        if (row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return null;
        RebuildComedyClubSeatingChart();
        var rt = FetchCellRectTransformVIP(row, col);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaCell>() : null;
    }

    private void GrowTableToFitLikeElasticPants(int wantRows, int wantCols)
    {
        int addRows = Mathf.Max(0, wantRows - totalRowsCountLetTheShowBegin);
        int addCols = Mathf.Max(0, wantCols - totalColumnsCountHighFive);
        if (addRows == 0 && addCols == 0) return;
        ConvertAllSpecsToPercentages();
        for (int i = 0; i < addRows; i++) InsertRowLikeANinja(totalRowsCountLetTheShowBegin);
        for (int i = 0; i < addCols; i++) InsertColumnLikeANinja(totalColumnsCountHighFive);
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private bool IsThisRootDecoratorOnTheGuestList(Transform t)
    {
        return Ux_TonkersTableTopiaHierarchyRules.IsColumnBackdropHost(t);
    }

    private RectTransform EnsureColumnBackdropHostLikeWallpaperCrew()
    {
        if (_rt == null)
        {
            _rt = GetComponent<RectTransform>();
        }

        if (_rt == null)
        {
            return null;
        }

        if (_columnBackdropHostRT == null || _columnBackdropHostRT.parent != _rt)
        {
            _columnBackdropHostRT = null;

            for (int i = 0; i < _rt.childCount; i++)
            {
                RectTransform child = _rt.GetChild(i) as RectTransform;
                if (child != null && Ux_TonkersTableTopiaHierarchyRules.IsColumnBackdropHost(child))
                {
                    _columnBackdropHostRT = child;
                    break;
                }
            }
        }

        if (_columnBackdropHostRT == null)
        {
            GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject("cbgh");
            _columnBackdropHostRT = go.GetComponent<RectTransform>();
            Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaColumnBackdropHost>(go);
            Ux_TonkersTableTopiaObjectUtility.SetParent(_columnBackdropHostRT, _rt, "Create Column Backdrop Host");
            Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(go, "Create Column Backdrop Host");
        }

        _columnBackdropHostRT.gameObject.name = "cbgh";
        _columnBackdropHostRT.anchorMin = Vector2.zero;
        _columnBackdropHostRT.anchorMax = Vector2.one;
        _columnBackdropHostRT.pivot = new Vector2(0.5f, 0.5f);
        _columnBackdropHostRT.offsetMin = Vector2.zero;
        _columnBackdropHostRT.offsetMax = Vector2.zero;
        _columnBackdropHostRT.anchoredPosition = Vector2.zero;
        _columnBackdropHostRT.SetSiblingIndex(0);
        return _columnBackdropHostRT;
    }

    private RectTransform PullColumnBackdropFromScratchLikeSherlock(int columnIndex)
    {
        for (int i = 0; i < _columnBackdropScratch.Count; i++)
        {
            var rt = _columnBackdropScratch[i];
            if (rt == null) continue;
            var tag = rt.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>();
            if (tag == null || tag.columnNumberPrimeRib != columnIndex) continue;
            _columnBackdropScratch.RemoveAt(i);
            return rt;
        }
        return null;
    }

    private RectTransform CreateColumnBackdropLikeWallpaperStrip(RectTransform host, int columnIndex)
    {
        if (host == null)
        {
            return null;
        }

        GameObject go = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(GetColumnRuntimeName(columnIndex));
        RectTransform rect = go.GetComponent<RectTransform>();
        Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaColumnBackdrop>(go);
        Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaColumn>(go);
        Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Image>(go);
        Ux_TonkersTableTopiaObjectUtility.SetParent(rect, host, "Create Column");
        Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(go, "Create Column");
        return rect;
    }

    private void DestroyColumnBackdropLikeExpiredCoupon(RectTransform rt)
    {
        if (rt == null)
        {
            return;
        }

        Ux_TonkersTableTopiaObjectUtility.DestroyObject(rt.gameObject, "Destroy Column Backdrop");
    }

    private void RefreshColumnBackdropLayerLikeWallpaperCrew(float[] baseColumnWidths, float innerHeightPlayable)
    {
        SyncColumnWardrobes();

#if UNITY_EDITOR
        if (!Application.isPlaying && UnityEditor.EditorUtility.IsPersistent(gameObject))
        {
            return;
        }
#endif

        if (_backstageColumnRectsVIP.Count != totalColumnsCountHighFive || _backstageColumnCompsVIP.Count != totalColumnsCountHighFive)
        {
            RebuildManagedColumns();
        }

        float x = comfyPaddingLeftForElbows;

        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            float columnWidth = baseColumnWidths != null && c < baseColumnWidths.Length ? Mathf.Max(0f, baseColumnWidths[c]) : 0f;
            ColumnStyle style = c < fancyColumnWardrobes.Count ? fancyColumnWardrobes[c] : null;
            bool needBackdrop = style != null && style.backdropPictureOnTheHouse != null && style.useOneBigBackdropForWholeColumn;

            int spanCols = needBackdrop ? Mathf.Max(1, GetColumnBackdropSpanLikeWallpaperCrew(c)) : 1;
            int endColExclusive = Mathf.Min(totalColumnsCountHighFive, c + spanCols);

            float backdropWidth = 0f;
            for (int spanCol = c; spanCol < endColExclusive; spanCol++)
            {
                backdropWidth += baseColumnWidths != null && spanCol < baseColumnWidths.Length ? Mathf.Max(0f, baseColumnWidths[spanCol]) : 0f;
            }

            if (spanCols > 1)
            {
                backdropWidth += sociallyDistancedColumnsPixels * (spanCols - 1);
            }

            RectTransform backdrop = c < _backstageColumnRectsVIP.Count ? _backstageColumnRectsVIP[c] : null;
            Ux_TonkersTableTopiaColumn column = c < _backstageColumnCompsVIP.Count ? _backstageColumnCompsVIP[c] : null;

            if (backdrop != null)
            {
                backdrop.gameObject.SetActive(true);
                backdrop.gameObject.name = GetColumnRuntimeName(c);
                backdrop.SetSiblingIndex(c);

                Ux_TonkersTableTopiaColumnBackdrop tag = backdrop.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>();
                if (tag != null)
                {
                    tag.columnNumberPrimeRib = c;
                }

                ApplyRectPlacementWithPanache(
                    backdrop,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    x,
                    -comfyPaddingTopHat,
                    backdropWidth,
                    innerHeightPlayable,
                    _rt.rect.width,
                    _rt.rect.height);
            }

            if (column != null)
            {
                column.columnNumberPrimeRib = c;
                column.SetCachedBackgroundVisual(
                    needBackdrop,
                    needBackdrop ? style.backdropPictureOnTheHouse : null,
                    needBackdrop ? style.backdropTintFlavor : Color.white,
                    needBackdrop && style.backdropUseSlicedLikePizza);
            }

            x += columnWidth + sociallyDistancedColumnsPixels;
        }
    }

    public int GetColumnBackdropSpanLikeWallpaperCrew(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= totalColumnsCountHighFive) return 1;

        int maxEndColumn = columnIndex;

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            if (!this.TryPeekMainCourseLikeABuffet(r, columnIndex, out _, out var mainCol, out var mainCell)) continue;
            if (mainCell == null) continue;
            if (mainCol != columnIndex) continue;

            int spanCols = Mathf.Max(1, mainCell.howManyColumnsAreSneakingIn);
            int endColumn = Mathf.Min(totalColumnsCountHighFive - 1, mainCol + spanCols - 1);

            if (endColumn > maxEndColumn) maxEndColumn = endColumn;
        }

        return maxEndColumn - columnIndex + 1;
    }

    public float GetLiveColumnWidthPixelsLikeTapeMeasure(int columnIndex)
    {
        EnsureWardrobeListsMatchHeadcount();
        var rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        if (rt == null) return 0f;

        columnIndex = Mathf.Clamp(columnIndex, 0, Mathf.Max(0, totalColumnsCountHighFive - 1));

        float innerWidthPlayable = Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        var widths = CalculateBaseColumnWidths(innerWidthPlayable);

        if (widths == null || columnIndex >= widths.Length) return 0f;
        return Mathf.Max(0f, widths[columnIndex]);
    }

    public float GetLiveRowHeightPixelsLikeTapeMeasure(int rowIndex)
    {
        EnsureWardrobeListsMatchHeadcount();
        var rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        if (rt == null) return 0f;

        rowIndex = Mathf.Clamp(rowIndex, 0, Mathf.Max(0, totalRowsCountLetTheShowBegin - 1));

        var heights = CalculateRowHeightsLikeAShortOrderCook(rt);

        if (heights == null || rowIndex >= heights.Length) return 0f;
        return Mathf.Max(0f, heights[rowIndex]);
    }

    private void RecordFullHierarchyUndoLikeParanoid(string actionName)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) UnityEditor.Undo.RegisterFullObjectHierarchyUndo(gameObject, actionName);
#endif
    }

    public void CaptureCurrentRectAsDesignSizeLikeBlueprint()
    {
        if (_rt == null) _rt = GetComponent<RectTransform>();
        if (_rt == null) return;

        var size = _rt.rect.size;
        if (size.x <= 0f || size.y <= 0f) return;

        designSizeForThisTableLikeBlueprint = size;
    }

    public float GetDesignInnerWidthLikeBlueprint()
    {
        if (designSizeForThisTableLikeBlueprint.x <= 0f) CaptureCurrentRectAsDesignSizeLikeBlueprint();
        return Mathf.Max(0f, designSizeForThisTableLikeBlueprint.x - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
    }

    public float GetDesignInnerHeightLikeBlueprint()
    {
        if (designSizeForThisTableLikeBlueprint.y <= 0f) CaptureCurrentRectAsDesignSizeLikeBlueprint();
        return Mathf.Max(0f, designSizeForThisTableLikeBlueprint.y - comfyPaddingTopHat - comfyPaddingBottomCushion);
    }

    public float ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(float rawSpec, float currentInnerWidth)
    {
        if (rawSpec <= 0f || !scaleFixedSizesWithResolutionLikeBlueprint) return rawSpec;

        float designInnerWidth = GetDesignInnerWidthLikeBlueprint();
        if (designInnerWidth <= 0.0001f || currentInnerWidth <= 0.0001f) return rawSpec;

        return rawSpec * (currentInnerWidth / designInnerWidth);
    }

    public float ResolveRowSpecForCurrentInnerHeightLikeBlueprint(float rawSpec, float currentInnerHeight)
    {
        if (rawSpec <= 0f || !scaleFixedSizesWithResolutionLikeBlueprint) return rawSpec;

        float designInnerHeight = GetDesignInnerHeightLikeBlueprint();
        if (designInnerHeight <= 0.0001f || currentInnerHeight <= 0.0001f) return rawSpec;

        return rawSpec * (currentInnerHeight / designInnerHeight);
    }

    public float ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(float currentPixels, float currentInnerWidth)
    {
        if (!scaleFixedSizesWithResolutionLikeBlueprint) return Mathf.Max(0f, currentPixels);

        float designInnerWidth = GetDesignInnerWidthLikeBlueprint();
        if (designInnerWidth <= 0.0001f || currentInnerWidth <= 0.0001f) return Mathf.Max(0f, currentPixels);

        return Mathf.Max(0f, currentPixels * (designInnerWidth / currentInnerWidth));
    }

    public float ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(float currentPixels, float currentInnerHeight)
    {
        if (!scaleFixedSizesWithResolutionLikeBlueprint) return Mathf.Max(0f, currentPixels);

        float designInnerHeight = GetDesignInnerHeightLikeBlueprint();
        if (designInnerHeight <= 0.0001f || currentInnerHeight <= 0.0001f) return Mathf.Max(0f, currentPixels);

        return Mathf.Max(0f, currentPixels * (designInnerHeight / currentInnerHeight));
    }

    private static string GetRowRuntimeName(int rowIndex)
    {
        return "r" + rowIndex;
    }

    private static string GetCellRuntimeName(int rowIndex, int columnIndex)
    {
        return "c" + rowIndex + "_" + columnIndex;
    }

    private static string GetColumnBackdropRuntimeName(int columnIndex)
    {
        return "cbg" + columnIndex;
    }

    private static bool NearlyEqualVector2(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) < 0.0001f && Mathf.Abs(a.y - b.y) < 0.0001f;
    }

    private List<Ux_TonkersTableTopiaCell> GetOrCreateCellCompCacheRow(int rowIndex)
    {
        while (_backstageCellCompsVIP.Count <= rowIndex)
        {
            _backstageCellCompsVIP.Add(new List<Ux_TonkersTableTopiaCell>(64));
        }

        return _backstageCellCompsVIP[rowIndex];
    }

    private List<RectTransform> GetOrCreateCellRectCacheRow(int rowIndex)
    {
        while (backstageCellRectsVIP.Count <= rowIndex)
        {
            backstageCellRectsVIP.Add(new List<RectTransform>(64));
        }

        return backstageCellRectsVIP[rowIndex];
    }

    private void RefreshCanvasCache()
    {
        _cachedCanvas = GetComponentInParent<Canvas>(true);
    }

    private Vector2 GetCachedCanvasScale()
    {
        if (_cachedCanvas == null)
        {
            RefreshCanvasCache();
        }
        if (_cachedCanvas == null)
        {
            return Vector2.one;
        }
        Canvas rootCanvas = _cachedCanvas.rootCanvas != null ? _cachedCanvas.rootCanvas : _cachedCanvas;
        return rootCanvas.pixelRect.size;
    }

    private static void FlushRuntimeDirtyLayouts()
    {
        for (int i = 0; i < _runtimeDirtyLayouts.Count; i++)
        {
            Ux_TonkersTableTopiaLayout layout = _runtimeDirtyLayouts[i];
            if (layout == null)
            {
                continue;
            }

            layout._runtimeLayoutQueued = false;

            if (!layout.isActiveAndEnabled)
            {
                continue;
            }

            RectTransform layoutRect = layout._rt;
            if (layoutRect == null)
            {
                continue;
            }

            Vector2 currentRectSize = layoutRect.rect.size;
            Vector2 currentCanvasScale = layout.GetCachedCanvasScale();

            if (!layout._structureDirty &&
                !layout.layoutNeedsAFreshCoatOfPaint &&
                NearlyEqualVector2(layout._lastAppliedRectSize, currentRectSize) &&
                NearlyEqualVector2(layout._lastAppliedCanvasScale, currentCanvasScale))
            {
                continue;
            }

            layout.UpdateSeatingLikeAProUsher();
            layout.layoutNeedsAFreshCoatOfPaint = false;
            layout._structureDirty = false;
            layout._lastAppliedRectSize = layoutRect.rect.size;
            layout._lastAppliedCanvasScale = layout.GetCachedCanvasScale();
        }

        _runtimeDirtyLayouts.Clear();
    }

    private void QueueRuntimeLayout()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_runtimeLayoutQueued)
        {
            return;
        }

        if (!_runtimeDirtyLayoutsHooked)
        {
            _runtimeDirtyLayoutsHooked = true;
            Canvas.willRenderCanvases -= FlushRuntimeDirtyLayouts;
            Canvas.willRenderCanvases += FlushRuntimeDirtyLayouts;
        }

        _runtimeLayoutQueued = true;
        _runtimeDirtyLayouts.Add(this);
    }

    private float CalculateInnerContentWidthLikeSnackLine(float[] columnWidths)
    {
        float total = 0f;
        for (int c = 0; c < totalColumnsCountHighFive; c++) total += columnWidths[c];
        if (totalColumnsCountHighFive > 1) total += sociallyDistancedColumnsPixels * (totalColumnsCountHighFive - 1);
        return total;
    }

    private float CalculateTotalContentWidthCalories(float[] columnWidths)
    {
        return comfyPaddingLeftForElbows + comfyPaddingRightForElbows + CalculateInnerContentWidthLikeSnackLine(columnWidths);
    }

    private float CalculateHorizontalGrandEntrance(float innerWidthPlayable, float totalInnerContentWidth)
    {
        if (innerWidthPlayable <= totalInnerContentWidth) return 0f;

        float extraSpace = innerWidthPlayable - totalInnerContentWidth;
        switch (horizontalSchmoozingPreference)
        {
            case HorizontalAlignment.Center:
                return extraSpace * 0.5f;

            case HorizontalAlignment.Right:
                return extraSpace;

            default:
                return 0f;
        }
    }

    private bool AreWeStretchingHorizontally(RectTransform rt)
    {
        return Mathf.Abs(rt.anchorMin.x - rt.anchorMax.x) > 0.001f;
    }

    private int FindLastVisibleMainCellColumnInRow(int rowNumberIndex)
    {
        if (rowNumberIndex < 0 || rowNumberIndex >= _backstageCellCompsVIP.Count) return -1;

        List<Ux_TonkersTableTopiaCell> rowComps = _backstageCellCompsVIP[rowNumberIndex];
        List<RectTransform> rowRects = rowNumberIndex < backstageCellRectsVIP.Count ? backstageCellRectsVIP[rowNumberIndex] : null;
        if (rowComps == null || rowRects == null) return -1;

        for (int c = totalColumnsCountHighFive - 1; c >= 0; c--)
        {
            if (c >= rowComps.Count || c >= rowRects.Count) continue;

            Ux_TonkersTableTopiaCell cell = rowComps[c];
            RectTransform rt = rowRects[c];
            if (cell == null || rt == null) continue;
            if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null) continue;
            if (!rt.gameObject.activeInHierarchy) continue;

            return c;
        }

        return -1;
    }

    private void EnsurePercentConversionScratchCapacityLikeReceipt(int columnCount, int rowCount)
    {
        if (_originalColumnSpecsScratch.Length < columnCount) Array.Resize(ref _originalColumnSpecsScratch, columnCount);
        if (_originalRowSpecsScratch.Length < rowCount) Array.Resize(ref _originalRowSpecsScratch, rowCount);
        if (_wasFixedColumnScratch.Length < columnCount) Array.Resize(ref _wasFixedColumnScratch, columnCount);
        if (_wasFlexColumnScratch.Length < columnCount) Array.Resize(ref _wasFlexColumnScratch, columnCount);
        if (_wasFixedRowScratch.Length < rowCount) Array.Resize(ref _wasFixedRowScratch, rowCount);
        if (_wasFlexRowScratch.Length < rowCount) Array.Resize(ref _wasFlexRowScratch, rowCount);
    }

    private void ApplyDefaultRootRectLikeReferenceSpec()
    {
        if (_rt == null)
        {
            _rt = GetComponent<RectTransform>();
        }

        if (_rt == null)
        {
            return;
        }

        // Full-stretch root like the reference dump.
        _rt.anchorMin = Vector2.zero;
        _rt.anchorMax = Vector2.one;
        _rt.pivot = new Vector2(0.5f, 0.5f);
        _rt.anchoredPosition = Vector2.zero;
        _rt.sizeDelta = Vector2.zero;
        _rt.localScale = Vector3.one;
        _rt.localRotation = Quaternion.identity;
    }

    private void ApplyDefaultTonkersTablePresetLikeReferenceSpec()
    {
        scaleFixedSizesWithResolutionLikeBlueprint = true;
        designSizeForThisTableLikeBlueprint = s_DefaultDesignSizeLikeBlueprint;
        totalRowsCountLetTheShowBegin = 3;
        totalColumnsCountHighFive = 3;
        comfyPaddingLeftForElbows = 0f;
        comfyPaddingRightForElbows = 0f;
        comfyPaddingTopHat = 0f;
        comfyPaddingBottomCushion = 0f;
        toggleZebraStripesForRows = true;
        toggleZebraStripesForColumns = true;
        zebraRowColorA = s_DefaultZebraRowColorA;
        zebraRowColorB = s_DefaultZebraRowColorB;
        zebraColumnColorA = s_DefaultZebraColumnColorA;
        zebraColumnColorB = s_DefaultZebraColumnColorB;
        sociallyDistancedColumnsPixels = 0f;
        sociallyDistancedRowsPixels = 0f;
        shareThePieEvenlyForColumns = false;
        shareThePieEvenlyForRows = false;
        autoHugWidthLikeAGoodFriend = true;
        autoHugHeightBecauseWhyNot = true;
        horizontalSchmoozingPreference = HorizontalAlignment.Left;
        verticalSchmoozingPreference = VerticalAlignment.Top;
        autoHireContentSizerBecauseLazy = false;

        if (fancyColumnWardrobes == null)
        {
            fancyColumnWardrobes = new List<ColumnStyle>(3);
        }
        else
        {
            fancyColumnWardrobes.Clear();
        }

        fancyColumnWardrobes.Add(new ColumnStyle
        {
            requestedWidthMaybePercentIfNegative = -0.25f,
            useOneBigBackdropForWholeColumn = true,
            backdropUseSlicedLikePizza = true
        });
        fancyColumnWardrobes.Add(new ColumnStyle
        {
            requestedWidthMaybePercentIfNegative = -0.5f,
            useOneBigBackdropForWholeColumn = true,
            backdropUseSlicedLikePizza = true
        });
        fancyColumnWardrobes.Add(new ColumnStyle
        {
            requestedWidthMaybePercentIfNegative = -0.25f,
            useOneBigBackdropForWholeColumn = true,
            backdropUseSlicedLikePizza = true
        });

        if (snazzyRowWardrobes == null)
        {
            snazzyRowWardrobes = new List<RowStyle>(3);
        }
        else
        {
            snazzyRowWardrobes.Clear();
        }

        snazzyRowWardrobes.Add(new RowStyle
        {
            requestedHeightMaybePercentIfNegative = -0.25f,
            lastVisibleCellEatsLeftovers = true,
            backdropUseSlicedLikePizza = true
        });
        snazzyRowWardrobes.Add(new RowStyle
        {
            requestedHeightMaybePercentIfNegative = -0.5f,
            lastVisibleCellEatsLeftovers = true,
            backdropUseSlicedLikePizza = true
        });
        snazzyRowWardrobes.Add(new RowStyle
        {
            requestedHeightMaybePercentIfNegative = -0.25f,
            lastVisibleCellEatsLeftovers = true,
            backdropUseSlicedLikePizza = true
        });

        ApplyDefaultRootRectLikeReferenceSpec();
    }

    private bool IsVisibleMainCellLikeSeatbelt(Ux_TonkersTableTopiaCell cell, RectTransform rt)
    {
        return cell != null && rt != null && !(cell.isMashedLikePotatoes && cell.mashedIntoWho != null) && rt.gameObject.activeInHierarchy;
    }

    private void OnCanvasHierarchyChanged()
    {
        RefreshCanvasCache();
        MarkLayoutDirty();
        RequestLayoutRefresh();
    }

    public Ux_TonkersTableTopiaRow FetchRowComponentVIP(int index)
    {
        if (index < 0 || index >= totalRowsCountLetTheShowBegin)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        return index < _backstageRowCompsVIP.Count ? _backstageRowCompsVIP[index] : null;
    }

    public Ux_TonkersTableTopiaCell FetchCellComponentVIP(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        if (row >= _backstageCellCompsVIP.Count)
        {
            return null;
        }

        List<Ux_TonkersTableTopiaCell> rowComps = _backstageCellCompsVIP[row];
        if (rowComps == null || col >= rowComps.Count)
        {
            return null;
        }

        return rowComps[col];
    }

    private void PrepareStructureCachesForRebuild()
    {
        if (totalColumnsCountHighFive < 1)
        {
            totalColumnsCountHighFive = 1;
        }

        if (totalRowsCountLetTheShowBegin < 1)
        {
            totalRowsCountLetTheShowBegin = 1;
        }

        backstageRowRectsVIP.Clear();
        _backstageRowCompsVIP.Clear();
        _backstageColumnRectsVIP.Clear();
        _backstageColumnCompsVIP.Clear();

        for (int i = 0; i < backstageCellRectsVIP.Count; i++)
        {
            backstageCellRectsVIP[i].Clear();
        }

        for (int i = 0; i < _backstageCellCompsVIP.Count; i++)
        {
            _backstageCellCompsVIP[i].Clear();
        }
    }

    private void CollectRootChildren(RectTransform tableRectTransformMainStage)
    {
        _rootChildrenRoundup.Clear();

        for (int i = 0; i < tableRectTransformMainStage.childCount; i++)
        {
            RectTransform childRect = tableRectTransformMainStage.GetChild(i) as RectTransform;
            if (childRect != null)
            {
                _rootChildrenRoundup.Add(childRect);
            }
        }
    }

    private void SplitRootChildrenIntoManagedAndForeign()
    {
        _managedRowsLine.Clear();
        _nonManagedRootBackpack.Clear();

        for (int i = 0; i < _rootChildrenRoundup.Count; i++)
        {
            RectTransform childRect = _rootChildrenRoundup[i];
            if (childRect == null)
            {
                continue;
            }

            if (Ux_TonkersTableTopiaHierarchyRules.IsManagedRow(childRect))
            {
                _managedRowsLine.Add(childRect);
                continue;
            }

            if (Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(childRect))
            {
                _nonManagedRootBackpack.Add(childRect);
            }
        }
    }

    private void EnsureManagedRowsExist(RectTransform tableRectTransformMainStage)
    {
        while (_managedRowsLine.Count < totalRowsCountLetTheShowBegin)
        {
            RectTransform rowRect = CreateManagedRow(_managedRowsLine.Count);
            ParentTableChild(rowRect, tableRectTransformMainStage);
            _managedRowsLine.Add(rowRect);
        }
    }

    private RectTransform CreateManagedRow(int rowIndex)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject rowObject = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(GetRowRuntimeName(rowIndex));
            RectTransform rowRect = rowObject.GetComponent<RectTransform>();
            Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaRow>(rowObject);
            Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(rowObject, "Add Tonkers Table Topia Row");
            return rowRect;
        }
#endif

        return RentRowRT();
    }

    private void RebuildManagedRow(int rowIndex)
    {
        RectTransform rowRect = _managedRowsLine[rowIndex];
        rowRect.name = GetRowRuntimeName(rowIndex);

        Ux_TonkersTableTopiaRow rowComponent = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaRow>(rowRect.gameObject);
        rowComponent.rowNumberWhereShenanigansOccur = rowIndex;

        CollectRowChildren(rowRect);
        SplitRowChildrenIntoManagedAndForeign();
        EnsureManagedCellsExist(rowRect, rowIndex);

        RectTransform foreignTargetCell = _managedCellsLine.Count > 0 ? _managedCellsLine[0] : null;
        if (foreignTargetCell != null && _nonManagedRowBackpack.Count > 0)
        {
            EscortNonVIPsToTarget(rowRect, foreignTargetCell);
        }

        TrimExtraManagedCells(foreignTargetCell);
        CacheManagedRow(rowIndex, rowRect, rowComponent);
    }

    private void CollectRowChildren(RectTransform rowRect)
    {
        _rowChildrenRoundup.Clear();

        for (int i = 0; i < rowRect.childCount; i++)
        {
            RectTransform childRect = rowRect.GetChild(i) as RectTransform;
            if (childRect != null)
            {
                _rowChildrenRoundup.Add(childRect);
            }
        }
    }

    private void SplitRowChildrenIntoManagedAndForeign()
    {
        _managedCellsLine.Clear();
        _nonManagedRowBackpack.Clear();

        for (int i = 0; i < _rowChildrenRoundup.Count; i++)
        {
            RectTransform childRect = _rowChildrenRoundup[i];
            if (childRect == null)
            {
                continue;
            }

            if (Ux_TonkersTableTopiaHierarchyRules.IsManagedCell(childRect))
            {
                _managedCellsLine.Add(childRect);
                continue;
            }

            if (Ux_TonkersTableTopiaHierarchyRules.IsForeignContent(childRect))
            {
                _nonManagedRowBackpack.Add(childRect);
            }
        }
    }

    private void EnsureManagedCellsExist(RectTransform rowRect, int rowIndex)
    {
        while (_managedCellsLine.Count < totalColumnsCountHighFive)
        {
            RectTransform cellRect = CreateManagedCell(rowIndex, _managedCellsLine.Count);
            ParentTableChild(cellRect, rowRect);
            _managedCellsLine.Add(cellRect);
        }
    }

    private RectTransform CreateManagedCell(int rowIndex, int columnIndex)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject cellObject = Ux_TonkersTableTopiaObjectUtility.CreateUiObject(GetCellRuntimeName(rowIndex, columnIndex));
            RectTransform cellRect = cellObject.GetComponent<RectTransform>();
            Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaCell>(cellObject);
            Ux_TonkersTableTopiaObjectUtility.RegisterCreatedObject(cellObject, "Add Tonkers Table Topia Cell");
            return cellRect;
        }
#endif

        return RentCellRT();
    }

    private void TrimExtraManagedCells(RectTransform foreignTargetCell)
    {
        for (int i = _managedCellsLine.Count - 1; i >= totalColumnsCountHighFive; i--)
        {
            RectTransform extraCellRect = _managedCellsLine[i];

            if (foreignTargetCell != null)
            {
                EscortNonVIPsToTarget(extraCellRect, foreignTargetCell);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.DestroyObjectImmediate(extraCellRect.gameObject);
            }
            else
#endif
            {
                ReturnCellRT(extraCellRect);
            }

            _managedCellsLine.RemoveAt(i);
        }
    }

    private void CacheManagedRow(int rowIndex, RectTransform rowRect, Ux_TonkersTableTopiaRow rowComponent)
    {
        List<RectTransform> rectCacheRow = GetOrCreateCellRectCacheRow(rowIndex);
        List<Ux_TonkersTableTopiaCell> compCacheRow = GetOrCreateCellCompCacheRow(rowIndex);
        rectCacheRow.Clear();
        compCacheRow.Clear();

        for (int columnIndex = 0; columnIndex < totalColumnsCountHighFive; columnIndex++)
        {
            RectTransform cellRect = _managedCellsLine[columnIndex];
            cellRect.name = GetCellRuntimeName(rowIndex, columnIndex);

            Ux_TonkersTableTopiaCell cellComp = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaCell>(cellRect.gameObject);
            cellComp.rowNumberWhereThePartyIs = rowIndex;
            cellComp.columnNumberPrimeRib = columnIndex;
            cellComp.EnsureCachedContentSizeFitter(autoHireContentSizerBecauseLazy);

            bool shouldBeActiveBecauseNotMashed = !(cellComp.isMashedLikePotatoes && cellComp.mashedIntoWho != null);
            if (cellRect.gameObject.activeSelf != shouldBeActiveBecauseNotMashed)
            {
                cellRect.gameObject.SetActive(shouldBeActiveBecauseNotMashed);
            }

            rectCacheRow.Add(cellRect);
            compCacheRow.Add(cellComp);
        }

        backstageRowRectsVIP.Add(rowRect);
        _backstageRowCompsVIP.Add(rowComponent);
    }

    private void TrimExtraManagedRows()
    {
        if (_managedRowsLine.Count <= totalRowsCountLetTheShowBegin)
        {
            return;
        }

        RectTransform targetCell = FetchCellRectTransformVIP(0, 0);

        for (int rowIndex = _managedRowsLine.Count - 1; rowIndex >= totalRowsCountLetTheShowBegin; rowIndex--)
        {
            RectTransform extraRow = _managedRowsLine[rowIndex];

            if (targetCell != null)
            {
                EscortNonVIPsToTarget(extraRow, targetCell);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.DestroyObjectImmediate(extraRow.gameObject);
            }
            else
#endif
            {
                ReturnRowRT(extraRow);
            }
        }
    }

    private void MoveRootForeignChildrenToFirstCell(RectTransform tableRectTransformMainStage)
    {
        if (_nonManagedRootBackpack.Count == 0)
        {
            return;
        }

        RectTransform target = FetchCellRectTransformVIP(0, 0);
        if (target != null)
        {
            EscortNonVIPsToTarget(tableRectTransformMainStage, target);
        }
    }

    private void MoveForeignChildrenNow(Transform target)
    {
        for (int i = 0; i < _stowawaysOnThisCrazyTrain.Count; i++)
        {
            MoveForeignChildPreservingLayout(_stowawaysOnThisCrazyTrain[i], target);
        }
    }

    private void MoveForeignChildPreservingLayout(Transform child, Transform target)
    {
        if (child == null || target == null)
        {
            return;
        }

        RectTransform childRect = child as RectTransform;
        Ux_TonkersTableTopiaRectTransformSnapshot snapshot = Ux_TonkersTableTopiaRectTransformSnapshot.Capture(childRect);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.SetTransformParent(child, target, "Move Guests With Style");
            if (childRect != null)
            {
                UnityEditor.Undo.RecordObject(childRect, "Move Guests With Style");
            }

            ApplyForeignChildLayoutAfterMove(childRect, snapshot);
            return;
        }
#endif

        child.SetParent(target, false);
        ApplyForeignChildLayoutAfterMove(childRect, snapshot);
    }

    private static void ApplyForeignChildLayoutAfterMove(RectTransform childRect, Ux_TonkersTableTopiaRectTransformSnapshot snapshot)
    {
        if (childRect == null)
        {
            return;
        }

        if (childRect.IsFullStretchLikeYoga())
        {
            childRect.SnapCroutonToFillParentLikeGravy();
            return;
        }

        snapshot.Restore(childRect);
    }

#if UNITY_EDITOR

    private void MoveForeignChildrenDeferred(Transform target)
    {
        for (int i = 0; i < _stowawaysOnThisCrazyTrain.Count; i++)
        {
            Transform childLocal = _stowawaysOnThisCrazyTrain[i];
            Transform targetLocal = target;
            RectTransform childRect = childLocal as RectTransform;
            Ux_TonkersTableTopiaRectTransformSnapshot snapshot = Ux_TonkersTableTopiaRectTransformSnapshot.Capture(childRect);

            EditorApplication.delayCall += () =>
            {
                if (childLocal == null || targetLocal == null)
                {
                    return;
                }

                RectTransform delayedRect = childLocal as RectTransform;
                UnityEditor.Undo.SetTransformParent(childLocal, targetLocal, "Move Guests With Style");
                if (delayedRect != null)
                {
                    UnityEditor.Undo.RecordObject(delayedRect, "Move Guests With Style");
                }

                ApplyForeignChildLayoutAfterMove(delayedRect, snapshot);
            };
        }
    }

#endif

    private void InitializeCachedDependencies()
    {
        if (_rt == null)
            _rt = GetComponent<RectTransform>();

        if (_rt != null && (_pool == null || !_pool.IsFor(_rt)))
            _pool = new Ux_TablePool(_rt);

        RefreshCanvasCache();
    }

    private void EnsureDesignSizeInitialized()
    {
        if (designSizeForThisTableLikeBlueprint.x > 0f && designSizeForThisTableLikeBlueprint.y > 0f)
        {
            return;
        }

        CaptureCurrentRectAsDesignSizeLikeBlueprint();

        if (designSizeForThisTableLikeBlueprint.x <= 0f || designSizeForThisTableLikeBlueprint.y <= 0f)
        {
            designSizeForThisTableLikeBlueprint = s_DefaultDesignSizeLikeBlueprint;
        }
    }

    private void MarkStructureDirty()
    {
        _structureDirty = true;
        layoutNeedsAFreshCoatOfPaint = true;
    }

    private void MarkLayoutDirty()
    {
        layoutNeedsAFreshCoatOfPaint = true;
    }

    private void RequestLayoutRefresh()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            ScheduleEditorLayoutTouchUp();
            return;
        }
#endif

        QueueRuntimeLayout();
    }

#if UNITY_EDITOR

    private void ApplyEditorLayoutIfNeeded()
    {
        if (Application.isPlaying || !layoutNeedsAFreshCoatOfPaint)
        {
            return;
        }

        UpdateSeatingLikeAProUsher();
        layoutNeedsAFreshCoatOfPaint = false;
        _structureDirty = false;
        _lastAppliedRectSize = _rt != null ? _rt.rect.size : Vector2.zero;
        _lastAppliedCanvasScale = GetCachedCanvasScale();
    }

#endif

    private bool HasRuntimeLayoutChange()
    {
        if (_rt == null)
        {
            return false;
        }

        Vector2 currentRectSize = _rt.rect.size;
        Vector2 currentCanvasScale = GetCachedCanvasScale();

        return !NearlyEqualVector2(_lastAppliedRectSize, currentRectSize)
            || !NearlyEqualVector2(_lastAppliedCanvasScale, currentCanvasScale);
    }

    private void CacheColumnSpecState(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float spec = i < fancyColumnWardrobes.Count ? fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f;
            _originalColumnSpecsScratch[i] = spec;
            _wasFixedColumnScratch[i] = spec > 0f;
            _wasFlexColumnScratch[i] = Mathf.Approximately(spec, 0f);
        }
    }

    private void CacheRowSpecState(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float spec = i < snazzyRowWardrobes.Count ? snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f;
            _originalRowSpecsScratch[i] = spec;
            _wasFixedRowScratch[i] = spec > 0f;
            _wasFlexRowScratch[i] = Mathf.Approximately(spec, 0f);
        }
    }

    private void ApplyConvertedColumnSpecs(int count, float innerWidth)
    {
        GetAvailablePercentageSpace(
            count,
            _colWidthsBuf,
            _wasFixedColumnScratch,
            _wasFlexColumnScratch,
            sociallyDistancedColumnsPixels,
            innerWidth,
            out float available,
            out bool hasFlex);

        float percentageSum = 0f;

        for (int i = 0; i < count; i++)
        {
            ColumnStyle style = fancyColumnWardrobes[i];

            if (_wasFixedColumnScratch[i])
            {
                style.requestedWidthMaybePercentIfNegative = Mathf.Max(0f, _originalColumnSpecsScratch[i]);
                continue;
            }

            if (_wasFlexColumnScratch[i])
            {
                style.requestedWidthMaybePercentIfNegative = 0f;
                continue;
            }

            float percentage = available > 0f ? Mathf.Clamp01(_colWidthsBuf[i] / available) : 0f;
            style.requestedWidthMaybePercentIfNegative = percentage > 0f ? -percentage : 0f;
            percentageSum += percentage;
        }

        if (!hasFlex && percentageSum > 1.0001f)
            NormalizeColumnPercentageSpecs();
    }

    private void ApplyConvertedRowSpecs(int count, float innerHeight)
    {
        GetAvailablePercentageSpace(
            count,
            _rowHeightsBuf,
            _wasFixedRowScratch,
            _wasFlexRowScratch,
            sociallyDistancedRowsPixels,
            innerHeight,
            out float available,
            out bool hasFlex);

        float percentageSum = 0f;

        for (int i = 0; i < count; i++)
        {
            RowStyle style = snazzyRowWardrobes[i];

            if (_wasFixedRowScratch[i])
            {
                style.requestedHeightMaybePercentIfNegative = Mathf.Max(0f, _originalRowSpecsScratch[i]);
                continue;
            }

            if (_wasFlexRowScratch[i])
            {
                style.requestedHeightMaybePercentIfNegative = 0f;
                continue;
            }

            float percentage = available > 0f ? Mathf.Clamp01(_rowHeightsBuf[i] / available) : 0f;
            style.requestedHeightMaybePercentIfNegative = percentage > 0f ? -percentage : 0f;
            percentageSum += percentage;
        }

        if (!hasFlex && percentageSum > 1.0001f)
            NormalizeRowPercentageSpecs();
    }

    private static void GetAvailablePercentageSpace(
        int count,
        float[] resolvedSizes,
        bool[] fixedMask,
        bool[] flexMask,
        float spacing,
        float innerSize,
        out float available,
        out bool hasFlex)
    {
        float fixedSum = 0f;
        hasFlex = false;

        for (int i = 0; i < count; i++)
        {
            if (fixedMask[i])
            {
                fixedSum += Mathf.Max(0f, resolvedSizes[i]);
                continue;
            }

            if (flexMask[i])
                hasFlex = true;
        }

        float spacingSum = spacing * Mathf.Max(0, count - 1);
        available = Mathf.Max(1f, innerSize - spacingSum - fixedSum);
    }

    private void NormalizeColumnPercentageSpecs()
    {
        Ux_TonkersTableTopiaDistributionUtility.NormalizeNegativePercentageSpecs(
            totalColumnsCountHighFive,
            i => fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative,
            (i, value) => fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = value);
    }

    private void NormalizeRowPercentageSpecs()
    {
        Ux_TonkersTableTopiaDistributionUtility.NormalizeNegativePercentageSpecs(
            totalRowsCountLetTheShowBegin,
            i => snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative,
            (i, value) => snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = value);
    }

    private static void ParentTableChild(RectTransform child, RectTransform parent)
    {
        if (child == null || parent == null)
        {
            return;
        }

        child.SetParent(parent, false);
        NormalizeTableChildTransform(child);
    }

    private static void NormalizeTableChildTransform(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;

        Vector3 localPosition = rectTransform.localPosition;
        localPosition.z = 0f;
        rectTransform.localPosition = localPosition;

        Vector3 anchoredPosition3D = rectTransform.anchoredPosition3D;
        anchoredPosition3D.z = 0f;
        rectTransform.anchoredPosition3D = anchoredPosition3D;
    }

    public RectTransform FetchColumnRectTransformVIP(int index)
    {
        if (index < 0 || index >= totalColumnsCountHighFive)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        return index < _backstageColumnRectsVIP.Count ? _backstageColumnRectsVIP[index] : null;
    }

    public Ux_TonkersTableTopiaColumn FetchColumnComponentVIP(int index)
    {
        if (index < 0 || index >= totalColumnsCountHighFive)
        {
            return null;
        }

        if (_structureDirty)
        {
            RebuildComedyClubSeatingChart();
        }

        return index < _backstageColumnCompsVIP.Count ? _backstageColumnCompsVIP[index] : null;
    }

    private void RebuildManagedColumns()
    {
        RectTransform host = EnsureColumnBackdropHostLikeWallpaperCrew();
        _backstageColumnRectsVIP.Clear();
        _backstageColumnCompsVIP.Clear();

        if (host == null)
        {
            return;
        }

        _columnBackdropScratch.Clear();

        for (int i = 0; i < host.childCount; i++)
        {
            RectTransform child = host.GetChild(i) as RectTransform;
            if (child == null)
            {
                continue;
            }

            if (child.GetComponent<Ux_TonkersTableTopiaColumn>() == null && child.GetComponent<Ux_TonkersTableTopiaColumnBackdrop>() == null)
            {
                continue;
            }

            _columnBackdropScratch.Add(child);
        }

        while (_columnBackdropScratch.Count < totalColumnsCountHighFive)
        {
            RectTransform columnRect = CreateColumnBackdropLikeWallpaperStrip(host, _columnBackdropScratch.Count);
            if (columnRect == null)
            {
                break;
            }

            _columnBackdropScratch.Add(columnRect);
        }

        int liveCount = Mathf.Min(totalColumnsCountHighFive, _columnBackdropScratch.Count);

        for (int c = 0; c < liveCount; c++)
        {
            RectTransform columnRect = _columnBackdropScratch[c];
            if (columnRect == null)
            {
                continue;
            }

            if (columnRect.parent != host)
            {
                Ux_TonkersTableTopiaObjectUtility.SetParent(columnRect, host, "Reparent Column");
            }

            columnRect.SetSiblingIndex(c);
            columnRect.gameObject.name = GetColumnRuntimeName(c);

            Ux_TonkersTableTopiaColumnBackdrop backdrop = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaColumnBackdrop>(columnRect.gameObject);
            if (backdrop != null)
            {
                backdrop.columnNumberPrimeRib = c;
            }

            Ux_TonkersTableTopiaColumn column = Ux_TonkersTableTopiaObjectUtility.GetOrAddComponent<Ux_TonkersTableTopiaColumn>(columnRect.gameObject);
            if (column != null)
            {
                column.columnNumberPrimeRib = c;
            }

            _backstageColumnRectsVIP.Add(columnRect);
            _backstageColumnCompsVIP.Add(column);
        }

        for (int i = _columnBackdropScratch.Count - 1; i >= totalColumnsCountHighFive; i--)
        {
            RectTransform extra = _columnBackdropScratch[i];
            if (extra == null)
            {
                continue;
            }

            DestroyColumnBackdropLikeExpiredCoupon(extra);
        }
    }

    private static string GetColumnRuntimeName(int columnIndex)
    {
        return "col" + columnIndex;
    }

    private float GetCurrentInnerWidthLikeTapeMeasure()
    {
        RectTransform rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        return rt != null ? Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows) : 0f;
    }

    private float GetCurrentInnerHeightLikeTapeMeasure()
    {
        RectTransform rt = _rt != null ? _rt : (_rt = GetComponent<RectTransform>());
        return rt != null ? Mathf.Max(0f, rt.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion) : 0f;
    }

    private float[] CaptureLiveColumnWidthsLikeTapeMeasure()
    {
        float[] widths = new float[Mathf.Max(0, totalColumnsCountHighFive)];
        for (int i = 0; i < widths.Length; i++)
        {
            widths[i] = GetLiveColumnWidthPixelsLikeTapeMeasure(i);
        }
        return widths;
    }

    private float[] CaptureLiveRowHeightsLikeTapeMeasure()
    {
        float[] heights = new float[Mathf.Max(0, totalRowsCountLetTheShowBegin)];
        for (int i = 0; i < heights.Length; i++)
        {
            heights[i] = GetLiveRowHeightPixelsLikeTapeMeasure(i);
        }
        return heights;
    }

    private void ApplyPreservedColumnWidthsAfterInsertLikeTapeMeasure(int insertedIndex, float[] preservedWidths)
    {
        if (preservedWidths == null || preservedWidths.Length == 0)
        {
            return;
        }

        SyncColumnWardrobes();

        float currentInnerWidth = GetCurrentInnerWidthLikeTapeMeasure();

        for (int newIndex = 0; newIndex < totalColumnsCountHighFive; newIndex++)
        {
            if (newIndex == insertedIndex)
            {
                continue;
            }

            int oldIndex = newIndex < insertedIndex ? newIndex : newIndex - 1;
            if (oldIndex < 0 || oldIndex >= preservedWidths.Length)
            {
                continue;
            }

            fancyColumnWardrobes[newIndex].requestedWidthMaybePercentIfNegative =
                ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, preservedWidths[oldIndex]), currentInnerWidth);
        }

        shareThePieEvenlyForColumns = false;
    }

    private void ApplyPreservedColumnWidthsAfterDeleteLikeTapeMeasure(int deletedIndex, float[] preservedWidths)
    {
        if (preservedWidths == null || preservedWidths.Length == 0)
        {
            return;
        }

        SyncColumnWardrobes();

        float currentInnerWidth = GetCurrentInnerWidthLikeTapeMeasure();

        for (int newIndex = 0; newIndex < totalColumnsCountHighFive; newIndex++)
        {
            int oldIndex = newIndex < deletedIndex ? newIndex : newIndex + 1;
            if (oldIndex < 0 || oldIndex >= preservedWidths.Length)
            {
                continue;
            }

            fancyColumnWardrobes[newIndex].requestedWidthMaybePercentIfNegative =
                ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, preservedWidths[oldIndex]), currentInnerWidth);
        }

        shareThePieEvenlyForColumns = false;
    }

    private void ApplyPreservedRowHeightsAfterInsertLikeTapeMeasure(int insertedIndex, float[] preservedHeights)
    {
        if (preservedHeights == null || preservedHeights.Length == 0)
        {
            return;
        }

        SyncRowWardrobes();

        float currentInnerHeight = GetCurrentInnerHeightLikeTapeMeasure();

        for (int newIndex = 0; newIndex < totalRowsCountLetTheShowBegin; newIndex++)
        {
            if (newIndex == insertedIndex)
            {
                continue;
            }

            int oldIndex = newIndex < insertedIndex ? newIndex : newIndex - 1;
            if (oldIndex < 0 || oldIndex >= preservedHeights.Length)
            {
                continue;
            }

            snazzyRowWardrobes[newIndex].requestedHeightMaybePercentIfNegative =
                ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, preservedHeights[oldIndex]), currentInnerHeight);
        }

        shareThePieEvenlyForRows = false;
    }

    private void ApplyPreservedRowHeightsAfterDeleteLikeTapeMeasure(int deletedIndex, float[] preservedHeights)
    {
        if (preservedHeights == null || preservedHeights.Length == 0)
        {
            return;
        }

        SyncRowWardrobes();

        float currentInnerHeight = GetCurrentInnerHeightLikeTapeMeasure();

        for (int newIndex = 0; newIndex < totalRowsCountLetTheShowBegin; newIndex++)
        {
            int oldIndex = newIndex < deletedIndex ? newIndex : newIndex + 1;
            if (oldIndex < 0 || oldIndex >= preservedHeights.Length)
            {
                continue;
            }

            snazzyRowWardrobes[newIndex].requestedHeightMaybePercentIfNegative =
                ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(Mathf.Max(0f, preservedHeights[oldIndex]), currentInnerHeight);
        }

        shareThePieEvenlyForRows = false;
    }

#if UNITY_EDITOR

    private bool IsDirectPrefabAssetLikeMuseumPiece()
    {
        return !Application.isPlaying && UnityEditor.EditorUtility.IsPersistent(gameObject);
    }

    internal bool EditorNeedsUpgrade()
    {
        return _editorUpgradeVersion < CurrentEditorUpgradeVersion;
    }

    public void MarkEditorUpgradeApplied()
    {
        if (!EditorNeedsUpgrade())
        {
            ClearOutdatedPrefabLogState();
            return;
        }

        _editorUpgradeVersion = CurrentEditorUpgradeVersion;
        ClearOutdatedPrefabLogState();
        EditorUtility.SetDirty(this);
    }

    private void ReportOutdatedPrefabUpgradeRequirement()
    {
        if (Application.isPlaying)
        {
            return;
        }

        string prefabAssetPath = GetOutdatedPrefabAssetPath();
        if (string.IsNullOrEmpty(prefabAssetPath))
        {
            return;
        }

        if (!_loggedOutdatedPrefabPaths.Add(prefabAssetPath))
        {
            return;
        }

        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
        if (prefabAsset == null)
        {
            return;
        }

        Debug.LogError(
            "Tonkers Table Topia prefab needs upgrade. Open the prefab, select any tables at least once to upgrade it, then save the prefab.\nPrefab: " + prefabAssetPath,
            prefabAsset);
    }

    private string GetOutdatedPrefabAssetPath()
    {
        if (EditorUtility.IsPersistent(gameObject))
        {
            return AssetDatabase.GetAssetPath(gameObject);
        }

        GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
        if (prefabSource != null)
        {
            return AssetDatabase.GetAssetPath(prefabSource);
        }

        return string.Empty;
    }

    private void ClearOutdatedPrefabLogState()
    {
        string prefabAssetPath = GetOutdatedPrefabAssetPath();
        if (string.IsNullOrEmpty(prefabAssetPath))
        {
            return;
        }

        _loggedOutdatedPrefabPaths.Remove(prefabAssetPath);
    }

#endif
}