using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Renderers;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Activities
{
    [TestFixture]
    public class TargetingActivityTests
    {
        private TargetingData _targetingData;
        private TargetingActivity _targetingActivity;
        private bool _callbackHappened;
        private MapCoordinate _callbackTarget;
        private IRendererFactory _rendererFactory;
        private IGameplayRenderer _gameplayRenderer;
        private ActivityStack _activityStack;
        private IActivitySystem _activitySystem;
        private IPositionSystem _positionSystem;
        private ISystemContainer _systemContainer;

        [SetUp]
        public void SetUp()
        {
            _targetingData = GetTargetingData();
            _callbackTarget = null;
            _callbackHappened = false;

            _gameplayRenderer = Substitute.For<IGameplayRenderer>();
            _activityStack = new ActivityStack(null);
            _activityStack.Push(new GameplayActivity() { Renderer = _gameplayRenderer });
            _activitySystem = Substitute.For<IActivitySystem>();
            _activitySystem.ActivityStack.Returns(_activityStack);
            _positionSystem = Substitute.For<IPositionSystem>();
            _systemContainer = Substitute.For<ISystemContainer>();
            _systemContainer.ActivitySystem.Returns(_activitySystem);
            _systemContainer.PositionSystem.Returns(_positionSystem);

            _targetingActivity = new TargetingActivity(_targetingData, _callback, _systemContainer, new MapCoordinate("Map", 0, 0));
        }

        private TargetingData GetTargetingData()
        {
            return new TargetingData
            {
                CellsHit = new List<Vector> { new Vector(0, 0) },
                Friendly = false,
                MoveToCell = false,
                Range = 4,
                ValidVectors = null
            };
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
        public void Complete_WithDefaultTarget_ReturnsTargetNextToOrigin()
        {
            var entity = Substitute.For<IEntity>();
            var entityList = new List<IEntity> { entity };
            entity.Has<Health>().Returns(true);
            _positionSystem.EntitiesAt(Arg.Any<MapCoordinate>()).ReturnsForAnyArgs(entityList);
            _systemContainer.PositionSystem.Returns(_positionSystem);

            _targetingActivity = new TargetingActivity(_targetingData, _callback, _systemContainer, new MapCoordinate("Map", 0, 0));
            _targetingActivity.Complete();

            _callbackHappened.Should().BeTrue();
            var distance = Math.Abs(_callbackTarget.X) + Math.Abs(_callbackTarget.Y);
            distance.Should().Be(1);
        }

    }
}
