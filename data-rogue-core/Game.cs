using data_rogue_core.Enums;
using data_rogue_core.Renderers;
using RLNET;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace data_rogue_core
{
    public class Game
    {
        public static GameState GameState;

        public static WorldState WorldState;
        public static RuleBook RuleBook;

        public static Menu ActiveMenu;
        public static string StaticDisplayText;

        private const int SCREEN_WIDTH = 100;
        private const int SCREEN_HEIGHT = 70;
        private const int DEBUG_SEED = 1;
        private static RLRootConsole _rootConsole;

        public static void Main()
        {
            InitialiseState();
            InitialiseRules();

            StartDataLoad();

            SetupRootConsole();
        }

        private static void InitialiseRules()
        {
            //throw new NotImplementedException();
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

                Thread.Sleep(2000);

                DisplayMainMenu();

            }).Start();
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
                    ConsoleGameplayRenderer.Render(_rootConsole, WorldState);
                    break;
            }

            _rootConsole.Draw();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            switch(GameState)
            {
                case GameState.Menu:
                    ActiveMenu.HandleKeyPress(keyPress);
                    break;
            }
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

                WorldState = WorldGenerator.Create(DEBUG_SEED);

                Thread.Sleep(1000);

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
