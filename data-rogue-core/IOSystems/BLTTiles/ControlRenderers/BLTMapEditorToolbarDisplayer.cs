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
    public class BLTMapEditorToolbarDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapEditorToolbarControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            var toolsOnBar = MapEditorToolbarControl.GetToolbarControls();

            var previousSkills = 0;

            BLT.Font("");

            foreach (var tool in toolsOnBar)
            {
                RenderSpriteIfSpecified(x + previousSkills * 12, y, spriteManager, "skill_frame", AnimationFrame.Idle0);

                var appearance = tool.Entity.Get<SpriteAppearance>();

                var skillSpriteBottom = "default_skill_icon";
                var skillSpriteTop = "";

                if (appearance != null)
                {
                    skillSpriteBottom = appearance.Bottom;
                    skillSpriteTop = appearance.Top;
                }

                BLTLayers.Set(BLTLayers.UIElements, control.ActivityIndex);
                RenderSpriteIfSpecified(x + previousSkills * 12 + 2, y + 2, spriteManager, skillSpriteBottom, AnimationFrame.Idle0);
                BLTLayers.Set(BLTLayers.UIElementPieces, control.ActivityIndex);
                RenderSpriteIfSpecified(x + previousSkills * 12 + 2, y + 2, spriteManager, skillSpriteTop, AnimationFrame.Idle0);

                previousSkills++;
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(24 * 10, 24);
        }

        public override IEntity EntityFromMouseData(IDataRogueControl control, ISystemContainer systemContainer, MouseData mouse)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;
            var toolsOnBar = MapEditorToolbarControl.GetToolbarControls().ToList();

            var relativeX = mouse.X - x;
            var relativeY = mouse.Y - y;

            if (relativeY <= 1 || relativeY >= 10)
            {
                return null;
            }

            var xFromLeftOfFirstFrame = relativeX - 2;
            var xIndex = xFromLeftOfFirstFrame / 12;
            var xFromLeftOfCurrentFrame = xFromLeftOfFirstFrame - xIndex * 12;

            if (IsInsideFrame(xFromLeftOfCurrentFrame))
            {
                return null;
            }

            if (xIndex >= 0 && xIndex < toolsOnBar.Count())
            {
                return toolsOnBar[xIndex].Entity;
            }

            return null;
        }

        private static bool IsInsideFrame(int xFromLeftOfCurrentFrame)
        {
            return xFromLeftOfCurrentFrame < 0 || xFromLeftOfCurrentFrame >= 8;
        }
    }
}
