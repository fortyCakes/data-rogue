using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace data_rogue_core.Activities
{
    public class TargetingActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Targeting;
        public override object Data => TargetingActivityData;

        public MapCoordinate CurrentTarget => TargetingActivityData.CurrentTarget;

        public override bool RendersEntireSpace => false;

        public bool Running => true;

        private readonly IActivitySystem _activitySystem;

        public TargetingActivityData TargetingActivityData;
        private MapCoordinate _targetFrom;
        private ISystemContainer _systemContainer;
        private IPositionSystem _positionSystem;
        private HashSet<MapCoordinate> _targetableCells;
        private IOSystemConfiguration _ioSystemConfiguration;

        public TargetingActivity(TargetingData targetingData, Action<MapCoordinate> callback, ISystemContainer systemContainer, MapCoordinate targetFrom, IOSystemConfiguration ioSystemConfiguration)
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

            _targetableCells = targetingData.TargetableCellsFrom(_targetFrom);

            PickInitialTarget();
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        private void PickInitialTarget()
        {
            var cellsByDistance = _targetableCells.Except(new[] { _targetFrom }).ToList();
            cellsByDistance = cellsByDistance.OrderBy(c =>
            {
                var vector = _targetFrom - c;
                return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            }).ToList();

            foreach(var coordinate in cellsByDistance)
            {
                var entities = _positionSystem.EntitiesAt(coordinate);
                if (entities.Any(e => e.Has<Health>()))
                {
                    TargetingActivityData.CurrentTarget = coordinate;
                    return;
                }
            }
        }

        public void Initialise()
        {
        }

        public void Complete()
        {
            if (CurrentTarget != null)
            {
                TargetingActivityData.Callback(CurrentTarget);
            }
            
            CloseActivity();
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.MouseActive)
            {
                var x = mouse.X;
                var y = mouse.Y;

                var hoveredLocation = systemContainer.RendererSystem.Renderer.GetMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

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

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            // None
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
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
                    CloseActivity();
                    break;
            }
        }

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            foreach (var mapConfiguration in _ioSystemConfiguration.MapConfigurations)
            {
                yield return new TargetingOverlayControl { Position = mapConfiguration.Position, TargetingActivityData = TargetingActivityData };
            }
        }

        private void CloseActivity()
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
    }

    public class TargetingActivityData
    {
        public TargetingData TargetingData { get; set; }
        public MapCoordinate CurrentTarget { get; set; }
        public Action<MapCoordinate> Callback { get; set; }

        public bool IsTargeted(MapCoordinate currentCell)
        {
            if (currentCell == CurrentTarget) return true;

            foreach(var diff in TargetingData.CellsHit)
            {
                if (currentCell == CurrentTarget + diff)
                {
                    return true;
                }
            }

            return false;
        }
    }
}