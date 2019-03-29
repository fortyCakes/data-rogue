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
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;

namespace data_rogue_core
{
    public class DataRogueGame
    {
        public GraphicsMode GraphicsMode = GraphicsMode.Console;

        public ISystemContainer SystemContainer;

        public IIOSystem IOSystem;

        public string Seed { get; set; }

        private RLRootConsole _rootConsole;
        private bool _leaving = false;
        public bool Loading { get; set; } = false;

        public void Run(string seed, List<Type> eventRules, IIOSystem ioSystem = null, EntityDataProviders entityDataProviders = null)
        {
            Seed = seed;

            IOSystem = ioSystem ?? new RLNetConsoleIOSystem();

            InitialiseIOSystem();

            CreateAndRegisterSystems(entityDataProviders, IOSystem.RendererFactory, seed);

            InitialiseState();

            StartDataLoad(eventRules);

            RunRootConsole(ioSystem);
        }

        private void InitialiseIOSystem()
        {
            IOSystem.Initialise(OnRootConsoleUpdate, OnRootConsoleRender);
        }

        private void RunRootConsole(IIOSystem ioSystem)
        {
            ioSystem.Run();
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

            SystemContainer.ActivitySystem.Push(new GameplayActivity());

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


        private void CreateAndRegisterSystems(EntityDataProviders entityDataProviders, IRendererFactory rendererFactory, string seed)
        {
            if (entityDataProviders == null)
            {
                entityDataProviders = EntityDataProviders.Default;
            }

            SystemContainer = new SystemContainer(entityDataProviders, rendererFactory);

            SystemContainer.CreateSystems(seed);

            SystemContainer.ActivitySystem.QuitAction = Quit;
        }

        private void DisplayMainMenu()
        {
            SystemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(SystemContainer.ActivitySystem, SystemContainer.PlayerSystem, SystemContainer.SaveSystem)));
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

            IOSystem.Draw();
        }

        private void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            if (!_leaving && !Loading)
            {
                var keyPress = IOSystem.GetKeyPress();

                SystemContainer.PlayerControlSystem.HandleInput(keyPress, IOSystem.GetMouseData());

                while (!SystemContainer.TimeSystem.WaitingForInput && SystemContainer.ActivitySystem.ActivityStack.Count > 0 && SystemContainer.ActivitySystem.Peek().Type == ActivityType.Gameplay && !_leaving)
                {
                    SystemContainer.TimeSystem.Tick();
                }
            }
        }

        public void DisplayLoadingScreen(string text)
        {
            SystemContainer.ActivitySystem.Push(new LoadingScreenActivity(text));
        }

        public void Quit()
        {
            _leaving = true;
            _rootConsole.Close();
        }

    }
}
