# Tonkers Table Topia (Unity UGUI)

Simple UGUI tables that behave like stretchable layout tables.

* Created for designing quick, fast, stretchable layouts in seconds, that adapt to any resolution with minimum input from the designer.
* Use the nested tables feature often, to easily get the look you desire.

Tonkers Table Topia is a runtime and editor table system for Unity UGUI. It gives you real rows, columns, merges, nested tables, styling, alignment tools, and a WYSIWYG editing workflow that feels like working with an actual table instead of wrestling random RectTransforms into submission.

You can click cells, drag resize handles, merge and unmerge, move contents between cells, drop hierarchy objects directly into the table, style rows and columns, and keep the whole thing working in edit mode and play mode without building your UI out of desperation and loose anchors.

Quick video:  
[FYI: Outdated Early Version in Video]  
[Feel free to make an updated tutorial and I will link it here]

* [https://youtu.be/Ax740NRROhc](https://youtu.be/Ax740NRROhc)

The game this editor was built for and used in:

* [https://store.steampowered.com/app/3844910/](https://store.steampowered.com/app/3844910/)

## What This Does

Tonkers Table Topia gives UGUI a real table model with a matching editor workflow:

* Rows and columns with mixed sizing rules.
* Merge and unmerge support.
* Insert and delete rows, columns, and cells.
* Nested tables inside cells.
* Table, row, column, and cell background styling.
* Runtime-safe movement and adoption of foreign UI content.
* Alignment helpers for cells, selections, rows, columns, and whole tables.
* A WYSIWYG table editor with headers, selection tools, resize handles, zoom, scrolling, context menus, and scene highlighting.
* Dedicated runtime row, column, and cell components for direct scripting access.

This is still plain UGUI. No external package is required.

## Key Features and Benefits

### WYSIWYG editor workflow

* WYSIWYG preview with row and column headers.
* Click to select a cell.
* Shift-click and drag to extend the selection rectangle.
* Drag to paint a rectangular cell selection.
* Row header and column header selection.
* Top-left header selection for the whole table.
* Double-click a cell with a nested table to jump into it.
* Unified editor modes:
  * Highlight
  * Resize
  * Select
  * Move
* Mode-specific editor panels for structure, merge, resize, alignment, selection, and move workflows.
* Scrollable preview canvas for larger tables.
* Zoom slider with Fit and 1:1 buttons.
* Ctrl or Cmd + mouse wheel zoom in the preview.
* Persistent Preview and Scene toggles in the toolbar.
* Scene highlight for selected cells.
* Per-cell badges showing detected UI content and nested tables.
* Toggle for showing or hiding preview badges.
* Right-click context menus on:
  * cells
  * row headers
  * column headers
  * table header corner
  * resize split handles

### Table structure editing

* Insert row above or below selection.
* Insert column left or right of selection.
* Delete single or multiple selected rows.
* Delete single or multiple selected columns.
* Delete a cell through the cell inspector.
* Merge rectangular selections.
* Unmerge a single merged cell.
* Unmerge everything inside a selected rectangle.
* Bulk delete safety checks for merges and minimum row or column limits.
* Smart handling of foreign children and nested tables during structure edits.
* Optional preservation of current resolved row heights when inserting or deleting rows.
* Optional preservation of current resolved column widths when inserting or deleting columns.

### Sizing and layout

* Mixed row and column sizing:
  * positive values = fixed pixels
  * negative values = percent of inner available size
  * zero = flexible share of remaining space
* Blueprint-style scaling for positive fixed row and column values.
* Per-table design size capture and reuse.
* One-click capture of the current `RectTransform` size as the table design size.
* Inspector editing of fixed values using live resolved pixels.
* Inspector editing of percent values using live computed percentages.
* Drag resize handles in the WYSIWYG preview.
* Live resize HUD showing starting value, new value, and percent delta for the two affected rows or columns.
* Distribute all rows evenly.
* Distribute all columns evenly.
* Split two adjacent columns evenly.
* Split two adjacent rows evenly.
* Make last row flexible.
* Make last column flexible.
* Table container size controls:
  * size by anchored percent
  * size by fixed pixels
* Automatic conversion helpers when switching sizing workflows.
* Row and column spacing controls.
* Table padding support.
* Per-cell inner padding support for hosted content.
* Optional auto hug width.
* Optional auto hug height.
* Optional auto-add `ContentSizeFitter` to each cell.
* Horizontal and vertical extra-space alignment for the whole table.

### Visual styling

* Table background image and tint.
* Table background simple or sliced rendering.
* Per-row background sprites and tints.
* Per-row simple or sliced rendering.
* Per-column background sprites and tints.
* Per-column simple or sliced rendering.
* Column backdrop modes:
  * repeated across cell spans
  * one stretched backdrop for the whole column
* Per-cell background sprite and tint.
* Per-cell simple or sliced rendering.
* Zebra striping by rows.
* Zebra striping by columns.
* Row and column zebra stripes can be combined.
* Editor-wide WYSIWYG preview colors saved in `EditorPrefs`.
* WYSIWYG preview draws real resolved row, column, and cell backdrops instead of only placeholder bands.

### Alignment tools

* Cell-level alignment:
  * Left
  * Center
  * Right
  * Top
  * Middle
  * Bottom
  * Full stretch
* Selection-level alignment.
* Row-level alignment.
* Column-level alignment.
* Table-level alignment.
* Detection helpers so editor buttons can reflect current alignment state.
* Alignment helpers also work for foreign content hosted inside cells.
* Full-stretch content and cell padding are respected by the alignment helpers.

### Content helpers and UI spawning

* Drag external hierarchy objects into table cells from the editor.
* Add common UGUI controls to cells from context menus.
* Editor quick-add menu also includes an Empty Panel helper.
* Add arbitrary prefab content to a cell, row, column, or table through runtime helpers.
* Supported runtime standard content helpers include:
  * Button
  * Image
  * RawImage
  * Text
  * Toggle
  * Slider
  * Scrollbar
  * ScrollRect
  * InputField
  * Dropdown
* Runtime content factory enum for standard content creation.
* Content counting, clearing, listing, and type classification helpers.
* Direct row, column, and cell component APIs for adding content and nested tables.

### Nested tables

* Create a child table inside a cell.
* Select a nested child table from the inspector or context menu.
* Delete a nested child table from the inspector or context menu.
* Double-click a cell with a nested table in the preview to enter it.
* “Up to Parent Tonkers Table” navigation in the layout inspector.
* Parent table lookup helper at runtime.

### Runtime behavior and safety

* Runtime-safe layout refresh in edit mode and play mode.
* Queued play mode refresh using `Canvas.willRenderCanvases`.
* Pooling for managed row and cell scaffold objects.
* Dedicated managed column backdrop objects.
* Cached row, column, and cell component access.
* Cached background image access on managed nodes.
* Centralized hierarchy and object utility helpers.
* Centralized foreign content movement and restore helpers.
* Centralized editor-safe deferred actions.
* Better undo coverage for structure and style edits in the editor.
* Cleaner managed hierarchy naming than the original version.
* Editor guard that rejects invalid multiple TableTopia components on the same `GameObject`.

## Installation

Copy the runtime scripts into your project, for example:

`Assets/UI/TonkersTableTopia/`

Copy the editor scripts into your project, for example:

`Assets/Editor/TonkersTableTopia/`

The package includes the main layout, row, column, and cell components plus supporting runtime extensions, utilities, content helpers, sizing helpers, alignment helpers, editor extensions, inspectors, and context menu tooling.

Add **Tonkers Table Topia** to a `RectTransform` under a Canvas:

* Add Component
* Layout
* Tonkers Table Topia

## Usage

1. Add `Ux_TonkersTableTopiaLayout` to a `RectTransform`.
2. Set row and column counts.
3. Use the WYSIWYG preview to select cells, rows, or columns.
4. Insert, delete, merge, unmerge, distribute, and align directly from the inspector or context menus.
5. Switch to Resize mode and drag split handles to resize rows and columns.
6. Use the design-size controls if you want fixed pixel specs to scale relative to an authored table size.
7. Choose whether row and column sizes should be preserved when inserting or deleting structure.
8. Style the table using row, column, cell, and table background settings.
9. Enable zebra stripes, spacing, padding, and cell inner padding where needed.
10. Use Move mode to move foreign content or nested tables between cells.
11. Drag hierarchy objects directly into cells in the preview.
12. Add nested child tables to cells where needed.
13. Use runtime helpers to add content, inspect cells, merge ranges, or rebuild layout during play.

## Core Concepts

### 1. Row and column size specs

Each row and column uses one of three sizing modes:

* `> 0f`: fixed pixels
* `< 0f`: percent of inner available size, for example `-0.25f`
* `== 0f`: flexible share of the remaining space

Percent sizing is resolved after padding and row or column spacing are removed.

### 2. Blueprint scaling for fixed sizes

Positive fixed specs can scale relative to a saved design size:

* `scaleFixedSizesWithResolutionLikeBlueprint`
* `designSizeForThisTableLikeBlueprint`

When enabled, stored fixed values act as authored blueprint values and are resolved against the current table size. This lets one table behave more predictably across multiple container sizes.

### 3. Table container sizing

The table container itself can be edited in two ways:

* anchored percent of parent
* fixed pixel width and height

This is separate from row and column sizing.

### 4. Auto hug width and height

The table supports both:

* `autoHugWidthLikeAGoodFriend`
* `autoHugHeightBecauseWhyNot`

When the table is not stretched on that axis, it can resize itself to the content it lays out.

### 5. Extra-space alignment

When the table resolves to smaller content than its container allows, the content area can be positioned inside the container using whole-table horizontal and vertical alignment preferences.

### 6. Row leftover fill

Each row can decide whether its last visible main cell should absorb unused row width:

* `RowStyle.lastVisibleCellEatsLeftovers`

This is useful when you want a row to visually consume leftover width instead of leaving a trailing gap.

### 7. Background resolution

Cell visuals resolve in this order:

1. Per-cell background sprite and tint.
2. Column backdrop across the visible span, when applicable.
3. Zebra striping.
4. Otherwise no cell image.

Additional layers can also exist:

* table root background
* row backdrops
* full-column backdrops

### 8. Whole-column backdrops

Column styles support:

* `useOneBigBackdropForWholeColumn`
* `backdropUseSlicedLikePizza`

This allows a column to render as one continuous strip instead of repeated per-cell background pieces.

### 9. Merges and main cells

Merged cells still use one main cell with covered cells pointing at it.

Useful consequences:

* queries can return distinct main cells only
* selection logic can resolve back to the true main cell
* move operations and some editor operations start from the merged main cell instead of an arbitrary covered cell

### 10. Nested tables

Any cell can host another `Ux_TonkersTableTopiaLayout`.

Nested tables are treated as first-class content and can be created, selected, deleted, moved, and navigated through the editor.

### 11. Foreign content

The system distinguishes between:

* managed scaffold objects created by the table
* foreign content, meaning your real UI objects

Foreign content is preserved and reparented carefully during layout rebuilds, merges, deletes, moves, and hierarchy adoption.

### 12. Cell inner padding

Cells can optionally define their own inner content padding:

* `useInnerPaddingPillowFort`
* `innerPaddingLeftMarshmallow`
* `innerPaddingRightMarshmallow`
* `innerPaddingTopMarshmallow`
* `innerPaddingBottomMarshmallow`

Full-stretch hosted content, alignment helpers, and snap-to-fill helpers respect this padding.

## WYSIWYG Editor Workflow

### Modes

#### Highlight

* Click to select a cell.
* Shift-click to extend the selection.
* Drag to paint a rectangular selection.
* Use merge, unmerge, structure tools, and alignment tools against the active selection.

#### Resize

* Drag split handles between columns or rows.
* Resize respects fixed, percent, and flexible semantics.
* Use context menus on split handles for quick even-split and percent presets.
* Live resize HUD shows what changed.

#### Select

* Click a cell to select its cell object in the hierarchy.
* Use this when you want direct hierarchy inspection or custom editing.

#### Move

* Click and drag from a source cell.
* Release over a destination cell.
* Foreign content and nested tables move with layout preserved.

### Preview controls

* Scrollable preview area for larger tables.
* Zoom slider.
* Fit button.
* 1:1 button.
* Ctrl or Cmd + mouse wheel zoom.
* Preview toggle.
* Scene highlight toggle.
* Badge toggle.

### Headers and context menus

* Click row headers to select full rows.
* Click column headers to select full columns.
* Click the top-left header corner to select the whole table.
* Right-click cells, headers, and resize handles for quick actions.

### Inspector views

* Table-level controls.
* Active row inspector from row header selection.
* Active column inspector from column header selection.
* Active cell or combined selection inspector for cell selections.
* Design-size tools.
* Table-size controls.
* Alternating color controls.
* Preview color controls.
* Optional default inspector foldout for advanced access.

## Selected Public API

### Layout lifecycle and refresh

* `RebuildComedyClubSeatingChart()`
* `FlagLayoutAsNeedingSpaDay()`
* `UpdateSeatingLikeAProUsher()`
* `ConvertAllSpecsToPercentages()`
* `CaptureCurrentRectAsDesignSizeLikeBlueprint()`
* `GetDesignInnerWidthLikeBlueprint()`
* `GetDesignInnerHeightLikeBlueprint()`

### Structure editing

* `InsertRowLikeANinja(int index)`
* `InsertColumnLikeANinja(int index)`
* `InsertRowLikeANinjaGetFirstCell(int index)`
* `InsertColumnLikeANinjaGetFirstCell(int index)`
* `TryPolitelyDeleteRow(int index)`
* `TryPolitelyDeleteColumn(int index)`
* `TryKindlyDeleteCell(int row, int col)`
* `SafeDeleteRowAtWithWittyConfirm(int index)`
* `SafeDeleteColumnAtWithWittyConfirm(int index)`
* `BulkDeleteRowsLikeABoss(int startRowInclusive, int endRowInclusive)`
* `BulkDeleteColumnsLikeAChamp(int startColInclusive, int endColInclusive)`
* `SwapRowsLikeMusicalChairs(int aIndex, int bIndex)`
* `SwapColumnsLikeTradingCards(int aIndex, int bIndex)`
* `MergeCellsLikeAGroupHug(int startRow, int startCol, int rowCount, int colCount)`
* `UnmergeCellEveryoneGetsTheirOwnChair(int row, int col)`
* `UnmergeEverythingInRectLikeItNeverHappened(int startRow, int startCol, int rowCount, int colCount)`

### Nested tables

* `CreateChildTableInCellLikeABaby(int row, int col)`
* `TryFindChildTableInCellLikeSherlock(int row, int col, out Ux_TonkersTableTopiaLayout kid)`
* `AddNestedTableToCellLikeRussianDoll(int row, int col, bool ensureSnapToFill = true)`
* `FindParentTableLikeFamilyTree()`
* `FindFirstChildTableLikeEasterEgg(this RectTransform parent, bool includeInactive = true)`

### Sizing and distribution

* `ComputeColumnPercentagesLikeASpreadsheet()`
* `ComputeRowPercentagesLikeASpreadsheet()`
* `DistributeAllColumnsEvenlyLikeAShortOrderCook()`
* `DistributeAllRowsEvenlyLikeAShortOrderCook()`
* `SplitTwoColumnsEvenlyLikePeas(int leftIndex)`
* `SplitTwoRowsEvenlyLikePeas(int topIndex)`
* `NormalizeWardrobePercentsToOneDadBod()`
* `SetColumnPercentageAndRebalanceOthersLikeASpreadsheet(int index, float percent)`
* `SetRowPercentageAndRebalanceOthersLikeASpreadsheet(int index, float percent)`
* `GetLiveColumnWidthPixelsLikeTapeMeasure(int columnIndex)`
* `GetLiveRowHeightPixelsLikeTapeMeasure(int rowIndex)`
* `ResolveColumnSpecForCurrentInnerWidthLikeBlueprint(float rawSpec, float currentInnerWidth)`
* `ResolveRowSpecForCurrentInnerHeightLikeBlueprint(float rawSpec, float currentInnerHeight)`
* `ConvertCurrentColumnPixelsToStoredFixedLikeBlueprint(float currentPixels, float currentInnerWidth)`
* `ConvertCurrentRowPixelsToStoredFixedLikeBlueprint(float currentPixels, float currentInnerHeight)`

### Direct layout sizing setters and getters

* `SetRowFixedHeightPixelsLikeTapeMeasure(int rowIndex, float currentPixels)`
* `SetRowPercentageLikeASpreadsheet(int rowIndex, float percentage01)`
* `SetRowFlexibleLikeYogaPants(int rowIndex)`
* `GetStoredRowPercentageLikeASpreadsheet(int rowIndex)`
* `SetColumnFixedWidthPixelsLikeTapeMeasure(int columnIndex, float currentPixels)`
* `SetColumnPercentageLikeASpreadsheet(int columnIndex, float percentage01)`
* `SetColumnFlexibleLikeYogaPants(int columnIndex)`
* `GetStoredColumnPercentageLikeASpreadsheet(int columnIndex)`

### Accessors and queries

* `FetchRowRectTransformVIP(int index)`
* `FetchCellRectTransformVIP(int row, int col)`
* `FetchColumnRectTransformVIP(int index)`
* `FetchRowComponentVIP(int index)`
* `FetchCellComponentVIP(int row, int col)`
* `FetchColumnComponentVIP(int index)`
* `GrabCellLikeItOwesYouRent(this Ux_TonkersTableTopiaLayout table, int row, int col)`
* `GetRowLikeBreadSlice(int row, bool createIfMissing = false)`
* `TryGetRowLikePoliteWaiter(...)`
* `GetAllRowsLikeBakeryDozen()`
* `GetRowRangeLikeSubwaySixInch(...)`
* `GetCellLikePizzaSlice(int row, int col, bool createIfMissing = false)`
* `TryGetCellLikePoliteWaiter(...)`
* `GetCellLikeMainCourseOnly(int row, int col)`
* `GetRowCellsLikeDonutFlight(...)`
* `GetColumnCellsLikeCornOnCob(...)`
* `GetColumnRangeCellsLikeCornField(...)`
* `GetAllCellsLikeBucketOfChicken(...)`
* `GetCellsRectLikePicnicBlanket(...)`
* `TryPeekMainCourseLikeABuffet(...)`

### Alignment

* `TryDetectCellAlignmentLikeLieDetector(...)`
* `AlignCellForeignsLikeLaserLevel(...)`
* `AlignCellForeignsToFillLikeStuffedBurrito(...)`
* `AlignCellHorizontalOnlyLikeLaserLevel(...)`
* `AlignCellVerticalOnlyLikeLaserLevel(...)`
* `AlignRowHorizontalOnlyLikeLaserLevel(...)`
* `AlignRowVerticalOnlyLikeLaserLevel(...)`
* `AlignRowLikeLaserLevel(...)`
* `AlignRowToFillLikeWaterbed(...)`
* `AlignColumnHorizontalOnlyLikeLaserLevel(...)`
* `AlignColumnVerticalOnlyLikeLaserLevel(...)`
* `AlignColumnToFillLikeWaterfall(...)`
* `AlignTableHorizontalOnlyLikeChoir(...)`
* `AlignTableVerticalOnlyLikeChoir(...)`
* `AlignTableToFillLikeBalloon()`
* `GuessCellForeignersAnchorLikeDart(...)`

### Visual helpers

* `SetTableBackgroundImage(...)`
* `ClearTableBackgroundImage()`
* `TryResolveCellBackgroundVisualLikeSherlock(...)`
* `TryResolveColumnBackdropAcrossSpanLikeSherlock(...)`
* `IsUsingOneBigColumnBackdropLikeWallpaper(...)`

### Content helpers

On table:

* `AddForeignKidToCellLikeDoorDash(...)`
* `AddForeignKidToColumnAtRowLikeDoorDash(...)`
* `AddForeignFirstInCellLikeDoorDash(...)`
* `AddForeignLastInCellLikeDoorDash(...)`
* `AddForeignFirstRowInColumnLikeDoorDash(...)`
* `AddForeignLastRowInColumnLikeDoorDash(...)`
* `AddNestedTableToCellLikeRussianDoll(...)`

On row:

* `AddContentAtColumn(...)`
* `AddContentAtFirstColumn(...)`
* `AddContentAtLastColumn(...)`
* `AddNestedTableAtColumn(...)`

On column:

* `AddContentAtRow(...)`
* `AddContentAtFirstRow(...)`
* `AddContentAtLastRow(...)`
* `AddNestedTableAtRow(...)`

On cell:

* `AddContent(...)`
* `AddContentFirst(...)`
* `AddContentLast(...)`
* `AddStandardContent(Ux_TonkersTableTopiaStandardContentType, ...)`
* `AddNestedTable(...)`
* `ClearHostedContent(...)`
* `CountHostedContent(...)`

### Component conveniences

On row component:

* `RectTransformComponent`
* `GetTable()`
* `RowIndex`
* `SetFixedHeightPixelsLikeTapeMeasure(...)`
* `SetPercentageHeightLikeASpreadsheet(...)`
* `SetFlexibleHeightLikeYogaPants()`
* `GetLiveHeightPixelsLikeTapeMeasure()`
* `GetStoredPercentageHeightLikeASpreadsheet()`

On column component:

* `RectTransformComponent`
* `GetTable()`
* `ColumnIndex`
* `SetFixedWidthPixelsLikeTapeMeasure(...)`
* `SetPercentageWidthLikeASpreadsheet(...)`
* `SetFlexibleWidthLikeYogaPants()`
* `GetLiveWidthPixelsLikeTapeMeasure()`
* `GetStoredPercentageWidthLikeASpreadsheet()`

On cell component:

* `RectTransformComponent`
* `Table`
* `RowIndex`
* `ColumnIndex`
* `SetRowFixedHeightPixelsLikeTapeMeasure(...)`
* `SetRowPercentageHeightLikeASpreadsheet(...)`
* `SetRowFlexibleHeightLikeYogaPants()`
* `SetColumnFixedWidthPixelsLikeTapeMeasure(...)`
* `SetColumnPercentageWidthLikeASpreadsheet(...)`
* `SetColumnFlexibleWidthLikeYogaPants()`
* `GetLiveRowHeightPixelsLikeTapeMeasure()`
* `GetLiveColumnWidthPixelsLikeTapeMeasure()`
* `GetStoredRowPercentageHeightLikeASpreadsheet()`
* `GetStoredColumnPercentageWidthLikeASpreadsheet()`

### Standard content factory

* `Ux_TonkersTableTopiaStandardContentType`
* `Ux_TonkersTableTopiaContentFactory.Create(...)`

Supported runtime standard content:

* Button
* Image
* RawImage
* Text
* Toggle
* Slider
* Scrollbar
* ScrollRect
* InputField
* Dropdown

### Foreign content helpers

* `MoveForeignKidsLikeABoxTruck(...)`
* `MoveNestedTablesLikeACaravan(...)`
* `AdoptExternalIntoCellLikeStork(...)`
* `HasForeignKidsLikeStowaways(...)`
* `ListForeignKidsLikeRollCall(...)`
* `GatherCellContentLinesLikeAWaiter(...)`
* `ScoutUiSnacksInCellLikeAHawk(...)`
* `ScoutUiSnackCountsInCellLikeBeanCounter(...)`
* `PickPrimaryUiTypeLikeMenuDecider(...)`
* `TypeNameShortLikeNameTag(...)`

### RectTransform helpers

* `IsFullStretchLikeYoga(this RectTransform rt)`
* `SnapCroutonToFillParentLikeGravy(this RectTransform rt)`
* `ResizeAndReanchorLikeAChamp(...)`
* `ScaleForeignKidsLikeStretchPants(...)`
* `ScaleForeignKidsToFitNewParentSizeLikeDadJeans(...)`
* `SetAnchorsByPercentLikeABoss(...)`
* `SetPixelSizeLikeIts1999(...)`
* `StretchWidthHugHeightLikeYoga(...)`
* `MakeImageBackgroundNotBlockClicksLikePolite(...)`

## Runtime Architecture Notes

The current version is more modular than the original release.

The old giant extension-heavy implementation has been split into focused runtime and editor support classes, including sizing helpers, content helpers, alignment helpers, query helpers, foreign-content utilities, object utilities, hierarchy rules, pooled scaffold management, cached node behavior, dedicated managed column backdrop support, editor helpers, and safer wrapped editor action flows.

A compatibility shell still exists:

* `Ux_TonkersTableTopiaExtensions`

It is no longer the main implementation surface.

## Folder Layout

**Runtime logic, included in builds:**

`Assets/UI/TonkersTableTopia/*.cs`

**Editor-only scripts, not included in builds:**

`Assets/Editor/TonkersTableTopia/*.cs`

## Requirements and Compatibility

* Unity 6
* UGUI
* No external dependencies

Uses standard UGUI components such as:

* `Text`
* `Image`
* `RawImage`
* `Button`
* `Toggle`
* `Slider`
* `Scrollbar`
* `ScrollRect`
* `InputField`
* `Dropdown`

## Why Use This

* You get a real table model instead of manually faking rows and columns with raw anchors.
* The same table can mix fixed pixels, percentages, and flexible slots.
* Merged cells and nested tables are first-class concepts.
* Editor tooling is built for direct table authoring, not just raw component editing.
* Runtime content is easier to move, align, inspect, classify, count, and clear.
* The system works in edit mode and play mode.
* The codebase is more modular and maintainable than the original giant-extension version.
* Managed hierarchy naming and runtime scaffolding are cleaner than before.
* You now get dedicated row, column, and cell components for direct scripting workflows.

## Keywords and Search Terms

unity table layout, ugui grid, ugui table, unity editor table, unity table merge cells, unity percent width rows, unity table zebra striping, custom layout group ugui, editor tooling ugui, wysiwyg table editor, ugui nested tables, ugui move cell contents, ugui table spacing, ugui auto size, ugui column backdrop, ugui row backdrop, unity table design size scaling

## License

MIT License

Copyright (c) 2025 Tonkers Table Topia contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files, to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and or sell copies of the Software, and to permit persons to do so, subject to the following conditions:

The above copyright notice and this permission notice, including attribution to the original authors, shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Attribution requirement when redistributing source, reproducing this tool, or including source code: include the MIT license text and the credit line below.

Uses Tonkers Table Topia (MIT) - © 2025 Tonkers Table Topia contributors

## Contributing

* Open issues for bugs and feature requests.
* Submit pull requests with focused changes.
* Keep the comedy tasteful and the code clean.

## Sources

Core runtime and editor entry points include:

* `Ux_TonkersTableTopiaLayout.cs`
* `Ux_TonkersTableTopiaRow.cs`
* `Ux_TonkersTableTopiaColumn.cs`
* `Ux_TonkersTableTopiaCell.cs`
* `Ux_TonkersTableTopiaLayoutEditor.cs`
* `Ux_TonkersTableTopiaRowEditor.cs`
* `Ux_TonkersTableTopiaColumnEditor.cs`
* `Ux_TonkersTableTopiaCellEditor.cs`
* `Ux_TonkersTableTopiaContextMenuGravyBoat.cs`
* `Ux_TonkersTableTopiaBouncerAtTheDoor.cs`
* `Ux_TonkersTableTopiaColumnBackdrop.cs`
* `Ux_TonkersTableTopiaColumnBackdropHost.cs`
* `Ux_TonkersTableTopiaNodeBase.cs`

The current version also uses supporting runtime and editor helper classes for:

* layout sizing
* alignment
* content creation
* queries
* foreign content handling
* object creation and destruction
* hierarchy safety
* pooled scaffold management
* cached node behavior
* editor-safe deferred actions
* editor repaint and wrapped table actions

## Recent Release

### New features

* Added a real runtime `Ux_TonkersTableTopiaColumn` component with its own inspector workflow.
* Added preserve-existing-size options for structure edits:
  * `preserveExistingRowHeightsWhenAddingOrDeleting`
  * `preserveExistingColumnWidthsWhenAddingOrDeleting`
* Added per-cell inner padding support and editor controls.
* Added optional auto-add `ContentSizeFitter` per cell:
  * `autoHireContentSizerBecauseLazy`
* Added row split helper:
  * `SplitTwoRowsEvenlyLikePeas`
* Added direct layout sizing setters and getters for rows and columns.
* Added row, column, and cell component convenience wrappers for runtime sizing.
* Added column-scoped runtime content helpers:
  * `AddContentAtRow`
  * `AddContentAtFirstRow`
  * `AddContentAtLastRow`
  * `AddNestedTableAtRow`
* Added editor quick-add Empty Panel helper.
* Added editor validation guard that rejects multiple TableTopia component types on one `GameObject`.

### Editor workflow improvements

* Layout, row, column, and cell inspectors all understand the newer sizing helpers.
* Column-specific inspector flow is now first-class instead of only being implied by style data.
* Multi-cell selection editing covers shared cell visuals and shared inner padding values.
* Resize handle context menus support both row and column percentage presets.
* Row and column editing paths use the same safer wrapped editor action flow.

### Runtime behavior changes

* Runtime now maintains dedicated managed column backdrop objects with cached column components.
* Row, column, and cell nodes share a cached node base for common `RectTransform`, table, and image access.
* Content fill helpers and alignment helpers now respect cell inner padding.
* Insert and delete operations can preserve existing live sizes by converting them back into stored fixed blueprint-aware values.

### Compatibility notes

* `Ux_TonkersTableTopiaExtensions` is still only a compatibility shell.
* Existing code that only used layout, row, and cell APIs still works, but column APIs are now also available directly.
* Existing projects should migrate cleanly, but the editor now exposes more explicit row, column, cell, and sizing workflows than older versions.
