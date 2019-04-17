using System;
using System.Drawing;

namespace data_rogue_core.Activities
{
    public interface IDataRogueControl
    {
        bool IsFocused { get; set; }
        bool IsPressed { get; set; }
        Rectangle Position { get; set; }

        event PositionEventHandler Click;
    }
}