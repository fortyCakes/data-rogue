using System;
using System.Drawing;
using BearLib;
using BLTWrapper;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTextBox
    {
        private const int TILE_SIZE = 4;

        public static Size Render(int x, int y, string buttonText, ISpriteManager spriteManager)
        {
            var spriteSheet = spriteManager.Get("textbox_grey_small");
            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");

            BLT.Print(x + 4, y, buttonText);

            var textSize = BLT.Measure(new String('@', 30));

            var textBoxSize = textSize.Width + 4;

            if (textBoxSize % TILE_SIZE != 0)
            {
                textBoxSize = (textBoxSize / TILE_SIZE + 1) * TILE_SIZE;
            }

            var numberOfTiles = textBoxSize / TILE_SIZE;

            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");

            for (int i = 0; i < numberOfTiles; i++)
            {
                TileDirections direction = TileDirections.None;
                if (i != 0)
                {
                    direction |= TileDirections.Left;
                }

                if (i != numberOfTiles - 1)
                {
                    direction |= TileDirections.Right;
                }

                BLT.PutExt(x + TILE_SIZE * i, y - 2, 0, -2, spriteSheet.Tile(direction));
            }

            return new Size(numberOfTiles * TILE_SIZE, TILE_SIZE);
        }
    }
}