using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using OpenTK.Input;
using RLNET;
using System;
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
        private KeyCombination _keyCombination;
        private readonly IOSystemConfiguration _ioSystemConfiguration;
        private BLTSpriteManager _spriteManager;
        public const int TILE_SPACING = 8;

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
                CheckInput();
                _update(null, null);
                _render(null, null);
            } while (!isClosed);
        }

        private void CheckInput()
        {
            if (BLT.HasInput())
            {
                var input = BLT.Read();

                if (input == BLT.TK_CLOSE)
                {
                    isClosed = true;
                }

                //if (IsMouseEvent(input))
                //{
                //    ResolveMouseInput(input);
                //}
                //else
                {
                    ResolveKeyboardInput(input);
                }
            }
        }

        private void ResolveKeyboardInput(int input)
        {
            var key = KeyConverter.FromBLTInput(input);

            _keyCombination = new KeyCombination
            {
                Key = key,
                Ctrl = BLT.State(BLT.TK_CONTROL) != 0,
                Shift = BLT.State(BLT.TK_SHIFT) != 0,
                Alt = BLT.State(BLT.TK_ALT) != 0,
            };
        }

        public void Draw()
        {
            BLT.Refresh();
        }

        public KeyCombination GetKeyPress()
        {
            var ret = _keyCombination;
            _keyCombination = null;
            return ret;
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

            var config = $"window: size={_ioSystemConfiguration.InitialWidth*TILE_SPACING}x{_ioSystemConfiguration.InitialHeight* TILE_SPACING}, cellsize=4x4, title='{_ioSystemConfiguration.WindowTitle}';";

            BLT.Set(config);

            BLT.Set("text font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, spacing=2x3;");
            BLT.Set("textLarge font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, resize=16x24, resize-filter=nearest, spacing=4x6;");
            BLT.Set("textXLarge font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, resize=32x48, resize-filter=nearest, spacing=8x12;");
            
            _spriteManager = new BLTSpriteManager();
            _spriteManager.Add(_spriteLoader.LoadSingleSprite("selector_left", "Images/Sprites/Misc/selector-left.png", 8, 14, 1, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadSingleSprite("selector_right", "Images/Sprites/Misc/selector-right.png", 8, 14, 1, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadSingleSprite("unknown", "Images/Sprites/Misc/unknown.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("textbox_blue", "Images/Sprites/UITiles/textbox_blue.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("textbox_grey", "Images/Sprites/UITiles/textbox_grey.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("textbox_grey_small", "Images/Sprites/UITiles/textbox_grey.png", 16, 16, 1, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("textbox_white", "Images/Sprites/UITiles/textbox_white.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("button_unpressed", "Images/Sprites/UITiles/blue_button_unpressed.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadTileset_BoxType("button_pressed", "Images/Sprites/UITiles/blue_button_pressed.png", 16, 16, 2, TILE_SPACING));
            _spriteManager.Add(_spriteLoader.LoadFourDirectionSprite("arrow", "Images/Sprites/Misc/arrow.png", 16, 16, 2, TILE_SPACING));

            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, new BLTTilesGameplayRenderer(_ioSystemConfiguration)},
                {ActivityType.Menu, new BLTTilesMenuRenderer(_spriteManager)},
                {ActivityType.StaticDisplay, new BLTTilesStaticTextRenderer(_spriteManager, TILE_SPACING)},
                {ActivityType.Form, new BLTTilesFormRenderer(_spriteManager) },
                {ActivityType.Targeting, new BLTTilesTargetingRenderer( _ioSystemConfiguration) }
            };

            RendererFactory = new RendererFactory(renderers);

            BLT.Refresh();
        }
    }
}
