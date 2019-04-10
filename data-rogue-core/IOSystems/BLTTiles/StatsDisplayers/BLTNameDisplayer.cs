using System.Collections.Generic;
using BearLib;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTNameDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "Name";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity player, List<MapCoordinate> playerFov, ref int line)
        {
            BLT.Layer(BLTLayers.Text);
            BLT.Font("text");
            var text = $"Name: {player.DescriptionName}";
            BLT.Print(x, line, text);
            var size = BLT.Measure(text);
            line += size.Height + 1;
        }
    }
}