using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            config.StatsConfigurations = StatsConfigurations;
            config.MessageConfigurations = MessageConfigurations;

            config.AdditionalControlRenderers = new List<IDataRogueControlRenderer> { new BLTDefencesDisplayer() };

            return new BLTTilesIOSystem(config);
        }

        public static List<StatsConfiguration> StatsConfigurations => new List<StatsConfiguration> {
                new StatsConfiguration
                {
                    Position = new Rectangle(2, 2, 40 * TILE_SPACING - 2, 27 * TILE_SPACING - 2),
                    Displays = new List<InfoDisplay>
                    {
                        new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.Red },
                        new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "AuraFighter,Aura", BackColor = Color.Gold },
                        new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "TiltFighter,Tilt", BackColor = Color.Purple },
                        new InfoDisplay { ControlType =  typeof(Spacer) },
                        new InfoDisplay { ControlType = typeof(DefencesControl) },
                        new InfoDisplay { ControlType =  typeof(HoveredEntityDisplayBox), Parameters = "Health,HP;AuraFighter,Aura;TiltFighter,Tilt" }
                    }
                },
                new StatsConfiguration
                {
                    Position = new Rectangle(40 * TILE_SPACING - 38, 25 * TILE_SPACING - 22, 32, 16),
                    Displays = new List<InfoDisplay>
                    {
                        new InfoDisplay { ControlType = typeof(InteractionControl) }
                    }

                },
                new StatsConfiguration
                {
                    Position = new Rectangle(0,25 * TILE_SPACING - 18, 24*10, 24),
                    Displays = new List<InfoDisplay>
                    {
                        new InfoDisplay { ControlType = typeof(SkillBarControl) }
                    }
                }
            };

        public static List<MessageConfiguration> MessageConfigurations => new List<MessageConfiguration> { new MessageConfiguration {
                Position = new Rectangle(2, (int)(13.5 * TILE_SPACING), 40 * TILE_SPACING, 10 * TILE_SPACING - 2),
                NumberOfMessages = 15} };
    }
}
