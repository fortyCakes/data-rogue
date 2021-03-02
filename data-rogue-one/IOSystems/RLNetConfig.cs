using data_rogue_core.Activities;
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
            config.GameplayWindowControls = config.GameplayWindowControls.Where(c => !(c is FlowContainerControl)).ToList();
            config.GameplayWindowControls.Add(
                new FlowContainerControl
                {
                    Position = new Rectangle(77, 0, 23, 70),
                    Controls = new List<IDataRogueControl>
                    {
                        new NameControl(),
                        new TitleControl(),
                        new Spacer(),
                        new ComponentCounter { Parameters = "Health,HP", BackColor = Color.DarkRed},
                        new Spacer(),
                        new ComponentCounter { Parameters = "AuraFighter,Aura", BackColor = Color.Yellow},
                        new StatControl { Parameters = "Tension" },
                        new Spacer(),
                        new ComponentCounter{ Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                        new Spacer(),
                        new StatControl { Parameters = "AC" },
                        new StatControl { Parameters = "EV" },
                        new StatControl { Parameters = "SH" },
                        new StatInterpolationControl { Parameters = "Aegis: {0}/{1},CurrentAegisLevel,Aegis", Color = Color.LightBlue },
                        new Spacer(),
                        new LocationControl(),
                        new TimeControl(),
                        new Spacer(),
                        new WealthControl { Parameters = "Gold", Color = Color.Gold},
                        new Spacer(),
                        new VisibleEnemiesControl()
                    }
                });

            var ioSystem = new RLNetConsoleIOSystem(config);
            return ioSystem;
        }
    }
}
