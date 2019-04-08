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

        public BLTTilesFormRenderer(ISpriteSheet backgroundSpriteSheet, ISpriteSheet selectorSpriteLeft, ISpriteSheet selectorSpriteRight)
        {
            _backgroundSpriteSheet = backgroundSpriteSheet;
            _selectorSpriteLeft = selectorSpriteLeft;
            _selectorSpriteRight = selectorSpriteRight;
        }

        public void Render(Form form)
        {
            BLT.Clear();

            _height = BLT.State(BLT.TK_HEIGHT);
            _width = BLT.State(BLT.TK_WIDTH);

            BLTTilesBackgroundRenderer.RenderBackground(_width, _height, _backgroundSpriteSheet);

            // throw new NotImplementedException();
        }
    }
}