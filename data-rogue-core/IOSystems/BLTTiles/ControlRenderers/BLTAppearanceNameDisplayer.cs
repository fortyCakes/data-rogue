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
using data_rogue_core.Utils;

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

            BLTLayers.Set(BLTLayers.MapTileBottom, statsControl.ActivityIndex);
            var boxSprite = spriteManager.Get("textbox_grey");
            BLT.Put(x, y, boxSprite.Tile(TileDirections.Down | TileDirections.Right));
            BLT.Put(x + BLTTilesIOSystem.TILE_SPACING, y, boxSprite.Tile(TileDirections.Down | TileDirections.Left));
            BLT.Put(x, y + BLTTilesIOSystem.TILE_SPACING, boxSprite.Tile(TileDirections.Up | TileDirections.Right));
            BLT.Put(x + BLTTilesIOSystem.TILE_SPACING, y + BLTTilesIOSystem.TILE_SPACING, boxSprite.Tile(TileDirections.Up | TileDirections.Left));

            var appearance = entity.Has<SpriteAppearance>() ? entity.Get<SpriteAppearance>() : new SpriteAppearance { Top = "unknown" };

            if (!string.IsNullOrEmpty(appearance.Bottom))
            {
                BLTLayers.Set(BLTLayers.MapEntityBottom, statsControl.ActivityIndex);
                BLT.Put(x + 4, y + 4, spriteManager.Tile(appearance.Bottom));
            }
            if (!string.IsNullOrEmpty(appearance.Top))
            {
                BLTLayers.Set(BLTLayers.MapEntityTop, statsControl.ActivityIndex);
                BLT.Put(x + 4, y + 4, spriteManager.Tile(appearance.Top));
            }

            BLTLayers.Set(BLTLayers.Text, statsControl.ActivityIndex);

            var text = $"{entity.GetBLTName()}";

            y += 5;
            RenderText(x + 18, y, statsControl.ActivityIndex, out _, text, statsControl.Color, false, font: "textLarge");
            y += 13;
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var statsControl = (display as IDataRogueInfoControl);
            var entity = statsControl.Entity;
            BLT.Font("textLarge");
            var textSize = BLT.Measure(entity.GetBLTName());
            BLT.Font("");

            return new Size(Math.Max(60, textSize.Width + 22), Height);
        }

        public static int Height = 18;
    }
}