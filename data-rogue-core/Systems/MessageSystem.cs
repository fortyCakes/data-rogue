using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class MessageSystem : IInitialisableSystem ,IMessageSystem
    {
        public MessageSystem()
        {
            AllMessages = new List<Message>();
        }

        public void Write(string message, Color? color = null)
        {
            if (!color.HasValue) color = Color.White;

            AllMessages.Add(new Message{Text = message, Color = color.Value});
        }

        public List<Message> AllMessages { get; }

        public List<Message> RecentMessages(int messages)
        {
            return AllMessages.Skip(Math.Max(0, AllMessages.Count - messages)).ToList();
        }

        public void Initialise()
        {
            AllMessages.Clear();
        }
    }
}