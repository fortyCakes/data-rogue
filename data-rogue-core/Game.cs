using System;
using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngine;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Forms.StaticForms;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core
{
    public static class Game
    {
        public static ActivityStack ActivityStack;
        public static GraphicsMode GraphicsMode = GraphicsMode.Console;
        public static IRendererFactory RendererFactory { get; set; }

        public static WorldState WorldState;

        public static ISystemContainer SystemContainer;

        private const int SCREEN_WIDTH = 100;
        private const int SCREEN_HEIGHT = 70;

        private const string DEBUG_SEED = "DEBUG";

        public static string Seed => DEBUG_SEED;

        private static RLRootConsole _rootConsole;
        private static bool _leaving = false;

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
            string fontFileName = "Images\\Tileset\\Alloy_curses_12x12.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, SCREEN_WIDTH, SCREEN_HEIGHT, 12, 12, 1, consoleTitle);

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
                        {ActivityType.StaticDisplay, new ConsoleStaticTextRenderer(_rootConsole)},
                        {ActivityType.Form, new ConsoleFormRenderer(_rootConsole) },
                        {ActivityType.Targeting, new ConsoleTargetingRenderer(_rootConsole) }
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
            SystemContainer.EventSystem.Initialise();

            SystemContainer.EventSystem.RegisterRules(
                new InputHandlerRule(SystemContainer),
                new PhysicalCollisionRule(SystemContainer),
                new BumpAttackRule(SystemContainer),
                new BranchGeneratorRule(SystemContainer),
                new RolledAttackRule(SystemContainer),
                new DealDamageRule(SystemContainer),
                new PeopleDieWhenTheyAreKilledRule(SystemContainer),
                new SpendTimeRule(SystemContainer),
                new PlayerDeathRule(SystemContainer),
                new CompleteMoveRule(SystemContainer),
                new GetBaseStatRule(SystemContainer),
                new TiltDamageRule(SystemContainer),
                new EnemiesInViewAddTensionRule(SystemContainer),
                new ActivateSkillRule(SystemContainer)
            );
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
            SystemContainer = new SystemContainer();

            SystemContainer.CreateSystems(DEBUG_SEED);
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
            if (!_leaving)
            {
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
                RLMouse mouse = _rootConsole.Mouse;
                InputEventData eventData = new InputEventData(keyPress, mouse);

                SystemContainer.EventSystem.Try(EventType.Input, null, eventData);

                while (WorldState != null && !SystemContainer.TimeSystem.WaitingForInput && ActivityStack.Count > 0 && ActivityStack.Peek().Type == ActivityType.Gameplay)
                {
                    SystemContainer.TimeSystem.Tick();
                }

                
            }
        }

        public static void CreateCharacter()
        {
            ActivityStack.Push(CharacterCreationForm.GetCharacterCreationActivity());
        }

        public static void StartNewGame(CharacterCreationForm characterCreationForm)
        {
            DisplayLoadingScreen("Generating world...");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WorldState = WorldGenerator.Create(SystemContainer, characterCreationForm);
            }).Start();
        }

        public static void LoadGame()
        {
            DisplayLoadingScreen("Loading save file...");

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                WorldState = SaveSystem.Load(SystemContainer);

                ActivityStack.Pop();

            }).Start();
        }

        public static void DisplayLoadingScreen(string text)
        {
            ActivityStack.Push(new LoadingScreenActivity(text, RendererFactory));
        }

        public static void Quit()
        {
            _leaving = true;
            _rootConsole.Close();
        }

    }
}
