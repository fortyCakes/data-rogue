using BearLib;
using BLTWrapper;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using System;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesMenuRenderer : IMenuRenderer
    {
        private ISpriteSheet _backgroundSpriteSheet;

        public BLTTilesMenuRenderer(ISpriteSheet backgroundSpriteSheet)
        {
            _backgroundSpriteSheet = backgroundSpriteSheet;
        }


        public void Render(Menu menu)
        {
            BLT.Clear();

            RenderBackground();

            //throw new NotImplementedException();
        }

        private void RenderBackground()
        {
            var width = BLT.State(BLT.TK_WIDTH);
            var height = BLT.State(BLT.TK_HEIGHT);

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

                    BLT.Put(x, y, sprite);
                }
            }

            BLT.Print(5,5,"Test");
        }
    }
}