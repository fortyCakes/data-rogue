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
    public class BLTTilesGameplayRenderer : IGameplayRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private readonly ISpriteManager _spriteManager;
        private readonly ISpriteSheet _shadeSprite;
        private readonly List<IStatsRendererHelper> _statsDisplayers;

        public BLTTilesGameplayRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            _spriteManager = spriteManager;
            _shadeSprite = _spriteManager.Get("shade");

            _statsDisplayers = BLTStatsRendererHelper.DefaultStatsDisplayers.OfType<IStatsRendererHelper>().ToList();

            _statsDisplayers.AddRange(ioSystemConfiguration.AdditionalStatsDisplayers);
        }

        public MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y)
        {
            foreach (MapConfiguration map in _ioSystemConfiguration.MapConfigurations)
            {
                if (IsOnMap(map, x, y))
                {
                    var lookupX = cameraPosition.X - map.Position.Width / (2 * BLTTilesIOSystem.TILE_SPACING) + x / BLTTilesIOSystem.TILE_SPACING;
                    var lookupY = cameraPosition.Y - map.Position.Height / (2 * BLTTilesIOSystem.TILE_SPACING) + y / BLTTilesIOSystem.TILE_SPACING;

                    return new MapCoordinate(cameraPosition.Key, lookupX, lookupY);
                }
            }

            return null;
        }

        private bool IsOnMap(MapConfiguration map, int x, int y)
        {
            return x >= map.Position.Left && x <= map.Position.Right && y >= map.Position.Top && y < map.Position.Bottom;
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

            foreach(var statsConfiguration in _ioSystemConfiguration.StatsConfigurations)
            {
                RenderStats(statsConfiguration, systemContainer, playerFov);
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

                    renderTracker[x + 1, y + 1] = isInFov || currentMap.SeenCoordinates.Contains(coordinate);
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
                            tilesTracker[x + 1, y + 1, 1] = new SpriteAppearance {Top = "unknown"};
                        }
                    }
                }
            }

            RenderMapSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, false);

            RenderMapSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, false);

            RenderMapSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, true);

            RenderMapSprites(mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, true);

            RenderMapShade(renderTracker, fovTracker, renderWidth, renderHeight, mapConfiguration);
        }

        private void RenderStats(StatsConfiguration statsConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var player = systemContainer.PlayerSystem.Player;
            int y = statsConfiguration.Position.Top;

            foreach (StatsDisplay display in statsConfiguration.Displays)
            {
                IStatsRendererHelper statsDisplayer = _statsDisplayers.Single(s => s.DisplayType == display.DisplayType);
                var tuple = new ValueTuple<int, ISpriteManager>(statsConfiguration.Position.Left, _spriteManager);
                statsDisplayer.Display(tuple, display, systemContainer, player, playerFov, ref y);
            }
        }

        private void RenderMapShade(bool[,] renderTracker, bool[,] fovTracker, int renderWidth, int renderHeight, MapConfiguration mapConfiguration)
        {
            BLT.Layer(BLTLayers.MapShade);
            BLT.Font("");

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x + 1, y + 1])
                    {
                        if (!fovTracker[x + 1, y + 1])
                        {
                            var sprite = _shadeSprite.Tile(TileDirections.None);
                            BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                        }
                        else
                        {
                            var aboveConnect = (fovTracker[x + 1, y]);
                            var belowConnect = (fovTracker[x + 1, y + 2]);
                            var leftConnect = (fovTracker[x, y + 1]);
                            var rightConnect = (fovTracker[x + 2, y + 1]);

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

        private void RenderMapSprites(MapConfiguration mapConfiguration, bool[,] renderTracker, int renderWidth, int renderHeight, SpriteAppearance[,,] tilesTracker, int z, bool top)
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
                    if (renderTracker[x + 1, y + 1])
                    {
                        var appearance = tilesTracker[x + 1, y + 1, z];
                        if (appearance != null)
                        {
                            var spriteName = top ? appearance.Top : appearance.Bottom;
                            if (spriteName != null)
                            {
                                TileDirections directions = BLTTileDirectionHelper.GetDirections(tilesTracker, x + 1, y + 1, z, top);
                                var sprite = _spriteManager.Tile(spriteName, directions);
                                BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                            }
                        }
                    }
                }
            }
        }

        private bool IsRemembered(IMap currentMap, MapCoordinate coordinate, IEntity e)
        {
            return currentMap.SeenCoordinates.Contains(coordinate) && e.Has<Memorable>();
        }

        private List<MapCoordinate> CalculatePlayerFov(ISystemContainer systemContainer)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];

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