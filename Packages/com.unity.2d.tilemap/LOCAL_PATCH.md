# DearLife local patch

Embedded from Unity 2022.3.62f3's built-in `com.unity.2d.tilemap@1.0.0` on 2026-10-28.
The original files, license and `.meta` GUIDs are preserved.

## Mouse grid position overflow

`Editor/PaintableGrid.cs`, `UpdateMouseGridPosition` delegates its existing limit
to the internal `ClampMouseGridPosition` helper: compute each coordinate delta
as `long` before subtraction and use `Math.Abs(long)` for the existing 500-cell
limit. The original integer subtraction could wrap, and `Mathf.Abs(int.MinValue)`
throws `OverflowException` repeatedly during Scene view mouse/camera updates.
Public APIs and the existing movement limit are unchanged.

`Tests/Editor/TilemapPackageTests.cs` covers minimum/maximum coordinates, movement
direction across both integer extremes, normal movement and the 500-cell limit.
Run `TilemapPackageEditorTests` in Unity Test Runner, EditMode.

This embedded package takes precedence over the built-in package; do not patch
`Library/PackageCache`. When upgrading Unity, compare the upstream implementation
and remove or rebase this embedded package after verifying the regression tests.
