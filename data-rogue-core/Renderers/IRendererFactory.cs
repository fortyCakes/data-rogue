using System.Collections.Generic;
using data_rogue_core.Activities;
using data_rogue_core.Renderers;

namespace data_rogue_core
{
    public interface IRendererFactory
    {
        IRenderer GetRendererFor(ActivityType ForActivity);
    }

    public class RendererFactory : IRendererFactory
    {
        private Dictionary<ActivityType, IRenderer> Renderers { get; }

        public RendererFactory(Dictionary<ActivityType, IRenderer> renderers)
        {
            Renderers = renderers;
        }

        public IRenderer GetRendererFor(ActivityType ForActivity)
        {
            return Renderers[ForActivity];
        }
    }
}