using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Drawing;

namespace data_rogue_core.Activities
{
    public interface IDataRogueControl
    {
        bool IsFocused { get; set; }
        bool IsPressed { get; set; }

        bool CanHandleMouse { get; }

        ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer);

        Rectangle Position { get; set; }

        int ActivityIndex { get; set; }

        Color Color { get; }
        Color BackColor { get; }

        void Click(object sender, PositionEventHandlerArgs eventArgs);
        event PositionEventHandler OnClick;

        bool Visible { get; set; }

        bool FillsContainer { get; }
    }

    public interface IDataRogueInfoControl : IDataRogueControl
    {
        void SetData(IEntity entity, InfoDisplay display);

        IEntity Entity { get; }
        string Parameters { get; }
    }
}