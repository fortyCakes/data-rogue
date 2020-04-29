using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTargetingOverlayRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(TargetingOverlayControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as TargetingOverlayControl;
            var targetingActivityData = display.TargetingActivity;
            var targetingSprites = new Dictionary<CellTargeting, ISpriteSheet>
            {
                { CellTargeting.Targetable, spriteManager.Get("targetable") },
                { CellTargeting.CurrentTarget, spriteManager.Get("current_target") }
            };

            BLT.Layer(BLTLayers.MapShade);
            BLT.Font("");

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var targetableCells = targetingActivityData.TargetingData.TargetableCellsFrom(playerPosition);

            var renderWidth = control.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = control.Position.Height / BLTTilesIOSystem.TILE_SPACING;

            int offsetX = renderWidth / 2;
            int offsetY = renderHeight / 2;

            var sprites = new CellTargeting[renderWidth + 2, renderHeight + 2];

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    var currentCell = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);

                    var targetable = targetableCells.Any(c => c == currentCell);
                    var isTarget = targetingActivityData.IsTargeted(currentCell);

                    var cellTargeting = CellTargeting.None;

                    if (targetable) cellTargeting = CellTargeting.Targetable;
                    if (isTarget) cellTargeting = CellTargeting.CurrentTarget;

                    if (cellTargeting != CellTargeting.None)
                    {
                        sprites[x + 1, y + 1] = cellTargeting;
                    }
                }
            }

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    var cellTargeting = sprites[x + 1, y + 1];

                    if (cellTargeting != CellTargeting.None)
                    {

                        var renderX = control.Position.Left + x * BLTTilesIOSystem.TILE_SPACING;
                        var renderY = control.Position.Top + y * BLTTilesIOSystem.TILE_SPACING;

                        var aboveConnect = sprites[x + 1, y + 1] == sprites[x + 1, y];
                        var belowConnect = sprites[x + 1, y + 1] == sprites[x + 1, y + 2];
                        var leftConnect = sprites[x + 1, y + 1] == sprites[x, y + 1];
                        var rightConnect = sprites[x + 1, y + 1] == sprites[x + 2, y + 1];

                        var directions = TileDirections.None;
                        if (aboveConnect) directions |= TileDirections.Up;
                        if (belowConnect) directions |= TileDirections.Down;
                        if (leftConnect) directions |= TileDirections.Left;
                        if (rightConnect) directions |= TileDirections.Right;

                        BLT.Put(renderX, renderY, targetingSprites[cellTargeting].Tile(directions));
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