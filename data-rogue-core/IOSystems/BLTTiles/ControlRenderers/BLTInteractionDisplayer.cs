using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class BLTInteractionDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(InteractionControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            List<IEntity> interactables = systemContainer.InteractableSystem.GetInteractablesNear(systemContainer.PlayerSystem.Player);

            if (interactables.Any())
            {
                var possibleInteractions = interactables
                    .SelectMany(e => e.Components.OfType<Interaction>()
                    .Select(i => new { Entity = e, Interactable = i }));

                (IEntity, Interaction) currentInteraction = systemContainer.InteractableSystem.GetCurrentInteractionFor(systemContainer.PlayerSystem.Player);

                var interactEntity = currentInteraction.Item1;
                var interactable = currentInteraction.Item2;

                BLT.Font("");
                SpriteAppearance appearance = interactEntity.Has<SpriteAppearance>() ? interactEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
                AnimationFrame frame = interactEntity.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(interactEntity) : AnimationFrame.Idle0;

                BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
                string appearanceBottom = appearance.Bottom;
                RenderSpriteIfSpecified(x + 4, y + 4, spriteManager, appearanceBottom, frame);

                BLTLayers.Set(BLTLayers.UIElementPieces + 1, display.ActivityIndex);
                string appearanceTop = appearance.Top;
                RenderSpriteIfSpecified(x + 4, y + 4, spriteManager, appearanceTop, frame);

                BLTLayers.Set(BLTLayers.Text, display.ActivityIndex);
                BLT.Font("text");
                BLT.Color(Color.LightBlue);
                BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 6, y + BLTTilesIOSystem.TILE_SPACING / 2, interactable.Verb);
                BLT.Color(Color.White);
                BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 6, y + 4 + BLTTilesIOSystem.TILE_SPACING / 2, interactEntity.DescriptionName);

                RenderBackgroundBox(x, y, display.ActivityIndex, control.Position.Size, spriteManager);
            }
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            return new Size(control.Position.Width, 16);
        }
    }
}