using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTComponentCounterDisplayer : BLTControlRenderer
    {
        public static int Height => 8;

        public override Type DisplayType => typeof(ComponentCounter);

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(80, Height);
        }

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = (control as IDataRogueInfoControl);
            var x = display.Position.X;
            var y = display.Position.Y;

            BLT.Font("");

            var color = display.BackColor;

            var totalLength = 80;

            Counter counter = GetCounter(display.Parameters, display.Entity, out string counterText);

            var percentage = (decimal)counter.Current / counter.Max;
            
            RenderBarContainer(x, y, spriteManager, totalLength, display);

            RenderBarFill(x, y, spriteManager, totalLength, percentage, display);

            RenderBarFillMask(x, y, spriteManager, totalLength, percentage, color, display);

            RenderText(x, y, counter, counterText, display);
        }

        private void RenderText(int x, int y, Counter counter, string counterText, IDataRogueControl display)
        {
            string text = $"[offset=0,2]{counterText}: {counter}";
            
            RenderText(x+2, y + 2, display.ActivityIndex, out _, text, display.Color, false);
        }

        private static void RenderBarFillMask(int x, int y, ISpriteManager spriteManager, int totalLength, decimal percentage, Color color, IDataRogueControl display)
        {
            BLTLayers.Set(BLTLayers.UIMasks, display.ActivityIndex);
            BLT.Color(color);
            var mask_main = spriteManager.Tile("bar_fill_mask");

            RenderBarContent(x, y, totalLength, percentage, mask_main, mask_main, mask_main, mask_main);
            BLT.Color("");
        }

        private static void RenderBarFill(int x, int y, ISpriteManager spriteManager, int totalLength, decimal percentage, IDataRogueControl display)
        {
            BLTLayers.Set(BLTLayers.UIElementPieces, display.ActivityIndex);
            var bar_left = spriteManager.Tile("bar_left");
            var bar_fill = spriteManager.Tile("bar_fill");
            var bar_full_right = spriteManager.Tile("bar_full_right");
            var bar_right = spriteManager.Tile("bar_right");

            RenderBarContent(x, y, totalLength, percentage, bar_left, bar_fill, bar_full_right, bar_right);
        }

        private static void RenderBarContainer(int x, int y, ISpriteManager spriteManager, int totalLength, IDataRogueControl display)
        {
            BLTLayers.Set(BLTLayers.UIElements, display.ActivityIndex);
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