using BearLib;
using BLTWrapper;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesGameplayRenderer : IGameplayRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private readonly ISpriteManager _spriteManager;
        private readonly ISpriteSheet _shadeSprite;

        public BLTTilesGameplayRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _spriteManager = spriteManager;
            _shadeSprite = _spriteManager.Get("shade");
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            return new MapCoordinate(cameraPosition.Key, x/8, y/8);
        }

        public void Render(ISystemContainer systemContainer)
        {
            BLT.Clear();

            if (ReferenceEquals(systemContainer?.PlayerSystem?.Player, null))
            {
                return;
            }

            var playerFov = CalculatePlayerFov(systemContainer);

            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer, playerFov);
            }
        }

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            var renderWidth = mapConfiguration.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = mapConfiguration.Position.Height / BLTTilesIOSystem.TILE_SPACING;

            int offsetX = renderWidth / 2;
            int offsetY = renderHeight / 2;

            var tilesTracker = new SpriteAppearance[renderWidth + 2, renderHeight + 2, 2];
            var renderTracker = new bool[renderWidth + 2, renderHeight + 2];
            var fovTracker = new bool[renderWidth + 2, renderHeight + 2];

            for (int x = -1; x < renderWidth + 1; x++)
            {
                for (int y = -1; y < renderHeight + 1; y++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
                    var isInFov = playerFov.Contains(coordinate);

                    renderTracker[x+1,y + 1] = isInFov || currentMap.SeenCoordinates.Contains(coordinate);
                    fovTracker[x + 1, y + 1] = isInFov;

                    var entities = systemContainer.PositionSystem.EntitiesAt(coordinate);
                    var mapCell = entities.Last();
                    var topEntity = entities
                        .OrderByDescending(a => a.Get<Appearance>().ZOrder)
                        .FirstOrDefault(e => isInFov || IsRemembered(currentMap, coordinate, e));

                    if (topEntity == mapCell) topEntity = null;

                    tilesTracker[x + 1, y + 1, 0] = mapCell.Get<SpriteAppearance>();

                    if (topEntity != null)
                    {
                        if (topEntity.Has<SpriteAppearance>())
                        {
                            tilesTracker[x + 1, y + 1, 1] = topEntity.Get<SpriteAppearance>();
                        }
                        else
                        {
                            tilesTracker[x + 1, y + 1, 1] = new SpriteAppearance { Top = "unknown" };
                        }
                    }
                }
            }

            RenderSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, false);

            RenderSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, false);

            RenderSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, true);

            RenderSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, true);

            RenderShade(renderTracker, fovTracker, renderWidth, renderHeight, mapConfiguration);
        }

        private void RenderShade(bool[,] renderTracker, bool[,] fovTracker, int renderWidth, int renderHeight, MapConfiguration mapConfiguration)
        {
            BLT.Layer(BLTLayers.MapShade);
            BLT.Font("");

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x + 1, y + 1])
                    {
                        if (!fovTracker[x+1,y+1])
                        {
                            var aboveConnect = !(fovTracker[x + 1, y]);
                            var belowConnect = !(fovTracker[x + 1, y+2]);
                            var leftConnect = !(fovTracker[x, y + 1]);
                            var rightConnect = !(fovTracker[x + 2, y + 1]);

                            var directions = TileDirections.None;
                            if (aboveConnect) directions |= TileDirections.Up;
                            if (belowConnect) directions |= TileDirections.Down;
                            if (leftConnect) directions |= TileDirections.Left;
                            if (rightConnect) directions |= TileDirections.Right;

                            var sprite = _shadeSprite.Tile(directions);
                            BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                        }
                    }
                }
            }
        }

        private void RenderSprites(MapConfiguration mapConfiguration, bool[,] renderTracker, int renderWidth, int renderHeight, SpriteAppearance[,,] tilesTracker, int z, bool top)
        {
            if (z == 0)
            {
                BLT.Layer(top ? BLTLayers.MapTileTop : BLTLayers.MapTileBottom);
            }
            else
            {
                BLT.Layer(top ? BLTLayers.MapEntityTop : BLTLayers.MapEntityBottom);
            }
            BLT.Font("");

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x+1, y+1])
                    {
                        var appearance = tilesTracker[x + 1, y + 1, z];
                        if (appearance != null)
                        {
                            var spriteName = top ? appearance.Top : appearance.Bottom;
                            if (spriteName != null)
                            {
                                TileDirections directions = GetDirections(tilesTracker, x + 1, y + 1, z, top);
                                var sprite = _spriteManager.Tile(spriteName, directions);
                                BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                            }
                        }
                    }
                }
            }
        }

        private TileDirections GetDirections(SpriteAppearance[,,] tilesTracker, int x, int y, int z, bool top)
        {
            var appearance = tilesTracker[x, y, z];
            var connect = GetConnect(appearance, top);
            var directions = TileDirections.None;
            if (appearance == null || connect == null) return directions;

            var above = tilesTracker[x, y - 1, z];
            var below = tilesTracker[x, y + 1, z];
            var left = tilesTracker[x - 1, y, z];
            var right = tilesTracker[x + 1, y, z];

            var aboveConnect = GetConnect(above, top) == connect;
            var belowConnect = GetConnect(below, top) == connect;
            var leftConnect = GetConnect(left, top) == connect;
            var rightConnect = GetConnect(right, top) == connect;

            if (aboveConnect) directions |= TileDirections.Up;
            if (belowConnect) directions |= TileDirections.Down;
            if (leftConnect) directions |= TileDirections.Left;
            if (rightConnect) directions |= TileDirections.Right;

            return directions;
        }

        private static string GetConnect(SpriteAppearance appearance, bool top)
        {
            if (appearance == null) return null;
            return !top ? appearance.BottomConnect : appearance.TopConnect;
        }

        private bool IsRemembered(IMap currentMap, MapCoordinate coordinate, IEntity e)
        {
            return currentMap.SeenCoordinates.Contains(coordinate) && e.Has<Memorable>();
        }

        private static bool IsFullTile(int x, int y)
        {
            return x % BLTTilesIOSystem.TILE_SPACING == 0 && y % BLTTilesIOSystem.TILE_SPACING == 0;
        }

        private List<MapCoordinate> CalculatePlayerFov(ISystemContainer systemContainer)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            MapCoordinate playerPosition = systemContainer.PositionSystem.CoordinateOf(systemContainer.PlayerSystem.Player);

            var playerFov = currentMap.FovFrom(playerPosition, 9);

            foreach (var coordinate in playerFov)
            {
                currentMap.SetSeen(coordinate);
            }

            return playerFov;
        }

    }
}