using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Drawing;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.Menus;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;
using OpenTK.Graphics.ES11;

namespace data_rogue_core
{
    public static class Game
    {
        public static ActivityStack ActivityStack;
        public static GraphicsMode GraphicsMode = GraphicsMode.Console;
        public static IRendererFactory RendererFactory { get; set; }

        public static WorldState WorldState;

        public static IEntityEngineSystem EntityEngineSystem;
        public static IEventRuleSystem EventSystem;
        public static IPositionSystem PositionSystem;
        public static IPlayerControlSystem PlayerControlSystem;

        private const int SCREEN_WIDTH = 100;
        private const int SCREEN_HEIGHT = 70;
        private const int DEBUG_SEED = 1;
        private static RLRootConsole _rootConsole;

        public static void Main()
        {
            SetupRootConsole();

            InitialiseRenderers();

            InitialiseState();

            StartDataLoad();

            RunRootConsole();
        }

        private static void InitialiseRenderers()
        {
            var consoleRenderers = new Dictionary<ActivityType, IRenderer>
            {
                { ActivityType.Gameplay, new ConsoleGameplayRenderer(_rootConsole) },
                { ActivityType.Menu, new ConsoleMenuRenderer(_rootConsole) },
                { ActivityType.StaticDisplay, new ConsoleStaticTextRenderer(_rootConsole) }
            };

            RendererFactory = new RendererFactory(consoleRenderers);
        }

        private static void RunRootConsole()
        {
            _rootConsole.Run();
        }

        private static void InitialiseRules()
        {
            EventSystem.Initialise();

            EventSystem.RegisterRule(new InputHandlerRule(PlayerControlSystem));
            EventSystem.RegisterRule(new PhsyicalCollisionRule(PositionSystem));
        }

        private static void InitialiseState()
        {
            ActivityStack = new ActivityStack();

            ActivityStack.Push(new GameplayActivity(RendererFactory));

            DisplayStaticText("Loading...");
        }

        

        private static void StartDataLoad()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                CreateAndRegisterSystems();
                InitialiseRules();

                ActivityStack.Pop();
                DisplayMainMenu();

            }).Start();
        }

        private static void CreateAndRegisterSystems()
        {
            EntityEngineSystem = new EntityEngineSystem();

            EventSystem = new EventRuleSystem();

            PositionSystem = new PositionSystem();
            EntityEngineSystem.Register(PositionSystem);

            PlayerControlSystem = new PlayerControlSystem(PositionSystem, EventSystem);
            EntityEngineSystem.Register(PlayerControlSystem);
        }

        private static void DisplayMainMenu()
        {
            ActivityStack.Push(GetMainMenu());
        }

        private static void SetupRootConsole()
        {
            string fontFileName = "Images\\Tileset\\terminal8x8.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, SCREEN_WIDTH, SCREEN_HEIGHT, 8, 8, 1, consoleTitle);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            var currentActivity = ActivityStack.Peek();

            currentActivity.Render();

            _rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            EventSystem.Try(EventType.Input, null, keyPress);
        }

        private static MenuActivity GetMainMenu()
        {
            var menu = new Menu("Main Menu", HandleMainMenuSelection,
                new MenuItem("New Game", Color.White),
                new MenuItem("Load Game", Color.Gray, false),
                new MenuItem("Quit", Color.White)
                );

            return new MenuActivity(menu, RendererFactory);
        }

        private static void HandleMainMenuSelection(MenuItem item)
        {
            switch(item.Text)
            {
                case "Quit":
                    ActivityStack.Pop();
                    Quit();
                    break;
                case "New Game":
                    ActivityStack.Pop();
                    StartNewGame();
                    break;

            }
        }

        private static void StartNewGame()
        {
            DisplayStaticText("Generating world...");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WorldState = WorldGenerator.Create(DEBUG_SEED, EntityEngineSystem);

                ActivityStack.Pop();

            }).Start();
        }

        private static void DisplayStaticText(string text)
        {
            ActivityStack.Push(new StaticTextActivity(text, RendererFactory));
        }

        private static void Quit()
        {
            _rootConsole.Close();
        }

    }
}
