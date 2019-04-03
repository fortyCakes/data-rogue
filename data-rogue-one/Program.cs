using System.Collections.Generic;
using System.Drawing;
using data_rogue_core;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;

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
                new StatsDisplay { DisplayType = DisplayType.Name },
                new StatsDisplay {DisplayType = DisplayType.Title},
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay { DisplayType = DisplayType.ComponentCounter, Parameters = "Health,HP", BackColor = Color.DarkRed},
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay { DisplayType = DisplayType.ComponentCounter, Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                new StatsDisplay {DisplayType = DisplayType.Stat, Parameters = "Tension" },
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay { DisplayType = DisplayType.ComponentCounter, Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay {DisplayType = DisplayType.Stat, Parameters = "AC" },
                new StatsDisplay {DisplayType = DisplayType.Stat, Parameters = "EV" },
                new StatsDisplay {DisplayType = DisplayType.Stat, Parameters = "SH" },
                new StatsDisplay {DisplayType = DisplayType.StatInterpolation, Parameters = "Aegis: {0}/{1},CurrentAegisLevel,Aegis", Color = Color.LightBlue },
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay {DisplayType = DisplayType.Location},
                new StatsDisplay { DisplayType = DisplayType.Time },
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay { DisplayType = DisplayType.Wealth, Parameters = "Gold", Color = Color.Gold},
                new StatsDisplay { DisplayType = DisplayType.Spacer},
                new StatsDisplay { DisplayType = DisplayType.VisibleEnemies}
            } } };


            var ioSystem = new RLNetConsoleIOSystem(config);

            theGame.Run("SEED_DEBUG", GameRules.Rules, ioSystem);
        }
    }
}
