﻿using BearLib;
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
    [Obsolete]
    public static class BLTTilesBackgroundRenderer_Old
    {
        public static void RenderBackground(int _width, int _height, ISpriteSheet backgroundSpriteSheet)
        {
            BLT.Layer((int)BLTLayers.Background);
            BLT.Font("");

            var width = _width / BLTTilesIOSystem.TILE_SPACING;
            var height = _height / BLTTilesIOSystem.TILE_SPACING;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileDirections directions = BLTTileDirectionHelper.GetDirections(x, width, y, height);

                    var sprite = backgroundSpriteSheet.Tile(directions);

                    BLT.Put(x * BLTTilesIOSystem.TILE_SPACING, y * BLTTilesIOSystem.TILE_SPACING, sprite);
                }
            }
        }
    }

    public class BLTTilesBackgroundRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(BackgroundControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var backgroundSpriteSheet = spriteManager.Get("textbox_blue");

            BLT.Layer((int)BLTLayers.Background);
            BLT.Font("");

            var width = control.Position.Width / BLTTilesIOSystem.TILE_SPACING;
            var height = control.Position.Height / BLTTilesIOSystem.TILE_SPACING;

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

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }
    }
}