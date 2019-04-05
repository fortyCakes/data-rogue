using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using OpenTK.Input;
using RLNET;

namespace data_rogue_core.IOSystems.RLNetConsole
{
    public class RLNetConsoleIOSystem : IIOSystem
    {
        private const int CONSOLE_SCREEN_WIDTH = 100;
        private const int CONSOLE_SCREEN_HEIGHT = 70;

        private RLRootConsole _rootConsole;
        public IRendererFactory RendererFactory { get; private set; }

        public void Initialise(UpdateEventHandler onUpdate, UpdateEventHandler onRender)
        {
            string fontFileName = "Images\\Tileset\\Alloy_curses_12x12.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, CONSOLE_SCREEN_WIDTH, CONSOLE_SCREEN_HEIGHT, 12, 12, 1, consoleTitle);

            _rootConsole.Update += onUpdate;
            _rootConsole.Render += onRender;

            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, new ConsoleGameplayRenderer(_rootConsole)},
                {ActivityType.Menu, new ConsoleMenuRenderer(_rootConsole)},
                {ActivityType.StaticDisplay, new ConsoleStaticTextRenderer(_rootConsole)},
                {ActivityType.Form, new ConsoleFormRenderer(_rootConsole) },
                {ActivityType.Targeting, new ConsoleTargetingRenderer(_rootConsole) }
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

        public void Close()
        {
            _rootConsole.Close();
        }
    }
}