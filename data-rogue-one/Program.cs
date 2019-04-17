using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core;
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

            //RLNetConsoleIOSystem ioSystem = GetRLNetIOSystem();

            BLTTilesIOSystem ioSystem = new BLTTilesIOSystem(BLTTilesIOSystem.DefaultConfiguration);

            var additionalComponents = typeof(Program).Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IEntityComponent))).ToList();

            theGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem, null, additionalComponents);
        }

        private static RLNetConsoleIOSystem GetRLNetIOSystem()
        {
            var config = RLNetConsoleIOSystem.DefaultConfiguration;

            config.WindowTitle = "data-rogue-one";
            config.StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(77, 0, 23, 70), Displays = new List<InfoDisplay> {
                new InfoDisplay { ControlType = "Name" },
                new InfoDisplay { ControlType =  "Title"},
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType = "ComponentCounter", Parameters = "Health,HP", BackColor = Color.DarkRed},
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType = "ComponentCounter", Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                new InfoDisplay { ControlType =  "Stat", Parameters = "Tension" },
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType = "ComponentCounter", Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType =  "Stat", Parameters = "AC" },
                new InfoDisplay { ControlType =  "Stat", Parameters = "EV" },
                new InfoDisplay { ControlType =  "Stat", Parameters = "SH" },
                new InfoDisplay { ControlType =  "StatInterpolation", Parameters = "Aegis: {0}/{1},CurrentAegisLevel,Aegis", Color = Color.LightBlue },
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType =  "Location"},
                new InfoDisplay { ControlType = "Time" },
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType = "Wealth", Parameters = "Gold", Color = Color.Gold},
                new InfoDisplay { ControlType = "Spacer"},
                new InfoDisplay { ControlType = "VisibleEnemies"}
            } } };

            var ioSystem = new RLNetConsoleIOSystem(config);
            return ioSystem;
        }
    }
}
