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
    public class BLTMapEditorSelectedCellDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MapEditorSelectedCellControl);

        Rectangle PrimaryCell = new Rectangle(5, 4, 8, 8);
        Rectangle SecondaryCell = new Rectangle(19, 4, 8, 8);
        Rectangle DefaultCell = new Rectangle(12, 18, 8, 8);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as MapEditorSelectedCellControl;

            RenderBackgroundBox(x, y, control.ActivityIndex, GetSizeInternal(spriteManager, control, systemContainer, playerFov), spriteManager);

            RenderEntitySprite(x + PrimaryCell.X, y + PrimaryCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.PrimaryCell);
            RenderEntitySprite(x + SecondaryCell.X, y + SecondaryCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.SecondaryCell);
            RenderEntitySprite(x + DefaultCell.X, y + DefaultCell.Y, control, systemContainer, spriteManager, systemContainer.ActivitySystem.MapEditorActivity.DefaultCell);

            RenderSpriteIfSpecified(x + PrimaryCell.X - 2, y + PrimaryCell.Y - 2, spriteManager, "skill_frame", AnimationFrame.Idle0);
            RenderSpriteIfSpecified(x + SecondaryCell.X - 2, y + SecondaryCell.Y - 2, spriteManager, "skill_frame", AnimationFrame.Idle0);
            RenderSpriteIfSpecified(x + DefaultCell.X - 2, y + DefaultCell.Y - 2,  spriteManager, "skill_frame", AnimationFrame.Idle0);
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(4 * BLTTilesIOSystem.TILE_SPACING, 4 * BLTTilesIOSystem.TILE_SPACING);
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