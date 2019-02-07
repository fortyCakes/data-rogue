using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Renderers
{
    public interface ITargetingRenderer : IRenderer
    {
        void Render(WorldState worldState, ISystemContainer systemContainer, TargetingActivityData targetingActivityData);
    }
}