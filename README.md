# Tonkers Table Topia (Unity UGUI)

Finally: simple tables in Unity UGUI without suffering.

- Note: Don't forget to checkout & BUY the Arcade Style game on steam, the game for which this editor script was made and used in.
- https://store.steampowered.com/app/3844910/

I got tired of never being able to make basic tables in UGUI. Years. Of. Wrestling. I just wanted rows, columns, merge cells, zebra stripes, and pixels or percents that behave. So I built this. Now you can click, merge, split, resize, and laugh maniacally as your UGUI tables behave like a polite spreadsheet. No more “everyone fend for yourselves”, this is a sane, WYSIWYG table layout management system.

---

## Repository Metadata

| Field | Value |
|---|---|
| Repository name | **Tonkers Table Topia** |
| Short description | WYSIWYG table layout system for Unity UGUI with merge or split, zebra striping, percent or pixel sizing, and a full Editor UI. MIT licensed. |
| Topics or tags | unity, unity-editor, ugui, ui, layout, table, grid, editorwindow, editor-tooling, wysiwyg, runtime, mit |
| Suggested visibility | Public |
| Primary language | C# |
| License | MIT |

---

## Key Features and Benefits

- WYSIWYG Editor preview with row and column headers
- Click to select cells, Shift to select rectangles
- Merge and unmerge cells
- Insert and delete rows or columns with safety prompts
- Drag splitters to resize rows or columns in the preview
- Even or custom sizing, in pixels or normalized percent
- Zebra striping by rows and by columns
- Per row and per column background sprites and tints
- Optional per cell sprite override and color
- Optional ContentSizeFitter auto sizing on cells
- Runtime safe layout update in edit and play mode
- Small, readable C# with comedy seasoning
- MIT license, attribution required when redistributing source

---

## Installation

1. Copy the runtime classes into your project: Assets/UI/TonkersTableTopia/ (or any assets folder)

- Ux_TonkersTableTopiaLayout.cs
- Ux_TonkersTableTopiaRow.cs
- Ux_TonkersTableTopiaCell.cs
- Ux_TonkersTableTopiaExtensions.cs

2. Copy the Editor scripts into: Assets/Editor/TonkersTableTopia/ (or any editor foldeR)

- Ux_TonkersTableTopiaLayoutEditor.cs
- Ux_TonkersTableTopiaRowEditor.cs
- Ux_TonkersTableTopiaCellEditor.cs

3. Let Unity compile. Add **Tonkers Table Topia** to any GameObject with a `RectTransform` via:
- Add Component | Layout | Tonkers Table Topia

Optional:

- Assembly definitions if you want separation:

- Assets/UI/TonkersTableTopia/TonkersTableTopia.Runtime.asmdef
- Assets/Editor/TonkersTableTopia/TonkersTableTopia.Editor.asmdef // Editor only

---

## Usage

1. Add `Ux_TonkersTableTopiaLayout` to a `RectTransform`.
2. Set row and column counts. Toggle the WYSIWYG preview in the Inspector.
3. Click cells in the preview to select. Shift click to select a rectangle. Use Merge, Unmerge, Insert, Delete, and Distribute.
4. Toggle Resize Mode to drag splitters and resize rows or columns directly.
5. Use zebra options and background sprites to style cells, rows, and columns.

### Example, runtime

**Concepts**

**Sizing per column and per row:**
- Positive value means fixed pixels.
- Negative value means percent of available inner space, for example `-0.25f` is 25 percent.
- Zero means flexible share of the remaining space.
- Percent values are normalized, the Editor can convert current pixel layout to percents for consistent behavior when the parent changes size.

**Alternating color stripes:**
- Enable per row, per column, or both. When both are on, colors blend.

**ContentSizeFitter:**
- Optional on each cell, helpful when children use Layout Elements or preferred size text.

**Classes**
- `Ux_TonkersTableTopiaLayout`
- `Ux_TonkersTableTopiaRow`
- `Ux_TonkersTableTopiaCell`
- `Ux_TonkersTableTopiaLayoutEditor` (Editor)
- `Ux_TonkersTableTopiaRowEditor` (Editor)
- `Ux_TonkersTableTopiaCellEditor` (Editor)
- `Ux_TonkersTableTopiaExtensions` (static helpers)

**Public API, selected**

**Layout construction and sync:**
- `RebuildComedyClubSeatingChart()`
- `FlagLayoutAsNeedingSpaDay()`
- `ConvertAllSpecsToPercentages()`
- `SyncColumnWardrobes()`, `SyncRowWardrobes()`
- `UpdateSeatingLikeAProUsher()`

**Structure editing:**
- `InsertRowLikeANinja(int index)`, `InsertColumnLikeANinja(int index)`
- `TryPolitelyDeleteRow(int index)`, `TryPolitelyDeleteColumn(int index)`
- `SafeDeleteRowAtWithWittyConfirm(int index)`, `SafeDeleteColumnAtWithWittyConfirm(int index)`
- `SwapRowsLikeMusicalChairs(int aIndex, int bIndex)`
- `SwapColumnsLikeTradingCards(int aIndex, int bIndex)`
- `MergeCellsLikeAGroupHug(int startRow, int startCol, int rowCount, int colCount)`
- `UnmergeCellEveryoneGetsTheirOwnChair(int row, int col)`

**Accessors:**
- `FetchCellRectTransformVIP(int row, int col)`
- `FetchRowRectTransformVIP(int index)`
- Extension method: `GrabCellLikeItOwesYouRent(this Ux_TonkersTableTopiaLayout table, int row, int col)`

**Editor, WYSIWYG preview**

**Toggles:**
- WYSIWYG Preview, Scene Highlight, Resize Mode

**Zoom:**
- VZoom and HZoom sliders

**Selection:**
- Click selects a cell, Shift click extends to a rectangle

**Tools:**
- Insert Row Above or Below, Insert Column Left or Right
- Delete Row or Column with confirmation of relocated children
- Merge and Unmerge
- Distribute Rows and Columns evenly
- Make Last Row Flexible, Make Last Column Flexible

**Coloring:**
- Alternating row and column colors
- Editor wide preview colors saved in EditorPrefs

**Scene Highlight:**
- Outlines selected cells in the Scene view

---

## Folder Layout

**Runtime logic, included in builds:**
Assets/UI/TonkersTableTopia/*.cs

**Editor only scripts, not included in builds:**
Assets/Editor/TonkersTableTopia/*.cs

---

## Requirements and Compatibility

- Unity 6, UGUI (may work with older versions, untested)
- No external dependencies

---

## Why Use This

- Saves hours to weeks compared to manual UGUI hacking
- Familiar table editing workflow, minimal setup
- Works in edit and play mode
- Clean hierarchy naming like `TTT Row i` and `TTT Cell r,c`

---

## Keywords and Search Terms

unity table layout, ugui grid, ugui table, unity editor table, unity table merge cells, unity percent width rows, unity table zebra striping, custom layout group ugui, editor tooling ugui, wysiwyg table editor

---

## License

MIT License

Copyright (c) 2025 Tonkers Table Topia contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files, to deal in the Software
without restriction, including without limitation the rights to use, copy,
modify, merge, publish, distribute, sublicense, and or sell copies of the
Software, and to permit persons to do so, subject to the following conditions:

The above copyright notice and this permission notice, including attribution
to the original authors, shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Attribution requirement when redistributing source, reproducing this tool, or including source code: include the MIT license text and the credit line below.

Uses Tonkers Table Topia (MIT) - © 2025 Tonkers Table Topia contributors

---

## Contributing

- Open Issues for bugs and feature requests
- Submit Pull Requests with focused changes
- Keep the comedy tasteful and the code clean
