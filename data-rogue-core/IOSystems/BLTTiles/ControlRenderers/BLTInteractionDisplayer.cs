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
                    .SelectMany(e => e.Components.OfType<Interactable>()
                    .Select(i => new { Entity = e, Interactable = i }));

                (IEntity, Interactable) currentInteraction = systemContainer.InteractableSystem.GetCurrentInteractionFor(systemContainer.PlayerSystem.Player);

                var interactEntity = currentInteraction.Item1;
                var interactable = currentInteraction.Item2;

                BLT.Font("");
                SpriteAppearance appearance = interactEntity.Has<SpriteAppearance>() ? interactEntity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
                AnimationFrame frame = interactEntity.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(interactEntity) : AnimationFrame.Idle0;

                BLT.Layer(BLTLayers.UIElementPieces);
                string appearanceBottom = appearance.Bottom;
                RenderSpriteIfSpecified(x, y, spriteManager, appearanceBottom, frame);

                BLT.Layer(BLTLayers.UIElementPieces + 1);
                string appearanceTop = appearance.Top;
                RenderSpriteIfSpecified(x, y, spriteManager, appearanceTop, frame);

                BLT.Layer(BLTLayers.Text);
                BLT.Font("text");
                BLT.Color(Color.LightBlue);
                BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 6, y + BLTTilesIOSystem.TILE_SPACING / 2, interactable.Verb);
                BLT.Color(Color.White);
                BLT.Print(x + BLTTilesIOSystem.TILE_SPACING + 6, y + 4 + BLTTilesIOSystem.TILE_SPACING / 2, interactEntity.DescriptionName);

                RenderBackgroundBox(x, y, GetSizeInternal(spriteManager, control, systemContainer, playerFov), spriteManager);
            }
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(control.Position.Width, 16);
        }

        private static void RenderBackgroundBox(int x, int y, Size size, ISpriteManager spriteManager)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Font("");
            var width = size.Width / BLTTilesIOSystem.TILE_SPACING;
            var height = size.Height / BLTTilesIOSystem.TILE_SPACING;

            var spriteSheet = spriteManager.Get("textbox_blue");

            for (int xCoord = 0; xCoord < width; xCoord++)
            {
                for (int yCoord = 0; yCoord < height; yCoord++)
                {
                    BLT.Put(x + xCoord * BLTTilesIOSystem.TILE_SPACING, y + yCoord * BLTTilesIOSystem.TILE_SPACING, spriteSheet.Tile(BLTTileDirectionHelper.GetDirections(xCoord, width, yCoord, height)));
                }
            }
        }

        private static void RenderSpriteIfSpecified(int x, int y, ISpriteManager spriteManager, string spriteName, AnimationFrame frame)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                BLT.Put(x + 4, y + 4, spriteManager.Tile(spriteName, frame));
            }
        }
    }
}