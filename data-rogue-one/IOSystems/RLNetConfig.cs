using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.RLNetConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one.IOSystems
{
    public static class RLNetConfig
    {
        public static RLNetConsoleIOSystem GetRLNetIOSystem()
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
