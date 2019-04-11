using System;
using System.Collections.Generic;
using System.Linq;
using BearLib;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTHoveredEntityDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "HoveredEntity";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int y)
        {
            var hoveredCoordinate = systemContainer.PlayerControlSystem.HoveredCoordinate;

            if (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                var originalY = y;

                RenderEntityDetails(x, ref y, display, hoveredEntity, spriteManager);

                RenderBackgroundBox(x, originalY, 10, (int)Math.Ceiling((decimal)(y - originalY + 2) / BLTTilesIOSystem.TILE_SPACING), spriteManager);

            }
        }

        protected void RenderEntityDetails(int x, ref int y, StatsDisplay display, IEntity hoveredEntity, ISpriteManager spriteManager)
        {
            BLT.Font("");
            SpriteAppearance appearance = hoveredEntity.Has<SpriteAppearance>() ? hoveredEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };

            BLT.Layer(BLTLayers.UIElementPieces);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceBottom);

            BLT.Layer(BLTLayers.UIElementPieces + 1);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceTop);

            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");
            BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 4, y - 1 + BLTTilesIOSystem.TILE_SPACING / 2, hoveredEntity.DescriptionName);
            
            y += 12;

            foreach (var split in display.Parameters.Split(';'))
            {
                var counter = GetCounter(split, hoveredEntity, out string counterText);
                if (counter != null)
                {
                    var text = $"{counterText}: {counter}";
                    RenderText(x + 4, ref y, text, display.Color);
                }
            }
        }

        private static void RenderBackgroundBox(int x, int y, int width, int height, ISpriteManager spriteManager)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");

            var spriteSheet = spriteManager.Get("textbox_blue");

            for (int xCoord = 0; xCoord < width; xCoord++)
            {
                for (int yCoord = 0; yCoord < height; yCoord++)
                {
                    BLT.Put(x + xCoord * BLTTilesIOSystem.TILE_SPACING, y + yCoord * BLTTilesIOSystem.TILE_SPACING, spriteSheet.Tile(BLTTileDirectionHelper.GetDirections(xCoord, width, yCoord, height)));
                }
            }
        }

        private static void RenderSpriteIfSpecified(int x, int y, ISpriteManager spriteManager, string spriteName)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                BLT.Put(x + 2, y + 3, spriteManager.Tile(spriteName));
            }
        }
    }
}