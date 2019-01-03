using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Data;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IMessageSystem
    {
        void Write(string message, Color color);

        List<Message> AllMessages { get; }

        List<Message> RecentMessages(int messages);
    }
}