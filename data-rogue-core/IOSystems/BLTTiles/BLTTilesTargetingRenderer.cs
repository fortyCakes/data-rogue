using System;
using System.Collections.Generic;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesTargetingRenderer : ITargetingRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private ISpriteSheet _shadeSprite;

        public BLTTilesTargetingRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _targetingSprites = new Dictionary<CellTargeting, ISpriteSheet>
            {
                { CellTargeting.Targetable, spriteManager.Get("targetable") },
                { CellTargeting.CurrentTarget, spriteManager.Get("current_target") }
            };
            _shadeSprite = spriteManager.Get("shade");
        }

        public void Render(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            BLT.Layer(BLTLayers.MapShade);
            BLT.Font("");

            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer, targetingActivityData);
            }
        }

        private Dictionary<CellTargeting, ISpriteSheet> _targetingSprites;

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);
            var playerFov = currentMap.FovFrom(playerPosition, 9);

            var targetableCells = targetingActivityData.TargetingData.TargetableCellsFrom(playerPosition);

            var renderWidth = mapConfiguration.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = mapConfiguration.Position.Height / BLTTilesIOSystem.TILE_SPACING;

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
                    var isTarget = targetingActivityData.CurrentTarget == currentCell;

                    var cellTargeting = CellTargeting.None;

                    if (targetable) cellTargeting = CellTargeting.Targetable;
                    if (isTarget) cellTargeting = CellTargeting.CurrentTarget;

                    if (cellTargeting != CellTargeting.None)
                    {
                        var renderX = mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING;
                        var renderY = mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING;

                        var sprite = _targetingSprites[cellTargeting];

                        BLT.Put(renderX, renderY, sprite.Tile(TileDirections.None));
                    }
                }
            }
        }
    }
}