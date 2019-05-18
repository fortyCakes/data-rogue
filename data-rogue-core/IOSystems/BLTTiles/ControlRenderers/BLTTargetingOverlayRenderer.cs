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
            var targetingActivityData = display.TargetingActivityData;
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
                        var renderX = control.Position.Left + x * BLTTilesIOSystem.TILE_SPACING;
                        var renderY = control.Position.Top + y * BLTTilesIOSystem.TILE_SPACING;

                        var sprite = targetingSprites[cellTargeting];

                        BLT.Put(renderX, renderY, sprite.Tile(TileDirections.None));
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