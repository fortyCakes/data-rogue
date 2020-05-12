using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class NonPlayerTargeterTests
    {
        private const string MAP_KEY = "test";

        private ISystemContainer _systemContainer;
        private IPositionSystem _positionSystem;
        private IEntity _sender;
        private TargetingData _data;
        private IEntity _player;
        private IPlayerSystem _playerSystem;
        private IMapSystem _mapSystem;
        private MapCollection _mapCollection;
        private IMap _map;
        private MapCoordinate UNUSED;
        private MapCoordinate _callbackCoordinate;
        private Action<MapCoordinate> _callback;
        private MapCoordinate _senderLocation;

        [SetUp]
        public void SetUp()
        {
            _sender = CreateSkillUser();
            _player = CreatePlayer();

            _systemContainer = Substitute.For<ISystemContainer>();
            _positionSystem = Substitute.For<IPositionSystem>();
            _playerSystem = Substitute.For<IPlayerSystem>();
            _mapSystem = Substitute.For<IMapSystem>();
            _mapCollection = new MapCollection();
            _map = Substitute.For<IMap>();
            _systemContainer.PositionSystem.ReturnsForAnyArgs(_positionSystem);
            _systemContainer.PlayerSystem.ReturnsForAnyArgs(_playerSystem);
            _systemContainer.MapSystem.ReturnsForAnyArgs(_mapSystem);
            _systemContainer.Random.ReturnsForAnyArgs(new RNG("test seed"));
            _mapSystem.MapCollection.ReturnsForAnyArgs(_mapCollection);
            _mapCollection[new MapKey("test")] = _map;
            
            _playerSystem.Player.ReturnsForAnyArgs(_player);

            _data = CreateTargetingData();

            UNUSED = new MapCoordinate("UNUSED", 0, 0);
            _callbackCoordinate = UNUSED;

            _callback = m =>
            {
                _callbackCoordinate = m;
            };

            _senderLocation = new MapCoordinate(MAP_KEY, 0, 0);

            SetPosition(_sender, _senderLocation);
        }

        private void FovIs(params MapCoordinate[] coordinates)
        {
            _map.FovFrom(Arg.Any<IPositionSystem>(), Arg.Any<MapCoordinate>(), Arg.Any<int>()).ReturnsForAnyArgs(coordinates.ToList());
        }

        private TargetingData CreateTargetingData()
        {
            return new TargetingData();
        }

        private static IEntity CreateSkillUser()
        {
            var substitute = Substitute.For<IEntity>();

            substitute.DescriptionName.ReturnsForAnyArgs("sender");
            substitute.Has<Health>().Returns(true);

            return substitute;
        }

        private static IEntity CreatePlayer()
        {
            var substitute = Substitute.For<IEntity>();

            substitute.Has<Health>().Returns(true);
            substitute.DescriptionName.ReturnsForAnyArgs("player");

            return substitute;
        }

        private void SetPosition(IEntity of, MapCoordinate coordinate)
        {
            _positionSystem.CoordinateOf(of).Returns(coordinate);
            _positionSystem.EntitiesAt(coordinate).Returns(new List<IEntity> {of});
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_HostileAndPlayerInRange_TargetsPlayer()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 2);
            SetPosition(_player, playerLocation);
            _data.Range = 5;
            FovIs(new MapCoordinate("test", 0, 2));

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(playerLocation);
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_MeleeRangeSkill_HostileAndPlayerInMelee_TargetsPlayer()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 1);
            SetPosition(_player, playerLocation);
            _data.Range = 0;
            FovIs(new MapCoordinate("test", 0, 1));

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(playerLocation);
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_MeleeRange_HostileAndPlayerNotInRange_DoesNotCallBack()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 2);
            SetPosition(_player, playerLocation);

            _data.Range = 0;

            FovIs(playerLocation);

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(UNUSED);
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_HostileAndPlayerNotInRange_DoesNotCallBack()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 5);
            SetPosition(_player, playerLocation);
            _data.Range = 3;
            FovIs(playerLocation);

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(UNUSED);
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_PlayerInRangeButNotInFov_DoesNotCallBack()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 5);
            SetPosition(_player, playerLocation);
            _data.Range = 9;
            FovIs(new MapCoordinate("test", 4, 4));

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(UNUSED);
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_FriendlySkill_TargetsSelfAndNotPlayer()
        {
            var playerLocation = new MapCoordinate(MAP_KEY, 0, 5);
            SetPosition(_player, playerLocation);
            _data.Range = 9;
            _data.Friendly = true;
            FovIs(new MapCoordinate("test", 0, 5), _senderLocation);

            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, _callback);

            _callbackCoordinate.Should().BeEquivalentTo(_senderLocation);
        }
    }
}
