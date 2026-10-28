using NUnit.Framework;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace UnityEditor.U2D.Tilemap.Editor.PackageTests
{
    internal class TilemapPackageEditorTests
    {
        [Test]
        public void GridPaintPaletteWindow_IsLoadedFromDll()
        {
            Assert.That(typeof(GridPaintPaletteWindow).Assembly.FullName, Contains.Substring("Unity.2D.Tilemap.Editor"));
        }

        [TestCase(0, 0, int.MinValue, 0, -500, 0)]
        [TestCase(0, 0, 0, int.MinValue, 0, -500)]
        [TestCase(0, 0, int.MaxValue, int.MaxValue, 500, 500)]
        [TestCase(int.MinValue, int.MinValue, 0, 0, int.MinValue + 500, int.MinValue + 500)]
        [TestCase(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, int.MinValue + 500, int.MaxValue - 500)]
        [TestCase(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, int.MaxValue - 500, int.MinValue + 500)]
        [TestCase(int.MaxValue - 1, int.MinValue + 1, int.MaxValue, int.MinValue, int.MaxValue, int.MinValue)]
        [TestCase(0, 0, 500, -500, 500, -500)]
        [TestCase(0, 0, 501, -501, 500, -500)]
        [TestCase(12, -34, 15, -30, 15, -30)]
        [TestCase(12, -34, 12, -34, 12, -34)]
        public void ClampMouseGridPosition_ClampsWithoutOverflow(
            int currentX, int currentY, int nextX, int nextY, int expectedX, int expectedY)
        {
            var result = PaintableGrid.ClampMouseGridPosition(
                new Vector2Int(currentX, currentY), new Vector2Int(nextX, nextY));
            Assert.That(result, Is.EqualTo(new Vector2Int(expectedX, expectedY)));
        }
    }
}
