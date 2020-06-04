using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Forms;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Menus;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using OpenTK.Input;
using RLNET;

namespace data_rogue_core.Systems
{
    public class ControlSystem : IControlSystem
    {
        private ISystemContainer _systemContainer;
        private readonly IEntityDataProvider keyBindingsDataProvider;
        private List<KeyBinding> _keyBindings;
        private IActivitySystem _activitySystem;
        private ITargetingSystem _targetingSystem;

        public MapCoordinate HoveredCoordinate { get; set; }

        public ControlSystem(ISystemContainer systemContainer, IEntityDataProvider keyBindingsDataProvider)
        {
            this._systemContainer = systemContainer;
            this.keyBindingsDataProvider = keyBindingsDataProvider;
            _activitySystem = systemContainer.ActivitySystem;
            _targetingSystem = systemContainer.TargetingSystem;

        }

        public void Initialise()
        {
            SetKeyBindings();
        }

        private void SetKeyBindings()
        {
            var keyBindingsData = keyBindingsDataProvider.GetData();

            var bindings = EntitySerializer.DeserializeAll(_systemContainer, keyBindingsData).SingleOrDefault();

            if (bindings != null)
            {
                _keyBindings = bindings.Components.OfType<KeyBinding>().ToList();
            }
            else
            {
                _keyBindings = new List<KeyBinding>();
            }
        }

        public void HandleInput(KeyCombination keyPress, MouseData mouse)
        {
            var actionData = GetActionFromBoundKey(keyPress);
            if (_activitySystem.ActivityStack.Any())
            {
                IActivity inputActivity = _activitySystem.GetActivityAcceptingInput();

                if (inputActivity != null && ((inputActivity as GameplayActivity)?.Running ?? true))
                {
                    if (actionData != null)
                    {
                        inputActivity.HandleAction(_systemContainer, actionData);
                    }

                    if (keyPress != null)
                    {
                        inputActivity.HandleKeyboard(_systemContainer, keyPress);
                    }

                    if (mouse != null)
                    {
                        inputActivity.HandleMouse(_systemContainer, mouse);
                    }
                }
            }
        }

        private ActionEventData GetActionFromBoundKey(KeyCombination keyPress)
        {
            var keyBinding = _keyBindings.SingleOrDefault(k => k.Key.Equals(keyPress));

            if (keyBinding == null)
            {
                if (keyPress == null || keyPress.Key == Key.Unknown) return null;

                return new ActionEventData{ Action =  ActionType.None, Parameters = null, KeyPress = keyPress};
            }

            var action = keyBinding.Action;

            ActionType actionType = ExtractActionType(action);
            var parameters = ExtractParameters(action);

            return new ActionEventData { Action = actionType, Parameters = parameters, KeyPress = keyPress, Speed = 1000 };
        }

        public static string ExtractParameters(string action)
        {
            if (action.Contains("("))
            {
                action = action.Remove(0, action.IndexOf('(') + 1);
                action = action.Remove(action.Length - 1);
            }
            else
            {
                return null;
            }

            return action;
        }

        public static ActionType ExtractActionType(string action)
        {
            if (action.Contains("("))
            {
                action = action.Remove(action.IndexOf('('));
            }

            return (ActionType)Enum.Parse(typeof(ActionType), action);
        }

        public void HandleMouseInput(MouseData mouse)
        {
            var x = mouse.X;
            var y = mouse.Y;

            if (_activitySystem.Peek() is GameplayActivity activity && _systemContainer.PlayerSystem.Player != null)
            {
                IUnifiedRenderer renderer = _systemContainer.RendererSystem.Renderer;
                var hoveredLocation = renderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                SetHoveredLocation(hoveredLocation);
            }
                    
        }

        private void SetHoveredLocation(MapCoordinate mapCoordinate)
        {
            HoveredCoordinate = mapCoordinate;
        }
    }
}