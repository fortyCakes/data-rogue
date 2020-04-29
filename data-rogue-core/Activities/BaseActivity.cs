using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Activities
{
    public abstract class BaseActivity : IActivity
    {

        public IEnumerable<IDataRogueControl> Controls { get; set; } = new List<IDataRogueControl>();
        public abstract ActivityType Type { get; }
        public abstract bool RendersEntireSpace { get; }

        public void Layout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            Controls = GetLayout(renderer, systemContainer, rendererHandle, controlRenderers, playerFov, width, height).ToList();
        }

        public abstract IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height);
        public abstract void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        public abstract void HandleMouse(ISystemContainer systemContainer, MouseData mouse);
        public abstract void HandleAction(ISystemContainer systemContainer, ActionEventData action);
    }
}