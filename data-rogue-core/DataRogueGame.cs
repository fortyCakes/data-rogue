﻿using System;
using System.Collections.Generic;
using data_rogue_core.Renderers;
using RLNET;
using System.Threading;
using data_rogue_core.Activities;
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

        public void Run(string seed)
        {
            Seed = seed;

            SetupRootConsole();

            CreateAndRegisterSystems();

            InitialiseRenderers();

            InitialiseState();

            StartDataLoad();

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

        private void InitialiseRules()
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
                new CheckEnoughAuraToActivateSkillRule(SystemContainer),
                new ApplyStatBoostEnchantmentRule(SystemContainer),
                new ApplyProcEnchantmentRule(SystemContainer, EventType.Attack),
                new SpendAuraOnCompleteSkillRule(SystemContainer),
                new SpendTimeOnCompleteSkillRule(SystemContainer),
                new DoXpGainRule(),
                new XpGainMessageRule(SystemContainer),
                new LevelUpOnXPGainRule(SystemContainer),
                new GainSingleXPOnKillRule(SystemContainer),
                new ApplyEquipmentStatsRule(SystemContainer)
            );
        }


        private void InitialiseState()
        {
            SystemContainer.ActivitySystem.Initialise();

            SystemContainer.ActivitySystem.Push(new GameplayActivity(SystemContainer.RendererSystem.RendererFactory));

            DisplayLoadingScreen("Loading...");
        }
        
        private void StartDataLoad()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                InitialiseRules();

                SystemContainer.ActivitySystem.Pop();
                DisplayMainMenu();

            }).Start();
        }


        private void CreateAndRegisterSystems()
        {
            SystemContainer = new SystemContainer();

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
                RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
                RLMouse mouse = _rootConsole.Mouse;
                InputEventData eventData = new InputEventData(keyPress, mouse);

                SystemContainer.EventSystem.Try(EventType.Input, null, eventData);

                while (!SystemContainer.TimeSystem.WaitingForInput && SystemContainer.ActivitySystem.ActivityStack.Count > 0 && SystemContainer.ActivitySystem.Peek().Type == ActivityType.Gameplay)
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
