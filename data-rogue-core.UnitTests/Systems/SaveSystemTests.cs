using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
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
    public class SaveSystemTests
    {
        private ISystemContainer _systemContainer;
        private IEntityEngine _entityEngine;
        private List<IEntity> _mutableEntities;
        private IWorldGenerator _worldGenerator;
        private ISaveSystem _saveSystem;
        private IMapSystem _mapSystem;
        private MapCollection _mapCollection;
        private IMessageSystem _messageSystem;
        private List<Message> _messages;
        private ITimeSystem _timeSystem;
        private ulong _currentTime;

        [SetUp]
        public void SetUp()
        {
            _systemContainer = Substitute.For<ISystemContainer>();

            _entityEngine = Substitute.For<IEntityEngine>();
            _systemContainer.EntityEngine.Returns(_entityEngine);
            _mutableEntities = new List<IEntity>();
            _entityEngine.MutableEntities.Returns(_mutableEntities);

            _mapSystem = Substitute.For<IMapSystem>();
            _systemContainer.MapSystem.Returns(_mapSystem);
            _mapCollection = new MapCollection();
            _mapSystem.MapCollection.Returns(_mapCollection);

            _messageSystem = Substitute.For<IMessageSystem>();
            _systemContainer.MessageSystem.Returns(_messageSystem);
            _messages = new List<Message>();
            _messageSystem.AllMessages.Returns(_messages);

            _timeSystem = Substitute.For<ITimeSystem>();
            _systemContainer.TimeSystem.Returns(_timeSystem);
            _currentTime = 0;
            _timeSystem.CurrentTime.Returns(_currentTime);

            _worldGenerator = Substitute.For<IWorldGenerator>();

            _saveSystem = new SaveSystem(_systemContainer, _worldGenerator);
        }

        [Test]
        public void SaveSystem_GetSaveState_NoData_ReturnsBlankSaveState()
        {
            var result = _saveSystem.GetSaveState();

            var expected = GetBlankSaveState();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void SaveSystem_GetSaveState_WithEntity_ReturnsSaveStateWithSerialisedEntity()
        {
            var entity = new Entity(0, "New Entity", new IEntityComponent[0]);
            _mutableEntities.Add(entity);

            var result = _saveSystem.GetSaveState();

            var expected = EntitySerializer.Serialize(entity);

            result.Entities.Single().Should().Be(expected);
        }

        [Test]
        public void SaveSystem_GetSaveState_WithMap_ReturnsSaveStateWithSerialisedMap()
        {
            var mapCell = new Entity(0, "Map Cell", new IEntityComponent[] { new Appearance(), new Physical() });

            _mutableEntities.Add(mapCell);
            var map = new Map("key", mapCell);

            _mapCollection.Add(map.MapKey, map);

            var result = _saveSystem.GetSaveState();

            var expected = MapSerializer.Serialize(map);

            result.Maps.Single().Should().Be(expected);
        }

        [Test]
        public void SaveSystem_SerialisesCurrentTime()
        {
            _systemContainer.TimeSystem.CurrentTime = 3;
            
            var result = _saveSystem.GetSaveState();

            result.Time.Should().Be(3);
        }

        [Test]
        public void SaveSystem_WithMessage_SerialisesMessage()
        {
            var message = new Message {Text = "message", Color = Color.AliceBlue};

            _messages.Add(message);

            var result = _saveSystem.GetSaveState();

            result.Messages.Single().Should().Be(MessageSerializer.Serialize(message));
        }

        private static SaveState GetBlankSaveState()
        {
            return new SaveState
            {
                Entities = new List<string>(),
                Maps = new List<string>(),
                Messages = new List<string>(),
                Seed = null,
                Time = 0
            };
        }
    }
}
