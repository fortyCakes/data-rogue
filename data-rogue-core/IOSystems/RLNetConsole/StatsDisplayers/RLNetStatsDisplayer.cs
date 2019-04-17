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

    public abstract class RLNetStatsRendererHelper : IDataRogueControlRenderer
    {
        public static List<RLNetStatsRendererHelper> DefaultStatsDisplayers => new List<RLNetStatsRendererHelper>
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

        public abstract string DisplayType { get; }

        Type IDataRogueControlRenderer.DisplayType => throw new NotImplementedException();

        public void Display(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var console = handle as RLConsole;
            throw new NotImplementedException();
            //DisplayInternal(console, display, systemContainer, player, playerFov, ref line);
        }

        public Size GetSize(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            throw new NotImplementedException();
        }

        protected abstract void DisplayInternal(RLConsole console, InfoDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line);

        protected void PrintEntityDetails(InfoDisplay display, IEntity entity, RLConsole console, ref int line)
        {
            var appearance = entity.Get<Appearance>();
            console.Print(1, line, appearance.Glyph.ToString(), appearance.Color.ToRLColor(), display.BackColor.ToRLColor());
            console.Print(3, line, entity.DescriptionName, display.Color.ToRLColor(), display.BackColor.ToRLColor());
            line++;
            if (entity.Has<Health>())
            {
                ConsoleRendererHelper.PrintBar(console, 1, line, console.Width - 2, nameof(Health.HP), entity.Get<Health>().HP, RLColor.Red);
                line++;
            }
            line++;
        }
    }
}
