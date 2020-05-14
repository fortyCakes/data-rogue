using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTHoveredEntityDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(HoveredEntityDisplayBox);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            var hoveredCoordinate = systemContainer.ControlSystem.HoveredCoordinate;

            if (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                RenderEntityDetails(x, y, display, hoveredEntity, spriteManager);

                RenderBackgroundBox(x, y, GetSizeInternal(spriteManager, control, systemContainer, playerFov), spriteManager);

            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var height = 12;
            var display = control as IDataRogueInfoControl;

            foreach (var split in display.Parameters.Split(';'))
            {
                height += BLTComponentCounterDisplayer.Height;
            }

            height = BLTTilesIOSystem.TILE_SPACING * (int)Math.Ceiling((decimal)height / BLTTilesIOSystem.TILE_SPACING);

            return new Size(10 * BLTTilesIOSystem.TILE_SPACING, height);
        }

        protected static void RenderEntityDetails(int x, int y, IDataRogueInfoControl display, IEntity hoveredEntity, ISpriteManager spriteManager)
        {
            BLT.Font("");
            SpriteAppearance appearance = hoveredEntity.Has<SpriteAppearance>() ? hoveredEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
            int frame = hoveredEntity.Has<Animated>() ? hoveredEntity.Get<Animated>().CurrentFrame : 0;

            BLT.Layer(BLTLayers.UIElementPieces);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceBottom, frame);

            BLT.Layer(BLTLayers.UIElementPieces + 1);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(x, y, spriteManager, appearanceTop, frame);

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
                    RenderText(x + 4, y, out var textSize, text, display.Color);
                    y += 3;
                }
            }
        }

        private static void RenderBackgroundBox(int x, int y, Size size, ISpriteManager spriteManager)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");
            var width = size.Width / BLTTilesIOSystem.TILE_SPACING;
            var height = size.Height / BLTTilesIOSystem.TILE_SPACING;

            var spriteSheet = spriteManager.Get("textbox_blue");

            for (int xCoord = 0; xCoord < width; xCoord++)
            {
                for (int yCoord = 0; yCoord < height; yCoord++)
                {
                    BLT.Put(x + xCoord * BLTTilesIOSystem.TILE_SPACING, y + yCoord * BLTTilesIOSystem.TILE_SPACING, spriteSheet.Tile(BLTTileDirectionHelper.GetDirections(xCoord, width, yCoord, height)));
                }
            }
        }

        private static void RenderSpriteIfSpecified(int x, int y, ISpriteManager spriteManager, string spriteName, int frame)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                BLT.Put(x + 2, y + 3, spriteManager.Tile(spriteName, frame));
            }
        }
    }
}