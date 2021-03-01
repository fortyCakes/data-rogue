using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.IOSystems.BLTTiles;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
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
        void MouseDown(object sender, PositionEventHandlerArgs eventArgs);
        void MouseUp(object sender, PositionEventHandlerArgs eventArgs);

        event PositionEventHandler OnClick;
        event PositionEventHandler OnMouseDown;
        event PositionEventHandler OnMouseUp;

        bool Visible { get; set; }

        bool FillsContainer { get; }

        bool Layout(List<IDataRogueControlRenderer> controlRenderers, ISystemContainer systemContainer, object handle, List<MapCoordinate> playerFov, Rectangle boundingBox);

        void Paint(List<IDataRogueControlRenderer> _controlRenderers, object handle, ISystemContainer systemContainer, List<MapCoordinate> playerFov);
    }
}