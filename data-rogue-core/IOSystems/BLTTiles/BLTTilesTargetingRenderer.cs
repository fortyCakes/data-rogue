using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesTargetingRenderer : ITargetingRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;

        public BLTTilesTargetingRenderer(IOSystemConfiguration ioSystemConfiguration)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public void Render(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            throw new System.NotImplementedException();
        }
    }
}