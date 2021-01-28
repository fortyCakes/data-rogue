using System.Collections.Generic;
using System.Windows.Forms;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public interface IUnifiedRenderer
    {
        Padding ActivityPadding { get; }

        void Render(ISystemContainer systemContainer, IActivity activity, int activityIndex);
        MapCoordinate GetMapCoordinateFromMousePosition(MapCoordinate cameraPosition, int x, int y);
        IDataRogueControlRenderer GetRendererFor(IDataRogueControl mouseOverControl);
        IDataRogueControl GetControlFromMousePosition(ISystemContainer systemContainer, IActivity activity, MapCoordinate cameraPosition, int x, int y);
    }
}