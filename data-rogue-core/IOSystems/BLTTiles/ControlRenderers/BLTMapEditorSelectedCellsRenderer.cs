using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMapEditorHighlightCellsRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapEditorHighlightControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as MapEditorHighlightControl;
            var mapEditor = systemContainer.ActivitySystem.MapEditorActivity;

            var cell = mapEditor.PrimaryCell;
            var cellAppearance = cell.Get<SpriteAppearance>();

            var topSprite = cellAppearance.Top != null ? spriteManager.Get(cellAppearance.Top) : null;
            var bottomSprite = cellAppearance.Bottom != null ? spriteManager.Get(cellAppearance.Bottom) : null;

            BLT.Font("");

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            var highlightedCells = mapEditor.GetHighlightedCells();

            var renderWidth = control.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = control.Position.Height / BLTTilesIOSystem.TILE_SPACING;

            int offsetX = renderWidth / 2;
            int offsetY = renderHeight / 2;

            var checks = new bool[renderWidth + 2, renderHeight + 2];

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    var currentCell = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);

                    checks[x + 1, y + 1] = highlightedCells.Contains(currentCell);
                }
            }

            BLT.Color(Color.FromArgb(128, 255, 255, 255));

            if (bottomSprite != null)
            {
                RenderLayer(BLTLayers.MapShade, control, bottomSprite, renderWidth, renderHeight, checks);
            }

            if (topSprite != null)
            {
                RenderLayer(BLTLayers.MapShade + 1, control, topSprite, renderWidth, renderHeight, checks);
            }

            BLT.Color(Color.White);
        }

        private static void RenderLayer(int layer, IDataRogueControl control, ISpriteSheet sprite, int renderWidth, int renderHeight, bool[,] checks)
        {
            BLTLayers.Set(layer, control.ActivityIndex);

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    if (checks[x + 1, y + 1])
                    {

                        var renderX = control.Position.Left + x * BLTTilesIOSystem.TILE_SPACING;
                        var renderY = control.Position.Top + y * BLTTilesIOSystem.TILE_SPACING;

                        var aboveConnect = checks[x + 1, y + 1] == checks[x + 1, y];
                        var belowConnect = checks[x + 1, y + 1] == checks[x + 1, y + 2];
                        var leftConnect = checks[x + 1, y + 1] == checks[x, y + 1];
                        var rightConnect = checks[x + 1, y + 1] == checks[x + 2, y + 1];

                        var directions = TileDirections.None;
                        if (aboveConnect) directions |= TileDirections.Up;
                        if (belowConnect) directions |= TileDirections.Down;
                        if (leftConnect) directions |= TileDirections.Left;
                        if (rightConnect) directions |= TileDirections.Right;

                        BLT.Put(renderX, renderY, sprite.Tile(directions));
                    }
                }
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(control.Position.Width, control.Position.Height);
        }
    }
}