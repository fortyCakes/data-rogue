using System;
using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Threading;
using data_rogue_core.Activities;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.IOSystems.BLTTiles;

namespace data_rogue_core
{
    public class DataRogueGame
    {
        public ISystemContainer SystemContainer;

        public IIOSystem IOSystem;

        public string Seed { get; set; }

        private bool _leaving = false;
        public bool Loading { get; set; } = false;

        public void Run(string seed, List<Type> eventRules, IIOSystem ioSystem = null, EntityDataProviders entityDataProviders = null, IList<Type> additionalComponentTypes = null)
        {
            Seed = seed;

            IOSystem = ioSystem ?? new RLNetConsoleIOSystem(RLNetConsoleIOSystem.DefaultConfiguration);
            entityDataProviders = entityDataProviders ?? EntityDataProviders.Default;

            InitialiseIOSystem(entityDataProviders);

            CreateAndRegisterSystems(entityDataProviders, additionalComponentTypes, IOSystem.Renderer, seed, IOSystem.Configuration);

            InitialiseState();

            StartDataLoad(eventRules);

            RunRootConsole(ioSystem);
        }

        private void InitialiseIOSystem(EntityDataProviders entityDataProviders)
        {
            IOSystem.Initialise(OnRootConsoleUpdate, OnRootConsoleRender, entityDataProviders.GraphicsDataProvider);
        }

        private void RunRootConsole(IIOSystem ioSystem)
        {
            ioSystem.Run();
        }

        private void InitialiseRules(List<Type> eventRules)
        {
            SystemContainer.EventSystem.Initialise();

            SystemContainer.EventSystem.RegisterRules(EventRuleFactory.CreateRules(SystemContainer, eventRules));
        }


        private void InitialiseState()
        {
            SystemContainer.ActivitySystem.Initialise(IOSystem.Configuration.DefaultPosition, IOSystem.Configuration.DefaultPadding);

            SystemContainer.ActivitySystem.Push(new GameplayActivity(SystemContainer.ActivitySystem.DefaultPosition, SystemContainer.ActivitySystem.DefaultPadding, IOSystem.Configuration, SystemContainer.PlayerSystem));

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


        private void CreateAndRegisterSystems(EntityDataProviders entityDataProviders, IList<Type> additionalComponentTypes, IUnifiedRenderer renderer, string seed, IOSystemConfiguration ioSystemConfiguration)
        {
            SystemContainer = new SystemContainer(entityDataProviders, renderer, additionalComponentTypes);

            SystemContainer.CreateSystems(seed);

            SystemContainer.RendererSystem.IOSystemConfiguration = ioSystemConfiguration;
            SystemContainer.ActivitySystem.QuitAction = Quit;
        }

        private void DisplayMainMenu()
        {
            SystemContainer.ActivitySystem.Push(new MenuActivity(new MainMenu(SystemContainer)));
        }

        private void OnRootConsoleRender(object sender, GameLoopEventArgs e)
        {
            if (!_leaving)
            {
                SystemContainer.AnimationSystem.Tick();
                SystemContainer.AnimatedMovementSystem.Tick();
                SystemContainer.ParticleSystem.Tick();

                Stack<IActivity> renderStack = new Stack<IActivity>();

                foreach (IActivity activity in SystemContainer.ActivitySystem.ActivityStack)
                {
                    renderStack.Push(activity);
                    if (activity.RendersEntireSpace)
                    {
                        break;
                    }
                }

                var activityIndex = 0;

                foreach (IActivity activity in renderStack)
                {
                    if ((activity as GameplayActivity)?.Running ?? true)
                    {
                        SystemContainer.RendererSystem.Renderer.Render(SystemContainer, activity, activityIndex++);
                    }
                }

                IOSystem.Draw();
            }
        }

        private void OnRootConsoleUpdate(object sender, GameLoopEventArgs e)
        {
            if (!_leaving && !Loading)
            {
                var keyPress = IOSystem.GetKeyPress();

                SystemContainer.ControlSystem.HandleInput(keyPress, IOSystem.GetMouseData());

                if (!SystemContainer.AnimatedMovementSystem.IsBlockingAnimationPlaying())
                {
                    var throttle = 1000;
                    while (
                       !SystemContainer.TimeSystem.WaitingForInput
                       && SystemContainer.ActivitySystem.ActivityStack.Count > 0
                       && SystemContainer.ActivitySystem.GetActivityAcceptingInput().Type == ActivityType.Gameplay
                       && SystemContainer.PlayerSystem.Player != null
                       && !_leaving
                       && throttle-- > 0)
                    {
                        SystemContainer.TimeSystem.Tick();
                    }
                }
            }
        }

        public void DisplayLoadingScreen(string text)
        {
            SystemContainer.ActivitySystem.Push(new LoadingScreenActivity(SystemContainer.ActivitySystem, text));
        }

        public void Quit()
        {
            _leaving = true;
            IOSystem.Close();
        }

    }
}
