using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Appearance = data_rogue_core.Components.Appearance;

namespace data_rogue_core.IOSystems
{

    public abstract class RLNetControlRenderer : IDataRogueControlRenderer
    {
        public static List<IDataRogueControlRenderer> DefaultStatsDisplayers => ReflectiveEnumerator.GetEnumerableOfType<RLNetControlRenderer>().OfType<IDataRogueControlRenderer>().ToList();

        public abstract Type DisplayType { get; }

        public void Paint(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var console = handle as RLConsole;
            DisplayInternal(console, display, systemContainer, playerFov);
        }

        public virtual IEntity EntityFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            return null;
        }

        public bool Layout(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            var console = handle as RLConsole;
            var size = GetSizeInternal(console, display, systemContainer, playerFov);

            display.Position = new Rectangle(boundingBox.Location, size);

            return false;
        }

        public string StringFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse)
        {
            return null;
        }

        protected abstract void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
        protected abstract Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        protected void PrintEntityDetails(IDataRogueInfoControl display, IEntity entity, RLConsole console, int plusY)
        {
            var line = display.Position.Y + plusY;

            var appearance = entity.Get<Appearance>();
            console.Print(display.Position.X, line, appearance.Glyph.ToString(), appearance.Color.ToRLColor(), display.BackColor.ToRLColor());
            if (entity.Has<Health>())
            {
                ConsoleRendererHelper.PrintBar(console, display.Position.X + 2, line, display.Position.Width - 4, nameof(Health.HP), entity.Get<Health>().HP, RLColor.Red);
            }
        }
    }
}
