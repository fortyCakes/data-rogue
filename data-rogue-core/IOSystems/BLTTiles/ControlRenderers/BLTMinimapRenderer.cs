using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTMinimapRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MinimapControl);
        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");
            var sprite = spriteManager.Tile("minimap_tile");

            var cameraPosition = systemContainer.RendererSystem.CameraPosition;
            var currentMap = systemContainer.MapSystem.MapCollection[cameraPosition.Key];
            var cameraX = cameraPosition.X;
            var cameraY = cameraPosition.Y;

            int offsetX = 31;
            int offsetY = 31;

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    var lookupX = cameraX - offsetX + x;
                    var lookupY = cameraY - offsetY + y;

                    MapCoordinate coordinate = new MapCoordinate(currentMap.MapKey, lookupX, lookupY);
                    var isInFov = playerFov.Contains(coordinate);
                    var color = DetermineMapColor(systemContainer, coordinate, currentMap, isInFov);

                    if (color != Color.Transparent)
                    {
                        BLT.Color(color);
                        BLT.Put(control.Position.Left + x * BLTTilesIOSystem.TILE_SPACING / 8, control.Position.Top + y * BLTTilesIOSystem.TILE_SPACING / 8, sprite);
                    }
                }
            }
        }

        private Color DetermineMapColor(ISystemContainer systemContainer, MapCoordinate coordinate, IMap currentMap, bool isInFov)
        {
            if (!isInFov && !currentMap.SeenCoordinates.Contains(coordinate)) return Color.Transparent;

            var color = DetermineBaseMapColor(isInFov, systemContainer, coordinate);

            if (!isInFov)
            {
                color = DarkenColor(color);
            }

            return color;
        }

        private Color DarkenColor(Color color)
        {
            const double factor = 0.75;
            int r = (int)(color.R * factor);
            int g = (int)(color.G * factor);
            int b = (int)(color.B * factor);
            return Color.FromArgb(r, g, b);
        }

        private static Color DetermineBaseMapColor(bool isInFov, ISystemContainer systemContainer, MapCoordinate coordinate)
        {
            Color baseColor;
            var entities = systemContainer.PositionSystem.EntitiesAt(coordinate);

            if (entities.Any(e => e.Has<PlayerControlledBehaviour>()))
            {
                return Color.Green;
            }

            if (isInFov && entities.Any(e => e.Has<Health>()))
            {
                return Color.Red;
            }

            if (isInFov && entities.Any(e => e.Has<Item>()))
            {
                return Color.Yellow;
            }

            if (entities.Any(e => e.Has<Portal>() || e.Has<Stairs>()))
            {
                return Color.Magenta;
            }

            var cell = systemContainer.MapSystem.CellAt(coordinate);
            var cellPhysics = cell.Get<Physical>();

            if (!cellPhysics.Passable)
            {
                if (cellPhysics.Transparent)
                {
                    return Color.DarkBlue;
                }
                else
                {
                    return Color.DarkSlateGray;
                }
            }
            else
            {
                if (cellPhysics.Transparent)
                {
                    return Color.LightGray;
                }
                else
                {
                    return Color.RosyBrown;
                }
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(32 * BLTTilesIOSystem.TILE_SPACING / 8, 32 * BLTTilesIOSystem.TILE_SPACING / 8);
        }
    }
}