using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Controls;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesBackgroundRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(BackgroundControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var backgroundSpriteSheet = spriteManager.Get("textbox_blue");

            BLTLayers.Set(BLTLayers.Background, control.ActivityIndex);
            BLT.Font("");

            var width = (int)Math.Ceiling(control.Position.Width / (double)BLTTilesIOSystem.TILE_SPACING);
            var height = (int)Math.Ceiling(control.Position.Height / (double)BLTTilesIOSystem.TILE_SPACING);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileDirections directions = BLTTileDirectionHelper.GetDirections(x, width, y, height);

                    var sprite = backgroundSpriteSheet.Tile(directions);

                    BLT.Put(control.Position.Left + x * BLTTilesIOSystem.TILE_SPACING, control.Position.Top + y * BLTTilesIOSystem.TILE_SPACING, sprite);
                }
            }
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }
    }
}