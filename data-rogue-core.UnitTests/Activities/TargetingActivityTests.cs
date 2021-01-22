using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace data_rogue_core.UnitTests.Activities
{
    [TestFixture]
    public class TargetingActivityTests
    {
        private Targeting _targetingData;
        private TargetingActivity _targetingActivity;
        private bool _callbackHappened;
        private MapCoordinate _callbackTarget;
        private IUnifiedRenderer _renderer;
        private ActivityStack _activityStack;
        private IActivitySystem _activitySystem;
        private IPositionSystem _positionSystem;
        private ITargetingSystem _targetingSystem;
        private ISystemContainer _systemContainer;
        private IOSystemConfiguration _ioConfig;

        [SetUp]
        public void SetUp()
        {
            _targetingData = GetTargetingData();
            _callbackTarget = null;
            _callbackHappened = false;

            _renderer = Substitute.For<IUnifiedRenderer>();
            var rendererSystem = Substitute.For<IRendererSystem>();
            rendererSystem.Renderer.Returns(_renderer);

            _activityStack = new ActivityStack();
            _ioConfig = new IOSystemConfiguration();
            _activityStack.Push(new GameplayActivity(_ioConfig));
            _activitySystem = Substitute.For<IActivitySystem>();
            _activitySystem.ActivityStack.Returns(_activityStack);
            _positionSystem = Substitute.For<IPositionSystem>();
            _targetingSystem = Substitute.For<ITargetingSystem>();
            _systemContainer = Substitute.For<ISystemContainer>();
            _systemContainer.ActivitySystem.Returns(_activitySystem);
            _systemContainer.PositionSystem.Returns(_positionSystem);
            _systemContainer.RendererSystem.Returns(rendererSystem);
            _systemContainer.TargetingSystem.Returns(_targetingSystem);

            SetTargetableCells(new MapCoordinate("Map", 1, 0));

            _targetingActivity = new TargetingActivity(_targetingData, _callback, _systemContainer, new MapCoordinate("Map", 0, 0), _ioConfig);
            _activityStack.Push(_targetingActivity);
        }

        private Targeting GetTargetingData()
        {
            return new Targeting
            {
                CellsHit = new VectorList { new Vector(1, 0) },
                Friendly = false,
                MoveToCell = false,
                Range = 4,
                ValidVectors = null
            };
        }

        private void SetTargetableCells(params MapCoordinate[] cells)
        {
            _targetingSystem.TargetableCellsFrom(Arg.Any<Targeting>(), Arg.Any<MapCoordinate>()).ReturnsForAnyArgs(new HashSet<MapCoordinate> ( cells ));
            _targetingActivity = new TargetingActivity(_targetingData, _callback, _systemContainer, new MapCoordinate("Map", 0, 0), _ioConfig);
        }

        private void _callback(MapCoordinate obj)
        {
            _callbackHappened = true;
            _callbackTarget = obj;
        }

        [Test]
        public void Complete_ReturnsNoTarget()
        {
            _targetingActivity.Complete();

            _callbackHappened.Should().BeFalse();
        }

        [Test]
        public void Complete_ClosesActivity()
        {
            _targetingActivity.Complete();

            _activitySystem.Received(1).RemoveActivity(_targetingActivity);
        }

        [Test]
        public void Complete_WithDefaultTarget_ReturnsDefaultTarget()
        {
            var entity = Substitute.For<IEntity>();
            var entityList = new List<IEntity> { entity };
            entity.Has<Health>().Returns(true);
            _positionSystem.EntitiesAt(Arg.Any<MapCoordinate>()).ReturnsForAnyArgs(entityList);
            _systemContainer.PositionSystem.Returns(_positionSystem);

            _targetingActivity = new TargetingActivity(_targetingData, _callback, _systemContainer, new MapCoordinate("Map", 0, 0), _ioConfig);
            _targetingActivity.Complete();

            _callbackHappened.Should().BeTrue();
            var distance = Math.Abs(_callbackTarget.X) + Math.Abs(_callbackTarget.Y);
            distance.Should().Be(1);
        }

        [Test]
        public void HandleAction_Move_NoCurrentTarget_MovesFromOrigin()
        {
            var action = new ActionEventData { Action = ActionType.Move, Parameters = "1,0" };
            _targetingActivity.HandleAction(_systemContainer, action);

            TargetShouldBe(1, 0);
        }

        [Test]
        public void HandleAction_Move_CurrentTarget_MovesFromCurrentTarget()
        {
            SetTargetableCells(new MapCoordinate("Map", 1, 0), new MapCoordinate("Map", 2, 0));

            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 1, 0);

           var action = new ActionEventData { Action = ActionType.Move, Parameters = "1,0" };
            _targetingActivity.HandleAction(_systemContainer, action);

            TargetShouldBe(2, 0);
        }

        [Test]
        public void HandleAction_Select_CallsBack()
        {
            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 1, 0);

            var action = new ActionEventData { Action = ActionType.Select };
            _targetingActivity.HandleAction(_systemContainer, action);

            _callbackHappened.Should().BeTrue();
        }

        [Test]
        public void HandleAction_Escape_ClosesActivity()
        {
            var action = new ActionEventData { Action = ActionType.EscapeMenu };
            _targetingActivity.HandleAction(_systemContainer, action);

            _callbackHappened.Should().BeFalse();
            _activitySystem.Received(1).RemoveActivity(_targetingActivity);
        }

        [Test]
        public void HandleMouse_NotActive_DoesNotChangeTarget()
        {
            var mouse = new MouseData() { MouseActive = false };

            _targetingActivity.HandleMouse(_systemContainer, mouse);

            _targetingActivity.CurrentTarget.Should().BeNull();
            _callbackHappened.Should().BeFalse();
        }

        [Test]
        public void HandleMouse_ActiveOutsideTargetArea_DoesNotChangeTarget()
        {
            var mouse = new MouseData() { MouseActive = true };

            _renderer
                .GetMapCoordinateFromMousePosition(Arg.Any<MapCoordinate>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new MapCoordinate("Map", 1000, 1000));

            _targetingActivity.HandleMouse(_systemContainer, mouse);

            _targetingActivity.CurrentTarget.Should().BeNull();
            _callbackHappened.Should().BeFalse();
        }

        [Test]
        public void HandleMouse_ActiveOutsideMap_DoesNotChangeTarget()
        {
            var mouse = new MouseData() { MouseActive = true };

            _renderer
                .GetMapCoordinateFromMousePosition(Arg.Any<MapCoordinate>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns((MapCoordinate)null);

            _targetingActivity.HandleMouse(_systemContainer, mouse);

            _targetingActivity.CurrentTarget.Should().BeNull();
            _callbackHappened.Should().BeFalse();
        }

        [Test]
        public void HandleMouse_ActiveInTargetArea_SetsTarget()
        {
            SetTargetableCells(new MapCoordinate("Map", 1, 0), new MapCoordinate("Map", 2, 2));
            var mouse = new MouseData() { MouseActive = true };

            _renderer
                .GetMapCoordinateFromMousePosition(Arg.Any<MapCoordinate>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new MapCoordinate("Map", 2, 2));

            _targetingActivity.HandleMouse(_systemContainer, mouse);

            TargetShouldBe(2, 2);
            _callbackHappened.Should().BeFalse();
        }

        [Test]
        public void HandleMouse_LeftClick_CompletesActivity()
        {
            SetTargetableCells(new MapCoordinate("Map", 1, 0), new MapCoordinate("Map", 2, 2));
            var mouse = new MouseData() { MouseActive = true, IsLeftClick = true };

            _renderer
                .GetMapCoordinateFromMousePosition(Arg.Any<MapCoordinate>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(new MapCoordinate("Map", 2, 2));

            _targetingActivity.HandleMouse(_systemContainer, mouse);

            TargetShouldBe(2, 2);
            _callbackHappened.Should().BeTrue();
        }

        [Test]
        public void IsTargeted_CurrentTarget_ReturnsTrue()
        {
            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 0, 0);
            _targetingActivity.TargetingData.CellsHit = new VectorList{new Vector(0,0)};

            _targetingActivity.GetTargetingStatus(new MapCoordinate("Map", 0, 0)).Should().Be(TargetingStatus.Targeted);
        }

        [Test]
        public void IsTargeted_NotCurrentTarget_ReturnsFalse()
        {
            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 0, 0);
            _targetingActivity.TargetingData.CellsHit = new VectorList { new Vector(0, 0) };

            _targetingActivity.GetTargetingStatus(new MapCoordinate("Map", 15, 0)).Should().Be(TargetingStatus.NotTargeted);
        }

        [Test]
        public void IsTargeted_InCellsHit_ReturnsTrue()
        {
            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 0, 0);
            _targetingActivity.TargetingData.CellsHit = new VectorList { new Vector(0, 0), new Vector(1, 1) };

            _targetingActivity.GetTargetingStatus(new MapCoordinate("Map", 1, 1)).Should().Be(TargetingStatus.Targeted);
        }

        [Test]
        public void IsTargeted_OnDirectPath_ReturnsTrue()
        {
            _positionSystem.DirectPath(Arg.Any<MapCoordinate>(), Arg.Any<MapCoordinate>()).Returns(new List<MapCoordinate>
            {
                new MapCoordinate("Map", 1, -1),
                new MapCoordinate("Map", 2, -2),
                new MapCoordinate("Map", 3, -3),
                new MapCoordinate("Map", 4, -4),
                new MapCoordinate("Map", 5, -5)
            });

            _targetingActivity.TargetingData.HitCellsOnPath = true;
            _targetingActivity.CurrentTarget = new MapCoordinate("Map", 5, -5);
            _targetingActivity.TargetingData.CellsHit = new VectorList { new Vector(0, 0) };

            _targetingActivity.GetTargetingStatus(new MapCoordinate("Map", 1, -1)).Should().Be(TargetingStatus.Targeted);
        }

        private void TargetShouldBe(int x, int y)
        {
            _targetingActivity.CurrentTarget.Should().BeEquivalentTo(new MapCoordinate("Map", x, y));
        }
    }
}
