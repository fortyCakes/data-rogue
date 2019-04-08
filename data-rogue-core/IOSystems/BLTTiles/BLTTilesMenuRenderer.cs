﻿using BearLib;
using BLTWrapper;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using System;
using System.Text;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesMenuRenderer : IMenuRenderer
    {
        private ISpriteSheet _backgroundSpriteSheet;
        private readonly ISpriteSheet _selectorSprite;

        public BLTTilesMenuRenderer(ISpriteSheet backgroundSpriteSheet, ISpriteSheet selectorSprite)
        {
            _backgroundSpriteSheet = backgroundSpriteSheet;
            _selectorSprite = selectorSprite;
        }


        public void Render(Menu menu)
        {
            BLT.Clear();

            RenderBackground();

            RenderTitleBar(menu);

            RenderMenuActions(menu);

            RenderMenuText(menu);

            //throw new NotImplementedException();
        }

        private void RenderMenuActions(Menu menu)
        {
            if (menu.AvailableActions.Count > 1)
            {
                BLT.Layer((int)BLTLayers.Text);
                BLT.Font("text");

                var width = BLT.State(BLT.TK_WIDTH);

                StringBuilder text = new StringBuilder("[[");

                foreach (var action in menu.AvailableActions)
                {
                    var selected = action == menu.SelectedAction;

                    if (selected)
                    {
                        text.Append($"[color=#42a7f4]{action}[/color]|");
                    }
                    else
                    {
                        text.Append(action + "|");
                    }

                    text.Remove(text.Length - 1, 1);
                    text.Append("]]");

                    var size = BLT.Measure(text.ToString());

                    BLT.Print(width - size.Width - 2, 2, text.ToString());
                }
            }
        }

        private void RenderTitleBar(Menu menu)
        {
            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("textLarge");

            BLT.Print(2, 2, menu.MenuName);
        }

        private void RenderMenuText(Menu menu)
        {
            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("text");

            BLT.Print(2, 8, "[color=red]menu text");

            BLT.Font("");
            BLT.Put(40, 8, _selectorSprite.Tile(TileDirections.Left));
        }

        private void RenderBackground()
        {
            BLT.Layer((int)BLTLayers.Background);
            BLT.Font("");

            var width = BLT.State(BLT.TK_WIDTH) / BLTTilesIOSystem.TILE_SPACING;
            var height = BLT.State(BLT.TK_HEIGHT) / BLTTilesIOSystem.TILE_SPACING;

            for (int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    var directions = TileDirections.None;
                    if (x != 0) directions |= TileDirections.Left;
                    if (x != width - 1) directions |= TileDirections.Right;
                    if (y != 0) directions |= TileDirections.Up;
                    if (y != height - 1) directions |= TileDirections.Down;

                    var sprite = _backgroundSpriteSheet.Tile(directions);

                    BLT.Put(x * BLTTilesIOSystem.TILE_SPACING, y * BLTTilesIOSystem.TILE_SPACING, sprite);
                }
            }
        }
    }
}