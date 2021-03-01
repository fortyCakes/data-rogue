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
using data_rogue_core.Utils;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class TargetingActivity : BaseActivity
    {
        public override ActivityType Type => ActivityType.Targeting;

        public override bool RendersEntireSpace => false;
        public override bool AcceptsInput => true;

        public bool Running => true;

        private readonly IActivitySystem _activitySystem;
        
        private IPositionSystem _positionSystem;
        private ITargetingSystem _targetingSystem;
        private HashSet<MapCoordinate> _targetableCells;
        private IOSystemConfiguration _ioSystemConfiguration;
        private readonly ISystemContainer _systemContainer;
        private MapCoordinate _currentTarget;
        private IEnumerable<MapCoordinate> _path = new List<MapCoordinate>();
        public Targeting TargetingData { get; set; }

        public Action<MapCoordinate> Callback { get; set; }
        public MapCoordinate TargetFrom { get; internal set; }

        public TargetingActivity(Rectangle position, Padding padding, Targeting targetingData, Action<MapCoordinate> callback, ISystemContainer systemContainer, MapCoordinate targetFrom, IOSystemConfiguration ioSystemConfiguration) : base(position, padding)
        {
            _activitySystem = systemContainer.ActivitySystem;
            _systemContainer = systemContainer;
            _positionSystem = systemContainer.PositionSystem;
            _targetingSystem = systemContainer.TargetingSystem;

            TargetingData = targetingData;
            CurrentTarget = null;
            Callback = callback;
            TargetFrom = targetFrom;

            _targetableCells = _targetingSystem.TargetableCellsFrom(TargetingData, TargetFrom);

            PickInitialTarget();
            _ioSystemConfiguration = ioSystemConfiguration;
        }

        public override void InitialiseControls()
        {
            foreach (MapControl mapConfiguration in _ioSystemConfiguration.GameplayWindowControls.OfType<MapControl>())
            {
                Controls.Add(new TargetingOverlayControl { Position = mapConfiguration.Position, TargetingActivity = this });
            }
        }

        public MapCoordinate CurrentTarget
        {
            get => _currentTarget;
            set
            {
                _currentTarget = value;

                if (TargetingData.HitCellsOnPath)
                {
                    if (CurrentTarget != null && TargetFrom != null)
                    {
                        _path = _systemContainer.PositionSystem.DirectPath(TargetFrom, CurrentTarget) ?? new List<MapCoordinate>();
                    }
                    else
                    {
                        _path = new List<MapCoordinate>();
                    }
                }
            }
        }

        private void PickInitialTarget()
        {
            var cellsByDistance = _targetableCells.Except(new[] { TargetFrom }).ToList();
            cellsByDistance = cellsByDistance.OrderBy(c =>
            {
                var vector = TargetFrom - c;
                return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            }).ToList();

            foreach(var coordinate in cellsByDistance)
            {
                var entities = _positionSystem.EntitiesAt(coordinate);
                if (entities.Any(e => e.Has<Health>()))
                {
                    CurrentTarget = coordinate;
                    return;
                }
            }
        }

        public void Complete()
        {
            if (CurrentTarget != null)
            {
                Callback(CurrentTarget);
            }
            
            CloseActivity();
        }

        public override void HandleMouse(ISystemContainer systemContainer, MouseData mouse)
        {
            if (mouse.MouseActive)
            {
                var x = mouse.X;
                var y = mouse.Y;

                var hoveredLocation = systemContainer.RendererSystem.Renderer.GetGameplayMapCoordinateFromMousePosition(_systemContainer.RendererSystem.CameraPosition, x, y);

                if (hoveredLocation != null)
                {
                    MapCoordinate playerPosition = _positionSystem.CoordinateOf(_systemContainer.PlayerSystem.Player);

                    if (_targetableCells.Contains(hoveredLocation))
                    {
                        CurrentTarget = hoveredLocation;
                    }
                    else
                    {
                        CurrentTarget = null;
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
            if (action != null)
            {
                switch (action.Action)
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
        }

        private void CloseActivity()
        {
            _activitySystem.RemoveActivity(this);
        }

        private void TryMoveTarget(Vector vector)
        {
            var newTarget = (CurrentTarget ?? TargetFrom) + vector;

            if (_targetableCells.Contains(newTarget))
            {
                CurrentTarget = newTarget;
            }
        }

        public Matrix Rotation
        {
            get
            {
                if (!TargetingData.Rotatable || CurrentTarget == null || TargetFrom == null) return Matrix.Identity;

                var dx = CurrentTarget.X - TargetFrom.X;
                var dy = CurrentTarget.Y - TargetFrom.Y;

                return TargetingRotationHelper.GetSkillRotation(dy: dy, dx: dx);
            }
        }

        public TargetingStatus GetTargetingStatus(MapCoordinate currentCell)
        {
            foreach (var diff in TargetingData.CellsHit)
            {
                if (currentCell == CurrentTarget + Rotation * diff)
                {
                    return TargetingStatus.Targeted;
                }
            }

            if (_path.Contains(currentCell))
            {
                return TargetingStatus.Targeted;
            }

            if (_targetableCells.Contains(currentCell))
            {
                return TargetingStatus.Targetable;
            }

            return TargetingStatus.NotTargeted;
        }
    }
}