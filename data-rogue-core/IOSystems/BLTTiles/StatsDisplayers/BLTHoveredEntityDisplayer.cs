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

                RenderEntityDetails(x, ref y, display, hoveredEntity, spriteManager);
            }

            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");
            BLT.Print(x, y + 40, hoveredCoordinate?.ToString());
        }

        protected void RenderEntityDetails(int x, ref int y, StatsDisplay display, IEntity hoveredEntity, ISpriteManager spriteManager)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");

            var spriteSheet = spriteManager.Get("textbox_blue");

            int height = 4;
            int width = 10;

            for (int xCoord = 0; xCoord < width; xCoord++)
            {
                for (int yCoord = 0; yCoord < height; yCoord++)
                {
                    BLT.Put(x + xCoord * BLTTilesIOSystem.TILE_SPACING, y + yCoord * BLTTilesIOSystem.TILE_SPACING, spriteSheet.Tile(BLTTileDirectionHelper.GetDirections(xCoord, width, yCoord, height)));
                }
            }

            SpriteAppearance appearance = hoveredEntity.Has<SpriteAppearance>() ? hoveredEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };

            BLT.Layer(BLTLayers.UIElementPieces);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceBottom);

            BLT.Layer(BLTLayers.UIElementPieces + 1);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceTop);
            
            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");
            BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 2, y + 2 + BLTTilesIOSystem.TILE_SPACING / 2, hoveredEntity.DescriptionName);
            

            //throw new System.NotImplementedException();
        }

        private static void RenderSpriteIfSpecified(int x, int y, ISpriteManager spriteManager, string spriteName)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                BLT.Put(x + 2, y + 2, spriteManager.Tile(spriteName));
            }
        }
    }
}