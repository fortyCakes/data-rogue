﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTMapRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            RenderMap(spriteManager, control, systemContainer, playerFov);
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }

        private void RenderMap(ISpriteManager spriteManager, IDataRogueControl mapConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
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
                            tilesTracker[x + 1, y + 1, 1] = new SpriteAppearance { Top = "unknown" };
                        }
                    }
                }
            }

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, false);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, false);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 0, true);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, 1, true);

            RenderMapShade(spriteManager, renderTracker, fovTracker, renderWidth, renderHeight, mapConfiguration);
        }

        private void RenderMapShade(ISpriteManager spriteManager, bool[,] renderTracker, bool[,] fovTracker, int renderWidth, int renderHeight, IDataRogueControl mapConfiguration)
        {
            var shadeSprite = spriteManager.Get("shade");

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
                            var sprite = shadeSprite.Tile(TileDirections.None);
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

                            var sprite = shadeSprite.Tile(directions);
                            BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                        }
                    }
                }
            }
        }

        private void RenderMapSprites(ISpriteManager spriteManager, IDataRogueControl mapConfiguration, bool[,] renderTracker, int renderWidth, int renderHeight, SpriteAppearance[,,] tilesTracker, int z, bool top)
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
                                var sprite = spriteManager.Tile(spriteName, directions);
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
    }
}