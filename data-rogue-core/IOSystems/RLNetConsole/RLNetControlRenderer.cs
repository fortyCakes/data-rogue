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

namespace data_rogue_core.IOSystems
{

    public abstract class RLNetControlRenderer : IDataRogueControlRenderer
    {
        public static List<RLNetControlRenderer> DefaultStatsDisplayers => new List<RLNetControlRenderer>
        {
            new RLNetNameDisplayer(),
            new RLNetTitleDisplayer(),
            new RLNetTimeDisplayer(),
            new RLNetLocationDisplayer(),
            new RLNetHoveredEntityDisplayer(),
            new RLNetSpacerDisplayer(),
            new RLNetStatDisplayer(),
            new RLNetStatInterpolationDisplayer(),
            new RLNetVisibleEnemiesDisplayer(),
            new RLNetWealthDisplayer(),
            new RLNetComponentCounterDisplayer(),
            new RLNetDescriptionDisplayer(),
            new RLNetAppearanceNameDisplayer(),
            new RLNetExperienceDisplayer()
        };

        public abstract Type DisplayType { get; }

        public void Display(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var console = handle as RLConsole;
            DisplayInternal(console, display, systemContainer, playerFov);
        }

        public Size GetSize(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var console = handle as RLConsole;
            return GetSizeInternal(console, display, systemContainer, playerFov);
        }

        protected abstract void DisplayInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
        protected abstract Size GetSizeInternal(RLConsole console, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        protected void PrintEntityDetails(IDataRogueInfoControl display, IEntity entity, RLConsole console)
        {
            var line = display.Position.Y;

            var appearance = entity.Get<Appearance>();
            console.Print(1, line, appearance.Glyph.ToString(), appearance.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(3, line, entity.DescriptionName, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            if (entity.Has<Health>())
            {
                ConsoleRendererHelper.PrintBar(console, 1, line, console.Width - 2, nameof(Health.HP), entity.Get<Health>().HP, RLColor.Red);
            }
        }
    }
}
