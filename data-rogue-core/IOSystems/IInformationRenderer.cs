using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public interface IUnifiedRenderer : IRenderer
    {
        void Render(ISystemContainer systemContainer, IActivity activity);
    }
}