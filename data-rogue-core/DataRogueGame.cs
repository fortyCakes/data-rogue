using System;
using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.EventSystem.Rules;
using data_rogue_core.EventSystem;
using data_rogue_core.Components;

namespace data_rogue_core
{
    public class DataRogueGame
    {
        public GraphicsMode GraphicsMode = GraphicsMode.Console;

        public ISystemContainer SystemContainer;

        private const int CONSOLE_SCREEN_WIDTH = 100;
        private const int CONSOLE_SCREEN_HEIGHT = 70;

        public const string DEBUG_SEED = "DEBUG";

        public string Seed { get; set; }

        private RLRootConsole _rootConsole;
        private bool _leaving = false;
        public bool Loading { get; set; } = false;

        public void Run(string seed, List<Type> eventRules, EntityDataProviders entityDataProviders = null)
        {
            Seed = seed;

            SetupRootConsole();

            CreateAndRegisterSystems(entityDataProviders);

            InitialiseRenderers();

            InitialiseState();

            StartDataLoad(eventRules);

            RunRootConsole();
        }

        private void SetupRootConsole()
        {
            string fontFileName = "Images\\Tileset\\Alloy_curses_12x12.png";
            string consoleTitle = "data-rogue-core";

            _rootConsole = new RLRootConsole(fontFileName, CONSOLE_SCREEN_WIDTH, CONSOLE_SCREEN_HEIGHT, 12, 12, 1, consoleTitle);

            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;


        }

        private void InitialiseRenderers()
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

            SystemContainer.RendererSystem.RendererFactory = new RendererFactory(renderers);
        }

        private void RunRootConsole()
        {
            _rootConsole.Run();
        }

        private void InitialiseRules(List<Type> eventRules)
        {
            SystemContainer.EventSystem.Initialise();

            SystemContainer.EventSystem.RegisterRules(EventRuleFactory.CreateRules(SystemContainer, eventRules));

            SystemContainer.EventSystem.RegisterRule(new ApplyProcEnchantmentRule(SystemContainer, EventType.Attack));
        }


        private void InitialiseState()
        {
            SystemContainer.ActivitySystem.Initialise();

            SystemContainer.ActivitySystem.Push(new GameplayActivity(SystemContainer.RendererSystem.RendererFactory));

            DisplayLoadingScreen("Loading...");
        }
        
        private void StartDataLoad(List<Type> eventRules)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                InitialiseRules(eventRules);

                SystemContainer.ActivitySystem.Pop();
                DisplayMainMenu();

            }).Start();
        }


        private void CreateAndRegisterSystems(EntityDataProviders entityDataProviders)
        {
            if (entityDataProviders == null)
            {
                entityDataProviders = EntityDataProviders.Default;
            }

            SystemContainer = new SystemContainer(entityDataProviders);

            SystemContainer.CreateSystems(DEBUG_SEED);

            SystemContainer.RendererSystem.QuitAction = Quit;
        }

        private void DisplayMainMenu()
        {
            SystemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(SystemContainer.ActivitySystem, SystemContainer.PlayerSystem, SystemContainer.SaveSystem, SystemContainer.RendererSystem), SystemContainer.RendererSystem.RendererFactory));
        }

        private void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            Stack<IActivity> renderStack = new Stack<IActivity>();

            foreach (IActivity activity in SystemContainer.ActivitySystem.ActivityStack)
            {
                renderStack.Push(activity);
                if (activity.RendersEntireSpace)
                {
                    break;
                }
            }

            foreach (IActivity activity in renderStack)
            {
                activity.Render(SystemContainer);
            }

            _rootConsole.Draw();
        }

        private void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            if (!_leaving && !Loading)
            {
                var keyPress = KeyCombination.FromRLKeyPress(_rootConsole.Keyboard.GetKeyPress());

                SystemContainer.PlayerControlSystem.HandleInput(keyPress, _rootConsole.Mouse);

                while (!SystemContainer.TimeSystem.WaitingForInput && SystemContainer.ActivitySystem.ActivityStack.Count > 0 && SystemContainer.ActivitySystem.Peek().Type == ActivityType.Gameplay && !_leaving)
                {
                    SystemContainer.TimeSystem.Tick();
                }
            }
        }

        public void DisplayLoadingScreen(string text)
        {
            SystemContainer.ActivitySystem.Push(new LoadingScreenActivity(text, SystemContainer.RendererSystem.RendererFactory));
        }

        public void Quit()
        {
            _leaving = true;
            _rootConsole.Close();
        }

    }
}
