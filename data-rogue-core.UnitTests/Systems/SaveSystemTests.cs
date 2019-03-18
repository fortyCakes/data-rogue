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
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class SaveSystemTests
    {
        private ISystemContainer _systemContainer;
        private ISaveSystem _saveSystem;

        [SetUp]
        public void SetUp()
        {
            _systemContainer = new SystemContainer();
            _systemContainer.CreateSystems("Test");
            _systemContainer.EntityEngine.Initialise(_systemContainer);

            _systemContainer.MapSystem.Initialise();

            _saveSystem = _systemContainer.SaveSystem;
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
            var entity = _systemContainer.EntityEngine.New("New Entity");

            var result = _saveSystem.GetSaveState();

            var expected = EntitySerializer.Serialize(entity);

            result.Entities.Single().Should().Be(expected);
        }

        [Test]
        public void SaveSystem_GetSaveState_WithMap_ReturnsSaveStateWithSerialisedMap()
        {
            var map = new Map("key", _systemContainer.EntityEngine.New("Map Cell", new Appearance(), new Physical()));

            _systemContainer.MapSystem.MapCollection.Add(map.MapKey, map);

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

            _systemContainer.MessageSystem.Write(message.Text, message.Color);

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
