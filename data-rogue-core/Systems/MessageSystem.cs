using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class MessageSystem : IInitialisableSystem , IMessageSystem
    {
        public MessageSystem()
        {
            AllMessages = new List<Message>();
        }

        public void Write(string message)
        {
            Write(message, Color.White);
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

        public void Initialise()
        {
            AllMessages.Clear();
        }

        public DeferredMessageContext DeferredMessage()
        {
            var message = new Message();
            AllMessages.Add(message);

            return new DeferredMessageContext(message, m => AllMessages.Remove(m));
        }
    }

    public class DeferredMessageContext : IDisposable
    {
        private Message Message;
        private readonly Action<Message> _ifUnused;

        public DeferredMessageContext(Message message, Action<Message> ifUnused)
        {
            Message = message;
            _ifUnused = ifUnused;
        }

        public void Dispose()
        {
            if (string.IsNullOrEmpty(Message.Text))
            {
                _ifUnused(Message);
            }
        }

        public void SetMessage(string message)
        {
            Message.Text = message;
        }
    }
}