using System;
using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Forms;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesFormRenderer : IFormRenderer
    {
        private readonly ISpriteSheet _backgroundSpriteSheet;
        private readonly ISpriteSheet _selectorSpriteLeft;
        private readonly ISpriteSheet _selectorSpriteRight;
        private int _height;
        private int _width;
        private ISpriteSheet _buttonPressed;
        private ISpriteSheet _button;

        public BLTTilesFormRenderer(ISpriteSheet backgroundSpriteSheet, ISpriteSheet selectorSpriteLeft, ISpriteSheet selectorSpriteRight, ISpriteSheet button, ISpriteSheet buttonPressed)
        {
            _backgroundSpriteSheet = backgroundSpriteSheet;
            _selectorSpriteLeft = selectorSpriteLeft;
            _selectorSpriteRight = selectorSpriteRight;
            _button = button;
            _buttonPressed = buttonPressed;
        }

        public void Render(Form form)
        {
            BLT.Clear();

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            BLTTilesBackgroundRenderer.RenderBackground(_width, _height, _backgroundSpriteSheet);

            RenderTitleBar(form);

            RenderFormButtons(form);

            // throw new NotImplementedException();
        }

        private void RenderFormButtons(Form form)
        {
            var xCoordinate = 6;

            foreach (FormButton flag in (FormButton[])Enum.GetValues(typeof(FormButton)))
            {
                if (flag == FormButton.None) continue;

                if (form.Buttons.HasFlag(flag))
                {
                    var buttonSize = RenderButton(xCoordinate, _height - 12, flag.ToString(), form.FormSelection.SelectedItem == flag.ToString());

                    xCoordinate += buttonSize.Width + 4;
                }
            }
        }

        private Size RenderButton(int x, int y, string buttonText, bool pressed)
        {
            var spriteSheet = pressed ? _buttonPressed : _button;

            BLT.Font("text");

            var buttonSize = 4; // base size
            var textSize = BLT.Measure(buttonText).Width;
            buttonSize += textSize;
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

        private void RenderTitleBar(Form form)
        {
            BLT.Layer((int)BLTLayers.Text);
            BLT.Font("textLarge");

            var x = 2;
            var y = 2;

            BLT.Print(x, y, form.Title);
        }
    }
}