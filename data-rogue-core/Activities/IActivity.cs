﻿using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using data_rogue_core.Maps;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public interface IActivity
    {
        ActivityType Type { get; }
        object Data { get; }
        bool RendersEntireSpace { get; }
        
        void Initialise();

        void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard);
        void HandleMouse(ISystemContainer systemContainer, MouseData mouse);
        void HandleAction(ISystemContainer systemContainer, ActionEventData action);
        IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height);
    }
}