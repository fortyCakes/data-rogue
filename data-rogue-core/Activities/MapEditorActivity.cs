using System.Collections.Generic;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class MapEditorActivity: BaseActivity
    {
        public override ActivityType Type => ActivityType.MapEditor;
        public override bool RendersEntireSpace => true;
        public override bool AcceptsInput => true;

        private readonly IOSystemConfiguration _ioSystemConfiguration;

        public MapEditorActivity(IOSystemConfiguration ioSystemConfiguration)
        {
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public void Initialise()
        {
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            throw new System.NotImplementedException();
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            throw new System.NotImplementedException();
        }
    }
}