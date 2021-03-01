using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTMenuEntityRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MenuEntityControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;

            RenderEntitySprite(display, display.Entity, systemContainer, spriteManager);
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            return new Size(BLTTilesIOSystem.TILE_SPACING, BLTTilesIOSystem.TILE_SPACING);
        }

        protected static void RenderEntitySprite(IDataRogueInfoControl display, IEntity entity, ISystemContainer systemContainer, ISpriteManager spriteManager)
        {
            BLT.Font("");
            SpriteAppearance appearance = entity.Has<SpriteAppearance>() ? entity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
            AnimationFrame frame = entity.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(entity) : AnimationFrame.Idle0;

            BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
            string appearanceBottom = appearance.Bottom;
            RenderSpriteIfSpecified(display.Position.X, display.Position.Y, spriteManager, appearanceBottom, frame);

            BLTLayers.Set(BLTLayers.UIElementPieces + 1, display.ActivityIndex);
            string appearanceTop = appearance.Top;
            RenderSpriteIfSpecified(display.Position.X, display.Position.Y, spriteManager, appearanceTop, frame);
        }

        public override IEntity EntityFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            return (display as IDataRogueInfoControl).Entity;
        }
    }
}