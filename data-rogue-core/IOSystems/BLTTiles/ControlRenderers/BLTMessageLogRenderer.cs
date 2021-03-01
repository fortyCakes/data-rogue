using System;
using System.Collections.Generic;
using System.Drawing;
using BearLib;
using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{

    internal class BLTMessageLogRenderer : BLTControlRenderer
    {
        public override Type DisplayType => typeof(MessageLogControl);

        protected override void DisplayInternal(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var messageLog = control as MessageLogControl;

            var numberOfMessages = int.Parse(messageLog.Parameters);

            BLTLayers.Set(BLTLayers.Text, control.ActivityIndex);
            BLT.Font("text");

            var messagesToRender = systemContainer.MessageSystem.RecentMessages(numberOfMessages);
            messagesToRender.Reverse();

            var x = control.Position.Left;
            var y = control.Position.Bottom;

            foreach (var message in messagesToRender)
            {
                var size = BLT.Measure(message.Text);
                y -= size.Height + 1;

                BLT.Color(message.Color);
                BLT.Print(x, y, message.Text);
            }

            BLT.Color("");
            BLT.Font("");
        }

        protected override Size Measure(ISpriteManager spriteManager, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }
    }
}