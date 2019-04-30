using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_one
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var theGame = new DataRogueGame();

            IIOSystem ioSystem = 
                //GetRLNetIOSystem();
                GetBLTTilesIOSystem();

            var additionalComponents = typeof(Program).Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IEntityComponent))).ToList();

            theGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem, null, additionalComponents);
        }

        private static BLTTilesIOSystem GetBLTTilesIOSystem()
        {
            return new BLTTilesIOSystem(BLTTilesIOSystem.DefaultConfiguration);
        }

        private static RLNetConsoleIOSystem GetRLNetIOSystem()
        {
            var config = RLNetConsoleIOSystem.DefaultConfiguration;

            config.WindowTitle = "data-rogue-one";
            config.StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(77, 0, 23, 70), Displays = new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(NameControl) },
                new InfoDisplay { ControlType = typeof(TitleControl)},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.DarkRed},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                new InfoDisplay { ControlType = typeof(StatControl), Parameters = "Tension" },
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(StatControl), Parameters = "AC" },
                new InfoDisplay { ControlType = typeof(StatControl), Parameters = "EV" },
                new InfoDisplay { ControlType = typeof(StatControl), Parameters = "SH" },
                new InfoDisplay { ControlType = typeof(StatInterpolationControl), Parameters = "Aegis: {0}/{1},CurrentAegisLevel,Aegis", Color = Color.LightBlue },
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(LocationControl)},
                new InfoDisplay { ControlType = typeof(TimeControl) },
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(WealthControl), Parameters = "Gold", Color = Color.Gold},
                new InfoDisplay { ControlType = typeof(Spacer)},
                new InfoDisplay { ControlType = typeof(VisibleEnemiesControl)}
            } } };

            var ioSystem = new RLNetConsoleIOSystem(config);
            return ioSystem;
        }
    }
}
