# Tonkers Table Topia (Unity UGUI)

Finally: simple Layout tables in Unity without suffering.

I got tired of never being able to make basic tables in UGUI. Years. Of. Wrestling. I just wanted rows, columns, merge cells, zebra stripes, and pixels or percents that behave. So I built this. Now you can click, merge, split, resize, and laugh maniacally as your UGUI tables behave like a polite spreadsheet. No more "everyone fend for yourselves", this is a sane, WYSIWYG table layout management system.

Quick Video: Shows table editor in action

* [https://youtu.be/Ax740NRROhc](https://youtu.be/Ax740NRROhc)

The game for which this editor script was made and used in.

* [https://store.steampowered.com/app/3844910/](https://store.steampowered.com/app/3844910/)

## Key Features and Benefits

* WYSIWYG Editor preview with row and column headers.
* Click to select cells, Shift to select rectangles.
* Merge and unmerge cells.
* Insert and delete rows or columns with safety prompts; bulk delete across selections with checks and smart relocation of non table children.
* Drag splitters to resize rows or columns in the preview.
* Live resize HUD showing starting size, new size, and percent deltas for the two affected rows or columns.
* Even or custom sizing, in pixels or normalized percent.
* Distribute rows or columns evenly; one click "Make Last Row or Column Flexible."
* Row and column spacing controls for gaps.
* Zebra striping by rows and by columns.
* Per row and per column background sprites and tints; optional per cell overrides.
* Per row and per column custom anchors and pivot overrides.
* Optional ContentSizeFitter auto sizing on cells.
* Alignment tools: Left, Center, Right; Top, Middle, Bottom; or Full stretch, applied to cells, selections, rows, columns, or full table.
* Move mode: drag contents or nested tables from one cell to another.
* Drag external objects from heirarchy direclty into table cells.
* Nested tables: create a child table inside any cell, select or delete it, double click a cell with a nested table to jump into it, "Up to Parent Tonkers Table" button in the inspector.
* Header interactions: click a column or row header to select it, right click headers or split handles for context menus including even split helpers.
* Cell badges in the preview indicate what common UI components are inside each cell and whether a nested table exists.
* Table size controls: choose Size By Percent anchors or fixed pixels; automatic conversion to percentages and anchor adjustment.
* Editor wide WYSIWYG preview color preferences for headers and alternating row bands saved in EditorPrefs.
* Scene highlight: outlines selected cells in the Scene view.
* Runtime safe layout updates in edit and play mode.
* Auto hug height option to fit table to its content when not stretched vertically.
* Scale aware child handling, non table children in rows and cells keep sensible size and alignment when the table or canvas scale changes.
* Play mode pooling of rows and cells to reduce allocations.
* Small, readable C# with seasoning.
* MIT license, attribution required when redistributing source.

## Installation

1. Copy the runtime classes into your project: `Assets/UI/TonkersTableTopia/`

   * `Ux_TonkersTableTopiaLayout.cs`
   * `Ux_TonkersTableTopiaRow.cs`
   * `Ux_TonkersTableTopiaCell.cs`
   * `Ux_TonkersTableTopiaExtensions.cs`

2. Copy the Editor scripts into: `Assets/Editor/TonkersTableTopia/`

   * `Ux_TonkersTableTopiaLayoutEditor.cs`
   * `Ux_TonkersTableTopiaRowEditor.cs`
   * `Ux_TonkersTableTopiaCellEditor.cs`
   * `Ux_TonkersTableTopiaContextMenuGravyBoat.cs`
   * `Ux_TonkersTableTopiaBouncerAtTheDoor.cs`

3. Let Unity compile. Add **Tonkers Table Topia** to any GameObject under a Canvas:

   * Add Component, Layout, Tonkers Table Topia

## Usage

1. Add `Ux_TonkersTableTopiaLayout` to a `RectTransform`.
2. Set row and column counts.
3. Click cells in the preview to select. Shift click to select a rectangle. Use Merge, Unmerge, Insert, Delete, and Distribute.
4. Toggle Resize Mode to drag splitters and resize rows or columns directly; watch the resize HUD for live percent deltas.
5. Use zebra options and background sprites to style cells, rows, and columns.
6. Use right click context menus on cells, headers, and splitters for quick actions, including adding common UI children and even split helpers.
7. Use Move Mode to drag contents or a nested table from a source cell to a destination cell.
8. Use the Size By Percent toggle to switch the table container between percent anchored and fixed pixels; anchors update automatically.

### Example, runtime

**Concepts**

**Sizing per column and per row:**

* Positive value means fixed pixels.
* Negative value means percent of available inner space, for example `-0.25f` is 25 percent.
* Zero means flexible share of the remaining space.
* Percent values are normalized; the editor can convert current pixel layout to percents for consistent behavior when the parent changes size.

**Row and column spacing:**

* Set `sociallyDistancedRowsPixels` and `sociallyDistancedColumnsPixels` for gaps between rows and columns. Gaps are excluded from percent calculations.

**Alternating color stripes:**

* Enable per row, per column, or both. When both are on, colors blend.

**ContentSizeFitter:**

* Optional on each cell, helpful when children use Layout Elements or preferred size text.

**Nested tables:**

* Add a child table inside any cell; double click a cell with a nested table to open it; use the "Up to Parent Tonkers Table" button to navigate back.

**Auto hug height:**

* When enabled and the table is not vertically stretched, the table height fits the total content height.

**Editor, WYSIWYG preview**

**Toggles:**

* WYSIWYG Preview, Scene Highlight, Size By Percent.

**Modes:**

* Highlight cells, Resize, Select objects, Move.

**Zoom:**

* Horizontal zoom; vertical zoom grab handle at the bottom of the preview.

**Selection:**

* Click selects a cell; Shift extends to a rectangle; click headers to select entire row or column.

**Tools:**

* Insert Row Above or Below, Insert Column Left or Right.
* Delete Row or Column, or bulk delete over a selection, with confirmations and smart relocation of foreign children.
* Merge and Unmerge.
* Distribute Rows and Columns evenly.
* Make Last Row Flexible, Make Last Column Flexible.
* Split two adjacent rows or columns evenly from splitter context menus.

**Coloring:**

* Alternating row and column colors.
* Editor wide preview colors saved in EditorPrefs.

**Scene Highlight:**

* Outlines selected cells in the Scene view.

**Badges:**

* Small icons in each cell preview show common UI elements present and whether a nested table exists.

**Context Menus:**

* Right click cells, headers, or splitters for quick actions, percent presets, even split helpers, and Add UI primitives.

## Public API, selected

**Layout construction and sync:**

* `RebuildComedyClubSeatingChart()`
* `FlagLayoutAsNeedingSpaDay()`
* `ConvertAllSpecsToPercentages()`
* `SyncColumnWardrobes()`, `SyncRowWardrobes()`
* `UpdateSeatingLikeAProUsher()`
* `NormalizeWardrobePercentsToOneDadBod()`

**Structure editing:**

* `InsertRowLikeANinja(int index)`, `InsertColumnLikeANinja(int index)`
* `TryPolitelyDeleteRow(int index)`, `TryPolitelyDeleteColumn(int index)`, `TryKindlyDeleteCell(int row, int col)`
* `SafeDeleteRowAtWithWittyConfirm(int index)`, `SafeDeleteColumnAtWithWittyConfirm(int index)`
* `BulkDeleteRowsLikeABoss(int startRowInclusive, int endRowInclusive)`
* `BulkDeleteColumnsLikeAChamp(int startColInclusive, int endColInclusive)`
* `SwapRowsLikeMusicalChairs(int aIndex, int bIndex)`
* `SwapColumnsLikeTradingCards(int aIndex, int bIndex)`
* `MergeCellsLikeAGroupHug(int startRow, int startCol, int rowCount, int colCount)`
* `UnmergeCellEveryoneGetsTheirOwnChair(int row, int col)`
* `UnmergeEverythingInRectLikeItNeverHappened(int startRow, int startCol, int rowCount, int colCount)`
* `CreateChildTableInCellLikeABaby(int row, int col)`
* `TryFindChildTableInCellLikeSherlock(int row, int col, out Ux_TonkersTableTopiaLayout child)`
* `MoveForeignKidsLikeABoxTruck(RectTransform from, RectTransform to)`
* `MoveNestedTablesLikeACaravan(RectTransform from, RectTransform to)`

**Sizing helpers and math:**

* `DistributeAllColumnsEvenlyLikeAShortOrderCook()`
* `DistributeAllRowsEvenlyLikeAShortOrderCook()`
* `SplitTwoColumnsEvenlyLikePeas(int leftIndex)`
* `SplitTwoRowsEvenlyLikePeas(int topIndex)`
* `ComputeColumnPercentagesLikeASpreadsheet()`
* `ComputeRowPercentagesLikeASpreadsheet()`
* `SetAnchorsByPercentLikeABoss(RectTransform rt, float widthPercent, float heightPercent)`
* `SetPixelSizeLikeIts1999(RectTransform rt, float width, float height)`

**Alignment:**

* Cell level: `AlignCellHorizontalOnlyLikeLaserLevel(int row, int col, HorizontalAlignment h)`, `AlignCellVerticalOnlyLikeLaserLevel(int row, int col, VerticalAlignment v)`, `AlignCellForeignsLikeLaserLevel(int row, int col, HorizontalAlignment h, VerticalAlignment v)`, `AlignCellForeignsToFillLikeStuffedBurrito(int row, int col)`
* Row level: `AlignRowHorizontalOnlyLikeLaserLevel(int row, HorizontalAlignment h)`, `AlignRowVerticalOnlyLikeLaserLevel(int row, VerticalAlignment v)`, `AlignRowToFillLikeWaterbed(int row)`
* Column level: `AlignColumnHorizontalOnlyLikeLaserLevel(int col, HorizontalAlignment h)`, `AlignColumnVerticalOnlyLikeLaserLevel(int col, VerticalAlignment v)`, `AlignColumnToFillLikeWaterfall(int col)`
* Table level: `AlignTableHorizontalOnlyLikeChoir(HorizontalAlignment h)`, `AlignTableVerticalOnlyLikeChoir(VerticalAlignment v)`, `AlignTableToFillLikeBalloon()`
* Queries for selections, rows, columns, table:

  * `TryDetectCellAlignmentLikeLieDetector(int row, int col, out bool isFullLikeBurrito, out HorizontalAlignment h, out VerticalAlignment v)`
  * `IsSelectionHorizAlignedLikeDejaVu(...)`, `IsSelectionVertAlignedLikeDejaVu(...)`, `IsSelectionFullLikeBurritoWrap(...)`, `IsSelectionAlreadyAlignedLikeDejaVu(...)`
  * `IsRowHorizAlignedLikeMirror(int row, HorizontalAlignment h)`, `IsRowVertAlignedLikeMirror(int row, VerticalAlignment v)`, `IsRowFullLikeWaterbed(int row)`
  * `IsColumnHorizAlignedLikeMirror(int col, HorizontalAlignment h)`, `IsColumnVertAlignedLikeMirror(int col, VerticalAlignment v)`, `IsColumnFullLikeWaterfall(int col)`
  * `IsTableHorizAlignedLikeChoir(HorizontalAlignment h)`, `IsTableVertAlignedLikeChoir(VerticalAlignment v)`, `IsTableFullLikeBalloon()`

**Accessors and queries:**

* `FetchCellRectTransformVIP(int row, int col)`
* `FetchRowRectTransformVIP(int index)`
* `GrabCellLikeItOwesYouRent(this Ux_TonkersTableTopiaLayout table, int row, int col)` // extension
* `GetCellLikePizzaSlice(int row, int col, bool createIfMissing = false)`
* `GetCellLikeMainCourseOnly(int row, int col)`
* `GetRowLikeBreadSlice(int row, bool createIfMissing = false)`, `TryGetRowLikePoliteWaiter(...)`, `GetAllRowsLikeBakeryDozen()`, `GetRowRangeLikeSubwaySixInch(...)`
* `GetRowCellsLikeDonutFlight(int row, bool distinctMainsOnly = true)`
* `GetColumnCellsLikeCornOnCob(int col, bool distinctMainsOnly = true)`, `GetColumnRangeCellsLikeCornField(...)`
* `GetAllCellsLikeBucketOfChicken(bool distinctMainsOnly = true)`
* `GetCellsRectLikePicnicBlanket(int startRow, int startCol, int rowCount, int colCount, bool expandToWholeMergers = true, bool distinctMainsOnly = true)`
* `FindParentTableLikeFamilyTree()`

**Container and foreign children helpers:**

* `ScaleForeignKidsToFitNewParentSizeLikeDadJeans(RectTransform parent, Vector2 oldParentSize, Vector2 newParentSize)`
* `ResizeAndReanchorLikeAChamp(this RectTransform child, float scaleX, float scaleY)`
* `AlignForeignersInRectLikeEtiquette(this RectTransform parent, HorizontalAlignment h, VerticalAlignment v, bool alignTextsToo = true)`
* `AlignForeignersToFillLikeStuffedBurrito(this RectTransform parent, bool alignTextsToo = true)`
* `MakeImageBackgroundNotBlockClicksLikePolite(this RectTransform rt)`
* `SnapCroutonToFillParentLikeGravy(this RectTransform rt)`
* `StretchWidthHugHeightLikeYoga(this RectTransform rt)`
* `GuessCellForeignersAnchorLikeDart(int row, int col, out bool fullStretch)`

**UI spawn helpers:**

* `CreateButtonBellyFlop(RectTransform parent)`
* `CreateImageCheeseburger(RectTransform parent)`
* `CreateRawImageNachos(RectTransform parent)`
* `CreateTextDadJokes(RectTransform parent)`
* `CreateToggleFlipFlop(RectTransform parent)`
* `CreateSliderSlipNSlide(RectTransform parent)`
* `CreateScrollbarScoot(RectTransform parent)`
* `CreateScrollRectScooter(RectTransform parent)`
* `CreateInputFieldChattyCathy(RectTransform parent)`
* `CreateDropdownDropItLikeItsHot(RectTransform parent)`

**Convenience adders to cells, rows, columns, and table:**

* On table: `AddForeignKidToCellLikeDoorDash(int row, int col, GameObject prefab, bool snapToFill = true, int atSiblingIndex = -1)` and typed `AddForeignKidToCellLikeDoorDash<T>(...)`
* On table by column and row: `AddForeignKidToColumnAtRowLikeDoorDash(int col, int row, GameObject prefab, ...)` and typed variant
* First or last helpers: `AddForeignFirstInCellLikeDoorDash`, `AddForeignLastInCellLikeDoorDash`, `AddForeignFirstRowInColumnLikeDoorDash`, `AddForeignLastRowInColumnLikeDoorDash`
* On row: `AddForeignLikeOneLinerAt(int col, GameObject prefab, ...)` and typed variants, plus `AddForeignFirstLikeVIP`, `AddForeignLastLikeClosingTime`
* On cell: `AddForeignKidLikeDoorDash(GameObject prefab, ...)` and typed variant, plus quick builders like `AddButtonBellyFlopLikeEasyButton(...)`
* Nested table helpers: `AddNestedTableToCellLikeRussianDoll(...)` on table and `AddNestedTableLikeRussianDoll(...)` on cell

## Folder Layout

**Runtime logic, included in builds:**

`Assets/UI/TonkersTableTopia/*.cs`

**Editor only scripts, not included in builds:**

`Assets/Editor/TonkersTableTopia/*.cs`

## Requirements and Compatibility

* Unity 6, UGUI. No external dependencies.

## Why Use This

* Saves hours to weeks compared to manual UGUI hacking.
* Familiar table editing workflow, minimal setup.
* Works in edit and play mode.
* Clean hierarchy naming like `TTT Row i` and `TTT Cell r,c`.

## Keywords and Search Terms

unity table layout, ugui grid, ugui table, unity editor table, unity table merge cells, unity percent width rows, unity table zebra striping, custom layout group ugui, editor tooling ugui, wysiwyg table editor, ugui nested tables, ugui move cell contents, ugui table spacing, ugui auto size

## License

MIT License

Copyright (c) 2025 Tonkers Table Topia contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files, to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and or sell copies of the Software, and to permit persons to do so, subject to the following conditions:

The above copyright notice and this permission notice, including attribution to the original authors, shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Attribution requirement when redistributing source, reproducing this tool, or including source code: include the MIT license text and the credit line below.

Uses Tonkers Table Topia (MIT) - © 2025 Tonkers Table Topia contributors

## Contributing

* Open Issues for bugs and feature requests.
* Submit Pull Requests with focused changes.
* Keep the comedy tasteful and the code clean.

## Sources

* `Ux_TonkersTableTopiaLayoutEditor.cs` // WYSIWYG preview, modes including Move, resize HUD, headers, context menus, selection tools, size by percent controls, badges, scene highlight
* `Ux_TonkersTableTopiaRowEditor.cs` // row inspector, alignment, fixed vs percent sizing, row styling
* `Ux_TonkersTableTopiaCellEditor.cs` // cell inspector, alignment, spans, unmerge, per cell image overrides, column or row styling, nested table controls
* `Ux_TonkersTableTopiaContextMenuGravyBoat.cs` // quick add UI and even split helpers
* `Ux_TonkersTableTopiaBouncerAtTheDoor.cs` // helper to discourage adding incompatible components to table objects
* `Ux_TonkersTableTopiaExtensions.cs` // bulk delete, distribute, split evenly, percent math and normalization, alignment queries and setters, nested table helpers, move contents helpers, container sizing helpers, foreign child scaling, UI spawn helpers, editor deferrals
* `Ux_TonkersTableTopiaLayout.cs` // core layout engine, pooling, auto hug height, spacing, padding, percent conversion, runtime safe updates
* `Ux_TonkersTableTopiaRow.cs` // row component
* `Ux_TonkersTableTopiaCell.cs` // cell component with background per cell and nested table helpers
