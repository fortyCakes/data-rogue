using System;
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
    public class BLTMapRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            RenderMap(spriteManager, control, systemContainer, playerFov);
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }

        protected virtual List<IEntity> GetEntitiesAt(ISystemContainer systemContainer, IMap map, MapCoordinate mapCoordinate)
        {
            return systemContainer.PositionSystem.EntitiesAt(mapCoordinate)
                .Where(e => !e.Has<CanAddToMap>() || e.Get<CanAddToMap>().VisibleDuringPlay)
                .ToList();
        }

        protected virtual void RenderMap(ISpriteManager spriteManager, IDataRogueControl mapConfiguration, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var cameraPosition = systemContainer.RendererSystem.CameraPosition;

            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;
            var playerMoving = systemContainer.PlayerSystem.Player?.TryGet<Moving>();
            var playerMovingOffsetX = playerMoving == null ? 0 : -playerMoving.OffsetX;
            var playerMovingOffsetY = playerMoving == null ? 0 : -playerMoving.OffsetY;

            var renderWidth = mapConfiguration.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var renderHeight = mapConfiguration.Position.Height / BLTTilesIOSystem.TILE_SPACING;

            int offsetX = renderWidth / 2;
            int offsetY = renderHeight / 2;

            var tilesTracker = new SpriteAppearance[renderWidth + 2, renderHeight + 2, 2];
            var frameTracker = new AnimationFrame[renderWidth + 2, renderHeight + 2, 2];
            var offsetTracker = new BLTMapRendererOffset[renderWidth + 2, renderHeight + 2, 2];
            var particlesTracker = new List<IEntity>[renderWidth + 2, renderHeight + 2];
            var renderTracker = new bool[renderWidth + 2, renderHeight + 2];
            var fovTracker = new bool[renderWidth + 2, renderHeight + 2];

            for (int x = -1; x < renderWidth + 1; x++)
            {
                for (int y = -1; y < renderHeight + 1; y++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
                    var isInFov = playerFov?.Contains(coordinate) ?? true;

                    renderTracker[x + 1, y + 1] = isInFov || currentMap.SeenCoordinates.Contains(coordinate);
                    fovTracker[x + 1, y + 1] = isInFov;

                    var entities = GetEntitiesAt(systemContainer, currentMap, coordinate);

                    var particles = entities.Where(e => e.Has<TextParticle>()).ToList();
                    foreach(var particle in particles)
                    {
                        entities.Remove(particle);
                    }

                    var mapCell = entities.Last();
                    var topEntity = entities
                        .OrderByDescending(a => a.Get<Appearance>().ZOrder)
                        .FirstOrDefault(e => isInFov || IsRemembered(currentMap, coordinate, e));
                    

                    if (topEntity == mapCell) topEntity = null;

                    tilesTracker[x + 1, y + 1, 0] = mapCell.Get<SpriteAppearance>();

                    var animatedCell = mapCell.TryGet<Animated>();
                    if (animatedCell != null)
                    {
                        frameTracker[x + 1, y + 1, 0] = systemContainer.AnimationSystem.GetFrame(mapCell);
                    }
                    else
                    {
                        frameTracker[x + 1, y + 1, 0] = 0;
                    }

                    var movingCell = mapCell.TryGet<Moving>();
                    if (movingCell != null)
                    {
                        offsetTracker[x + 1, y + 1, 0] = new BLTMapRendererOffset(movingCell.OffsetX + playerMovingOffsetX, movingCell.OffsetY + playerMovingOffsetY);
                    }
                    else
                    {
                        offsetTracker[x + 1, y + 1, 0] = new BLTMapRendererOffset(playerMovingOffsetX, playerMovingOffsetY);
                    }

                    if (topEntity != null)
                    {
                        var spriteAppearance = topEntity.TryGet<SpriteAppearance>();

                        if (spriteAppearance != null)
                        {
                            tilesTracker[x + 1, y + 1, 1] = spriteAppearance;
                        }
                        else
                        {
                            tilesTracker[x + 1, y + 1, 1] = new SpriteAppearance { Top = "unknown" };
                        }

                        var animatedEntity = topEntity.TryGet<Animated>();
                        if (animatedEntity != null)
                        {
                            frameTracker[x + 1, y + 1, 1] = systemContainer.AnimationSystem.GetFrame(topEntity);
                        }
                        else
                        {
                            frameTracker[x + 1, y + 1, 1] = 0;
                        }

                        var moving = topEntity.TryGet<Moving>();
                        if (moving != null)
                        {
                            offsetTracker[x + 1, y + 1, 1] = new BLTMapRendererOffset(moving.OffsetX + playerMovingOffsetX, moving.OffsetY + playerMovingOffsetY);
                        }
                        else
                        {
                            offsetTracker[x + 1, y + 1, 1] = new BLTMapRendererOffset(playerMovingOffsetX, playerMovingOffsetY);
                        }
                    }

                    particlesTracker[x + 1, y + 1] = particles.ToList();
                }
            }

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, frameTracker, offsetTracker, 0, false);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, frameTracker, offsetTracker, 1, false);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, frameTracker, offsetTracker, 0, true);

            RenderMapSprites(spriteManager, mapConfiguration, renderTracker, renderWidth, renderHeight, tilesTracker, frameTracker, offsetTracker, 1, true);

            RenderMapParticles(systemContainer, renderTracker, fovTracker, particlesTracker, renderWidth, renderHeight, mapConfiguration);

            RenderMapShade(spriteManager, renderTracker, fovTracker, renderWidth, renderHeight, offsetTracker, mapConfiguration);
        }

        private void RenderMapParticles(ISystemContainer systemContainer, bool[,] renderTracker, bool[,] fovTracker, List<IEntity>[,] particlesTracker, int renderWidth, int renderHeight, IDataRogueControl mapConfiguration)
        {
            BLTLayers.Set(BLTLayers.MapParticles, mapConfiguration.ActivityIndex);
            BLT.Font("text");

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x + 1, y + 1] && fovTracker[x + 1, y + 1] && particlesTracker[x+1, y+1].Any())
                    {
                        foreach(var particle in particlesTracker[x+1, y+1])
                        {
                            var textParticle = particle.Get<TextParticle>();
                            BLT.Color(textParticle.Color);

                            var moving = particle.TryGet<Moving>();
                            var offsetX = (int)Math.Floor((moving?.OffsetX ?? 0) * BLTTilesIOSystem.TILE_SPACING);
                            var offsetY = (int)Math.Floor((moving?.OffsetY ?? 0) * BLTTilesIOSystem.TILE_SPACING);

                            BLT.Print(
                                (int)(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING + offsetX),
                                (int)(mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING + offsetY),
                                textParticle.Text);

                        }
                    }
                }
            }
        }

        private void RenderMapShade(ISpriteManager spriteManager, bool[,] renderTracker, bool[,] fovTracker, int renderWidth, int renderHeight, BLTMapRendererOffset[,,] offsetTracker, IDataRogueControl mapConfiguration)
        {
            var shadeSprite = spriteManager.Get("shade");

            BLTLayers.Set(BLTLayers.MapShade, mapConfiguration.ActivityIndex);
            BLT.Font("");
            BLT.Color(Color.White);

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x + 1, y + 1])
                    {
                        var offsetX = (int)Math.Floor(offsetTracker[x + 1, y + 1, 0].OffsetX * BLTTilesIOSystem.TILE_SPACING);
                        var offsetY = (int)Math.Floor(offsetTracker[x + 1, y + 1, 0].OffsetY * BLTTilesIOSystem.TILE_SPACING);

                        if (!fovTracker[x + 1, y + 1])
                        {
                            var sprite = shadeSprite.Tile(TileDirections.None);
                            BLT.Put(mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING + offsetX, mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING + offsetY, sprite);
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

                            

                            BLT.Put(
                                mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING + offsetX,
                                mapConfiguration.Position.Top + y * BLTTilesIOSystem.TILE_SPACING + offsetY, 
                                sprite);
                        }
                    }
                }
            }
        }

        private void RenderMapSprites(
            ISpriteManager spriteManager, IDataRogueControl mapConfiguration, 
            bool[,] renderTracker, int renderWidth, int renderHeight, 
            SpriteAppearance[,,] tilesTracker, AnimationFrame[,,] frameTracker, BLTMapRendererOffset[,,] offsetTracker,
            int z, bool top)
        {
            if (z == 0)
            {
                BLTLayers.Set(top ? BLTLayers.MapTileTop : BLTLayers.MapTileBottom, mapConfiguration.ActivityIndex);
            }
            else
            {
                BLTLayers.Set(top ? BLTLayers.MapEntityTop : BLTLayers.MapEntityBottom, mapConfiguration.ActivityIndex);
            }

            BLT.Font("");
            BLT.Color(Color.White);

            for (int x = 0; x < renderWidth; x++)
            {
                for (int y = 0; y < renderHeight; y++)
                {
                    if (renderTracker[x + 1, y + 1])
                    {
                        var appearance = tilesTracker[x + 1, y + 1, z];
                        var frame = frameTracker[x + 1, y + 1, z];
                        if (appearance != null)
                        {
                            var spriteName = top ? appearance.Top : appearance.Bottom;
                            if (spriteName != null)
                            {
                                TileDirections directions = BLTTileDirectionHelper.GetDirections(tilesTracker, x + 1, y + 1, z, top);
                                var sprite = spriteManager.Tile(spriteName, directions, frame);
                                var offsetX = (int)Math.Floor(offsetTracker[x + 1, y + 1, z].OffsetX * BLTTilesIOSystem.TILE_SPACING);
                                var offsetY = (int)Math.Floor(offsetTracker[x + 1, y + 1, z].OffsetY * BLTTilesIOSystem.TILE_SPACING);
                                BLT.Put(
                                    mapConfiguration.Position.Left + x * BLTTilesIOSystem.TILE_SPACING + offsetX, 
                                    mapConfiguration.Position.Top  + y * BLTTilesIOSystem.TILE_SPACING + offsetY, 
                                    sprite);
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

    internal class BLTMapRendererOffset
    {
        public double OffsetX;
        public double OffsetY;

        public BLTMapRendererOffset()
        {
        }

        public BLTMapRendererOffset(double offsetX, double offsetY)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}