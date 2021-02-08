using data_rogue_core.EntityEngineSystem;
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

namespace data_rogue_core.IOSystems.BLTTiles
{
    public abstract class BLTControlRenderer : IDataRogueControlRenderer
    {
        public static List<IDataRogueControlRenderer> DefaultControlRenderers => ReflectiveEnumerator.GetEnumerableOfType<BLTControlRenderer>().OfType<IDataRogueControlRenderer>().ToList();

        public abstract Type DisplayType { get; }

        public void Display(object handle, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            DisplayInternal((ISpriteManager)handle, control, systemContainer, playerFov);
        }

        public Size GetSize(object handle, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return GetSizeInternal((ISpriteManager)handle, control, systemContainer, playerFov);
        }

        protected abstract void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
        protected abstract Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

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

        protected static void RenderBackgroundBox(int x, int y, int activityIndex, Size size, ISpriteManager spriteManager)
        {
            BLTLayers.Set(BLTLayers.UIElements, activityIndex);
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