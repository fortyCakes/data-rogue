using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            config.GameplayWindowControls = new List<IDataRogueControl>
            {
                BLTTilesIOSystem.DefaultMap,
                BLTTilesIOSystem.DefaultMinimap,
                StatsConfiguration,
                SkillBar,
                BLTTilesIOSystem.DefaultInteraction,
                MessageConfiguration
            };

            config.AdditionalControlRenderers = new List<IDataRogueControlRenderer> { new BLTDefencesDisplayer() };

            return new BLTTilesIOSystem(config);
        }

        public static FlowContainerControl StatsConfiguration =>
                new FlowContainerControl
                {
                    Position = new Rectangle(2, 2, 40 * TILE_SPACING - 2, 27 * TILE_SPACING - 2),
                    Controls = new List<IDataRogueControl>
                    {
                        new ComponentCounter { Parameters = "Health,HP", BackColor = Color.Red },
                        new ComponentCounter { Parameters = "AuraFighter,Aura", BackColor = Color.Gold },
                        new ComponentCounter { Parameters = "TiltFighter,Tilt", BackColor = Color.Purple },
                        new Spacer(),
                        new DefencesControl(),
                        new HoveredEntityDisplayBox { Parameters = "Health,HP;AuraFighter,Aura;TiltFighter,Tilt" }
                    }
                };
        public static FlowContainerControl SkillBar =>
                new FlowContainerControl
                {
                    Position = new Rectangle(0, 25 * TILE_SPACING - 18, 24 * 10, 24),
                    Controls = new List<IDataRogueControl>
                    {
                        new SkillBarControl()
                    }
                };

        public static MessageLogControl MessageConfiguration => new MessageLogControl {
                Position = new Rectangle(2, (int)(13.5 * TILE_SPACING), 40 * TILE_SPACING, 10 * TILE_SPACING - 2),
                NumberOfMessages = 15} ;
    }
}
