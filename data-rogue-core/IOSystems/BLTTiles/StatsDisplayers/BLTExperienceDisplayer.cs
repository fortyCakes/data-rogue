using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTExperienceDisplayer : BLTStatsRendererHelper
    {
        public override string DisplayType => "Experience";
        protected override void DisplayInternal(int x, ISpriteManager spriteManager, StatsDisplay display, ISystemContainer systemContainer, IEntity entity, List<MapCoordinate> playerFov, ref int y)
        {
            var experience = entity.Get<Experience>();

            var text1 = $"Level: {experience.Level}";
            var text2 = $"   XP: {experience.Amount}";

            RenderText(x, ref y, text1, display.Color);
            RenderText(x, ref y, text2, display.Color);
        }
    }
}