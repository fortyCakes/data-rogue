using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTAppearanceNameDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(AppearanceName);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = display.Position.Left;
            var y = display.Position.Top;
            var statsControl = (display as IDataRogueInfoControl);
            var entity = statsControl.Entity;

            BLT.Font("");

            BLT.Layer(BLTLayers.MapTileBottom);
            var boxSprite = spriteManager.Get("textbox_grey");
            BLT.Put(x, y, boxSprite.Tile(TileDirections.Down | TileDirections.Right));
            BLT.Put(x + BLTTilesIOSystem.TILE_SPACING, y, boxSprite.Tile(TileDirections.Down | TileDirections.Left));
            BLT.Put(x, y + BLTTilesIOSystem.TILE_SPACING, boxSprite.Tile(TileDirections.Up | TileDirections.Right));
            BLT.Put(x + BLTTilesIOSystem.TILE_SPACING, y + BLTTilesIOSystem.TILE_SPACING, boxSprite.Tile(TileDirections.Up | TileDirections.Left));

            var appearance = entity.Has<SpriteAppearance>() ? entity.Get<SpriteAppearance>() : new SpriteAppearance { Top = "unknown" };

            if (!string.IsNullOrEmpty(appearance.Bottom))
            {
                BLT.Layer(BLTLayers.MapEntityBottom);
                BLT.Put(x + 4, y + 4, spriteManager.Tile(appearance.Bottom));
            }
            if (!string.IsNullOrEmpty(appearance.Top))
            {
                BLT.Layer(BLTLayers.MapEntityTop);
                BLT.Put(x + 4, y + 4, spriteManager.Tile(appearance.Top));
            }

            BLT.Layer(BLTLayers.Text);

            var text = $"{entity.DescriptionName}";

            y += 5;
            RenderText(x + 18, y, out _, text, statsControl.Color, false, font: "textLarge");
            y += 13;
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(60 , Height);
        }

        public static int Height = 18;
    }
}