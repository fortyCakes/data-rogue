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
using data_rogue_core.Utils;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTHoveredEntityDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(HoveredEntityDisplayBox);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            var hoveredCoordinate = systemContainer.ControlSystem.HoveredCoordinate;

            if (hoveredCoordinate != null && playerFov.Contains(hoveredCoordinate))
            {
                var entities = systemContainer.PositionSystem.EntitiesAt(hoveredCoordinate);

                var hoveredEntity = entities.Where(e => e.Has<Appearance>()).OrderByDescending(e => e.Get<Appearance>().ZOrder).First();

                RenderEntityDetails(x, y, display, hoveredEntity, systemContainer, spriteManager);

                RenderBackgroundBox(x, y, control.ActivityIndex, LayoutInternal(spriteManager, control, systemContainer, playerFov), spriteManager);

            }
        }

        protected override Size LayoutInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var height = 12;
            var display = control as IDataRogueInfoControl;

            foreach (var split in display.Parameters.Split(';'))
            {
                height += BLTComponentCounterDisplayer.Height;
            }

            height = BLTTilesIOSystem.TILE_SPACING * (int)Math.Ceiling((decimal)height / BLTTilesIOSystem.TILE_SPACING);

            return new Size(10 * BLTTilesIOSystem.TILE_SPACING, height);
        }

        protected static void RenderEntityDetails(int x, int y, IDataRogueInfoControl display, IEntity hoveredEntity, ISystemContainer systemContainer, ISpriteManager spriteManager)
        {
            BLT.Font("");
            SpriteAppearance appearance = hoveredEntity.Has<SpriteAppearance>() ? hoveredEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
            AnimationFrame frame = hoveredEntity.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(hoveredEntity) : AnimationFrame.Idle0;

            BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(x + 2, y + 3, spriteManager, appearanceBottom, frame);

            BLTLayers.Set(BLTLayers.UIElementPieces + 1, display.ActivityIndex);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(x + 2, y + 3, spriteManager, appearanceTop, frame);

            BLTLayers.Set(BLTLayers.Text, display.ActivityIndex);
            BLT.Font("text");
            BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 4, y - 1 + BLTTilesIOSystem.TILE_SPACING / 2, hoveredEntity.GetBLTName());

            y += 12;

            foreach (var split in display.Parameters.Split(';'))
            {
                var counter = GetCounter(split, hoveredEntity, out string counterText);
                if (counter != null)
                {
                    var text = $"{counterText}: {counter}";
                    RenderText(x + 4, y, display.ActivityIndex, out var textSize, text, display.Color);
                    y += 3;
                }
            }

            BLT.Font("text");
            BLT.Print(x + 4, y, hoveredEntity.Get<Description>().Detail);
        }
    }
}