using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMapEditorCellPickerDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapEditorCellPickerControl);

        Rectangle PrimaryCell = new Rectangle(4, 9, 8, 8);
        Rectangle SecondaryCell = new Rectangle(16, 9, 8, 8);
        Rectangle DefaultCell = new Rectangle(28, 9, 8, 8);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as MapEditorCellPickerControl;

            RenderText(x + 2, y + 2, control.ActivityIndex, out _, "Selected Cells", Color.LightSkyBlue, false);

            RenderText(x + 4, y + 19, control.ActivityIndex, out _, "Left", Color.White, false);
            RenderText(x + 16, y + 19, control.ActivityIndex, out _, "Right", Color.White, false);
            RenderText(x + 28, y + 19, control.ActivityIndex, out _, "Back", Color.White, false);

            RenderBackgroundBox(x, y, control.ActivityIndex, LayoutInternal(spriteManager, control, systemContainer, playerFov), spriteManager);

            RenderEntitySprite(x + PrimaryCell.X, y + PrimaryCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.PrimaryCell);
            RenderEntitySprite(x + SecondaryCell.X, y + SecondaryCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.SecondaryCell);
            RenderEntitySprite(x + DefaultCell.X, y + DefaultCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.DefaultCell);

            RenderSpriteIfSpecified(x + PrimaryCell.X - 2, y + PrimaryCell.Y - 2, spriteManager, "skill_frame", AnimationFrame.Idle0);
            RenderSpriteIfSpecified(x + SecondaryCell.X - 2, y + SecondaryCell.Y - 2, spriteManager, "skill_frame", AnimationFrame.Idle0);
            RenderSpriteIfSpecified(x + DefaultCell.X - 2, y + DefaultCell.Y - 2,  spriteManager, "skill_frame", AnimationFrame.Idle0);
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(5 * BLTTilesIOSystem.TILE_SPACING, 3 * BLTTilesIOSystem.TILE_SPACING);
        }

        public override string StringFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            var adjustedPosition = new Point(mouse.X - display.Position.X, mouse.Y - display.Position.Y);

            if (PrimaryCell.Contains(adjustedPosition)) return "Primary Cell";

            if (SecondaryCell.Contains(adjustedPosition)) return "Secondary Cell";

            if (DefaultCell.Contains(adjustedPosition)) return "Default Cell";

            return null;
        }
    }
}