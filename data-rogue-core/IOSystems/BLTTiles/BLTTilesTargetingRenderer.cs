using System.Collections.Generic;
using System.Linq;
using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTilesTargetingRenderer : ITargetingRenderer
    {
        private IOSystemConfiguration _ioSystemConfiguration;
        private ISpriteSheet _shadeSprite;

        public BLTTilesTargetingRenderer(IOSystemConfiguration ioSystemConfiguration, ISpriteManager spriteManager)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
            
            _shadeSprite = spriteManager.Get("shade");
        }

        public void Render(ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            

            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                RenderMap(mapConfiguration, systemContainer, targetingActivityData);
            }
        }

        private Dictionary<CellTargeting, ISpriteSheet> _targetingSprites;

        private void RenderMap(MapConfiguration mapConfiguration, ISystemContainer systemContainer, TargetingActivityData targetingActivityData)
        {
            
        }
    }
}