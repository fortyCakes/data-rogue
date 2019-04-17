using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Activities
{
    public interface IActivity
    {
        bool Running { get; }
        ActivityType Type { get; }
        object Data { get; }
        bool RendersEntireSpace { get; }

        [Obsolete]
        void Render(ISystemContainer systemContainer);
        void Initialise(IRenderer renderer);

        void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        void HandleMouse(ISystemContainer systemContainer, MouseData mouse);
        void HandleAction(ISystemContainer systemContainer, ActionEventData action);
        IEnumerable<IDataRogueControl> GetLayout(int width, int height);
    }
}