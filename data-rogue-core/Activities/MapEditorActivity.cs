using System.Collections.Generic;
using data_rogue_core.Controls;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class MapEditorActivity: BaseActivity, IMapActivity
    {
        private IMap _map;

        public override ActivityType Type => ActivityType.MapEditor;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        public MapCoordinate CameraPosition => new MapCoordinate(_map.MapKey, 0, 0);

        public MapEditorActivity(IMap map)
        {
            _map = map;
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            var config = GetRenderingConfiguration(width, height);

            return ControlFactory.GetControls(config, renderer, systemContainer, rendererHandle, controlRenderers, playerFov);
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            //throw new System.NotImplementedException();
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            //throw new System.NotImplementedException();
        }

        private IEnumerable<IRenderingConfiguration> GetRenderingConfiguration(int width, int height)
        {
            return new List<IRenderingConfiguration>
            {
                new MapConfiguration {Position=new System.Drawing.Rectangle(0,0, width, height)}
            };
        }
    }
}