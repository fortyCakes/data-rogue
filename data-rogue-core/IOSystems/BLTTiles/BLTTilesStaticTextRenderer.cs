using System.Drawing;
using BearLib;
using BLTWrapper;
using data_rogue_core.Renderers;
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesStaticTextRenderer : IStaticTextRenderer
    {
        private readonly ISpriteManager _spriteManager;

        public BLTTilesStaticTextRenderer(ISpriteManager spriteManager, int tileSpacing)
        {
            _tileSpacing = tileSpacing;
            _spriteManager = spriteManager;
        }

        private int _height;
        private int _width;
        private int _tileSpacing;

        public void Render(string text, bool renderEntireSpace)
        {
            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            var textboxSprite = _spriteManager.Get("textbox_blue");

            if (renderEntireSpace)
            {
                BLT.Clear();
                BLTTilesBackgroundRenderer_Old.RenderBackground(_width,_height, textboxSprite);

                BLT.Layer(BLTLayers.Text);
                BLT.Font("text");
                BLT.Print(new Rectangle(4, 4, _width - 8, _height - 8), text);
            }
            else
            {
                var textLength = text.Length;

                int lineLength;

                if (textLength < 80)
                {
                    lineLength = textLength;
                }
                else
                {
                    lineLength = 80;
                }

                var textLines = WordWrapper.WordWrap(text, lineLength);

                
                RenderTextBox(textLines, textboxSprite);
            }
        }

        private void RenderTextBox(string textLines, ISpriteSheet textboxSprite)
        {
            BLT.Font("text");

            var size = BLT.Measure(textLines);
            size.Width += 4;
            size.Height += 4;

            var widthInTiles = size.Width / _tileSpacing + (size.Width % _tileSpacing == 0 ? 0 : 1);
            var heightInTiles = size.Height / _tileSpacing + (size.Height % _tileSpacing == 0 ? 0 : 1);
            var boxWidth = widthInTiles * _tileSpacing;
            var boxHeight = heightInTiles * _tileSpacing;


            BLT.Layer(BLTLayers.Text);
            BLT.Print(_width / 2 - boxWidth / 2 + 2, _height / 2 - boxHeight / 2 + 2, textLines);

            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");
            for (int x = 0; x < widthInTiles; x++)
            {
                for (int y = 0; y < heightInTiles; y++)
                {
                    var directions = BLTTileDirectionHelper.GetDirections(x, widthInTiles, y, heightInTiles);

                    BLT.Put(_width/2 - boxWidth/2 + x * _tileSpacing, _height/2 - boxHeight/2 + y * _tileSpacing, textboxSprite.Tile(directions));
                }
            }
        }
    }
}