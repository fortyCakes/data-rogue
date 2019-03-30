using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using OpenTK.Input;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class RLNetConsoleIOSystem : IIOSystem
    {
        public static IOSystemConfiguration DefaultConfiguration = new IOSystemConfiguration
        {
            InitialHeight = 70,
            InitialWidth = 100,
            TileHeight = 12,
            TileWidth = 12,
            WindowTitle = "data-rogue window title",
            MapPosition = new Rectangle(0, 0, 76, 54),
            StatsPosition = new Rectangle(77, 0, 23, 70),
            MessagePosition = new Rectangle(0, 55, 76, 15)
        };

        public RLNetConsoleIOSystem(IOSystemConfiguration ioSystemConfiguration)
        {
            this.IOSystemConfiguration = ioSystemConfiguration;
        }

        private RLRootConsole _rootConsole;
        public IRendererFactory RendererFactory { get; private set; }
        public IOSystemConfiguration IOSystemConfiguration { get; }

        public void Initialise(UpdateEventHandler onUpdate, UpdateEventHandler onRender)
        {
            string fontFileName = "Images\\Tileset\\Alloy_curses_12x12.png";
            string consoleTitle = IOSystemConfiguration.WindowTitle;

            _rootConsole = new RLRootConsole(fontFileName, IOSystemConfiguration.InitialWidth, IOSystemConfiguration.InitialHeight, IOSystemConfiguration.TileWidth, IOSystemConfiguration.TileHeight, 1, consoleTitle);

            _rootConsole.Update += onUpdate;
            _rootConsole.Render += onRender;

            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, new ConsoleGameplayRenderer(_rootConsole, IOSystemConfiguration)},
                {ActivityType.Menu, new ConsoleMenuRenderer(_rootConsole)},
                {ActivityType.StaticDisplay, new ConsoleStaticTextRenderer(_rootConsole)},
                {ActivityType.Form, new ConsoleFormRenderer(_rootConsole) },
                {ActivityType.Targeting, new ConsoleTargetingRenderer(_rootConsole, IOSystemConfiguration) }
            };

            RendererFactory = new RendererFactory(renderers);
        }

        public void Draw()
        {
            _rootConsole.Draw();
        }

        public void Run()
        {
            _rootConsole.Run();
        }

        public KeyCombination GetKeyPress()
        {
            var rlKeyPress = _rootConsole.Keyboard.GetKeyPress();

            if (rlKeyPress == null) return null;

            return new KeyCombination
            {
                Ctrl = rlKeyPress.Control,
                Shift = rlKeyPress.Shift,
                Alt = rlKeyPress.Alt,
                Key = (Key)rlKeyPress.Key
            };
        }

        public MouseData GetMouseData()
        {
            var rlMouse = _rootConsole.Mouse;

            return new MouseData
            {
                IsLeftClick = rlMouse.GetLeftClick(),
                IsRightClick = rlMouse.GetRightClick(),
                X = rlMouse.X,
                Y = rlMouse.Y
            };
        }
    }
}