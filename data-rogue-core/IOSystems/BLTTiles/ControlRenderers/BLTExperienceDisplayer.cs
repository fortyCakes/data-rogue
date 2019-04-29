using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTExperienceDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(ExperienceControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var experience = entity.Get<Experience>();

            var text1 = $"Level: {experience.Level}";
            var text2 = $"   XP: {experience.Amount}";

            RenderText(display.Position.X, display.Position.Y, out var textSize, text1, display.Color);
            RenderText(display.Position.X, display.Position.Y + textSize.Height, out _, text2, display.Color);
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var display = control as IDataRogueInfoControl;
            var entity = display.Entity;

            var experience = entity.Get<Experience>();

            var text1 = $"Level: {experience.Level}";
            var text2 = $"   XP: {experience.Amount}";

            BLT.Font("");
            var size1 = BLT.Measure(text1);
            var size2 = BLT.Measure(text2);

            return new Size(Math.Max(size1.Width, size2.Width), size1.Height + size2.Height);
        }
    }
}