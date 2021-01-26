using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles.ControlRenderers
{
    public class BLTSkillBarDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(SkillBarControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            var playerSkills = systemContainer.SkillSystem.KnownSkills(systemContainer.PlayerSystem.Player);

            var skillsOnBar = playerSkills.Take(10);
            var previousSkills = 0;

            BLT.Font("");

            foreach (var skill in skillsOnBar)
            {
                RenderSpriteIfSpecified(x + previousSkills * 12, y, spriteManager, "skill_frame", AnimationFrame.Idle0);

                var appearance = skill.Get<SpriteAppearance>();

                var skillSpriteBottom = "default_skill_icon";
                var skillSpriteTop = "";

                if (appearance != null)
                {
                    skillSpriteBottom = appearance.Bottom;
                    skillSpriteTop = appearance.Top;
                }

                BLT.Layer(BLTLayers.UIElements);
                RenderSpriteIfSpecified(x + previousSkills * 12 + 2, y + 2, spriteManager, skillSpriteBottom, AnimationFrame.Idle0);
                BLT.Layer(BLTLayers.UIElementPieces);
                RenderSpriteIfSpecified(x + previousSkills * 12 + 2, y + 2, spriteManager, skillSpriteTop, AnimationFrame.Idle0);

                previousSkills++;
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(24 * 10, 24);
        }
    }
}
