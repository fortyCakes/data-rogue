﻿using data_rogue_core.IOSystems;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Activities
{
    public class InformationActivity : IActivity
    {
        public ActivityType Type => ActivityType.Information;
        public object Data => StatsDisplays;
        public bool RendersEntireSpace { get; set; }
        public IEntity Entity { get; }
        public List<StatsConfiguration> StatsDisplays { get; set; }
        public IInformationRenderer Renderer { get; private set; }
        public bool CloseOnKeyPress { get; }

        public bool Running => true;

        private readonly IActivitySystem _activitySystem;

        public InformationActivity(IActivitySystem activitySystem, List<StatsConfiguration> statsDisplays, IEntity entity, bool closeOnKeyPress = true, bool rendersEntireSpace = false)
        {
            Entity = entity;
            StatsDisplays = statsDisplays;
            CloseOnKeyPress = closeOnKeyPress;
            RendersEntireSpace = rendersEntireSpace;
            _activitySystem = activitySystem;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer, StatsDisplays, Entity, RendersEntireSpace);
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (IInformationRenderer)renderer;
        }
        
        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            if (keyboard != null && keyboard.Key != Key.Unknown && CloseOnKeyPress)
            {
                Close();
            }
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.IsLeftClick)
            {
                Close();
            }
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            //throw new System.NotImplementedException();
        }

        private void Close()
        {
            _activitySystem.RemoveActivity(this);
        }
    }
}
