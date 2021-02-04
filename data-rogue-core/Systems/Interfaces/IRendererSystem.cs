using System;
using data_rogue_core.Activities;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IRendererSystem
    {
        IOSystemConfiguration IOSystemConfiguration { get; set; }

        IUnifiedRenderer Renderer {get;set;}

        MapCoordinate CameraPosition { get; }

        bool LockCameraToPlayer { get; set; }
    }
}
