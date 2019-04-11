using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System;
using System.Drawing;
using BearLib;

namespace data_rogue_core.IOSystems.BLTTiles
{

    public abstract class BLTStatsRendererHelper : IStatsRendererHelper
    {
        public static List<BLTStatsRendererHelper> DefaultStatsDisplayers => new List<BLTStatsRendererHelper>
        {
            new BLTNameDisplayer(),
            new BLTTitleDisplayer(),
            new BLTTimeDisplayer(),
            new BLTLocationDisplayer(),
            new BLTHoveredEntityDisplayer(),
            new BLTSpacerDisplayer(),
            new BLTStatDisplayer(),
            new BLTStatInterpolationDisplayer(),
            new BLTVisibleEnemiesDisplayer(),
            new BLTWealthDisplayer(),
            new BLTComponentCounterDisplayer()
        };

        public abstract string DisplayType { get; }

        public void Display(object handle, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            (int, ISpriteManager)split = ((int, ISpriteManager))handle;

            var x = split.Item1;
            var spriteManager = split.Item2;
            DisplayInternal(x, spriteManager, display, systemContainer, player, playerFov, ref line);
        }

        protected abstract void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int y);
        

        protected static void RenderText(int x, ref int y, string text, Color color, bool updateY = true)
        {
            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");
            BLT.Color(color);
            BLT.Print(x, y, text);
            if (updateY)
            {
                var size = BLT.Measure(text);
                y += size.Height + 1;
            }

            BLT.Color("");
        }
    }
}