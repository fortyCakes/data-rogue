using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using data_rogue_core.Maps;
using System.Windows.Forms;
using System.Drawing;

namespace data_rogue_core.Activities
{
    public interface IActivity
    {
        void InitialiseControls();

        ActivityType Type { get; }
        bool RendersEntireSpace { get; }
        bool AcceptsInput { get; }

        void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        void HandleMouse(ISystemContainer systemContainer, MouseData mouse);
        void HandleAction(ISystemContainer systemContainer, ActionEventData action);
        IList<IDataRogueControl> Controls { get; }
        Rectangle Position { get; }
    }
}