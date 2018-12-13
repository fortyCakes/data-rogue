using System;
using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.EntitySystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;

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

        private const string DEBUG_SEED = "DEBUG";

        public static string Seed => DEBUG_SEED;

        private static RLRootConsole _rootConsole;

        public static void Main()
        {
            SetupRootConsole();

            InitialiseRenderers();

            InitialiseState();

            StartDataLoad();

            RunRootConsole();
        }

        private static void SetupRootConsole()
        {
            string fontFileName = "Images\\Tileset\\terminal8x8.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, SCREEN_WIDTH, SCREEN_HEIGHT, 8, 8, 1, consoleTitle);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;


        }

        private static void InitialiseRenderers()
        {
            Dictionary<ActivityType, IRenderer> renderers;
            switch(GraphicsMode)
            {
                case GraphicsMode.Console:
                    renderers = new Dictionary<ActivityType, IRenderer>()
                    {
                        {ActivityType.Gameplay, new ConsoleGameplayRenderer(_rootConsole)},
                        {ActivityType.Menu, new ConsoleMenuRenderer(_rootConsole)},
                        {ActivityType.StaticDisplay, new ConsoleStaticTextRenderer(_rootConsole)}
                    };
                    break;
                default:
                    throw new ApplicationException($"Renderers not found for graphics mode {GraphicsMode}.");
            }

            RendererFactory = new RendererFactory(renderers);
        }

        private static void RunRootConsole()
        {
            _rootConsole.Run();
        }

        private static void InitialiseRules()
        {
            EventSystem.Initialise();

            EventSystem.RegisterRule(new InputHandlerRule(PlayerControlSystem));
            EventSystem.RegisterRule(new PhysicalCollisionRule(PositionSystem));
            EventSystem.RegisterRule(new BranchGeneratorRule(EntityEngineSystem, PositionSystem, Seed));
        }


        private static void InitialiseState()
        {
            ActivityStack = new ActivityStack();

            ActivityStack.Push(new GameplayActivity(RendererFactory));

            DisplayLoadingScreen("Loading...");
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
            EntityEngineSystem = new EntityEngineSystem(new DataStaticEntityLoader());

            EventSystem = new EventRuleSystem();

            PositionSystem = new PositionSystem();
            EntityEngineSystem.Register(PositionSystem);

            PlayerControlSystem = new PlayerControlSystem(PositionSystem, EventSystem);
            EntityEngineSystem.Register(PlayerControlSystem);
        }

        private static void DisplayMainMenu()
        {
            ActivityStack.Push(MainMenu.GetMainMenu());
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            Stack<IActivity> renderStack = new Stack<IActivity>();

            foreach (IActivity activity in ActivityStack)
            {
                renderStack.Push(activity);
                if (activity.RendersEntireSpace)
                {
                    break;
                }
            }

            foreach (IActivity activity in renderStack)
            {
                activity.Render();
            }

            _rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (!ReferenceEquals(keyPress, null))
            {
                EventSystem.Try(EventType.Input, null, keyPress);
            }
        }

        public static void StartNewGame()
        {
            DisplayLoadingScreen("Generating world...");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WorldState = WorldGenerator.Create(Seed, EntityEngineSystem, PositionSystem);

                ActivityStack.Pop();

            }).Start();
        }

        public static void LoadGame()
        {
            DisplayLoadingScreen("Loading save file...");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WorldState = SaveSystem.Load(EntityEngineSystem);

                ActivityStack.Pop();

            }).Start();
        }

        public static void DisplayLoadingScreen(string text)
        {
            ActivityStack.Push(new LoadingScreenActivity(text, RendererFactory));
        }

        public static void Quit()
        {
            _rootConsole.Close();
        }

    }
}
