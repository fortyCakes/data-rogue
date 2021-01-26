using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;

namespace data_rogue_one.IOSystems
{
    public class DefencesControl : BaseInfoControl { }

    public class BLTDefencesDisplayer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(DefencesControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var x = control.Position.X;
            var y = control.Position.Y;
            var display = control as IDataRogueInfoControl;

            var player = systemContainer.PlayerSystem.Player;

            var ac = Math.Floor(systemContainer.EventSystem.GetStat(player, "AC"));
            var ev = Math.Floor(systemContainer.EventSystem.GetStat(player, "EV"));
            var sh = Math.Floor(systemContainer.EventSystem.GetStat(player, "SH"));
            var currentAegis = Math.Floor(systemContainer.EventSystem.GetStat(player, "CurrentAegisLevel"));
            var aegis = Math.Floor(systemContainer.EventSystem.GetStat(player, "Aegis"));
            var aegisText = $"{currentAegis}/{aegis}";
            var renderAegis = aegis > 0;
            var tiltFighter = player.Get<TiltFighter>();
            var brokenTicks = tiltFighter.BrokenTicks;
            var broken = brokenTicks > 0;

            if (broken)
            {
                BLT.Font("");
                BLT.Layer(BLTLayers.UIElements);
                RenderSpriteIfSpecified(x, y, spriteManager, "defence_broken", AnimationFrame.Idle0);

                BLT.Layer(BLTLayers.Text);
                BLT.Font("text");
                BLT.Print(x + 10, y + 2, $"DEFENCE BROKEN (Recovery: {100 - brokenTicks / 200}%)");
            }
            else
            {
                BLT.Font("");
                BLT.Layer(BLTLayers.UIElements);
                RenderSpriteIfSpecified(x, y, spriteManager, "AC", AnimationFrame.Idle0);
                RenderSpriteIfSpecified(x + 10, y, spriteManager, "SH", AnimationFrame.Idle0);
                RenderSpriteIfSpecified(x + 20, y, spriteManager, "EV", AnimationFrame.Idle0);
                if (renderAegis)
                {
                    var aegisSprite = currentAegis > 0 ? "aegis" : "aegis_none";
                    RenderSpriteIfSpecified(x + 30, y, spriteManager, aegisSprite, AnimationFrame.Idle0);
                }

                BLT.Layer(BLTLayers.Text);
                BLT.Font("text");

                PrintTextCentered(ac.ToString(), x + 4, y + 2);
                PrintTextCentered(sh.ToString(), x + 4 + 10, y + 2);
                PrintTextCentered(ev.ToString(), x + 4 + 20, y + 2);
                if (renderAegis)
                {
                    PrintTextCentered(aegisText, x + 4 + 30, y + 2);
                }
            }
        }

        private void PrintTextCentered(string text, int x, int y)
        {
            var textWidth = BLT.Measure(text).Width;
            BLT.Print(x - textWidth / 2, y, text);
        }

        protected override Size GetSizeInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return new Size(64, 10);
        }
    }
}