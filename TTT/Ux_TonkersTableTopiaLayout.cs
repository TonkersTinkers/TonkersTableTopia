using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Layout/Tonkers Table Topia")]
[DisallowMultipleComponent]
public class Ux_TonkersTableTopiaLayout : MonoBehaviour
{
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
    public bool autoHugWidthLikeAGoodFriend = false;

    [Tooltip("Auto-resize height because why not")]
    public bool autoHugHeightBecauseWhyNot = false;

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
    }

    [Serializable]
    public class RowStyle
    {
        public float requestedHeightMaybePercentIfNegative = 0f;
        public Sprite backdropPictureOnTheHouse = null;
        public Color backdropTintFlavor = Color.white;
        public bool customAnchorsAndPivotBecauseWeFancy = false;
        public Vector2 customAnchorMinPointy = new Vector2(0f, 1f);
        public Vector2 customAnchorMaxPointy = new Vector2(0f, 1f);
        public Vector2 customPivotSpinny = new Vector2(0f, 1f);

        [Tooltip("If true, the last visible cell eats leftovers so the row looks full")]
        public bool lastVisibleCellEatsLeftovers = true;
    }

    public List<ColumnStyle> fancyColumnWardrobes = new List<ColumnStyle>();
    public List<RowStyle> snazzyRowWardrobes = new List<RowStyle>();

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

    private readonly Stack<RectTransform> _rowPool = new Stack<RectTransform>();
    private readonly Stack<RectTransform> _cellPool = new Stack<RectTransform>();

    private RectTransform RentRowRT()
    {
        if (Application.isPlaying && _rowPool.Count > 0)
        {
            var rt = _rowPool.Pop();
            rt.gameObject.SetActive(true);
            return rt;
        }
        var go = new GameObject("TTT Row");
        var rtNew = go.AddComponent<RectTransform>();
        go.AddComponent<Ux_TonkersTableTopiaRow>();
        return rtNew;
    }

    private RectTransform RentCellRT()
    {
        if (Application.isPlaying && _cellPool.Count > 0)
        {
            var rt = _cellPool.Pop();
            rt.gameObject.SetActive(true);
            return rt;
        }
        var go = new GameObject("TTT Cell");
        var rtNew = go.AddComponent<RectTransform>();
        go.AddComponent<Ux_TonkersTableTopiaCell>();
        return rtNew;
    }

    private void ReturnRowRT(RectTransform rt)
    {
        if (!Application.isPlaying) return;
        if (rt == null) return;
        rt.gameObject.SetActive(false);
        rt.SetParent(_rt, false);
        _rowPool.Push(rt);
    }

    private void ReturnCellRT(RectTransform rt)
    {
        if (!Application.isPlaying) return;
        if (rt == null) return;
        rt.gameObject.SetActive(false);
        rt.SetParent(_rt, false);
        _cellPool.Push(rt);
    }

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void Reset()
    {
        totalColumnsCountHighFive = 3;
        totalRowsCountLetTheShowBegin = 3;
        fancyColumnWardrobes = new List<ColumnStyle>();
        snazzyRowWardrobes = new List<RowStyle>();
        for (int columnStepperCrouton = 0; columnStepperCrouton < totalColumnsCountHighFive; columnStepperCrouton++) fancyColumnWardrobes.Add(new ColumnStyle());
        for (int rowStepperWaffle = 0; rowStepperWaffle < totalRowsCountLetTheShowBegin; rowStepperWaffle++) snazzyRowWardrobes.Add(new RowStyle());
        toggleZebraStripesForRows = true;
        toggleZebraStripesForColumns = false;
        zebraRowColorA = Color.white;
        zebraRowColorB = new Color(0.96f, 0.96f, 0.96f, 1f);
        zebraColumnColorA = Color.white;
        zebraColumnColorB = new Color(0.96f, 0.96f, 0.96f, 1f);
        int cols = Mathf.Max(1, totalColumnsCountHighFive);
        int rows = Mathf.Max(1, totalRowsCountLetTheShowBegin);
        float colPct = 1f / cols;
        float rowPct = 1f / rows;
        for (int c = 0; c < cols; c++) fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -colPct;
        for (int r = 0; r < rows; r++) snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -rowPct;
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnEnable()
    {
        if (_rt == null) _rt = GetComponent<RectTransform>();
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnTransformChildrenChanged()
    {
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnRectTransformDimensionsChange()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            FlagLayoutAsNeedingSpaDay();
            return;
        }
#endif
        FlagLayoutAsNeedingSpaDay();
        this.DeferSpaDayToNextFrameLikeABarber();
    }

    public void ConvertAllSpecsToPercentages()
    {
        EnsureWardrobeListsMatchHeadcount();
        var rt = _rt;
        if (rt == null) return;
        float innerW = Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerH = Mathf.Max(0f, rt.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
        int cols = Mathf.Max(1, totalColumnsCountHighFive);
        int rows = Mathf.Max(1, totalRowsCountLetTheShowBegin);
        float colSpacingSum = sociallyDistancedColumnsPixels * Mathf.Max(0, cols - 1);
        float rowSpacingSum = sociallyDistancedRowsPixels * Mathf.Max(0, rows - 1);
        float[] colPixels = Ux_TonkersTableTopiaExtensions.DistributeLikeACaterer(cols, i => (i < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f, sociallyDistancedColumnsPixels, innerW);
        float[] rowPixels = Ux_TonkersTableTopiaExtensions.DistributeLikeACaterer(rows, i => (i < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f, sociallyDistancedRowsPixels, innerH);
        float availW = Mathf.Max(1f, innerW - colSpacingSum);
        float availH = Mathf.Max(1f, innerH - rowSpacingSum);
        bool anyFlexCol = false;
        var wasFlexCol = new bool[cols];
        for (int c = 0; c < cols; c++)
        {
            bool flex = c < fancyColumnWardrobes.Count && Mathf.Approximately(fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative, 0f);
            wasFlexCol[c] = flex;
            if (flex) anyFlexCol = true;
        }
        bool anyFlexRow = false;
        var wasFlexRow = new bool[rows];
        for (int r = 0; r < rows; r++)
        {
            bool flex = r < snazzyRowWardrobes.Count && Mathf.Approximately(snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative, 0f);
            wasFlexRow[r] = flex;
            if (flex) anyFlexRow = true;
        }
        float sumPct = 0f;
        for (int c = 0; c < cols; c++)
        {
            float pct = Mathf.Clamp01(colPixels[c] / availW);
            if (wasFlexCol[c]) fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = 0f;
            else fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -pct;
            sumPct += pct;
        }
        if (sumPct > 0f && !anyFlexCol)
        {
            for (int c = 0; c < cols; c++)
            {
                float pct = -fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative;
                fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -(pct / sumPct);
            }
        }
        sumPct = 0f;
        for (int r = 0; r < rows; r++)
        {
            float pct = Mathf.Clamp01(rowPixels[r] / availH);
            if (wasFlexRow[r]) snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = 0f;
            else snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -pct;
            sumPct += pct;
        }
        if (sumPct > 0f && !anyFlexRow)
        {
            for (int r = 0; r < rows; r++)
            {
                float pct = -snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative;
                snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -(pct / sumPct);
            }
        }
        this.NormalizeWardrobePercentsToOneDadBod();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        _insideValidationPass = true;
#endif
        EnsureWardrobeListsMatchHeadcount();
        FlagLayoutAsNeedingSpaDay();
#if UNITY_EDITOR
        _insideValidationPass = false;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (layoutNeedsAFreshCoatOfPaint)
            {
                UpdateSeatingLikeAProUsher();
                layoutNeedsAFreshCoatOfPaint = false;
            }
        }
#endif
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying) return;
        var rt = _rt;
        if (rt != null && this.StampAndCheckIfChangedLikePassport(rt)) layoutNeedsAFreshCoatOfPaint = true;
        if (layoutNeedsAFreshCoatOfPaint)
        {
            UpdateSeatingLikeAProUsher();
            layoutNeedsAFreshCoatOfPaint = false;
        }
    }

    public void FlagLayoutAsNeedingSpaDay()
    {
        layoutNeedsAFreshCoatOfPaint = true;
#if UNITY_EDITOR
        if (!Application.isPlaying) ScheduleEditorLayoutTouchUp();
#endif
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
        if (totalColumnsCountHighFive < 1) totalColumnsCountHighFive = 1;
        if (fancyColumnWardrobes.Count < totalColumnsCountHighFive)
        {
            while (fancyColumnWardrobes.Count < totalColumnsCountHighFive) fancyColumnWardrobes.Add(new ColumnStyle());
        }
        else if (fancyColumnWardrobes.Count > totalColumnsCountHighFive)
        {
            fancyColumnWardrobes.RemoveRange(totalColumnsCountHighFive, fancyColumnWardrobes.Count - totalColumnsCountHighFive);
        }
        if (totalRowsCountLetTheShowBegin < 1) totalRowsCountLetTheShowBegin = 1;
        if (snazzyRowWardrobes.Count < totalRowsCountLetTheShowBegin)
        {
            while (snazzyRowWardrobes.Count < totalRowsCountLetTheShowBegin) snazzyRowWardrobes.Add(new RowStyle());
        }
        else if (snazzyRowWardrobes.Count > totalRowsCountLetTheShowBegin)
        {
            snazzyRowWardrobes.RemoveRange(totalRowsCountLetTheShowBegin, snazzyRowWardrobes.Count - totalRowsCountLetTheShowBegin);
        }
    }

    public void RebuildComedyClubSeatingChart()
    {
        EnsureWardrobeListsMatchHeadcount();
        RectTransform tableRectTransformMainStage = _rt;
        if (tableRectTransformMainStage == null) return;
        if (totalColumnsCountHighFive < 1) totalColumnsCountHighFive = 1;
        if (totalRowsCountLetTheShowBegin < 1) totalRowsCountLetTheShowBegin = 1;

        backstageRowRectsVIP.Clear();
        backstageCellRectsVIP.Clear();

        _rootChildrenRoundup.Clear();
        for (int childGrabber = 0; childGrabber < tableRectTransformMainStage.childCount; childGrabber++) _rootChildrenRoundup.Add(tableRectTransformMainStage.GetChild(childGrabber) as RectTransform);

        _managedRowsLine.Clear();
        _nonManagedRootBackpack.Clear();
        for (int i = 0; i < _rootChildrenRoundup.Count; i++)
        {
            var childRect = _rootChildrenRoundup[i];
            if (childRect == null) continue;
            if (IsThisRowOnTheGuestList(childRect)) _managedRowsLine.Add(childRect);
            else _nonManagedRootBackpack.Add(childRect);
        }

        _managedRowsLine.Sort(_siblingIndexComparison);

        while (_managedRowsLine.Count < totalRowsCountLetTheShowBegin)
        {
            RectTransform freshlyMintedRowRect;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var freshlyMintedRowObject = new GameObject("TTT Row " + (_managedRowsLine.Count + 1));
                freshlyMintedRowRect = freshlyMintedRowObject.AddComponent<RectTransform>();
                freshlyMintedRowObject.AddComponent<Ux_TonkersTableTopiaRow>();
                Undo.RegisterCreatedObjectUndo(freshlyMintedRowObject, "Add Tonkers Table Topia Row");
            }
            else
#endif
            {
                freshlyMintedRowRect = RentRowRT();
                freshlyMintedRowRect.gameObject.name = "TTT Row " + (_managedRowsLine.Count + 1);
            }
            freshlyMintedRowRect.SetParent(tableRectTransformMainStage, false);
            _managedRowsLine.Add(freshlyMintedRowRect);
        }

        for (int rowLooperCroissant = 0; rowLooperCroissant < totalRowsCountLetTheShowBegin; rowLooperCroissant++)
        {
            var thisRowRect = _managedRowsLine[rowLooperCroissant];
            var thisRowComponent = thisRowRect.GetComponent<Ux_TonkersTableTopiaRow>() ?? thisRowRect.gameObject.AddComponent<Ux_TonkersTableTopiaRow>();
            thisRowComponent.rowNumberWhereShenanigansOccur = rowLooperCroissant;

            _rowChildrenRoundup.Clear();
            for (int kidCollector = 0; kidCollector < thisRowRect.childCount; kidCollector++) _rowChildrenRoundup.Add(thisRowRect.GetChild(kidCollector) as RectTransform);

            _managedCellsLine.Clear();
            _nonManagedRowBackpack.Clear();
            for (int j = 0; j < _rowChildrenRoundup.Count; j++)
            {
                var childRect = _rowChildrenRoundup[j];
                if (childRect == null) continue;
                if (IsThisCellOnTheGuestList(childRect)) _managedCellsLine.Add(childRect);
                else _nonManagedRowBackpack.Add(childRect);
            }

            _managedCellsLine.Sort(_siblingIndexComparison);

            while (_managedCellsLine.Count < totalColumnsCountHighFive)
            {
                RectTransform freshCellRect;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    var freshCellObject = new GameObject($"TTT Cell {rowLooperCroissant + 1},{_managedCellsLine.Count + 1}");
                    freshCellRect = freshCellObject.AddComponent<RectTransform>();
                    freshCellObject.AddComponent<Ux_TonkersTableTopiaCell>();
                    Undo.RegisterCreatedObjectUndo(freshCellObject, "Add Tonkers Table Topia Cell");
                }
                else
#endif
                {
                    freshCellRect = RentCellRT();
                    freshCellRect.gameObject.name = $"TTT Cell {rowLooperCroissant + 1},{_managedCellsLine.Count + 1}";
                }
                freshCellRect.SetParent(thisRowRect, false);
                _managedCellsLine.Add(freshCellRect);
            }

            var targetCellForRowForeign = _managedCellsLine[0];
            EscortNonVIPsToTarget(thisRowRect, targetCellForRowForeign);

            for (int cullStepper = _managedCellsLine.Count - 1; cullStepper >= totalColumnsCountHighFive; cullStepper--)
            {
                var extraCellRect = _managedCellsLine[cullStepper];
                EscortNonVIPsToTarget(extraCellRect, targetCellForRowForeign);
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(extraCellRect.gameObject);
                else
#endif
                {
                    ReturnCellRT(extraCellRect);
                }
                _managedCellsLine.RemoveAt(cullStepper);
            }

            _thisRowCellsRects.Clear();
            for (int columnLooperPretzel = 0; columnLooperPretzel < totalColumnsCountHighFive; columnLooperPretzel++)
            {
                var cellRect = _managedCellsLine[columnLooperPretzel];
                var cellComp = cellRect.GetComponent<Ux_TonkersTableTopiaCell>() ?? cellRect.gameObject.AddComponent<Ux_TonkersTableTopiaCell>();
                cellComp.rowNumberWhereThePartyIs = rowLooperCroissant;
                cellComp.columnNumberPrimeRib = columnLooperPretzel;

                if (autoHireContentSizerBecauseLazy)
                {
                    var fitterSnag = cellRect.GetComponent<ContentSizeFitter>();
                    if (fitterSnag == null)
                    {
                        fitterSnag = cellRect.gameObject.AddComponent<ContentSizeFitter>();
                        fitterSnag.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        fitterSnag.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    }
                }
                else
                {
                    var fitterSnag = cellRect.GetComponent<ContentSizeFitter>();
                    if (fitterSnag != null)
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying) DestroyImmediate(fitterSnag);
                        else
#endif
                            Destroy(fitterSnag);
                    }
                }

                cellRect.gameObject.name = $"TTT Cell {rowLooperCroissant + 1},{columnLooperPretzel + 1}";

                bool shouldBeActiveBecauseNotMashed = !(cellComp.isMashedLikePotatoes && cellComp.mashedIntoWho != null);
                if (cellRect.gameObject.activeSelf != shouldBeActiveBecauseNotMashed) cellRect.gameObject.SetActive(shouldBeActiveBecauseNotMashed);

                _thisRowCellsRects.Add(cellRect);
            }

            thisRowRect.gameObject.name = "TTT Row " + (rowLooperCroissant + 1);

            backstageRowRectsVIP.Add(thisRowRect);
            backstageCellRectsVIP.Add(new List<RectTransform>(_thisRowCellsRects));
        }

        if (_managedRowsLine.Count > totalRowsCountLetTheShowBegin)
        {
            var targetCell = FetchCellRectTransformVIP(0, 0);
            for (int rowReaper = _managedRowsLine.Count - 1; rowReaper >= totalRowsCountLetTheShowBegin; rowReaper--)
            {
                var extraRow = _managedRowsLine[rowReaper];
                if (targetCell != null) EscortNonVIPsToTarget(extraRow, targetCell);
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(extraRow.gameObject);
                else
#endif
                {
                    ReturnRowRT(extraRow);
                }
            }
        }

        if (_nonManagedRootBackpack.Count > 0)
        {
            var target = FetchCellRectTransformVIP(0, 0);
            if (target != null) EscortNonVIPsToTarget(tableRectTransformMainStage, target);
        }
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
        var rowStyle = (rowNumberIndex < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowNumberIndex] : null;
        bool needImage = rowStyle != null && rowStyle.backdropPictureOnTheHouse != null;
        var img = rowRect.FlipImageComponentLikeALightSwitch(needImage);
        if (img != null)
        {
            img.sprite = rowStyle.backdropPictureOnTheHouse;
            img.color = rowStyle.backdropTintFlavor;
            img.type = Image.Type.Sliced;
        }
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
        var tableRectTransformMainStage = _rt;
        if (tableRectTransformMainStage == null) return;
        RebuildComedyClubSeatingChart();
        if (totalColumnsCountHighFive == 0 || totalRowsCountLetTheShowBegin == 0) return;

        var calculatedRowHeightsBuffet = CalculateRowHeightsLikeAShortOrderCook(tableRectTransformMainStage);
        float totalContentHeightCalories = CalculateTotalContentHeightCalories(calculatedRowHeightsBuffet);

        if (autoHugHeightBecauseWhyNot && !AreWeStretchingVertically(tableRectTransformMainStage)) tableRectTransformMainStage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentHeightCalories);

        float innerWidthPlayable = Mathf.Max(0, tableRectTransformMainStage.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerHeightPlayable = Mathf.Max(0, tableRectTransformMainStage.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);

        var baseColumnWidths = CalculateBaseColumnWidths(innerWidthPlayable);

        float leftOffsetStart = comfyPaddingLeftForElbows;
        float currentYTopDown = CalculateVerticalGrandEntrance(tableRectTransformMainStage, totalContentHeightCalories) - comfyPaddingTopHat;

        for (int rowStepperWaffle = 0; rowStepperWaffle < totalRowsCountLetTheShowBegin; rowStepperWaffle++)
        {
            float rowHeightThisRound = calculatedRowHeightsBuffet[rowStepperWaffle];
            var rowRT = backstageRowRectsVIP[rowStepperWaffle];
            Vector2 oldRowSizeJeans = rowRT != null ? rowRT.rect.size : Vector2.zero;

            ParkRowInItsAssignedSpot(rowStepperWaffle, rowHeightThisRound, tableRectTransformMainStage, innerWidthPlayable, leftOffsetStart, currentYTopDown, tableRectTransformMainStage.rect.width, tableRectTransformMainStage.rect.height);
            ParkCellsInRowWithoutDingedBumpers(rowStepperWaffle, rowHeightThisRound, innerWidthPlayable, calculatedRowHeightsBuffet, baseColumnWidths);

            if (rowRT != null) rowRT.ScaleForeignKidsToFitNewParentSizeLikeDadJeans(oldRowSizeJeans, rowRT.rect.size);

            currentYTopDown -= (rowHeightThisRound + sociallyDistancedRowsPixels);
        }

        RefreshForeignGuestsLikeAMagician();
    }

    private void RefreshForeignGuestsLikeAMagician()
    {
        Canvas.ForceUpdateCanvases();
        var tableRT = _rt;
        if (tableRT != null) tableRT.ForceUpdateRectTransforms();
        SweepAllMashedForeignsToTheirMainLikeRoomba();
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            var rowRT = backstageRowRectsVIP[r];
            if (rowRT != null)
            {
                RefreshDirectForeignChildrenIn(rowRT);
                LayoutRebuilder.ForceRebuildLayoutImmediate(rowRT);
            }
            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                var cellRT = backstageCellRectsVIP[r][c];
                if (cellRT == null) continue;
                RefreshDirectForeignChildrenIn(cellRT);
                LayoutRebuilder.ForceRebuildLayoutImmediate(cellRT);
            }
        }
        Canvas.ForceUpdateCanvases();
        if (tableRT != null) tableRT.ForceUpdateRectTransforms();
    }

    private void SweepAllMashedForeignsToTheirMainLikeRoomba()
    {
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                var rt = FetchCellRectTransformVIP(r, c);
                if (rt == null) continue;
                var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
                if (cell == null) continue;
                if (!cell.isMashedLikePotatoes || cell.mashedIntoWho == null) continue;
                var main = cell.mashedIntoWho;
                var mainRT = FetchCellRectTransformVIP(main.rowNumberWhereThePartyIs, main.columnNumberPrimeRib);
                if (mainRT == null) continue;
                EscortNonVIPsToTarget(rt, mainRT);
            }
        }
    }

    private void RefreshDirectForeignChildrenIn(RectTransform parent)
    {
        if (parent == null) return;
        for (int i = 0; i < parent.childCount; i++)
        {
            var ch = parent.GetChild(i);
            if (IsThisRowOnTheGuestList(ch) || IsThisCellOnTheGuestList(ch)) continue;
            var rt = ch as RectTransform;
            if (rt == null) continue;
            if (rt.IsFullStretchLikeYoga()) rt.SnapCroutonToFillParentLikeGravy();
        }
    }

    private void ParkCellsInRowWithoutDingedBumpers(int rowNumberIndex, float rowHeightThisRound, float innerWidthPlayable, float[] allRowHeights, float[] baseColumnWidths)
    {
        int c = 0;
        float currentXLeftToRight = 0f;
        while (c < totalColumnsCountHighFive)
        {
            RectTransform cellRect = backstageCellRectsVIP[rowNumberIndex][c];
            var cellComp = cellRect ? cellRect.GetComponent<Ux_TonkersTableTopiaCell>() : null;
            float columnStartX = currentXLeftToRight;
            float thisColW = Mathf.Max(0f, (c < baseColumnWidths.Length) ? baseColumnWidths[c] : 0f);

            if (cellComp == null)
            {
                currentXLeftToRight += thisColW + sociallyDistancedColumnsPixels;
                c++;
                continue;
            }

            if (cellComp.isMashedLikePotatoes)
            {
                var img = cellRect.GetComponent<Image>();
                if (img != null) img.enabled = false;
                currentXLeftToRight += thisColW + sociallyDistancedColumnsPixels;
                c++;
                continue;
            }

            int spanCols = Mathf.Clamp(cellComp.howManyColumnsAreSneakingIn, 1, totalColumnsCountHighFive - c);
            float cellW = 0f;
            for (int i = 0; i < spanCols; i++) cellW += Mathf.Max(0f, (c + i < baseColumnWidths.Length) ? baseColumnWidths[c + i] : 0f);
            if (spanCols > 1) cellW += sociallyDistancedColumnsPixels * (spanCols - 1);
            float cellH = CalculateCellHeightForTheStretch(rowNumberIndex, cellComp.howManyRowsAreHoggingThisSeat, allRowHeights);
            if (cellComp.howManyRowsAreHoggingThisSeat <= 1) cellH = rowHeightThisRound;

            Vector2 aMin, aMax, piv;
            var colStyle = (c < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[c] : null;
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

            ApplyRectPlacementWithPanache(cellRect, aMin, aMax, piv, columnStartX, 0f, cellW, cellH, innerWidthPlayable, rowHeightThisRound);
            DressCellToImpress(cellRect, cellComp, c);
            cellRect.ScaleForeignKidsToFitNewParentSizeLikeDadJeans(oldCellSizeCargoShorts, cellRect.rect.size);

            currentXLeftToRight += cellW + sociallyDistancedColumnsPixels;
            c += spanCols;
        }
    }

    private void DressCellToImpress(RectTransform cellRect, Ux_TonkersTableTopiaCell cellComp, int columnIndex)
    {
        if (cellComp.isMashedLikePotatoes)
        {
            cellRect.FlipImageComponentLikeALightSwitch(false);
            return;
        }
        bool isMergedTopLeft = (cellComp.howManyRowsAreHoggingThisSeat > 1 || cellComp.howManyColumnsAreSneakingIn > 1);
        if (isMergedTopLeft)
        {
            cellRect.FlipImageComponentLikeALightSwitch(false);
            return;
        }
        if (cellComp.backgroundPictureBecausePlainIsLame != null)
        {
            var img = cellRect.FlipImageComponentLikeALightSwitch(true);
            img.sprite = cellComp.backgroundPictureBecausePlainIsLame;
            img.color = cellComp.backgroundColorLikeASunset;
            img.type = Image.Type.Sliced;
            return;
        }
        if (columnIndex < fancyColumnWardrobes.Count && fancyColumnWardrobes[columnIndex].backdropPictureOnTheHouse != null)
        {
            var cs = fancyColumnWardrobes[columnIndex];
            var img = cellRect.FlipImageComponentLikeALightSwitch(true);
            img.sprite = cs.backdropPictureOnTheHouse;
            img.color = cs.backdropTintFlavor;
            img.type = Image.Type.Sliced;
            return;
        }
        bool useRow = toggleZebraStripesForRows;
        bool useCol = toggleZebraStripesForColumns;
        if (useRow || useCol)
        {
            int safeRow = Mathf.Max(0, cellComp.rowNumberWhereThePartyIs);
            int safeCol = Mathf.Max(0, cellComp.columnNumberPrimeRib);
            Color rc = ((safeRow & 1) == 0) ? zebraRowColorA : zebraRowColorB;
            Color cc = ((safeCol & 1) == 0) ? zebraColumnColorA : zebraColumnColorB;
            Color final = useRow && useCol ? new Color((rc.r + cc.r) * 0.5f, (rc.g + cc.g) * 0.5f, (rc.b + cc.b) * 0.5f, Mathf.Max(rc.a, cc.a)) : (useRow ? rc : cc);
            var img = cellRect.FlipImageComponentLikeALightSwitch(true);
            img.sprite = null;
            img.type = Image.Type.Simple;
            img.color = final;
            return;
        }
        cellRect.FlipImageComponentLikeALightSwitch(false);
    }

    private float[] CalculateBaseColumnWidths(float innerWidthPlayable)
    {
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(
            totalColumnsCountHighFive,
            i =>
            {
                if (shareThePieEvenlyForColumns) return 0f;
                return (i < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f;
            },
            sociallyDistancedColumnsPixels,
            innerWidthPlayable,
            ref _colWidthsBuf
        );
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
        Ux_TonkersTableTopiaExtensions.DistributeLikeACatererInto(totalRowsCountLetTheShowBegin, i => (i < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f, sociallyDistancedRowsPixels, innerH, ref _rowHeightsBuf);
        return _rowHeightsBuf;
    }

    private bool AreWeStretchingVertically(RectTransform rt)
    {
        return Mathf.Abs(rt.anchorMin.y - rt.anchorMax.y) > 0.001f;
    }

    public void SyncColumnWardrobes()
    {
        if (totalColumnsCountHighFive < 1) totalColumnsCountHighFive = 1;
        if (fancyColumnWardrobes == null) fancyColumnWardrobes = new List<ColumnStyle>();
        if (fancyColumnWardrobes.Count < totalColumnsCountHighFive)
        {
            while (fancyColumnWardrobes.Count < totalColumnsCountHighFive) fancyColumnWardrobes.Add(new ColumnStyle());
        }
        else if (fancyColumnWardrobes.Count > totalColumnsCountHighFive)
        {
            fancyColumnWardrobes.RemoveRange(totalColumnsCountHighFive, fancyColumnWardrobes.Count - totalColumnsCountHighFive);
        }
    }

    public void SyncRowWardrobes()
    {
        if (totalRowsCountLetTheShowBegin < 1) totalRowsCountLetTheShowBegin = 1;
        if (snazzyRowWardrobes == null) snazzyRowWardrobes = new List<RowStyle>();
        if (snazzyRowWardrobes.Count < totalRowsCountLetTheShowBegin)
        {
            while (snazzyRowWardrobes.Count < totalRowsCountLetTheShowBegin) snazzyRowWardrobes.Add(new RowStyle());
        }
        else if (snazzyRowWardrobes.Count > totalRowsCountLetTheShowBegin)
        {
            snazzyRowWardrobes.RemoveRange(totalRowsCountLetTheShowBegin, snazzyRowWardrobes.Count - totalRowsCountLetTheShowBegin);
        }
    }

    public void SwapColumnsLikeTradingCards(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalColumnsCountHighFive || bIndex >= totalColumnsCountHighFive) return;
        RebuildComedyClubSeatingChart();
        if (aIndex > bIndex)
        {
            var t = aIndex;
            aIndex = bIndex;
            bIndex = t;
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
            Ux_TonkersTableTopiaCell cellA = rtB.GetComponent<Ux_TonkersTableTopiaCell>();
            Ux_TonkersTableTopiaCell cellB = rtA.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cellA != null)
            {
                cellA.columnNumberPrimeRib = aIndex;
                rtB.gameObject.name = $"TTT Cell {rowLooper + 1},{aIndex + 1}";
            }
            if (cellB != null)
            {
                cellB.columnNumberPrimeRib = bIndex;
                rtA.gameObject.name = $"TTT Cell {rowLooper + 1},{bIndex + 1}";
            }
        }
        FlagLayoutAsNeedingSpaDay();
    }

    public void SwapRowsLikeMusicalChairs(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalRowsCountLetTheShowBegin || bIndex >= totalRowsCountLetTheShowBegin) return;
    
        if (_rt == null) _rt = GetComponent<RectTransform>();
        RebuildComedyClubSeatingChart();
    
        if (aIndex > bIndex)
        {
            var t = aIndex;
            aIndex = bIndex;
            bIndex = t;
        }
    
        int rowCount = backstageRowRectsVIP != null ? backstageRowRectsVIP.Count : 0;
        if (rowCount <= aIndex || rowCount <= bIndex)
        {
            RebuildComedyClubSeatingChart();
            rowCount = backstageRowRectsVIP != null ? backstageRowRectsVIP.Count : 0;
            if (rowCount <= aIndex || rowCount <= bIndex) return;
        }
    
        RectTransform rowA = backstageRowRectsVIP[aIndex];
        RectTransform rowB = backstageRowRectsVIP[bIndex];
    
        int idxA = rowA.GetSiblingIndex();
        int idxB = rowB.GetSiblingIndex();
        rowA.SetSiblingIndex(idxB);
        rowB.SetSiblingIndex(idxA);
    
        backstageRowRectsVIP[aIndex] = rowB;
        backstageRowRectsVIP[bIndex] = rowA;
    
        var tempCells = backstageCellRectsVIP[aIndex];
        backstageCellRectsVIP[aIndex] = backstageCellRectsVIP[bIndex];
        backstageCellRectsVIP[bIndex] = tempCells;
    
        Ux_TonkersTableTopiaRow compA = rowB.GetComponent<Ux_TonkersTableTopiaRow>();
        Ux_TonkersTableTopiaRow compB = rowA.GetComponent<Ux_TonkersTableTopiaRow>();
        if (compA != null)
        {
            compA.rowNumberWhereShenanigansOccur = aIndex;
            rowB.gameObject.name = "TTT Row " + (aIndex + 1);
        }
        if (compB != null)
        {
            compB.rowNumberWhereShenanigansOccur = bIndex;
            rowA.gameObject.name = "TTT Row " + (bIndex + 1);
        }
    
        for (int columnStepper = 0; columnStepper < totalColumnsCountHighFive; columnStepper++)
        {
            RectTransform cellA = backstageCellRectsVIP[aIndex][columnStepper];
            RectTransform cellB = backstageCellRectsVIP[bIndex][columnStepper];
    
            Ux_TonkersTableTopiaCell compCellA = cellA.GetComponent<Ux_TonkersTableTopiaCell>();
            Ux_TonkersTableTopiaCell compCellB = cellB.GetComponent<Ux_TonkersTableTopiaCell>();
    
            if (compCellA != null)
            {
                compCellA.rowNumberWhereThePartyIs = aIndex;
                cellA.gameObject.name = $"TTT Cell {aIndex + 1},{columnStepper + 1}";
            }
            if (compCellB != null)
            {
                compCellB.rowNumberWhereThePartyIs = bIndex;
                cellB.gameObject.name = $"TTT Cell {bIndex + 1},{columnStepper + 1}";
            }
        }
    
        FlagLayoutAsNeedingSpaDay();
    }

    public void MergeCellsLikeAGroupHug(int startRow, int startCol, int rowCount, int colCount)
    {
        if (rowCount < 1 || colCount < 1) return;
        if (totalColumnsCountHighFive < 1 || totalRowsCountLetTheShowBegin < 1) return;
        RebuildComedyClubSeatingChart();
        this.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        if (rowCount == 1 && colCount == 1) return;
        this.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        UnmergeRectangleLikeItNeverHappened(startRow, startCol, rowCount, colCount);
        RectTransform mainCellRT = FetchCellRectTransformVIP(startRow, startCol);
        var mainCellComp = mainCellRT ? mainCellRT.GetComponent<Ux_TonkersTableTopiaCell>() : null;
        if (mainCellComp == null) return;
        RelocateStowawaysToMainCellLikeAValet(startRow, startCol, rowCount, colCount, mainCellRT);
        mainCellComp.howManyRowsAreHoggingThisSeat = rowCount;
        mainCellComp.howManyColumnsAreSneakingIn = colCount;
        mainCellComp.isMashedLikePotatoes = false;
        mainCellComp.mashedIntoWho = null;
        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (r == startRow && c == startCol) continue;
                var rt = FetchCellRectTransformVIP(r, c);
                if (!rt) continue;
                var comp = rt.GetComponent<Ux_TonkersTableTopiaCell>();
                if (comp == null) continue;
                comp.isMashedLikePotatoes = true;
                comp.mashedIntoWho = mainCellComp;
                rt.gameObject.SetActive(false);
            }
        }
        mainCellRT.gameObject.name = $"TTT Cell {startRow + 1},{startCol + 1} Mega Combo {rowCount}x{colCount}";
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
                var rt = FetchCellRectTransformVIP(r, c);
                if (!rt) continue;
                var comp = rt.GetComponent<Ux_TonkersTableTopiaCell>();
                if (comp == null) continue;
                comp.howManyRowsAreHoggingThisSeat = 1;
                comp.howManyColumnsAreSneakingIn = 1;
                comp.isMashedLikePotatoes = false;
                comp.mashedIntoWho = null;
                if (!rt.gameObject.activeSelf) rt.gameObject.SetActive(true);
                rt.gameObject.name = $"TTT Cell {r + 1},{c + 1}";
            }
        }
    }

    public void UnmergeEverythingInRectLikeItNeverHappened(int startRow, int startCol, int rowCount, int colCount)
    {
        if (rowCount < 1 || colCount < 1) return;
        if (totalRowsCountLetTheShowBegin < 1 || totalColumnsCountHighFive < 1) return;
        RebuildComedyClubSeatingChart();
        this.ClampRectToTableLikeASensibleSeatbelt(ref startRow, ref startCol, ref rowCount, ref colCount);
        this.ExpandRectToWholeMergersLikeACarpenter(ref startRow, ref startCol, ref rowCount, ref colCount);
        UnmergeRectangleLikeItNeverHappened(startRow, startCol, rowCount, colCount);
        FlagLayoutAsNeedingSpaDay();
    }

    public RectTransform FetchCellRectTransformVIP(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return null;
        return backstageCellRectsVIP.Count > row && backstageCellRectsVIP[row].Count > col ? backstageCellRectsVIP[row][col] : null;
    }

    public RectTransform FetchRowRectTransformVIP(int index)
    {
        if (index < 0 || index >= totalRowsCountLetTheShowBegin) return null;
        RectTransform tableRT = _rt;
        if (tableRT == null) return null;
        if (tableRT.childCount <= index) return null;
        var t = tableRT.GetChild(index);
        return t.GetComponent<Ux_TonkersTableTopiaRow>() != null ? (RectTransform)t : null;
    }

    private bool IsThisRowOnTheGuestList(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaRow>() != null;
    }

    private bool IsThisCellOnTheGuestList(Transform t)
    {
        return t != null && t.GetComponent<Ux_TonkersTableTopiaCell>() != null;
    }

    private void EscortNonVIPsToTarget(Transform from, Transform to)
    {
        if (from == null || to == null) return;
        _stowawaysOnThisCrazyTrain.Clear();
        for (int i = 0; i < from.childCount; i++)
        {
            var ch = from.GetChild(i);
            if (!IsThisRowOnTheGuestList(ch) && !IsThisCellOnTheGuestList(ch)) _stowawaysOnThisCrazyTrain.Add(ch);
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_insideValidationPass)
            {
                for (int k = 0; k < _stowawaysOnThisCrazyTrain.Count; k++)
                {
                    var tr = _stowawaysOnThisCrazyTrain[k];
                    var trLocal = tr;
                    var toLocal = to;
                    var rt = trLocal as RectTransform;
                    Vector2 snapMin = Vector2.zero, snapMax = Vector2.one, snapPivot = new Vector2(0.5f, 0.5f);
                    Vector2 snapAnchored = Vector2.zero, snapSizeDelta = Vector2.zero, snapOffMin = Vector2.zero, snapOffMax = Vector2.zero;
                    if (rt != null)
                    {
                        snapMin = rt.anchorMin;
                        snapMax = rt.anchorMax;
                        snapPivot = rt.pivot;
                        snapAnchored = rt.anchoredPosition;
                        snapSizeDelta = rt.sizeDelta;
                        snapOffMin = rt.offsetMin;
                        snapOffMax = rt.offsetMax;
                    }
                    EditorApplication.delayCall += () =>
                    {
                        if (trLocal == null || toLocal == null) return;
                        Undo.SetTransformParent(trLocal, toLocal, "Move Guests With Style");
                        var rtAfter = trLocal as RectTransform;
                        if (rtAfter == null) return;
                        if (rtAfter.IsFullStretchLikeYoga())
                        {
                            rtAfter.SnapCroutonToFillParentLikeGravy();
                        }
                        else
                        {
                            rtAfter.anchorMin = snapMin;
                            rtAfter.anchorMax = snapMax;
                            rtAfter.pivot = snapPivot;
                            rtAfter.sizeDelta = snapSizeDelta;
                            rtAfter.offsetMin = snapOffMin;
                            rtAfter.offsetMax = snapOffMax;
                            rtAfter.anchoredPosition = snapAnchored;
                        }
                    };
                }
                return;
            }
            Undo.RecordObject(this, "Move Guests With Style");
            for (int k = 0; k < _stowawaysOnThisCrazyTrain.Count; k++)
            {
                var tr = _stowawaysOnThisCrazyTrain[k];
                var rt = tr as RectTransform;
                Vector2 snapMin = Vector2.zero, snapMax = Vector2.one, snapPivot = new Vector2(0.5f, 0.5f);
                Vector2 snapAnchored = Vector2.zero, snapSizeDelta = Vector2.zero, snapOffMin = Vector2.zero, snapOffMax = Vector2.zero;
                if (rt != null)
                {
                    snapMin = rt.anchorMin;
                    snapMax = rt.anchorMax;
                    snapPivot = rt.pivot;
                    snapAnchored = rt.anchoredPosition;
                    snapSizeDelta = rt.sizeDelta;
                    snapOffMin = rt.offsetMin;
                    snapOffMax = rt.offsetMax;
                }
                Undo.SetTransformParent(tr, to, "Move Guests With Style");
                var rtAfter = tr as RectTransform;
                if (rtAfter == null) continue;
                if (rtAfter.IsFullStretchLikeYoga())
                {
                    rtAfter.SnapCroutonToFillParentLikeGravy();
                }
                else
                {
                    rtAfter.anchorMin = snapMin;
                    rtAfter.anchorMax = snapMax;
                    rtAfter.pivot = snapPivot;
                    rtAfter.sizeDelta = snapSizeDelta;
                    rtAfter.offsetMin = snapOffMin;
                    rtAfter.offsetMax = snapOffMax;
                    rtAfter.anchoredPosition = snapAnchored;
                }
            }
            return;
        }
#endif
        for (int k = 0; k < _stowawaysOnThisCrazyTrain.Count; k++)
        {
            var tr = _stowawaysOnThisCrazyTrain[k];
            var rt = tr as RectTransform;
            Vector2 snapMin = Vector2.zero, snapMax = Vector2.one, snapPivot = new Vector2(0.5f, 0.5f);
            Vector2 snapAnchored = Vector2.zero, snapSizeDelta = Vector2.zero, snapOffMin = Vector2.zero, snapOffMax = Vector2.zero;
            if (rt != null)
            {
                snapMin = rt.anchorMin;
                snapMax = rt.anchorMax;
                snapPivot = rt.pivot;
                snapAnchored = rt.anchoredPosition;
                snapSizeDelta = rt.sizeDelta;
                snapOffMin = rt.offsetMin;
                snapOffMax = rt.offsetMax;
            }
            tr.SetParent(to, false);
            var rtAfter = tr as RectTransform;
            if (rtAfter == null) continue;
            if (rtAfter.IsFullStretchLikeYoga())
            {
                rtAfter.SnapCroutonToFillParentLikeGravy();
            }
            else
            {
                rtAfter.anchorMin = snapMin;
                rtAfter.anchorMax = snapMax;
                rtAfter.pivot = snapPivot;
                rtAfter.sizeDelta = snapSizeDelta;
                rtAfter.offsetMin = snapOffMin;
                rtAfter.offsetMax = snapOffMax;
                rtAfter.anchoredPosition = snapAnchored;
            }
        }
    }

    public Ux_TonkersTableTopiaRow InsertRowLikeANinja(int index)
    {
        if (index < 0 || index > totalRowsCountLetTheShowBegin) index = Mathf.Clamp(index, 0, totalRowsCountLetTheShowBegin);
        ConvertAllSpecsToPercentages();
        int oldRows = Mathf.Max(1, totalRowsCountLetTheShowBegin);
        var currentPct = new List<float>(oldRows);
        for (int r = 0; r < oldRows; r++)
        {
            float p = 0f;
            if (r < snazzyRowWardrobes.Count)
            {
                float spec = snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative;
                p = spec < 0f ? Mathf.Clamp01(-spec) : 0f;
            }
            currentPct.Add(p);
        }
        float newPct = 1f / (oldRows + 1);
        totalRowsCountLetTheShowBegin++;
        var newbie = new RowStyle { requestedHeightMaybePercentIfNegative = -newPct };
        if (index >= 0 && index <= snazzyRowWardrobes.Count) snazzyRowWardrobes.Insert(index, newbie);
        else snazzyRowWardrobes.Add(newbie);
        for (int r = 0, oldIdx = 0; r < snazzyRowWardrobes.Count; r++)
        {
            if (r == index) continue;
            float scaled = Mathf.Clamp01(currentPct[oldIdx] * (1f - newPct));
            snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -scaled;
            oldIdx++;
        }
        RebuildComedyClubSeatingChart();
        for (int i = totalRowsCountLetTheShowBegin - 1; i > index; i--) SwapRowsLikeMusicalChairs(i, i - 1);
        shareThePieEvenlyForRows = false;
        FlagLayoutAsNeedingSpaDay();
        var rt = FetchRowRectTransformVIP(index);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaRow>() : null;
    }

    public Ux_TonkersTableTopiaCell InsertColumnLikeANinja(int index)
    {
        if (index < 0 || index > totalColumnsCountHighFive) index = Mathf.Clamp(index, 0, totalColumnsCountHighFive);
        ConvertAllSpecsToPercentages();
        int oldCols = Mathf.Max(1, totalColumnsCountHighFive);
        var currentPct = new List<float>(oldCols);
        for (int c = 0; c < oldCols; c++)
        {
            float p = 0f;
            if (c < fancyColumnWardrobes.Count)
            {
                float spec = fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative;
                p = spec < 0f ? Mathf.Clamp01(-spec) : 0f;
            }
            currentPct.Add(p);
        }
        float newPct = 1f / (oldCols + 1);
        totalColumnsCountHighFive++;
        var newbie = new ColumnStyle { requestedWidthMaybePercentIfNegative = -newPct };
        if (index >= 0 && index <= fancyColumnWardrobes.Count) fancyColumnWardrobes.Insert(index, newbie);
        else fancyColumnWardrobes.Add(newbie);
        for (int c = 0, oldIdx = 0; c < fancyColumnWardrobes.Count; c++)
        {
            if (c == index) continue;
            float scaled = Mathf.Clamp01(currentPct[oldIdx] * (1f - newPct));
            fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -scaled;
            oldIdx++;
        }
        RebuildComedyClubSeatingChart();
        for (int i = totalColumnsCountHighFive - 1; i > index; i--) SwapColumnsLikeTradingCards(i, i - 1);
        shareThePieEvenlyForColumns = false;
        FlagLayoutAsNeedingSpaDay();
        var rt = FetchCellRectTransformVIP(0, index);
        return rt != null ? rt.GetComponent<Ux_TonkersTableTopiaCell>() : null;
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
        return InsertColumnLikeANinja(index);
    }

    public bool TryPolitelyDeleteRow(int index)
    {
        if (index < 0 || index >= totalRowsCountLetTheShowBegin) return false;
        if (totalRowsCountLetTheShowBegin <= 1) return false;
        ConvertAllSpecsToPercentages();
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                Ux_TonkersTableTopiaCell cell = backstageCellRectsVIP[r][c].GetComponent<Ux_TonkersTableTopiaCell>();
                if (cell == null || cell.isMashedLikePotatoes) continue;
                int start = cell.rowNumberWhereThePartyIs;
                int end = cell.rowNumberWhereThePartyIs + Mathf.Max(1, cell.howManyRowsAreHoggingThisSeat) - 1;
                if (cell.howManyRowsAreHoggingThisSeat > 1 && start <= index && end >= index) return false;
            }
        }
        var table = _rt;
        RectTransform targetForRowLevel = GetPreferredTargetCellForRowDeletion(index, 0);
        if (targetForRowLevel == null) targetForRowLevel = table;
        var rowRT = backstageRowRectsVIP[index];
        EscortNonVIPsToTarget(rowRT, targetForRowLevel);
        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            var cellRT = FetchCellRectTransformVIP(index, c);
            if (cellRT == null) continue;
            RectTransform target = GetPreferredTargetCellForRowDeletion(index, c);
            if (target == null) target = table;
            EscortNonVIPsToTarget(cellRT, target);
        }
#if UNITY_EDITOR
        if (!Application.isPlaying) Undo.DestroyObjectImmediate(rowRT.gameObject);
        else
#endif
        {
            ReturnRowRT(rowRT);
        }
        int oldRows = totalRowsCountLetTheShowBegin;
        float removedPct = Mathf.Clamp01(index < snazzyRowWardrobes.Count && snazzyRowWardrobes[index].requestedHeightMaybePercentIfNegative < 0f ? -snazzyRowWardrobes[index].requestedHeightMaybePercentIfNegative : 0f);
        totalRowsCountLetTheShowBegin--;
        if (index >= 0 && index < snazzyRowWardrobes.Count)
        {
            snazzyRowWardrobes.RemoveAt(index);
            float denom = Mathf.Max(0.0001f, 1f - removedPct);
            for (int i = 0; i < snazzyRowWardrobes.Count; i++)
            {
                float p = snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative < 0f ? -snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f;
                p = Mathf.Clamp01(p / denom);
                snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative = -p;
            }
        }
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool TryPolitelyDeleteColumn(int index)
    {
        if (index < 0 || index >= totalColumnsCountHighFive) return false;
        if (totalColumnsCountHighFive <= 1) return false;
        ConvertAllSpecsToPercentages();
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            for (int c = 0; c < totalColumnsCountHighFive; c++)
            {
                Ux_TonkersTableTopiaCell cell = backstageCellRectsVIP[r][c].GetComponent<Ux_TonkersTableTopiaCell>();
                if (cell == null || cell.isMashedLikePotatoes) continue;
                int start = cell.columnNumberPrimeRib;
                int end = cell.columnNumberPrimeRib + Mathf.Max(1, cell.howManyColumnsAreSneakingIn) - 1;
                if (cell.howManyColumnsAreSneakingIn > 1 && start <= index && end >= index) return false;
            }
        }
        var table = _rt;
        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            Transform row = backstageRowRectsVIP[r];
            if (row == null) continue;
            if (index >= row.childCount) continue;
            var cellRT = row.GetChild(index) as RectTransform;
            if (cellRT == null) continue;
            RectTransform target = GetPreferredTargetCellForColumnDeletion(r, index);
            if (target == null) target = table;
            EscortNonVIPsToTarget(cellRT, target);
#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.DestroyObjectImmediate(cellRT.gameObject);
            else
#endif
            {
                ReturnCellRT(cellRT);
            }
        }
        float removedPct = Mathf.Clamp01(index < fancyColumnWardrobes.Count && fancyColumnWardrobes[index].requestedWidthMaybePercentIfNegative < 0f ? -fancyColumnWardrobes[index].requestedWidthMaybePercentIfNegative : 0f);
        totalColumnsCountHighFive--;
        if (index >= 0 && index < fancyColumnWardrobes.Count)
        {
            fancyColumnWardrobes.RemoveAt(index);
            float denom = Mathf.Max(0.0001f, 1f - removedPct);
            for (int i = 0; i < fancyColumnWardrobes.Count; i++)
            {
                float p = fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative < 0f ? -fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f;
                p = Mathf.Clamp01(p / denom);
                fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative = -p;
            }
        }
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    private void ApplyRectPlacementWithPanache(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 pivot, float x, float y, float w, float h, float containerW, float containerH)
    {
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        bool stretchX = Mathf.Abs(aMin.x - aMax.x) > 0.001f;
        bool stretchY = Mathf.Abs(aMin.y - aMax.y) > 0.001f;
        if (stretchX)
        {
            var offMin = rt.offsetMin;
            var offMax = rt.offsetMax;
            offMin.x = x;
            offMax.x = -(containerW - (x + w));
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }
        else
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            var pos = rt.anchoredPosition;
            pos.x = x;
            rt.anchoredPosition = pos;
        }
        if (stretchY)
        {
            float top = -y;
            float bottom = containerH - (top + h);
            var offMin = rt.offsetMin;
            var offMax = rt.offsetMax;
            offMin.y = bottom;
            offMax.y = -top;
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }
        else
        {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
            var pos = rt.anchoredPosition;
            pos.y = y;
            rt.anchoredPosition = pos;
        }
    }

    private RectTransform FindFirstAwakeCellInRow(int row)
    {
        if (row < 0 || row >= totalRowsCountLetTheShowBegin) return null;
        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            var rt = FetchCellRectTransformVIP(row, c);
            if (rt == null) continue;
            var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell == null) continue;
            if (cell.isMashedLikePotatoes && cell.mashedIntoWho != null) continue;
            if (!rt.gameObject.activeInHierarchy) continue;
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
        int neighborRow = (deletingRow > 0) ? deletingRow - 1 : ((deletingRow + 1 < totalRowsCountLetTheShowBegin) ? deletingRow + 1 : -1);
        if (neighborRow >= 0)
        {
            int col = Mathf.Clamp(preferredColumn, 0, Mathf.Max(0, totalColumnsCountHighFive - 1));
            var rt = FetchCellRectTransformVIP(neighborRow, col);
            if (rt != null)
            {
                var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
                if (cell != null && !(cell.isMashedLikePotatoes && cell.mashedIntoWho != null) && rt.gameObject.activeInHierarchy) return rt;
            }
            rt = FindFirstAwakeCellInRow(neighborRow);
            if (rt != null) return rt;
        }
        return FindFirstAwakeCellInTable();
    }

    private RectTransform GetPreferredTargetCellForColumnDeletion(int row, int deletingColumn)
    {
        if (totalColumnsCountHighFive <= 1) return null;
        int neighborCol = (deletingColumn + 1 < totalColumnsCountHighFive) ? deletingColumn + 1 : (deletingColumn > 0 ? deletingColumn - 1 : -1);
        if (neighborCol >= 0)
        {
            var rt = FetchCellRectTransformVIP(row, neighborCol);
            if (rt != null)
            {
                var cell = rt.GetComponent<Ux_TonkersTableTopiaCell>();
                if (cell != null && !(cell.isMashedLikePotatoes && cell.mashedIntoWho != null) && rt.gameObject.activeInHierarchy) return rt;
            }
            rt = FindFirstAwakeCellInRow(row);
            if (rt != null) return rt;
        }
        return FindFirstAwakeCellInTable();
    }

    public bool TryKindlyDeleteCell(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return false;
        return SafeDeleteColumnAtWithWittyConfirm(col);
    }

    public bool SafeDeleteRowAtWithWittyConfirm(int index)
    {
#if UNITY_EDITOR
        var foreign = CollectWanderingObjectsForRow(index);
        if (foreign.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Row Removal", BuildConfirmBodyHilarious($"Row {index + 1}", foreign), "Delete With Gusto", "Cancel Kindly")) return false;
        }
#endif
        return TryPolitelyDeleteRow(index);
    }

    public bool SafeDeleteColumnAtWithWittyConfirm(int index)
    {
#if UNITY_EDITOR
        var foreign = CollectWanderingObjectsForColumn(index);
        if (foreign.Count > 0)
        {
            if (!EditorUtility.DisplayDialog("Column Removal", BuildConfirmBodyHilarious($"Column {index + 1}", foreign), "Delete With Gusto", "Cancel Kindly")) return false;
        }
#endif
        return TryPolitelyDeleteColumn(index);
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
        var cell = FetchCellRectTransformVIP(row, col);
        if (cell == null) return null;
        if (TryFindChildTableInCellLikeSherlock(row, col, out var existing)) return existing;
        var go = new GameObject("TTT Nested Table");
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(cell, false);
        rt.SnapCroutonToFillParentLikeGravy();
        cell.MakeImageBackgroundNotBlockClicksLikePolite();
        var child = go.AddComponent<Ux_TonkersTableTopiaLayout>();
        child.totalRowsCountLetTheShowBegin = 1;
        child.totalColumnsCountHighFive = 3;
        child.shareThePieEvenlyForColumns = false;
        child.shareThePieEvenlyForRows = false;
        child.autoHugHeightBecauseWhyNot = true;
        child.fancyColumnWardrobes = new List<ColumnStyle>();
        child.snazzyRowWardrobes = new List<RowStyle>();
        for (int c = 0; c < child.totalColumnsCountHighFive; c++) child.fancyColumnWardrobes.Add(new ColumnStyle { requestedWidthMaybePercentIfNegative = -(1f / child.totalColumnsCountHighFive) });
        for (int r = 0; r < child.totalRowsCountLetTheShowBegin; r++) child.snazzyRowWardrobes.Add(new RowStyle { requestedHeightMaybePercentIfNegative = -1f });
        child.RebuildComedyClubSeatingChart();
        child.FlagLayoutAsNeedingSpaDay();
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
}

