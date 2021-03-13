using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;

namespace data_rogue_one.IOSystems
{
    public static class BLTTilesConfig
    {
        private const int TILE_SPACING = BLTTilesIOSystem.TILE_SPACING;

        public static IIOSystem GetBLTTilesIOSystem()
        {
            var config = BLTTilesIOSystem.DefaultConfiguration;
            config.WindowTitle = "Data Rogue One";

            MessageLogControl messageLog = BLTTilesIOSystem.DefaultMessageLog;
            messageLog.Margin = new Padding(2, 1, 1, 1);

            config.GameplayWindowControls = new List<IDataRogueControl>
            {
                BLTTilesIOSystem.DefaultMap,
                BLTTilesIOSystem.DefaultMinimap,
                StatsConfiguration,
                BLTTilesIOSystem.DefaultInteraction,
                BLTTilesIOSystem.DefaultMessageLog,
                new FlowContainerControl { Controls =
                    {
                        new SkillBarControl { Margin = new Padding(1) },
                        messageLog
                    },
                    FlowDirection = FlowDirection.BottomUp,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Padding(2)
                }
            };

            config.AdditionalControlRenderers = new List<IDataRogueControlRenderer> { new BLTDefencesDisplayer() };

            return new BLTTilesIOSystem(config);
        }

        public static FlowContainerControl StatsConfiguration =>
                new FlowContainerControl
                {
                    ShrinkToContents = true,
                    Margin = new Padding(2),
                    Controls = new List<IDataRogueControl>
                    {
                        new ComponentCounter { Parameters = "Health,HP", BackColor = Color.Red },
                        new ComponentCounter { Parameters = "AuraFighter,Aura", BackColor = Color.Gold },
                        new ComponentCounter { Parameters = "TiltFighter,Tilt", BackColor = Color.Purple },
                        new Spacer(),
                        new DefencesControl()
                        //new HoveredEntityDisplayBox { Parameters = "Health,HP;AuraFighter,Aura;TiltFighter,Tilt" }
                    }
                };           
                 

    }
}
