using data_rogue_core.Enums;
using data_rogue_core.Renderers;
using RLNET;
using System.Drawing;
using System.Threading;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.Systems;

namespace data_rogue_core
{
    public static class Game
    {
        public static GameState GameState;

        public static WorldState WorldState;

        public static IEntityEngineSystem EntityEngineSystem;
        public static IEventRuleSystem EventSystem;
        public static IPositionSystem PositionSystem;
        public static IPlayerControlSystem PlayerControlSystem;

        public static Menu ActiveMenu;
        public static string StaticDisplayText;

        private const int SCREEN_WIDTH = 100;
        private const int SCREEN_HEIGHT = 70;
        private const int DEBUG_SEED = 1;
        private static RLRootConsole _rootConsole;

        public static void Main()
        {
            InitialiseState();

            StartDataLoad();

            SetupRootConsole();
        }

        private static void InitialiseRules()
        {
            EventSystem.Initialise();

            EventSystem.RegisterRule(new InputHandlerRule(PlayerControlSystem));
            EventSystem.RegisterRule(new PhsyicalCollisionRule(PositionSystem));
        }

        private static void InitialiseState()
        {
            DisplayStaticText("Loading...");
        }

        private static void StartDataLoad()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                CreateAndRegisterSystems();
                InitialiseRules();

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
            ActiveMenu = GetMainMenu();
            GameState = GameState.Menu;
        }

        private static void SetupRootConsole()
        {
            string fontFileName = "Images\\Tileset\\terminal8x8.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, SCREEN_WIDTH, SCREEN_HEIGHT, 8, 8, 1, consoleTitle);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            _rootConsole.Run();
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            switch(GameState)
            {
                case GameState.Menu:
                    ConsoleMenuRenderer.Render(_rootConsole, ActiveMenu);
                    break;
                case GameState.StaticDisplay:
                    ConsoleStaticRenderer.Render(_rootConsole, StaticDisplayText);
                    break;
                case GameState.Playing:
                    ConsoleGameplayRenderer.Render(_rootConsole, WorldState, PositionSystem);
                    break;
            }

            _rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            EventSystem.Try(EventType.Input, null, keyPress);
        }

        private static Menu GetMainMenu()
        {
            return new Menu("Main Menu", HandleMainMenuSelection,
                new MenuItem("New Game", Color.White),
                new MenuItem("Load Game", Color.Gray, false),
                new MenuItem("Quit", Color.White)
                );
        }

        private static void HandleMainMenuSelection(MenuItem item)
        {
            switch(item.Text)
            {
                case "Quit":
                    Quit();
                    break;
                case "New Game":
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

                GameState = GameState.Playing;

            }).Start();
        }

        private static void DisplayStaticText(string text)
        {
            StaticDisplayText = text;
            GameState = GameState.StaticDisplay;
        }

        private static void Quit()
        {
            _rootConsole.Close();
        }

    }
}
