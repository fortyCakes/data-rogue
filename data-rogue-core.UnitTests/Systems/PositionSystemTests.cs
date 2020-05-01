using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using data_rogue_core.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class PositionSystemTests
    {
        private const string TEST_MAP_NAME = "key";
        private readonly MapKey TEST_MAP_KEY = new MapKey(TEST_MAP_NAME);

        [SetUp]
        public void SetUp()
        {
            systemContainer = new SystemContainer();

            systemContainer.CreateSystems("seed");

            positionSystem = systemContainer.PositionSystem;
            systemContainer.EntityEngine.Initialise(systemContainer);

            mover = GetTestEntity();
            map = SetUpTestMap();
        }
        
        IEntity mover;
        private IMap map;

        private SystemContainer systemContainer;
        private IPositionSystem positionSystem;

        [Test]
        public void CoordinateOf_HasPosition_ReturnsPosition()
        {
            var result = positionSystem.CoordinateOf(mover);

            result.Should().BeEquivalentTo(GetTestMapCoordinate());
        }

        [Test]
        public void CoordinateOf_HasNoPosition_ReturnsNull()
        {
            var entity = GetTestEntity(false);

            var result = positionSystem.CoordinateOf(entity);

            result.Should().BeNull();
        }

        [Test]
        public void EntitiesAt_ReturnsListOfEntities()
        {
            IEntity mapCell = SetUpTestMapCell(map, 0, 0);

            var expected = new List<IEntity>
            {
                mover,
                mapCell
            };

            IEnumerable<IEntity> result;

            result = positionSystem.EntitiesAt(TEST_MAP_KEY, 0, 0);
            result.Should().BeEquivalentTo(expected);

            result = positionSystem.EntitiesAt(GetTestMapCoordinate());
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Move_MovesEntity()
        {
            var result = positionSystem.CoordinateOf(mover);

            result.Should().BeEquivalentTo(GetTestMapCoordinate());

            positionSystem.Move(mover, new Vector(1,1));

            result = positionSystem.CoordinateOf(mover);

            result.Should().BeEquivalentTo(GetTestMapCoordinate(x: 1, y: 1));
        }

        [Test]
        public void Move_HasNoPosition_Throws()
        {
            var entity = GetTestEntity(false);

            Action action = () => positionSystem.Move(entity, new Vector(1, 1));

            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SetPosition_SetsPositionOfEntity()
        {
            positionSystem.Move(mover, new Vector(1, 1));
            MapCoordinate testMapCoordinate = GetTestMapCoordinate(x: 5, y: 10);

            positionSystem.SetPosition(mover, testMapCoordinate);

            var result = positionSystem.CoordinateOf(mover);

            result.Should().BeEquivalentTo(testMapCoordinate);
        }

        [Test]
        public void SetPosition_EntityHasNoPosition_SetsNewPosition()
        {
            var entity = GetTestEntity(false);
            MapCoordinate testMapCoordinate = GetTestMapCoordinate(x: 4, y: 4);

            positionSystem.SetPosition(entity, testMapCoordinate);

            var result = positionSystem.CoordinateOf(entity);

            result.Should().BeEquivalentTo(testMapCoordinate);
        }

        [Test]
        public void RemovePosition_EntityHasPosition_PositionIsRemoved()
        {
            positionSystem.RemovePosition(mover);

            mover.Has<Position>().Should().BeFalse();
            positionSystem.CoordinateOf(mover).Should().BeNull();
        }

        [Test]
        public void RemovePosition_EntityHasNoPosition_PositionIsRemoved()
        {
            var entity = GetTestEntity(false);

            positionSystem.RemovePosition(entity);

            entity.Has<Position>().Should().BeFalse();
            positionSystem.CoordinateOf(entity).Should().BeNull();
        }

        [Test]
        public void Any_EntitiesAtPosition_ReturnsTrue()
        {
            MapCoordinate testMapCoordinate = GetTestMapCoordinate();
            positionSystem.SetPosition(mover, testMapCoordinate);

            positionSystem.Any(testMapCoordinate).Should().BeTrue();
        }

        [Test]
        public void Any_NoEntitiesAtPosition_ReturnsFalse()
        {
            MapCoordinate testMapCoordinate = GetTestMapCoordinate();
            positionSystem.SetPosition(mover, GetTestMapCoordinate(x: 100, y: 100));

            positionSystem.Any(testMapCoordinate).Should().BeFalse();
        }

        [Test]
        public void Path_CallsPathfindingAlgorithm()
        {
            PositionSystem testPositionSystem = SetUpTestPositionSystemWithPathfindingAlgorithm(out IPathfindingAlgorithm pathfindingAlgorithm);

            MapCoordinate from = GetTestMapCoordinate(x: 0, y: 0);
            MapCoordinate to = GetTestMapCoordinate(x: 1, y: 0);
            testPositionSystem.Path(from, to);

            pathfindingAlgorithm.Path(Arg.Any<IMap>(), from, to).Received(1);
        }

        [Test]
        public void Path_DifferentMaps_ReturnsNull()
        {
            PositionSystem testPositionSystem = SetUpTestPositionSystemWithPathfindingAlgorithm(out IPathfindingAlgorithm pathfindingAlgorithm);

            MapCoordinate from = GetTestMapCoordinate(x: 0, y: 0);
            MapCoordinate to = GetTestMapCoordinate(key: "DIFFERENT_MAP", x: 1, y: 0);
            var result = testPositionSystem.Path(from, to);

            pathfindingAlgorithm.Path(Arg.Any<IMap>(), from, to).Received(0);
            result.Should().BeNull();
        }

        [Test]
        public void IsBlocked_NotBlocked_ReturnsFalse()
        {
            MapCoordinate testMapCoordinate = GetTestMapCoordinate();
            positionSystem.SetPosition(mover, testMapCoordinate);
            systemContainer.EntityEngine.AddComponent(mover, new Physical{Passable = true});

            var result = positionSystem.IsBlocked(testMapCoordinate);

            result.Should().BeFalse();
        }

        [Test]
        public void IsBlocked_Blocked_ReturnsTrue()
        {
            MapCoordinate testMapCoordinate = GetTestMapCoordinate();
            positionSystem.SetPosition(mover, testMapCoordinate);
            systemContainer.EntityEngine.AddComponent(mover, new Physical { Passable = false });

            var result = positionSystem.IsBlocked(testMapCoordinate);

            result.Should().BeTrue();
        }

        [Test]
        public void IsBlocked_BlockedByExcept_ReturnsFalse()
        {
            MapCoordinate testMapCoordinate = GetTestMapCoordinate();
            positionSystem.SetPosition(mover, testMapCoordinate);
            systemContainer.EntityEngine.AddComponent(mover, new Physical { Passable = false });

            var result = positionSystem.IsBlocked(testMapCoordinate, false, mover);

            result.Should().BeFalse();
        }

        private static PositionSystem SetUpTestPositionSystemWithPathfindingAlgorithm(out IPathfindingAlgorithm pathfindingAlgorithm)
        {
            pathfindingAlgorithm = Substitute.For<IPathfindingAlgorithm>();
            IEntityEngine entityEngine = Substitute.For<IEntityEngine>();
            IMapSystem mapSystem = Substitute.For<IMapSystem>();
            mapSystem.MapCollection.Returns(new MapCollection() {{new MapKey(TEST_MAP_NAME), Substitute.For<IMap>()}});
            var testPositionSystem = new PositionSystem(mapSystem, entityEngine, pathfindingAlgorithm);
            return testPositionSystem;
        }

        private IEntity SetUpTestMapCell(IMap map, int x, int y)
        {
            var mapCell = GetTestEntity(false);

            map.CellAt(x,y).Returns(mapCell);
            return mapCell;
        }

        private IMap SetUpTestMap()
        {
            var map = Substitute.For<IMap>();
            systemContainer.MapSystem.Initialise();
            systemContainer.MapSystem.MapCollection[TEST_MAP_KEY] = map;

            var cell = Substitute.For<IEntity>();
            map.CellAt(Arg.Any<int>(), Arg.Any<int>()).Returns(cell);
            cell.TryGet<Physical>().Returns(new Physical {Passable = true, Transparent = true});

            return map;
        }

        private IEntity GetTestEntity(bool withPosition = true)
        {
            var entity = systemContainer.EntityEngine.New("TestEntity");

            if (withPosition)
            {
                systemContainer.EntityEngine.AddComponent(entity, GetTestPosition());
            }

            return entity;
        }

        private static Position GetTestPosition()
        {
            return new Position{MapCoordinate = GetTestMapCoordinate()};
        }

        private static MapCoordinate GetTestMapCoordinate(string key = TEST_MAP_NAME, int x = 0, int y = 0)
        {
            return new MapCoordinate(key, x, y);
        }
    }
}