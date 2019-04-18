using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using RLNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace data_rogue_core.IOSystems
{
    public class RLNetMessageLogRenderer : RLNetControlRenderer
    {
        public override Type DisplayType => typeof(MessageLogControl);

        protected override void DisplayInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            var messageConfiguration = control as MessageLogControl;

            var MessageConsole = new RLConsole(control.Position.Width, control.Position.Height);

            MessageConsole.Clear();

            var messages = systemContainer.MessageSystem.RecentMessages(messageConfiguration.NumberOfMessages);
            messages.Reverse();

            int y = messageConfiguration.Position.Height - 1;
            foreach (Message message in messages)
            {
                MessageConsole.Print(0, y--, 1, message.Text, message.Color.ToRLColor(), null, MessageConsole.Width);
            }

            RLConsole.Blit(MessageConsole, 0, 0, MessageConsole.Width, MessageConsole.Height, console, messageConfiguration.Position.Left, messageConfiguration.Position.Top);
        }

        protected override Size GetSizeInternal(RLConsole console, IDataRogueControl control, ISystemContainer systemContainer, List<MapCoordinate> playerFov)
        {
            return control.Position.Size;
        }
    }
}
