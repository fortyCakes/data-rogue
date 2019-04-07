using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using OpenTK.Input;
using RLNET;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTTilesIOSystem : IIOSystem
    {
        public static IOSystemConfiguration DefaultConfiguration = new IOSystemConfiguration
        {
            InitialHeight = 25,
            InitialWidth = 40,
            TileHeight = 32,
            TileWidth = 32,
            WindowTitle = "data-rogue window title",
            MapConfigurations = new List<MapConfiguration> { new MapConfiguration { Position = new Rectangle(0, 0, 40, 25) } },
            StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(0, 0, 23, 70), Displays = new List<StatsDisplay> {
                new StatsDisplay { DisplayType = "Name" },
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "Health,HP", BackColor = Color.DarkRed}

            } } },
            MessageConfigurations = new List<MessageConfiguration> { new MessageConfiguration { Position = new Rectangle(0, 15, 40, 10) } }
        };

        private BLTSpriteLoader _spriteLoader;
        private bool isClosed;
        private UpdateEventHandler _update;
        private UpdateEventHandler _render;
        private readonly IOSystemConfiguration _ioSystemConfiguration;

        public BLTTilesIOSystem(IOSystemConfiguration ioSystemConfiguration)
        { 
            _spriteLoader = new BLTSpriteLoader();

            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public IRendererFactory RendererFactory { get; private set; }

        public void Close()
        {
            isClosed = true;
            BLT.Close();
        }

        public void Run()
        {

            do
            {
                _update(null, null);
                _render(null, null);
            } while (!isClosed && !BLT.Check(BLT.TK_CLOSE));
        }

        public void Draw()
        {
            BLT.Refresh();
        }

        public KeyCombination GetKeyPress()
        {
            var input = BLT.Peek();

            var key = (Key)input;
            var ctrl = BLT.Check(BLT.TK_CONTROL);
            var shift = BLT.Check(BLT.TK_SHIFT);
            var alt = BLT.Check(BLT.TK_ALT);

            return new KeyCombination
            {
                Key = key,
                Shift = shift,
                Ctrl = ctrl,
                Alt = alt
            };
        }

        public MouseData GetMouseData()
        {
            var x = BLT.State(BLT.TK_MOUSE_PIXEL_X);
            var y = BLT.State(BLT.TK_MOUSE_PIXEL_Y);

            var leftClick = BLT.Check(BLT.TK_MOUSE_LEFT);
            var rightClick = BLT.Check(BLT.TK_MOUSE_RIGHT);

            return new MouseData
            {
                IsLeftClick = leftClick,
                IsRightClick = rightClick,
                X = x,
                Y = y
            };
        }

        public void Initialise(UpdateEventHandler onUpdate, UpdateEventHandler onRender)
        {
            BLT.Open();

            _update = onUpdate;
            _render = onRender;

            var config = $"window: size={_ioSystemConfiguration.InitialWidth}x{_ioSystemConfiguration.InitialHeight}, cellsize={_ioSystemConfiguration.TileWidth}x{_ioSystemConfiguration.TileHeight}, title='{_ioSystemConfiguration.WindowTitle}';";

            BLT.Set(config);

            BLT.Set("font: Images/Tileset/SDS_8x8.ttf, size=8;");

            BoxTilesetSpriteSheet menuBackground = _spriteLoader.LoadTileset_BoxType("textbox_blue", "Images/Sprites/Misc/textbox_blue.png", 16, 16, 2);

            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, new BLTTilesGameplayRenderer(_ioSystemConfiguration)},
                {ActivityType.Menu, new BLTTilesMenuRenderer(menuBackground)},
                {ActivityType.StaticDisplay, new BLTTilesStaticTextRenderer()},
                {ActivityType.Form, new BLTTilesFormRenderer() },
                {ActivityType.Targeting, new BLTTilesTargetingRenderer( _ioSystemConfiguration) }
            };

            RendererFactory = new RendererFactory(renderers);
        }
    }
}
