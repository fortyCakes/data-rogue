using System;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IRendererSystem
    {
        IRendererFactory RendererFactory {get;set;}

        MapCoordinate CameraPosition { get; }

        Action QuitAction { get; set; }
    }
}
