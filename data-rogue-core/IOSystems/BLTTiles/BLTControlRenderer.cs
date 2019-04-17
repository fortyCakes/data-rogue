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

namespace data_rogue_core.IOSystems.BLTTiles
{
    public abstract class BLTControlRenderer : IDataRogueControlRenderer
    {
        public static List<IDataRogueControlRenderer> DefaultControlRenderers => new List<IDataRogueControlRenderer>
        {
            new BLTNameDisplayer(),
            new BLTTitleDisplayer(),
            new BLTTimeDisplayer(),
            new BLTLocationDisplayer(),
            new BLTHoveredEntityDisplayer(),
            new BLTSpacerDisplayer(),
            new BLTStatDisplayer(),
            new BLTStatInterpolationDisplayer(),
            new BLTWealthDisplayer(),
            new BLTComponentCounterDisplayer(),
            new BLTDescriptionDisplayer(),
            new BLTLargeTextDisplayer(),
            new BLTDescriptionDisplayer(),
            new BLTAppearanceNameDisplayer(),
            new BLTExperienceDisplayer()
        };

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

        protected static void RenderText(int x, int y, out Size textSize, string text, Color color, bool updateY = true, string font = "text")
        {
            BLT.Layer(BLTLayers.Text);
            BLT.Font(font);
            BLT.Color(color);
            BLT.Print(x, y, text);
            textSize = BLT.Measure(text);
            textSize.Height += 1;

            BLT.Color("");
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
    }
}