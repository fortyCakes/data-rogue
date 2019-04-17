using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using System;
using System.Drawing;

namespace data_rogue_core.Activities
{
    public interface IDataRogueControl
    {
        bool IsFocused { get; set; }
        bool IsPressed { get; set; }
        Rectangle Position { get; set; }

        Color Color { get; }
        Color BackColor { get; }

        event PositionEventHandler Click;
    }

    public interface IDataRogueInfoControl : IDataRogueControl
    {
        void SetData(IEntity entity, InfoDisplay display);

        IEntity Entity { get; }
        string Parameters { get; }
    }
}