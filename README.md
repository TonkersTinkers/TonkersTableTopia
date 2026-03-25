# Tonkers Table Topia (Unity UGUI)

Simple UGUI tables that behave like, stretchable layout tables.

* Created for designing Quick, Fast, Stretchable Layouts in seconds, that adapt to any resolution with minimum input from the designer.

* Use the Nested Tables feature often, to easily get the look you desire.

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

This is still plain UGUI. No external package is required.

## Key Features and Benefits

### WYSIWYG editor workflow

* WYSIWYG preview with row and column headers.
* Click to select a cell.
* Shift-click and drag to extend the selection rectangle.
* Row header and column header selection.
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

### Sizing and layout

* Mixed row and column sizing:

  * positive values = fixed pixels
  * negative values = percent of inner available size
  * zero = flexible share of remaining space
* Blueprint-style scaling for positive fixed row and column values.
* Per-table design size capture and reuse.
* One-click capture of the current RectTransform size as the table design size.
* Inspector editing of fixed values using live resolved pixels.
* Inspector editing of percent values using live computed percentages.
* Drag resize handles in the WYSIWYG preview.
* Live resize HUD showing starting value, new value, and percent delta for the two affected rows or columns.
* Distribute all rows evenly.
* Distribute all columns evenly.
* Split two adjacent columns evenly from a handle context menu.
* Make last row flexible.
* Make last column flexible.
* Table container size controls:

  * size by anchored percent
  * size by fixed pixels
* Automatic conversion helpers when switching sizing workflows.
* Row and column spacing controls.
* Table padding support.
* Optional auto hug width.
* Optional auto hug height.
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

### Content helpers and UI spawning

* Drag external hierarchy objects into table cells from the editor.
* Add common UGUI controls to cells from context menus.
* Add arbitrary prefab content to a cell, row, column, or table through runtime helpers.
* Supported quick content helpers include:

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
* Cached row and cell component access.
* Cached background image access on managed nodes.
* Centralized hierarchy and object utility helpers.
* Centralized foreign content movement and restore helpers.
* Centralized editor-safe deferred actions.
* Better undo coverage for structure and style edits in the editor.
* Cleaner managed hierarchy naming than the original version.

## Installation

Copy the runtime scripts into your project, for example:

`Assets/UI/TonkersTableTopia/`

Copy the editor scripts into your project, for example:

`Assets/Editor/TonkersTableTopia/`

The package includes the main layout, row, and cell components plus supporting runtime extensions, utilities, content helpers, sizing helpers, alignment helpers, editor extensions, inspectors, and context menu tooling.

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
7. Style the table using row, column, cell, and table background settings.
8. Enable zebra stripes, spacing, and padding where needed.
9. Use Move mode to move foreign content or nested tables between cells.
10. Drag hierarchy objects directly into cells in the preview.
11. Add nested child tables to cells where needed.
12. Use runtime helpers to add content, inspect cells, merge ranges, or rebuild layout during play.

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
* move operations and some editor operations now start from the merged main cell instead of an arbitrary covered cell

### 10. Nested tables

Any cell can host another `Ux_TonkersTableTopiaLayout`.

Nested tables are treated as first-class content and can be created, selected, deleted, moved, and navigated through the editor.

### 11. Foreign content

The system distinguishes between:

* managed scaffold objects created by the table
* foreign content, meaning your real UI objects

Foreign content is preserved and reparented carefully during layout rebuilds, merges, deletes, moves, and hierarchy adoption.

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

### Accessors and queries

* `FetchRowRectTransformVIP(int index)`
* `FetchCellRectTransformVIP(int row, int col)`
* `FetchRowComponentVIP(int index)`
* `FetchCellComponentVIP(int row, int col)`
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

On row:

* `AddContentAtColumn(...)`
* `AddContentAtFirstColumn(...)`
* `AddContentAtLastColumn(...)`
* `AddNestedTableAtColumn(...)`

On cell:

* `AddContent(...)`
* `AddContentFirst(...)`
* `AddContentLast(...)`
* `AddStandardContent(Ux_TonkersTableTopiaStandardContentType, ...)`
* `AddNestedTable(...)`
* `ClearHostedContent(...)`
* `CountHostedContent(...)`

### Standard content factory

* `Ux_TonkersTableTopiaStandardContentType`
* `Ux_TonkersTableTopiaContentFactory.Create(...)`

Supported standard content:

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

The old giant extension-heavy implementation has been split into focused runtime and editor support classes, including sizing helpers, content helpers, alignment helpers, query helpers, foreign-content utilities, object utilities, hierarchy rules, pooled scaffold management, cached node behavior, editor helpers, and safer wrapped editor action flows.

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
* `Ux_TonkersTableTopiaCell.cs`
* `Ux_TonkersTableTopiaLayoutEditor.cs`
* `Ux_TonkersTableTopiaRowEditor.cs`
* `Ux_TonkersTableTopiaCellEditor.cs`
* `Ux_TonkersTableTopiaContextMenuGravyBoat.cs`
* `Ux_TonkersTableTopiaBouncerAtTheDoor.cs`

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

* Added blueprint-style scaling for positive fixed row and column sizes:

  * `scaleFixedSizesWithResolutionLikeBlueprint`
  * `designSizeForThisTableLikeBlueprint`
* Added table-local design size capture:

  * `CaptureCurrentRectAsDesignSizeLikeBlueprint()`
* Added live size conversion helpers so editor pixel fields can work correctly even when stored values are blueprint-scaled fixed specs.
* Added true runtime auto-hug width support.
* Added full-column backdrop rendering support with dedicated column-wide art instead of only per-cell repeats.
* Added column backdrop mode control:

  * per-cell style across spans
  * one stretched backdrop for the whole column
* Added sliced or simple rendering control for:

  * table background
  * row backdrops
  * column backdrops
  * cell backgrounds
* Added row-level leftover fill support:

  * `RowStyle.lastVisibleCellEatsLeftovers`
* Added live column and row size query helpers:

  * `GetLiveColumnWidthPixelsLikeTapeMeasure`
  * `GetLiveRowHeightPixelsLikeTapeMeasure`
* Added percentage rebalance setters:

  * `SetColumnPercentageAndRebalanceOthersLikeASpreadsheet`
  * `SetRowPercentageAndRebalanceOthersLikeASpreadsheet`
* Added a standard content enum and content factory for runtime UI creation.
* Added higher-level row and cell content APIs:

  * `AddContent`
  * `AddContentAtColumn`
  * `AddStandardContent`
  * `AddNestedTable`
  * `ClearHostedContent`
  * `CountHostedContent`
* Added cached row and cell component accessors:

  * `FetchRowComponentVIP`
  * `FetchCellComponentVIP`
* Added reusable cached node behavior for row and cell components.
* Added compact managed naming for rows, cells, nested tables, and column backdrop objects.
* Added a unified editor design-size header with:

  * design size display
  * fixed-size blueprint scaling toggle
  * use current rect capture button
* Added a new WYSIWYG zoom workflow:

  * scrollable preview
  * zoom slider
  * Fit button
  * 1:1 button
  * Ctrl or Cmd + mouse wheel zoom
* Added preview badge toggle for editor-wide WYSIWYG display.
* Added drag rectangle selection in Highlight mode.
* Added preview rendering of real cell, row, and column background art in the WYSIWYG panel.
* Added mode-specific editor workbench panels for:

  * structure
  * merge
  * resize utilities
  * selection
  * move workflow
  * alignment
* Added persistent preview and scene highlight toggles through `EditorPrefs`.
* Added safer editor action wrappers through centralized editor extension helpers.

### Editor workflow improvements

* Reworked the layout inspector into a clearer mode-driven workflow instead of keeping everything in one long flat block.
* Replaced the old preview presentation with a proper scrollable zoomable canvas.
* Added clearer mode banners and mode-specific helper text.
* Added better separation between selection tools, structure tools, merge tools, resize tools, and move tools.
* Added persistent Preview and Scene toggle buttons in the toolbar itself.
* Added direct column and row style editing that understands blueprint-scaled fixed values.
* Replaced the old global “Fixed Widths” and “Fixed Heights” style of editing with per-style “Use Fixed Width” and “Use Fixed Height” controls.
* Column inspectors now support:

  * sliced background mode
  * one stretched column backdrop mode
* Row inspectors now support:

  * sliced background mode
  * last visible cell leftover fill toggle
* Cell selection inspector now supports multi-cell background editing with sliced mode.
* Turning off image sections now clears related visuals instead of only hiding UI.
* The WYSIWYG preview now shows resolved backdrops instead of only generic row banding.
* Badge collection in selections now works against distinct main cells instead of repeatedly listing mashed merged children.
* Move mode now resolves merged selections back to their main source cell before moving content.
* Header and context-menu actions now consistently go through safer wrapped editor table actions.
* Editor deferred actions and repaint requests are centralized instead of duplicated ad hoc in multiple places.

### Runtime behavior changes

* The runtime is now split into focused extension and utility classes instead of keeping nearly everything in one giant runtime extension class.
* Play mode layout refreshes are now queued and flushed from `Canvas.willRenderCanvases`.
* Dirty state is tracked more cleanly for structure and layout changes.
* Row and cell component caches are maintained during rebuilds.
* Managed scaffold pooling is wrapped in dedicated pool logic.
* Object creation, parenting, destruction, prefab instantiation, and undo-aware component flow are routed through centralized utilities.
* Foreign content movement now uses stored `RectTransform` snapshots and restore logic instead of repeated copy-paste reparent code.
* Background image components are reused and disabled more often instead of being constantly destroyed and recreated.
* Horizontal extra-space alignment is part of the actual runtime placement pass.
* Row leftover fill is now part of the actual runtime placement pass.
* Fixed pixel specs can now behave like authored blueprint values instead of being treated as permanently absolute pixels.

### Context menu and inspector changes

* Column header distribute actions now use the real distribution helpers instead of manually forcing raw negative percentages.
* Row header distribute actions now use the real distribution helpers instead of manually forcing raw negative percentages.
* Table header distribute actions now use the real distribution helpers instead of manually forcing raw negative percentages.
* Resize-handle percent presets now use the dedicated rebalance APIs instead of custom local scaling math.
* Cell alignment context actions now use horizontal-only and vertical-only alignment calls where appropriate.
* Row, column, and table alignment context actions are now routed through the wrapped editor action helper.
* Column and row deletion from context menus now go through more consistent editor action wrappers.
* Deferred return-to-table operations in row and cell inspectors now use centralized editor-safe helpers and force repaint more reliably afterward.

### Bug fixes

* Fixed `TryKindlyDeleteCell`. It now clears the targeted cell instead of incorrectly behaving like a whole-column delete path.
* Fixed `autoHugWidthLikeAGoodFriend` so width hugging actually participates in runtime layout.
* Fixed horizontal extra-space placement so `horizontalSchmoozingPreference` now affects actual runtime placement.
* Fixed row leftover fill so `lastVisibleCellEatsLeftovers` is now used during layout.
* Fixed several repeated `GetComponent`-heavy runtime paths by caching row and cell components.
* Fixed duplicated reparenting logic by centralizing layout-preserving reparent helpers.
* Fixed content movement paths to better preserve anchors, offsets, and sibling order.
* Fixed row and column swap cache consistency by updating cached rect and component references together.
* Fixed merged-cell content sweeping so it uses cached structure and cached access more consistently.
* Fixed layout placement for mixed stretch and non-stretch anchor situations.
* Fixed full-column art handling so real dedicated column-wide backdrops can exist instead of pretending everything is cell-local art.
* Fixed background resolution flow so cell visuals can intentionally yield to column-wide backdrop mode.
* Fixed editor validation reparent safety by deferring cleanup through centralized helpers.
* Fixed image background churn by reusing and disabling image components instead of destroying and recreating them constantly.
* Fixed WYSIWYG selection gathering across merged cells so combined inspectors work on distinct main cells.
* Fixed move-source resolution for merged cells so moving content starts from the correct main seat.
* Fixed resize math so dragging can preserve fixed, percent, and flexible semantics instead of flattening everything into one naive interpretation.
* Fixed resize HUD reporting so it reflects percent change derived from actual resolved specs more accurately.
* Fixed row and column style inspectors so displayed fixed pixel values reflect live resolved values under blueprint scaling.
* Fixed preview hit-testing so merged top-left cells can consume the correct preview space when selection and dragging occur.
* Fixed preview gap math so zoomed row and column spacing stays visually consistent.
* Fixed table background editing so sliced mode can be edited directly from the inspector.
* Fixed cell multi-edit background workflow so turning the image section off can actually clear cell sprites.

### API additions and modernization

* Added modern row APIs:

  * `RectTransformComponent`
  * `GetTable()`
  * `RowIndex`
  * `AddContentAtColumn`
  * `AddContentAtFirstColumn`
  * `AddContentAtLastColumn`
  * `AddNestedTableAtColumn`
* Added modern cell APIs:

  * `RectTransformComponent`
  * `Table`
  * `RowIndex`
  * `ColumnIndex`
  * `AddContent`
  * `AddContentFirst`
  * `AddContentLast`
  * `AddStandardContent`
  * `AddNestedTable`
  * `ClearHostedContent`
  * `CountHostedContent`
* Added `Ux_TonkersTableTopiaStandardContentType` for clearer runtime content spawning.
* Added list-filling overloads for several query helpers to reduce avoidable allocations.
* Added dedicated support helpers for:

  * sizing
  * content
  * foreign content
  * alignment
  * object utilities
  * hierarchy rules
  * editor-safe actions
* Kept older convenience methods as obsolete wrappers where practical so older calls still have a migration path.

### Performance and maintenance improvements

* Replaced the old monolithic runtime extension implementation with smaller focused modules.
* Reduced runtime allocations with reusable scratch buffers and list-based query overloads.
* Reduced rebuild spam by batching play mode updates through a canvas render hook.
* Reduced repeated hierarchy scanning by using cached structure and explicit dirty flags.
* Reduced repeated image lookup and creation through cached node visuals.
* Consolidated distribution math into dedicated distribution utilities.
* Consolidated alignment logic into dedicated alignment utilities.
* Consolidated foreign content handling into dedicated foreign-content utilities.
* Consolidated rect transform operations into dedicated rect transform utilities.
* Consolidated editor deferred actions, repaint requests, and wrapped editor table actions into editor utility helpers.
* Improved maintainability of inspectors and context menus by replacing repeated custom math with centralized APIs.

### Compatibility notes

* `Ux_TonkersTableTopiaExtensions` is no longer the main implementation surface. It remains as a compatibility shell.
* Several older row and cell helper names now act as wrappers around newer content APIs.
* Runtime scaffold object names are shorter and more machine-like than before.
* Full-column backdrops now create dedicated managed backdrop objects.
* The default reset preset is different from the older minimal setup.
* Positive fixed row and column specs can now scale relative to the table’s saved design size when blueprint scaling is enabled.
* The editor now assumes the newer wrapped action flow and sizing conversion helpers are available.
* Existing projects using the older table system should still migrate cleanly, but editor visuals and default behavior will feel more modern and more explicit than before.
