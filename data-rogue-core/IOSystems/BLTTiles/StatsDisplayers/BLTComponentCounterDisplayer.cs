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

            var totalLength = 100;

            var componentCounterSplits = display.Parameters.Split(',');
            var componentName = componentCounterSplits[0];
            var counterName = componentCounterSplits[1];

            var component = player.Get(componentName);
            FieldInfo[] fields = component.GetType().GetFields();
            var field = fields.Single(f => f.Name == counterName);
            var counter = (Counter)field.GetValue(component);

            var percentage = (decimal)counter.Current / counter.Max;

            //DisplayBarContainer
            BLT.Layer(BLTLayers.UIElements);
            BLT.Put(x, y, spriteManager.Tile("bar_container_left"));

            for (int i = 4; i < totalLength - 6; i += 2)
            {
                BLT.Put(x + i, y, spriteManager.Tile("bar_container_mid"));
            }

            BLT.Put(x + totalLength - 4 - 4, y, spriteManager.Tile("bar_container_right"));


            BLT.Layer(BLTLayers.UIElementPieces);
            var bar_left = spriteManager.Tile("bar_left");
            var bar_fill = spriteManager.Tile("bar_fill");
            var bar_full_right = spriteManager.Tile("bar_full_right");
            var bar_right = spriteManager.Tile("bar_right");

            RenderBarContent(x, y, totalLength, percentage, bar_left, bar_fill, bar_full_right, bar_right);

            BLT.Layer(BLTLayers.UIMasks);
            BLT.Color(color);
            var mask_main = spriteManager.Tile("bar_fill_mask");

            RenderBarContent(x, y, totalLength, percentage, mask_main, mask_main, mask_main, mask_main);
            BLT.Color("");

            y += 5;
        }

        private static void RenderBarContent(int x, int y, int totalLength, decimal percentage, int bar_left, int bar_fill, int bar_full_right, int bar_right)
        {
            if (percentage > 0)
            {
                BLT.Put(x + 1, y + 2, bar_left);

                var lengthToCover = percentage * totalLength - 6;

                for (int i = 3; i < lengthToCover-1; i += 2)
                {
                    BLT.Put(x + i, y + 2, bar_fill);
                }

                if (percentage == 1)
                {
                    BLT.Put(x + totalLength - 7, y + 2, bar_full_right);
                }
                else
                {
                    BLT.Put(x + (int)lengthToCover-1, y + 2, bar_right);
                }

            }
        }
    }
}