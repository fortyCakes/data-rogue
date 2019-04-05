using System.Collections.Generic;
using System.Drawing;
using data_rogue_core;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.Renderers.ConsoleRenderers;

namespace data_rogue_one
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var theGame = new DataRogueGame();

            var config = RLNetConsoleIOSystem.DefaultConfiguration;

            config.WindowTitle = "data-rogue-one";
            config.StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(77, 0, 23, 70), Displays = new List<StatsDisplay> {
                new StatsDisplay { DisplayType = "Name" },
                new StatsDisplay {DisplayType =  "Title"},
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "Health,HP", BackColor = Color.DarkRed},
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                new StatsDisplay {DisplayType =  "Stat", Parameters = "Tension" },
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "ComponentCounter", Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay {DisplayType =  "Stat", Parameters = "AC" },
                new StatsDisplay {DisplayType =  "Stat", Parameters = "EV" },
                new StatsDisplay {DisplayType =  "Stat", Parameters = "SH" },
                new StatsDisplay {DisplayType =  "StatInterpolation", Parameters = "Aegis: {0}/{1},CurrentAegisLevel,Aegis", Color = Color.LightBlue },
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay {DisplayType =  "Location"},
                new StatsDisplay { DisplayType = "Time" },
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "Wealth", Parameters = "Gold", Color = Color.Gold},
                new StatsDisplay { DisplayType = "Spacer"},
                new StatsDisplay { DisplayType = "VisibleEnemies"}
            } } };


            var ioSystem = new RLNetConsoleIOSystem(config);

            theGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem);
        }
    }
}
