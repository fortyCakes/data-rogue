using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class MessageSystem : IMessageSystem
    {
        public MessageSystem()
        {
            AllMessages = new List<Message>();
        }

        public void Write(string message, Color color)
        {
            AllMessages.Add(new Message{Text = message, Color = color});
        }

        public List<Message> AllMessages { get; }

        public List<Message> RecentMessages(int messages)
        {
            return AllMessages.Skip(Math.Max(0, AllMessages.Count - messages)).ToList();
        }
    }
}