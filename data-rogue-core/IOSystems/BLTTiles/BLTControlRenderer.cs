﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System;
using System.Drawing;
using BearLib;
using data_rogue_core.Data;
using System.Reflection;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Utils;
using data_rogue_core.Components;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public abstract class BLTControlRenderer : IDataRogueControlRenderer
    {
        public static List<IDataRogueControlRenderer> DefaultControlRenderers => ReflectiveEnumerator.GetEnumerableOfType<BLTControlRenderer>().OfType<IDataRogueControlRenderer>().ToList();

        public abstract Type DisplayType { get; }

        public void Paint(object handle, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            DisplayInternal((ISpriteManager)handle, control, systemContainer, playerFov);
        }

        public bool Layout(object handle, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var paddedBbox = boundingBox.PadIn(control.Margin);

            var size = Measure((ISpriteManager)handle, control, systemContainer, playerFov, paddedBbox, padding, horizontalAlignment, verticalAlignment);
            
            control.Position = new Rectangle(paddedBbox.Location, size);

            ApplyHorizontalAlignment(horizontalAlignment, control, paddedBbox, size);
            ApplyVerticalAlignment(verticalAlignment, control, paddedBbox, size);

            return false;
        }

        private void ApplyHorizontalAlignment(HorizontalAlignment horizontalAlignment, IDataRogueControl control, Rectangle paddedBbox, Size size)
        {
            var width = size.Width;
            var boxWidth = paddedBbox.Width;
            var newX = control.Position.X;

            switch(horizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    break;
                case HorizontalAlignment.Center:
                    newX = paddedBbox.Left + boxWidth / 2 - width / 2;
                    break;
                case HorizontalAlignment.Right:
                    newX = paddedBbox.Right - width;
                    break;
            }

            control.Position = new Rectangle(newX, control.Position.Y, size.Width, size.Height);
        }
        private void ApplyVerticalAlignment(VerticalAlignment verticalAlignment, IDataRogueControl control, Rectangle paddedBbox, Size size)
        {
            var height = size.Height;
            var boxHeight = paddedBbox.Height;
            var newY = control.Position.Y;

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    break;
                case VerticalAlignment.Center:
                    newY = paddedBbox.Top + boxHeight / 2 - height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    newY = paddedBbox.Bottom - height;
                    break;
            }

            control.Position = new Rectangle(control.Position.X, newY, size.Width, size.Height);
        }

        protected abstract void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
        protected abstract Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment);
        protected static void RenderText(int x, int y, int activityIndex, out Size textSize, string text, Color color, bool updateY = true, string font = "text")
        {
            BLTLayers.Set(BLTLayers.Text, activityIndex);
            BLT.Font(font);
            BLT.Color(color);
            BLT.Print(x, y, text);
            textSize = BLT.Measure(text);
            textSize.Height += 1;

            BLT.Color("");
        }

        protected static void RenderBackgroundBox(int x, int y, int activityIndex, Size size, ISpriteManager spriteManager, string spriteName = "textbox_blue")
        {
            BLTLayers.Set(BLTLayers.UIElements, activityIndex);
            BLT.Font("");
            var width = size.Width / BLTTilesIOSystem.TILE_SPACING;
            var height = size.Height / BLTTilesIOSystem.TILE_SPACING;

            var spriteSheet = spriteManager.Get(spriteName);

            for (int xCoord = 0; xCoord < width; xCoord++)
            {
                for (int yCoord = 0; yCoord < height; yCoord++)
                {
                    BLT.Put(x + xCoord * BLTTilesIOSystem.TILE_SPACING, y + yCoord * BLTTilesIOSystem.TILE_SPACING, spriteSheet.Tile(BLTTileDirectionHelper.GetDirections(xCoord, width, yCoord, height)));
                }
            }
        }

        protected static void RenderEntitySprite(int x, int y, IDataRogueControl display, ISystemContainer systemContainer, ISpriteManager spriteManager, IEntity entity)
        {
            if (entity != null)
            {
                BLT.Font("");
                SpriteAppearance appearance = entity.Has<SpriteAppearance>() ? entity.Get<SpriteAppearance>() : new SpriteAppearance { Bottom = "unknown" };
                AnimationFrame frame = entity.Has<Animated>() ? systemContainer.AnimationSystem.GetFrame(entity) : AnimationFrame.Idle0;
                BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
                string appearanceBottom = appearance.Bottom;
                RenderSpriteIfSpecified(x, y, spriteManager, appearanceBottom, frame);
                BLTLayers.Set(BLTLayers.UIElementPieces + 1, display.ActivityIndex);
                string appearanceTop = appearance.Top;
                RenderSpriteIfSpecified(x, y, spriteManager, appearanceTop, frame);
            }
        }

        protected static void RenderSpriteIfSpecified(int x, int y, ISpriteManager spriteManager, string spriteName, AnimationFrame frame)
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                BLT.Put(x, y, spriteManager.Tile(spriteName, frame));
            }
        }

        protected static Counter GetCounter(string parameters, IEntity entity, out string counterText)
        {
            var componentCounterSplits = parameters.Split(',');
            var componentName = componentCounterSplits[0];
            var counterName = componentCounterSplits[1];
            counterText = counterName;

            if (!entity.Has(componentName))
            {
                return null;
            }
            var component = entity.Get(componentName);

            FieldInfo[] fields = component.GetType().GetFields();
            var field = fields.Single(f => f.Name == counterName);
            return (Counter)field.GetValue(component);
        }

        public virtual IEntity EntityFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            return null;
        }

        public virtual string StringFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            return null;
        }
    }
}