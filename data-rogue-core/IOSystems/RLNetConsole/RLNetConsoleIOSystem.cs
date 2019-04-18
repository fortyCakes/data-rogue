﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems.BLTTiles;
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
            MapConfigurations = new List<MapConfiguration> { new MapConfiguration { Position = new Rectangle(0, 0, 76, 54) } },
            StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(77, 0, 23, 70), Displays = new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(NameControl) },
                new InfoDisplay {ControlType =  typeof(TitleControl)},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.DarkRed},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(LocationControl) },
                new InfoDisplay { ControlType = typeof(TimeControl) },
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(VisibleEnemiesControl) }

            } } },
            MessageConfigurations = new List<MessageConfiguration> { new MessageConfiguration { Position = new Rectangle(0, 55, 76, 15) ,
                NumberOfMessages = 10} }
        };

        public RLNetConsoleIOSystem(IOSystemConfiguration ioSystemConfiguration)
        {
            Configuration = ioSystemConfiguration;
        }

        private RLRootConsole _rootConsole;
        private MouseData _lastMouse;
        private GameLoopEventHandler _onUpdate;
        private GameLoopEventHandler _onRender;

        public IRendererFactory RendererFactory { get; private set; }
        public IOSystemConfiguration Configuration { get; set; }

        public void RenderHandler(object sender, UpdateEventArgs e)
        {
            _onRender(null, new GameLoopEventArgs((long)e.Time/1000));
        }

        public void UpdateHandler(object sender, UpdateEventArgs e)
        {
            _onUpdate(null, new GameLoopEventArgs((long)e.Time / 1000));
        }

        public void Initialise(GameLoopEventHandler onUpdate, GameLoopEventHandler onRender, IEntityDataProvider graphicsDataProvider)
        {
            string fontFileName = "Images\\Tileset\\Alloy_curses_12x12.png";
            string consoleTitle = Configuration.WindowTitle;

            _rootConsole = new RLRootConsole(fontFileName, Configuration.InitialWidth, Configuration.InitialHeight, Configuration.TileWidth, Configuration.TileHeight, 1, consoleTitle);

            _onUpdate = onUpdate;
            _onRender = onRender;
            _rootConsole.Update += UpdateHandler;
            _rootConsole.Render += RenderHandler;

            var controlRenderers = RLNetControlRenderer.DefaultStatsDisplayers.OfType<IDataRogueControlRenderer>().ToList();

            controlRenderers.AddRange(Configuration.AdditionalControlRenderers);

            var unifiedRenderer = new ConsoleUnifiedRenderer(_rootConsole, Configuration, controlRenderers);

            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, unifiedRenderer},
                {ActivityType.Menu, new ConsoleMenuRenderer(_rootConsole)},
                {ActivityType.StaticDisplay, unifiedRenderer},
                {ActivityType.Form, new ConsoleFormRenderer(_rootConsole) },
                {ActivityType.Targeting, new ConsoleTargetingRenderer(_rootConsole, Configuration) },
                {ActivityType.Information, unifiedRenderer}
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

            var mouse = new MouseData
            {
                IsLeftClick = rlMouse.GetLeftClick(),
                IsRightClick = rlMouse.GetRightClick(),
                X = rlMouse.X,
                Y = rlMouse.Y,
                MouseActive = (rlMouse.GetLeftClick() || rlMouse.GetLeftClick() || _lastMouse?.X != rlMouse.X || _lastMouse?.Y != rlMouse.Y)
            };

            _lastMouse = mouse;

            return mouse;
        }

        public void Close()
        {
            _rootConsole.Close();
        }
    }
}