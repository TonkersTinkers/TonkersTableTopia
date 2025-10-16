#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
#endif
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("Layout/Tonkers Table Topia")]
public class Ux_TonkersTableTopiaLayout : MonoBehaviour
{
    [Tooltip("How many rows are strutting on stage")]
    [Min(1)] public int totalRowsCountLetTheShowBegin = 1;

    [Tooltip("How many columns are bringing snacks")]
    [Min(1)] public int totalColumnsCountHighFive = 1;

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

    private void Reset()
    {
        fancyColumnWardrobes = new List<ColumnStyle>();
        snazzyRowWardrobes = new List<RowStyle>();
        for (int columnStepperCrouton = 0; columnStepperCrouton < totalColumnsCountHighFive; columnStepperCrouton++)
            fancyColumnWardrobes.Add(new ColumnStyle());
        for (int rowStepperWaffle = 0; rowStepperWaffle < totalRowsCountLetTheShowBegin; rowStepperWaffle++)
            snazzyRowWardrobes.Add(new RowStyle());

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
        for (int c = 0; c < cols; c++)
            fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -colPct;
        for (int r = 0; r < rows; r++)
            snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -rowPct;

        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnEnable()
    {
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnTransformChildrenChanged()
    {
        FlagLayoutAsNeedingSpaDay();
    }

    private void OnRectTransformDimensionsChange()
    {
        ConvertAllSpecsToPercentages();
        FlagLayoutAsNeedingSpaDay();
    }

    public void ConvertAllSpecsToPercentages()
    {
        EnsureWardrobeListsMatchHeadcount();
        var rt = GetComponent<RectTransform>();
        if (rt == null) return;

        float innerW = Mathf.Max(0f, rt.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerH = Mathf.Max(0f, rt.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);

        int cols = Mathf.Max(1, totalColumnsCountHighFive);
        int rows = Mathf.Max(1, totalRowsCountLetTheShowBegin);

        float colSpacingSum = sociallyDistancedColumnsPixels * Mathf.Max(0, cols - 1);
        float rowSpacingSum = sociallyDistancedRowsPixels * Mathf.Max(0, rows - 1);

        float[] colPixels = Distribute1D(cols, i =>
            (i < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[i].requestedWidthMaybePercentIfNegative : 0f,
            sociallyDistancedColumnsPixels, innerW);
        float[] rowPixels = Distribute1D(rows, i =>
            (i < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[i].requestedHeightMaybePercentIfNegative : 0f,
            sociallyDistancedRowsPixels, innerH);

        float availW = Mathf.Max(1f, innerW - colSpacingSum);
        float availH = Mathf.Max(1f, innerH - rowSpacingSum);

        float sumPct = 0f;
        for (int c = 0; c < cols; c++)
        {
            float pct = Mathf.Clamp01(colPixels[c] / availW);
            fancyColumnWardrobes[c].requestedWidthMaybePercentIfNegative = -pct;
            sumPct += pct;
        }
        if (sumPct > 0f)
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
            snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -pct;
            sumPct += pct;
        }
        if (sumPct > 0f)
        {
            for (int r = 0; r < rows; r++)
            {
                float pct = -snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative;
                snazzyRowWardrobes[r].requestedHeightMaybePercentIfNegative = -(pct / sumPct);
            }
        }
    }

    private float[] Distribute1D(int count, System.Func<int, float> getSpec, float spacing, float inner)
    {
        var result = new float[count];
        var fixedPx = new float[count];
        var perc = new float[count];
        var flexShares = new int[count];

        float fixedSum = 0f;
        float percSum = 0f;
        int flexCount = 0;

        for (int i = 0; i < count; i++)
        {
            float v = getSpec != null ? getSpec(i) : 0f;
            if (v > 0f) { fixedPx[i] = v; fixedSum += v; }
            else if (v < 0f) { float p = Mathf.Clamp01(-v); perc[i] = p; percSum += p; }
            else { flexShares[i] = 1; flexCount++; }
        }

        float remain = Mathf.Max(0f, inner - spacing * Mathf.Max(0, count - 1) - fixedSum);
        float usedByPerc = 0f;

        if (percSum > 0f)
        {
            float norm = percSum > 1f ? (1f / percSum) : 1f;
            for (int i = 0; i < count; i++)
            {
                if (perc[i] > 0f)
                {
                    float a = remain * (perc[i] * norm);
                    result[i] = fixedPx[i] + a;
                    usedByPerc += a;
                }
            }
        }

        float residual = Mathf.Max(0f, remain - usedByPerc);
        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int i = 0; i < count; i++)
                result[i] = result[i] > 0f ? result[i] : (fixedPx[i] + share);
        }
        else
        {
            for (int i = 0; i < count; i++)
                result[i] = result[i] > 0f ? result[i] : fixedPx[i];
            if (count > 0 && residual > 0f) result[count - 1] += residual;
        }

        for (int i = 0; i < count; i++) if (result[i] < 0f) result[i] = 0f;
        return result;
    }

    private void OnValidate()
    {
        EnsureWardrobeListsMatchHeadcount();
        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
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
        if (Application.isPlaying)
        {
            if (layoutNeedsAFreshCoatOfPaint)
            {
                UpdateSeatingLikeAProUsher();
                layoutNeedsAFreshCoatOfPaint = false;
            }
        }
    }

    public void FlagLayoutAsNeedingSpaDay()
    {
        layoutNeedsAFreshCoatOfPaint = true;
#if UNITY_EDITOR
        if (!Application.isPlaying)
            ScheduleEditorLayoutTouchUp();
#endif
    }

#if UNITY_EDITOR
    private static readonly System.Collections.Generic.HashSet<int> _scheduledEditorUpdates = new System.Collections.Generic.HashSet<int>();

    private void ScheduleEditorLayoutTouchUp()
    {
        int instanceIdentificationBadge = GetInstanceID();
        if (_scheduledEditorUpdates.Contains(instanceIdentificationBadge)) return;

        _scheduledEditorUpdates.Add(instanceIdentificationBadge);
        UnityEditor.EditorApplication.delayCall += () =>
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

    // TonkersTableTopiaLayout
    public void RebuildComedyClubSeatingChart()
    {
        EnsureWardrobeListsMatchHeadcount();
        RectTransform tableRectTransformMainStage = GetComponent<RectTransform>();
        if (tableRectTransformMainStage == null) return;

        if (totalColumnsCountHighFive < 1) totalColumnsCountHighFive = 1;
        if (totalRowsCountLetTheShowBegin < 1) totalRowsCountLetTheShowBegin = 1;

        backstageRowRectsVIP.Clear();
        backstageCellRectsVIP.Clear();

        var rootChildrenRoundup = new List<RectTransform>();
        for (int childGrabber = 0; childGrabber < tableRectTransformMainStage.childCount; childGrabber++)
            rootChildrenRoundup.Add(tableRectTransformMainStage.GetChild(childGrabber) as RectTransform);

        var managedRowsLine = new List<RectTransform>();
        var nonManagedRootBackpack = new List<RectTransform>();
        foreach (var childRect in rootChildrenRoundup)
        {
            if (childRect == null) continue;
            if (IsThisRowOnTheGuestList(childRect)) managedRowsLine.Add(childRect);
            else nonManagedRootBackpack.Add(childRect);
        }
        managedRowsLine.Sort((left, right) => left.GetSiblingIndex().CompareTo(right.GetSiblingIndex()));

        while (managedRowsLine.Count < totalRowsCountLetTheShowBegin)
        {
            var freshlyMintedRowObject = new GameObject("TTT Row " + (managedRowsLine.Count + 1));
            var freshlyMintedRowRect = freshlyMintedRowObject.AddComponent<RectTransform>();
            freshlyMintedRowObject.AddComponent<Ux_TonkersTableTopiaRow>();
#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.RegisterCreatedObjectUndo(freshlyMintedRowObject, "Add Tonkers Table Topia Row");
#endif
            freshlyMintedRowRect.SetParent(tableRectTransformMainStage, false);
            managedRowsLine.Add(freshlyMintedRowRect);
        }

        for (int rowLooperCroissant = 0; rowLooperCroissant < totalRowsCountLetTheShowBegin; rowLooperCroissant++)
        {
            var thisRowRect = managedRowsLine[rowLooperCroissant];
            var thisRowComponent = thisRowRect.GetComponent<Ux_TonkersTableTopiaRow>() ?? thisRowRect.gameObject.AddComponent<Ux_TonkersTableTopiaRow>();
            thisRowComponent.rowNumberWhereShenanigansOccur = rowLooperCroissant;

            var rowChildrenRoundup = new List<RectTransform>();
            for (int kidCollector = 0; kidCollector < thisRowRect.childCount; kidCollector++)
                rowChildrenRoundup.Add(thisRowRect.GetChild(kidCollector) as RectTransform);

            var managedCellsLine = new List<RectTransform>();
            var nonManagedRowBackpack = new List<RectTransform>();
            foreach (var childRect in rowChildrenRoundup)
            {
                if (childRect == null) continue;
                if (IsThisCellOnTheGuestList(childRect)) managedCellsLine.Add(childRect);
                else nonManagedRowBackpack.Add(childRect);
            }
            managedCellsLine.Sort((left, right) => left.GetSiblingIndex().CompareTo(right.GetSiblingIndex()));

            while (managedCellsLine.Count < totalColumnsCountHighFive)
            {
                var freshCellObject = new GameObject($"TTT Cell {rowLooperCroissant + 1},{managedCellsLine.Count + 1}");
                var freshCellRect = freshCellObject.AddComponent<RectTransform>();
                freshCellObject.AddComponent<Ux_TonkersTableTopiaCell>();
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.RegisterCreatedObjectUndo(freshCellObject, "Add Tonkers Table Topia Cell");
#endif
                freshCellRect.SetParent(thisRowRect, false);
                managedCellsLine.Add(freshCellRect);
            }

            var targetCellForRowForeign = managedCellsLine[0];
            EscortNonVIPsToTarget(thisRowRect, targetCellForRowForeign);

            for (int cullStepper = managedCellsLine.Count - 1; cullStepper >= totalColumnsCountHighFive; cullStepper--)
            {
                var extraCellRect = managedCellsLine[cullStepper];
                EscortNonVIPsToTarget(extraCellRect, targetCellForRowForeign);
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(extraCellRect.gameObject);
                else
#endif
                    Destroy(extraCellRect.gameObject);
                managedCellsLine.RemoveAt(cullStepper);
            }

            var thisRowCellsRects = new List<RectTransform>();
            for (int columnLooperPretzel = 0; columnLooperPretzel < totalColumnsCountHighFive; columnLooperPretzel++)
            {
                var cellRect = managedCellsLine[columnLooperPretzel];
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
                if (cellRect.gameObject.activeSelf != shouldBeActiveBecauseNotMashed)
                    cellRect.gameObject.SetActive(shouldBeActiveBecauseNotMashed);

                thisRowCellsRects.Add(cellRect);
            }

            thisRowRect.gameObject.name = "TTT Row " + (rowLooperCroissant + 1);
            backstageRowRectsVIP.Add(thisRowRect);
            backstageCellRectsVIP.Add(thisRowCellsRects);
        }

        if (managedRowsLine.Count > totalRowsCountLetTheShowBegin)
        {
            var targetCell = FetchCellRectTransformVIP(0, 0);
            for (int rowReaper = managedRowsLine.Count - 1; rowReaper >= totalRowsCountLetTheShowBegin; rowReaper--)
            {
                var extraRow = managedRowsLine[rowReaper];
                if (targetCell != null) EscortNonVIPsToTarget(extraRow, targetCell);
#if UNITY_EDITOR
                if (!Application.isPlaying) Undo.DestroyObjectImmediate(extraRow.gameObject);
                else
#endif
                    Destroy(extraRow.gameObject);
            }
        }

        if (nonManagedRootBackpack.Count > 0)
        {
            var target = FetchCellRectTransformVIP(0, 0);
            if (target != null) EscortNonVIPsToTarget(tableRectTransformMainStage, target);
        }
    }

    public void UpdateSeatingLikeAProUsher()
    {
        var tableRectTransformMainStage = GetComponent<RectTransform>();
        if (tableRectTransformMainStage == null) return;

        RebuildComedyClubSeatingChart();

        if (totalColumnsCountHighFive == 0 || totalRowsCountLetTheShowBegin == 0) return;

        float[] calculatedRowHeightsBuffet = CalculateRowHeightsLikeAShortOrderCook(tableRectTransformMainStage);
        float totalContentHeightCalories = CalculateTotalContentHeightCalories(calculatedRowHeightsBuffet);

        if (autoHugHeightBecauseWhyNot && !AreWeStretchingVertically(tableRectTransformMainStage))
            tableRectTransformMainStage.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentHeightCalories);

        float innerWidthPlayable = Mathf.Max(0, tableRectTransformMainStage.rect.width - comfyPaddingLeftForElbows - comfyPaddingRightForElbows);
        float innerHeightPlayable = Mathf.Max(0, tableRectTransformMainStage.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);

        float leftOffsetStart = comfyPaddingLeftForElbows;
        float currentYTopDown = CalculateVerticalGrandEntrance(tableRectTransformMainStage, totalContentHeightCalories) - comfyPaddingTopHat;

        for (int rowStepperWaffle = 0; rowStepperWaffle < totalRowsCountLetTheShowBegin; rowStepperWaffle++)
        {
            float rowHeightThisRound = calculatedRowHeightsBuffet[rowStepperWaffle];

            ParkRowInItsAssignedSpot(
                rowStepperWaffle,
                rowHeightThisRound,
                tableRectTransformMainStage,
                innerWidthPlayable,
                leftOffsetStart,
                currentYTopDown,
                tableRectTransformMainStage.rect.width,
                tableRectTransformMainStage.rect.height
            );

            float[] perCellWidthBuffet = CalculatePerCellWidthsForThisRow(rowStepperWaffle, innerWidthPlayable);
            ParkCellsInRowWithoutDingedBumpers(rowStepperWaffle, rowHeightThisRound, innerWidthPlayable, calculatedRowHeightsBuffet, perCellWidthBuffet);

            currentYTopDown -= (rowHeightThisRound + sociallyDistancedRowsPixels);
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

        ApplyRectPlacementWithPanache(
            rowRect,
            aMin,
            aMax,
            piv,
            offsetXLeft,
            yFromTop,
            innerWidthPlayable,
            rowHeightThisRound,
            totalW,
            totalH
        );

        DressRowInItsSundayBest(rowNumberIndex, rowRect);
    }

    private void DressRowInItsSundayBest(int rowNumberIndex, RectTransform rowRect)
    {
        var rowStyle = (rowNumberIndex < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowNumberIndex] : null;
        if (rowStyle != null && rowStyle.backdropPictureOnTheHouse != null)
        {
            var img = rowRect.GetComponent<Image>();
            if (img == null) img = rowRect.gameObject.AddComponent<Image>();
            img.sprite = rowStyle.backdropPictureOnTheHouse;
            img.color = rowStyle.backdropTintFlavor;
            img.type = Image.Type.Sliced;
            img.enabled = true;
        }
        else
        {
            var img = rowRect.GetComponent<Image>();
            if (img != null) img.enabled = false;
        }
    }

    private void ParkCellsInRowWithoutDingedBumpers(int rowNumberIndex, float rowHeightThisRound, float innerWidthPlayable, float[] allRowHeights, float[] perCellWidthBuffet)
    {
        float currentXLeftToRight = 0f;

        for (int columnStepperPretzel = 0; columnStepperPretzel < totalColumnsCountHighFive; columnStepperPretzel++)
        {
            var cellRect = backstageCellRectsVIP[rowNumberIndex][columnStepperPretzel];
            var cellComp = cellRect ? cellRect.GetComponent<Ux_TonkersTableTopiaCell>() : null;
            if (cellComp == null) continue;
            if (cellComp.isMashedLikePotatoes) continue;

            float cellW = Mathf.Max(0f, perCellWidthBuffet[columnStepperPretzel]);
            float cellH = CalculateCellHeightForTheStretch(rowNumberIndex, cellComp.howManyRowsAreHoggingThisSeat, allRowHeights);
            if (cellComp.howManyRowsAreHoggingThisSeat <= 1) cellH = rowHeightThisRound;

            Vector2 aMin, aMax, piv;
            var colStyle = (columnStepperPretzel < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[columnStepperPretzel] : null;

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

            ApplyRectPlacementWithPanache(
                cellRect,
                aMin,
                aMax,
                piv,
                currentXLeftToRight,
                0f,
                cellW,
                cellH,
                innerWidthPlayable,
                rowHeightThisRound
            );

            DressCellToImpress(cellRect, cellComp, columnStepperPretzel);

            currentXLeftToRight += cellW + sociallyDistancedColumnsPixels;

            if (cellComp.howManyColumnsAreSneakingIn > 1)
                columnStepperPretzel += cellComp.howManyColumnsAreSneakingIn - 1;
        }
    }

    private void DressCellToImpress(RectTransform cellRect, Ux_TonkersTableTopiaCell cellComp, int columnIndex)
    {
        Image img = cellRect.GetComponent<Image>();
        Sprite useSprite = null;
        Color useColor = Color.white;

        if (cellComp.backgroundPictureBecausePlainIsLame != null)
        {
            useSprite = cellComp.backgroundPictureBecausePlainIsLame;
            useColor = cellComp.backgroundColorLikeASunset;
        }
        else if (columnIndex < fancyColumnWardrobes.Count && fancyColumnWardrobes[columnIndex].backdropPictureOnTheHouse != null)
        {
            useSprite = fancyColumnWardrobes[columnIndex].backdropPictureOnTheHouse;
            useColor = fancyColumnWardrobes[columnIndex].backdropTintFlavor;
        }

        if (useSprite != null)
        {
            if (img == null) img = cellRect.gameObject.AddComponent<Image>();
            img.sprite = useSprite;
            img.color = useColor;
            img.type = Image.Type.Sliced;
            img.enabled = true;
            return;
        }

        bool useRow = toggleZebraStripesForRows;
        bool useCol = toggleZebraStripesForColumns;

        if (useRow || useCol)
        {
            if (img == null) img = cellRect.gameObject.AddComponent<Image>();

            int safeRow = Mathf.Max(0, cellComp.rowNumberWhereThePartyIs);
            int safeCol = Mathf.Max(0, cellComp.columnNumberPrimeRib);

            Color rc = ((safeRow & 1) == 0) ? zebraRowColorA : zebraRowColorB;
            Color cc = ((safeCol & 1) == 0) ? zebraColumnColorA : zebraColumnColorB;
            Color final = useRow && useCol
                ? new Color((rc.r + cc.r) * 0.5f, (rc.g + cc.g) * 0.5f, (rc.b + cc.b) * 0.5f, Mathf.Max(rc.a, cc.a))
                : (useRow ? rc : cc);

            img.sprite = null;
            img.type = Image.Type.Simple;
            img.color = final;
            img.enabled = true;
        }
        else
        {
            if (img != null) img.enabled = false;
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

    private float SumUpFixedWidthForThisCellIfAny(int rowIndex, int colIndex, Ux_TonkersTableTopiaCell cellComp)
    {
        if (cellComp == null) return -1f;

        var layoutElementPeek = backstageCellRectsVIP[rowIndex][colIndex].GetComponent<LayoutElement>();
        if (layoutElementPeek != null && layoutElementPeek.enabled && layoutElementPeek.preferredWidth > 0f)
            return layoutElementPeek.preferredWidth;

        if (cellComp.howManyColumnsAreSneakingIn <= 1)
        {
            if (colIndex < fancyColumnWardrobes.Count && fancyColumnWardrobes[colIndex].requestedWidthMaybePercentIfNegative > 0f)
                return fancyColumnWardrobes[colIndex].requestedWidthMaybePercentIfNegative;

            return -1f;
        }

        float sum = 0f;
        bool any = false;
        int maxCol = Mathf.Min(totalColumnsCountHighFive, colIndex + cellComp.howManyColumnsAreSneakingIn);

        for (int cc = colIndex; cc < maxCol; cc++)
        {
            if (cc < fancyColumnWardrobes.Count && fancyColumnWardrobes[cc].requestedWidthMaybePercentIfNegative > 0f)
            {
                sum += fancyColumnWardrobes[cc].requestedWidthMaybePercentIfNegative;
                any = true;
            }
        }

        if (cellComp.howManyColumnsAreSneakingIn > 1)
            sum += sociallyDistancedColumnsPixels * (Mathf.Min(cellComp.howManyColumnsAreSneakingIn, totalColumnsCountHighFive - colIndex) - 1);

        return any ? sum : -1f;
    }

    private float[] CalculatePerCellWidthsForThisRow(int rowIndex, float innerWidthPlayable)
    {
        var startCols = new List<int>();
        var spans = new List<int>();

        for (int colProbe = 0; colProbe < totalColumnsCountHighFive; colProbe++)
        {
            var cell = backstageCellRectsVIP[rowIndex][colProbe].GetComponent<Ux_TonkersTableTopiaCell>();
            if (cell == null) continue;
            if (cell.isMashedLikePotatoes) continue;

            int span = Mathf.Max(1, Mathf.Min(totalColumnsCountHighFive - colProbe, cell.howManyColumnsAreSneakingIn));
            startCols.Add(colProbe);
            spans.Add(span);

            if (cell.howManyColumnsAreSneakingIn > 1)
                colProbe += cell.howManyColumnsAreSneakingIn - 1;
        }

        int visibleCount = startCols.Count;
        float totalSpacingX = Mathf.Max(0, visibleCount - 1) * sociallyDistancedColumnsPixels;
        float availableRowWidth = Mathf.Max(0, innerWidthPlayable - totalSpacingX);

        var fixedPx = new float[visibleCount];
        var perc = new float[visibleCount];
        var flexShares = new int[visibleCount];

        float fixedSum = 0f;

        for (int iSlot = 0; iSlot < visibleCount; iSlot++)
        {
            int cStart = startCols[iSlot];
            var cell = backstageCellRectsVIP[rowIndex][cStart].GetComponent<Ux_TonkersTableTopiaCell>();

            fixedPx[iSlot] = Mathf.Max(0f, SumUpFixedWidthForThisCellIfAny(rowIndex, cStart, cell));
            fixedSum += fixedPx[iSlot];

            int span = spans[iSlot];
            for (int cc = cStart; cc < cStart + span; cc++)
            {
                float spec = (cc < fancyColumnWardrobes.Count) ? fancyColumnWardrobes[cc].requestedWidthMaybePercentIfNegative : 0f;
                if (spec < 0f) perc[iSlot] += Mathf.Clamp01(-spec);
                else if (spec == 0f) flexShares[iSlot]++;
            }
        }

        float residualAfterFixed = availableRowWidth - fixedSum;

        int sinkIndex = visibleCount > 0 ? visibleCount - 1 : -1;
        if (residualAfterFixed < 0f && sinkIndex >= 0)
        {
            fixedPx[sinkIndex] = Mathf.Max(1f, fixedPx[sinkIndex] + residualAfterFixed);
            residualAfterFixed = 0f;
        }

        float percSum = 0f;
        for (int i = 0; i < visibleCount; i++) percSum += perc[i];

        var percAlloc = new float[visibleCount];
        if (residualAfterFixed > 0f && percSum > 0f)
        {
            float norm = percSum > 1f ? (1f / percSum) : 1f;
            float used = 0f;

            for (int i = 0; i < visibleCount; i++)
            {
                percAlloc[i] = residualAfterFixed * (perc[i] * norm);
                used += percAlloc[i];
            }

            residualAfterFixed = Mathf.Max(0f, residualAfterFixed - used);
        }

        var flexAlloc = new float[visibleCount];
        int totalFlexShares = 0;
        for (int i = 0; i < visibleCount; i++) totalFlexShares += flexShares[i];

        if (residualAfterFixed > 0f)
        {
            if (totalFlexShares > 0)
            {
                for (int i = 0; i < visibleCount; i++)
                {
                    if (flexShares[i] > 0)
                        flexAlloc[i] = residualAfterFixed * ((float)flexShares[i] / totalFlexShares);
                }
            }
            else if (sinkIndex >= 0)
            {
                flexAlloc[sinkIndex] += residualAfterFixed;
            }
        }

        var perCellWidth = new float[totalColumnsCountHighFive];
        for (int i = 0; i < visibleCount; i++)
        {
            int cStart = startCols[i];
            perCellWidth[cStart] = Mathf.Max(0f, fixedPx[i] + percAlloc[i] + flexAlloc[i]);
        }

        return perCellWidth;
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
        var result = new float[totalRowsCountLetTheShowBegin];

        if (shareThePieEvenlyForRows)
        {
            float totalInnerHeight = Mathf.Max(0, tableRect.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
            float even = (totalInnerHeight - sociallyDistancedRowsPixels * Mathf.Max(0, totalRowsCountLetTheShowBegin - 1)) / Mathf.Max(1, totalRowsCountLetTheShowBegin);
            if (even < 0) even = 0;
            for (int rowWalker = 0; rowWalker < totalRowsCountLetTheShowBegin; rowWalker++) result[rowWalker] = even;
            return result;
        }

        float innerH = Mathf.Max(0, tableRect.rect.height - comfyPaddingTopHat - comfyPaddingBottomCushion);
        float spacingSum = sociallyDistancedRowsPixels * Mathf.Max(0, totalRowsCountLetTheShowBegin - 1);

        float fixedSum = 0f;
        float percSum = 0f;
        int flexCount = 0;

        for (int rowScan = 0; rowScan < totalRowsCountLetTheShowBegin; rowScan++)
        {
            float spec = (rowScan < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowScan].requestedHeightMaybePercentIfNegative : 0f;
            if (spec > 0f) fixedSum += spec;
            else if (spec < 0f) percSum += Mathf.Clamp01(-spec);
            else flexCount++;
        }

        float available = Mathf.Max(0f, innerH - spacingSum - fixedSum);

        float usedByPerc = 0f;
        float norm = percSum > 1f && percSum > 0f ? (1f / percSum) : 1f;

        for (int rowScan = 0; rowScan < totalRowsCountLetTheShowBegin; rowScan++)
        {
            float spec = (rowScan < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowScan].requestedHeightMaybePercentIfNegative : 0f;

            if (spec > 0f) result[rowScan] = spec;
            else if (spec < 0f)
            {
                float p = Mathf.Clamp01(-spec) * norm;
                float h = available * p;
                result[rowScan] = Mathf.Max(0f, h);
                usedByPerc += h;
            }
            else result[rowScan] = 0f;
        }

        float residual = Mathf.Max(0f, available - usedByPerc);

        if (flexCount > 0)
        {
            float share = residual / flexCount;
            for (int rowScan = 0; rowScan < totalRowsCountLetTheShowBegin; rowScan++)
            {
                float spec = (rowScan < snazzyRowWardrobes.Count) ? snazzyRowWardrobes[rowScan].requestedHeightMaybePercentIfNegative : 0f;
                if (Mathf.Approximately(spec, 0f)) result[rowScan] = Mathf.Max(0f, share);
            }
        }
        else if (totalRowsCountLetTheShowBegin > 0 && residual > 0f)
        {
            int sink = totalRowsCountLetTheShowBegin - 1;
            result[sink] += residual;
        }

        return result;
    }

    private float AskNicelyForPreferredHeight(RectTransform rt)
    {
        float h = LayoutUtility.GetPreferredHeight(rt);
        if (h <= 0f) h = rt.rect.height;
        return h;
    }

    private float AskNicelyForPreferredWidth(RectTransform rt)
    {
        float w = LayoutUtility.GetPreferredWidth(rt);
        if (w <= 0f) w = rt.rect.width;

        if (w <= 0f)
        {
            float maxChild = 0f;
            for (int childProbe = 0; childProbe < rt.childCount; childProbe++)
            {
                var ch = rt.GetChild(childProbe) as RectTransform;
                if (ch == null || !ch.gameObject.activeInHierarchy) continue;

                float pw = LayoutUtility.GetPreferredWidth(ch);
                if (pw <= 0f) pw = ch.rect.width;
                if (pw > maxChild) maxChild = pw;
            }
            w = maxChild;
        }

        if (w < 1f) w = 1f;
        return w;
    }

    private bool AreWeStretchingHorizontally(RectTransform rt)
    {
        return Mathf.Abs(rt.anchorMin.x - rt.anchorMax.x) > 0.001f;
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

    // TonkersTableTopiaLayout
    public void SwapColumnsLikeTradingCards(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalColumnsCountHighFive || bIndex >= totalColumnsCountHighFive) return;
        RebuildComedyClubSeatingChart();
        if (aIndex > bIndex) { var t = aIndex; aIndex = bIndex; bIndex = t; }

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

    // TonkersTableTopiaLayout
    public void SwapRowsLikeMusicalChairs(int aIndex, int bIndex)
    {
        if (aIndex == bIndex || aIndex < 0 || bIndex < 0 || aIndex >= totalRowsCountLetTheShowBegin || bIndex >= totalRowsCountLetTheShowBegin) return;
        RebuildComedyClubSeatingChart();
        if (aIndex > bIndex) { var t = aIndex; aIndex = bIndex; bIndex = t; }

        (snazzyRowWardrobes[aIndex], snazzyRowWardrobes[bIndex]) = (snazzyRowWardrobes[bIndex], snazzyRowWardrobes[aIndex]);

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
        if (startRow < 0 || startCol < 0 || startRow + rowCount > totalRowsCountLetTheShowBegin || startCol + colCount > totalColumnsCountHighFive) return;
        if (rowCount == 1 && colCount == 1) return;

        RebuildComedyClubSeatingChart();

        RectTransform mainCellRT = backstageCellRectsVIP[startRow][startCol];
        Ux_TonkersTableTopiaCell mainCellComp = mainCellRT.GetComponent<Ux_TonkersTableTopiaCell>();
        if (mainCellComp == null) return;

        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                Ux_TonkersTableTopiaCell cc = backstageCellRectsVIP[r][c].GetComponent<Ux_TonkersTableTopiaCell>();
                if (cc != null && cc.isMashedLikePotatoes && cc.mashedIntoWho != null)
                {
                    return;
                }
            }
        }

        mainCellComp.howManyRowsAreHoggingThisSeat = rowCount;
        mainCellComp.howManyColumnsAreSneakingIn = colCount;
        mainCellComp.isMashedLikePotatoes = false;
        mainCellComp.mashedIntoWho = null;

        for (int r = startRow; r < startRow + rowCount; r++)
        {
            for (int c = startCol; c < startCol + colCount; c++)
            {
                if (r == startRow && c == startCol) continue;

                RectTransform cellRT = backstageCellRectsVIP[r][c];
                Ux_TonkersTableTopiaCell cellComp = cellRT.GetComponent<Ux_TonkersTableTopiaCell>();

                if (cellComp != null)
                {
                    cellComp.isMashedLikePotatoes = true;
                    cellComp.mashedIntoWho = mainCellComp;
                }

                cellRT.gameObject.SetActive(false);
            }
        }

        mainCellRT.gameObject.name = $"Cell {startRow + 1},{startCol + 1} Mega Combo {rowCount}x{colCount}";
        FlagLayoutAsNeedingSpaDay();
    }

    // TonkersTableTopiaLayout
    public void UnmergeCellEveryoneGetsTheirOwnChair(int row, int col)
    {
        if (row < 0 || col < 0 || row >= totalRowsCountLetTheShowBegin || col >= totalColumnsCountHighFive) return;
        RectTransform cellRT = backstageCellRectsVIP[row][col];
        Ux_TonkersTableTopiaCell cellComp = cellRT.GetComponent<Ux_TonkersTableTopiaCell>();
        if (cellComp == null) return;
        if (cellComp.howManyRowsAreHoggingThisSeat <= 1 && cellComp.howManyColumnsAreSneakingIn <= 1) return;

        int spanRows = cellComp.howManyRowsAreHoggingThisSeat;
        int spanCols = cellComp.howManyColumnsAreSneakingIn;

        cellComp.howManyRowsAreHoggingThisSeat = 1;
        cellComp.howManyColumnsAreSneakingIn = 1;
        cellComp.isMashedLikePotatoes = false;
        cellComp.mashedIntoWho = null;

        for (int r = row; r < row + spanRows && r < totalRowsCountLetTheShowBegin; r++)
        {
            for (int c = col; c < col + spanCols && c < totalColumnsCountHighFive; c++)
            {
                RectTransform otherRT = backstageCellRectsVIP[r][c];
                Ux_TonkersTableTopiaCell otherComp = otherRT.GetComponent<Ux_TonkersTableTopiaCell>();
                if (otherComp == null) continue;
                if (r == row && c == col) continue;
                if (otherComp.mashedIntoWho == cellComp)
                {
                    otherComp.isMashedLikePotatoes = false;
                    otherComp.mashedIntoWho = null;
                    otherRT.gameObject.SetActive(true);
                }
            }
        }

        cellRT.gameObject.name = $"TTT Cell {row + 1},{col + 1}";
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

        RectTransform tableRT = GetComponent<RectTransform>();
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

        var buffer = new List<Transform>();
        for (int childScan = 0; childScan < from.childCount; childScan++)
        {
            var ch = from.GetChild(childScan);
            if (!IsThisRowOnTheGuestList(ch) && !IsThisCellOnTheGuestList(ch)) buffer.Add(ch);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying) Undo.RecordObject(this, "Move Guests With Style");
#endif
        foreach (var tr in buffer)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.SetTransformParent(tr, to, "Move Guests With Style");
            else
#endif
                tr.SetParent(to, false);
        }
    }

    public bool InsertRowLikeANinja(int index)
    {
        if (index < 0 || index > totalRowsCountLetTheShowBegin) index = Mathf.Clamp(index, 0, totalRowsCountLetTheShowBegin);
        totalRowsCountLetTheShowBegin++;

        if (index >= 0 && index <= snazzyRowWardrobes.Count) snazzyRowWardrobes.Insert(index, new RowStyle());
        else snazzyRowWardrobes.Add(new RowStyle());

        RebuildComedyClubSeatingChart();

        for (int i = totalRowsCountLetTheShowBegin - 1; i > index; i--) SwapRowsLikeMusicalChairs(i, i - 1);

        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool InsertColumnLikeANinja(int index)
    {
        if (index < 0 || index > totalColumnsCountHighFive) index = Mathf.Clamp(index, 0, totalColumnsCountHighFive);
        totalColumnsCountHighFive++;

        if (index >= 0 && index <= fancyColumnWardrobes.Count) fancyColumnWardrobes.Insert(index, new ColumnStyle());
        else fancyColumnWardrobes.Add(new ColumnStyle());

        RebuildComedyClubSeatingChart();

        for (int i = totalColumnsCountHighFive - 1; i > index; i--) SwapColumnsLikeTradingCards(i, i - 1);

        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool TryPolitelyDeleteRow(int index)
    {
        if (index < 0 || index >= totalRowsCountLetTheShowBegin) return false;
        if (totalRowsCountLetTheShowBegin <= 1) return false;

        RebuildComedyClubSeatingChart();

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

        var tableRT = GetComponent<RectTransform>();
        RectTransform targetForRowLevel = GetPreferredTargetCellForRowDeletion(index, 0);
        if (targetForRowLevel == null) targetForRowLevel = tableRT;

        var rowRT = backstageRowRectsVIP[index];
        EscortNonVIPsToTarget(rowRT, targetForRowLevel);

        for (int c = 0; c < totalColumnsCountHighFive; c++)
        {
            var cellRT = FetchCellRectTransformVIP(index, c);
            if (cellRT == null) continue;

            RectTransform target = GetPreferredTargetCellForRowDeletion(index, c);
            if (target == null) target = tableRT;

            EscortNonVIPsToTarget(cellRT, target);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying) Undo.DestroyObjectImmediate(rowRT.gameObject);
        else
#endif
            Destroy(rowRT.gameObject);

        totalRowsCountLetTheShowBegin--;
        if (index >= 0 && index < snazzyRowWardrobes.Count) snazzyRowWardrobes.RemoveAt(index);

        RebuildComedyClubSeatingChart();
        FlagLayoutAsNeedingSpaDay();
        return true;
    }

    public bool TryPolitelyDeleteColumn(int index)
    {
        if (index < 0 || index >= totalColumnsCountHighFive) return false;
        if (totalColumnsCountHighFive <= 1) return false;

        RebuildComedyClubSeatingChart();

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

        var tableRT = GetComponent<RectTransform>();

        for (int r = 0; r < totalRowsCountLetTheShowBegin; r++)
        {
            Transform row = backstageRowRectsVIP[r];
            if (row == null) continue;
            if (index >= row.childCount) continue;

            var cellRT = row.GetChild(index) as RectTransform;
            if (cellRT == null) continue;

            RectTransform target = GetPreferredTargetCellForColumnDeletion(r, index);
            if (target == null) target = tableRT;

            EscortNonVIPsToTarget(cellRT, target);

#if UNITY_EDITOR
            if (!Application.isPlaying) Undo.DestroyObjectImmediate(cellRT.gameObject);
            else
#endif
                Destroy(cellRT.gameObject);
        }

        totalColumnsCountHighFive--;
        if (index >= 0 && index < fancyColumnWardrobes.Count) fancyColumnWardrobes.RemoveAt(index);

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

        int neighborCol = (deletingColumn > 0) ? deletingColumn - 1 : ((deletingColumn + 1 < totalColumnsCountHighFive) ? deletingColumn + 1 : -1);
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
            if (!UnityEditor.EditorUtility.DisplayDialog(
                "Row Removal",
                BuildConfirmBodyHilarious($"Row {index + 1}", foreign),
                "Delete With Gusto",
                "Cancel Kindly"))
                return false;
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
            if (!UnityEditor.EditorUtility.DisplayDialog(
                "Column Removal",
                BuildConfirmBodyHilarious($"Column {index + 1}", foreign),
                "Delete With Gusto",
                "Cancel Kindly"))
                return false;
        }
#endif
        return TryPolitelyDeleteColumn(index);
    }

#if UNITY_EDITOR

    private string BuildConfirmBodyHilarious(string subject, System.Collections.Generic.List<GameObject> foreign)
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

    private System.Collections.Generic.List<GameObject> CollectWanderingObjectsForRow(int rowIndex)
    {
        var result = new System.Collections.Generic.List<GameObject>();
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

    private System.Collections.Generic.List<GameObject> CollectWanderingObjectsForColumn(int colIndex)
    {
        var result = new System.Collections.Generic.List<GameObject>();
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

#if UNITY_EDITOR

    private System.Collections.Generic.List<GameObject> CollectWanderingObjectsForCell(int row, int col)
    {
        var result = new System.Collections.Generic.List<GameObject>();
        var cellRT = FetchCellRectTransformVIP(row, col);
        if (cellRT == null) return result;

        for (int i = 0; i < cellRT.childCount; i++)
        {
            var ch = cellRT.GetChild(i);
            if (!IsThisCellOnTheGuestList(ch) && !IsThisRowOnTheGuestList(ch)) result.Add(ch.gameObject);
        }

        return result;
    }

#endif
}