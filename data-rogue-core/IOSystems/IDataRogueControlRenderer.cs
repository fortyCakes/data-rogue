using data_rogue_core.Activities;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace data_rogue_core.IOSystems
{
    public interface IDataRogueControlRenderer
    {
        Type DisplayType { get; }

        bool Layout(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov, Rectangle boundingBox, Padding padding, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment);

        void Paint(object handle, IDataRogueControl display, ISystemContainer systemContainer, List<MapCoordinate> playerFov);

        IEntity EntityFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse);
        string StringFromMouseData(IDataRogueControl display, ISystemContainer systemContainer, MouseData mouse);
    }
}
