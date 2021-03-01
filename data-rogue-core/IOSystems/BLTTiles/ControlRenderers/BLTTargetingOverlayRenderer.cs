using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
            var targetingSprites = new Dictionary<TargetingStatus, ISpriteSheet>
            {
                { TargetingStatus.Targetable, spriteManager.Get("targetable") },
                { TargetingStatus.Targeted, spriteManager.Get("current_target") }
            };

            BLTLayers.Set(BLTLayers.MapShade, control.ActivityIndex);
            BLT.Font("");

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var targetableCells = systemContainer.TargetingSystem.TargetableCellsFrom(targetingActivityData.TargetingData, playerPosition);

            var renderWidth = control.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = control.Position.Height / BLTTilesIOSystem.TILE_SPACING;

            int offsetX = renderWidth / 2;
            int offsetY = renderHeight / 2;

            var sprites = new TargetingStatus[renderWidth + 2, renderHeight + 2];

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    var currentCell = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
                    var targetingStatus = targetingActivityData.GetTargetingStatus(currentCell);

                    if (targetingStatus != TargetingStatus.NotTargeted)
                    {
                        sprites[x + 1, y + 1] = targetingStatus;
                    }
                }
            }

            for (int y = 0; y < renderHeight; y++)
            {
                for (int x = 0; x < renderWidth; x++)
                {
                    var targetingStatus = sprites[x + 1, y + 1];

                    if (targetingStatus != TargetingStatus.NotTargeted)
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

                        BLT.Put(renderX, renderY, targetingSprites[targetingStatus].Tile(directions));
                    }
                }
            }
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            return control.Position.Size;
        }
    }
}