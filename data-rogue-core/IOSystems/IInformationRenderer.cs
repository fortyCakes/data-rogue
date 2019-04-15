using System.Collections.Generic;
using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public interface IInformationRenderer : IRenderer
    {
        void Render(ISystemContainer systemContainer, List<StatsConfiguration> statsDisplays, bool rendersEntireSpace);
    }
}