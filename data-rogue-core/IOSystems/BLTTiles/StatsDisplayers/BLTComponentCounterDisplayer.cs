using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using BearLib;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTComponentCounterDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "ComponentCounter";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int y)
        {
            BLT.Font("");

            var color = display.BackColor;

            var totalLength = 80;

            Counter counter = GetCounter(display.Parameters, player, out string counterText);

            var percentage = (decimal)counter.Current / counter.Max;
            
            RenderBarContainer(x, y, spriteManager, totalLength);

            RenderBarFill(x, y, spriteManager, totalLength, percentage);

            RenderBarFillMask(x, y, spriteManager, totalLength, percentage, color);

            RenderText(x, ref y, counter, counterText, display);

            y += 8;
        }

        private void RenderText(int x, ref int y, Counter counter, string counterText, StatsDisplay display)
        {
            string text = $"[offset=0,2]{counterText}: {counter}";

            var yCoord = y + 2;
            RenderText(x+2, ref yCoord, text, display.Color, false);
        }

        private static void RenderBarFillMask(int x, int y, ISpriteManager spriteManager, int totalLength, decimal percentage, Color color)
        {
            BLT.Layer(BLTLayers.UIMasks);
            BLT.Color(color);
            var mask_main = spriteManager.Tile("bar_fill_mask");

            RenderBarContent(x, y, totalLength, percentage, mask_main, mask_main, mask_main, mask_main);
            BLT.Color("");
        }

        private static void RenderBarFill(int x, int y, ISpriteManager spriteManager, int totalLength, decimal percentage)
        {
            BLT.Layer(BLTLayers.UIElementPieces);
            var bar_left = spriteManager.Tile("bar_left");
            var bar_fill = spriteManager.Tile("bar_fill");
            var bar_full_right = spriteManager.Tile("bar_full_right");
            var bar_right = spriteManager.Tile("bar_right");

            RenderBarContent(x, y, totalLength, percentage, bar_left, bar_fill, bar_full_right, bar_right);
        }

        private static void RenderBarContainer(int x, int y, ISpriteManager spriteManager, int totalLength)
        {
            BLT.Layer(BLTLayers.UIElements);
            BLT.Put(x, y, spriteManager.Tile("bar_container_left"));

            for (int i = 4; i < totalLength - 6; i += 2)
            {
                BLT.Put(x + i, y, spriteManager.Tile("bar_container_mid"));
            }

            BLT.Put(x + totalLength - 4 - 4, y, spriteManager.Tile("bar_container_right"));
        }

        private static void RenderBarContent(int x, int y, int totalLength, decimal percentage, int bar_left, int bar_fill, int bar_full_right, int bar_right)
        {
            if (percentage > 0)
            {
                var lengthToCover = percentage * totalLength - 6;

                if (lengthToCover - 1 < 0)
                {
                    BLT.Put(x + 1, y + 2, bar_right);
                }
                else
                {
                    BLT.Put(x + 1, y + 2, bar_left);


                    for (int i = 3; i < lengthToCover - 1; i += 2)
                    {
                        BLT.Put(x + i, y + 2, bar_fill);
                    }

                    if (percentage == 1)
                    {
                        BLT.Put(x + totalLength - 7, y + 2, bar_full_right);
                    }
                    else
                    {
                        BLT.Put(x + (int)lengthToCover - 1, y + 2, bar_right);
                    }
                }
            }
        }
    }
}