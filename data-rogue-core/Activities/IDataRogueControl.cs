using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public interface IDataRogueControl
    {
        bool IsFocused { get; set; }
        bool IsPressed { get; set; }

        bool CanHandleMouse { get; }

        ActionEventData HandleMouse(MouseData mouse, IDataRogueControlRenderer renderer, ISystemContainer systemContainer);

        Rectangle Position { get; set;  }
        Rectangle LayoutPosition { get; }

        void MovePosition(int dx, int dy);

        Padding Padding { get; set; }
        Padding Margin { get; set; }

        int PaddedTop { get; }
        int PaddedLeft { get; }

        int ActivityIndex { get; set; }

        Color Color { get; }
        Color BackColor { get; }

        void Click(object sender, PositionEventHandlerArgs eventArgs);
        void MouseDown(object sender, PositionEventHandlerArgs eventArgs);
        void MouseUp(object sender, PositionEventHandlerArgs eventArgs);

        event PositionEventHandler OnClick;
        event PositionEventHandler OnMouseDown;
        event PositionEventHandler OnMouseUp;

        bool Visible { get; set; }

        bool FillsContainer { get; }

        void SetActivityIndex(int activityIndex);
        bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox);

        void Paint(List<IDataRogueControlRenderer> _controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
    }
}