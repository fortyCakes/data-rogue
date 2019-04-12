using data_rogue_core.Components;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Activities
{
    public class TargetingActivity : IActivity
    {
        public ActivityType Type => ActivityType.Targeting;
        public object Data => TargetingActivityData;

        public MapCoordinate CurrentTarget => TargetingActivityData.CurrentTarget;

        public bool RendersEntireSpace => false;
        private readonly IActivitySystem _activitySystem;

        public ITargetingRenderer Renderer { get; set; }

        public TargetingActivityData TargetingActivityData;
        private MapCoordinate _targetFrom;
        private ISystemContainer _systemContainer;
        private IPositionSystem _positionSystem;
        private IGameplayRenderer _gameplayRenderer;
        private HashSet<MapCoordinate> _targetableCells;

        public TargetingActivity(TargetingData targetingData, Action<MapCoordinate> callback, ISystemContainer systemContainer, MapCoordinate targetFrom)
        {
            _activitySystem = systemContainer.ActivitySystem;
            _systemContainer = systemContainer;
            _positionSystem = systemContainer.PositionSystem;

            TargetingActivityData = new TargetingActivityData
            {
                TargetingData = targetingData,
                CurrentTarget = null,
                Callback = callback
            };

            _targetFrom = targetFrom;
            _gameplayRenderer = GetGameplayRenderer();

            _targetableCells = targetingData.TargetableCellsFrom(_targetFrom);

            PickInitialTarget();
        }

        private void PickInitialTarget()
        {
            var cellsByDistance = _targetableCells.Except(new[] { _targetFrom }).ToList();
            cellsByDistance.OrderBy(c =>
            {
                var vector = _targetFrom - c;
                return Math.Sqrt(vector.X ^ 2 + vector.Y ^ 2);
            });

            foreach(var coordinate in cellsByDistance)
            {
                var entities = _systemContainer.PositionSystem.EntitiesAt(coordinate);
                if (entities.Any(e => e.Has<Health>()))
                {
                    TargetingActivityData.CurrentTarget = coordinate;
                }
            }
        }

        public void Initialise(IRenderer renderer)
        {
            Renderer = (ITargetingRenderer)renderer;
        }

        public void Render(ISystemContainer systemContainer)
        {
            Renderer.Render(systemContainer, TargetingActivityData);
        }

        public void Complete()
        {
            if (CurrentTarget != null)
            {
                TargetingActivityData.Callback(CurrentTarget);
            }
            else
            {
                Cancel();
            }
            
            _activitySystem.RemoveActivity(this);
        }

        public void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.MouseActive)
            {
                var x = mouse.X;
                var y = mouse.Y;

                var hoveredLocation = _gameplayRenderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                if (hoveredLocation != null)
                {
                    MapCoordinate playerPosition = _positionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

                    if (_targetableCells.Contains(hoveredLocation))
                    {
                        TargetingActivityData.CurrentTarget = hoveredLocation;
                    }
                    else
                    {
                        TargetingActivityData.CurrentTarget = null;
                    }
                }

                if (mouse.IsLeftClick)
                {
                    Complete();
                }
            }
        }

        public void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            // None
        }

        public void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            switch(action.Action)
            {
                case ActionType.Move:
                    var vector = Vector.Parse(action.Parameters);
                    TryMoveTarget(vector);
                    break;
                case ActionType.Select:
                    Complete();
                    break;
                case ActionType.EscapeMenu:
                    Cancel();
                    break;
            }
        }

        private void Cancel()
        {
            _activitySystem.RemoveActivity(this);
        }

        private void TryMoveTarget(Vector vector)
        {
            var newTarget = (TargetingActivityData.CurrentTarget ?? _targetFrom) + vector;

            if (_targetableCells.Contains(newTarget))
            {
                TargetingActivityData.CurrentTarget = newTarget;
            }
        }

        private IGameplayRenderer GetGameplayRenderer()
        {
            GameplayActivity gameplayActivity = (GameplayActivity)_activitySystem.ActivityStack.Single(a => a.Type == ActivityType.Gameplay);

            return gameplayActivity.Renderer;
        }

        
    }

    public class TargetingActivityData
    {
        public TargetingData TargetingData { get; internal set; }
        public MapCoordinate CurrentTarget { get; internal set; }
        public Action<MapCoordinate> Callback { get; set; }
    }
}