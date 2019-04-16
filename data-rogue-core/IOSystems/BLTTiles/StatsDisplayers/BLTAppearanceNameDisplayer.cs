using System.Collections.Generic;
using BearLib;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    public class BLTAppearanceNameDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "AppearanceName";

        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var appearance = entity.Has<SpriteAppearance>() ? entity.Get<SpriteAppearance>() : new SpriteAppearance {Top = "unknown"};

            if (!string.IsNullOrEmpty(appearance.Bottom))
            {
                BLT.Font("");
                BLT.Layer(BLTLayers.MapEntityBottom);
                BLT.Put(x, y, spriteManager.Tile(appearance.Bottom));
            }
            if (!string.IsNullOrEmpty(appearance.Top))
            {
                BLT.Font("");
                BLT.Layer(BLTLayers.MapEntityTop);
                BLT.Put(x, y, spriteManager.Tile(appearance.Top));
            }

            var text = $"{entity.DescriptionName}";

            y += 1;
            RenderText(x + 10, ref y, text, display.Color, false, font: "textLarge");
            y += 9;
        }
    }
}