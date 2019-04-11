using System.Drawing;
using BearLib;
using BLTWrapper;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public static class BLTButton
    {
        public static Size RenderButton(int x, int y, string buttonText, bool pressed, ISpriteManager _spriteManager)
        {
            var spriteSheet = pressed ? _spriteManager.Get("button_pressed") : _spriteManager.Get("button_unpressed");

            BLT.Font("text");
            
            var textSize = BLT.Measure(buttonText).Width;
            var buttonSize = textSize + 4;
            if (buttonSize % 8 != 0)
            {
                buttonSize = (buttonSize / 8 + 1) * 8;
            }

            var numberOfTiles = buttonSize / 8;

            BLT.Font("");
            BLT.Layer((int)BLTLayers.UIElements);
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

                BLT.Put(x + 8*i, y, spriteSheet.Tile(direction));
            }

            BLT.Font("text");
            BLT.Layer((int)BLTLayers.Text);
            BLT.Print(x + buttonSize / 2 - textSize / 2, y + (pressed ? 3 : 2), buttonText);

            return new Size(8*numberOfTiles, 8);
        }
    }
}