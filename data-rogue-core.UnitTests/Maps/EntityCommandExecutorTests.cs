using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Maps.MapGenCommands;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.UnitTests.Maps
{
    [TestFixture]
    public class EntityCommandExecutorTests
    {
        private ISystemContainer _systemContainer;
        private IPrototypeSystem _prototypeSystem;
        private EntityCommandExecutor _executor;
        private IEntity _defaultCell;
        private Map _map;
        private readonly string MAP_KEY = "MAP_KEY";

        [SetUp]
        public void SetUp()
        {
            _systemContainer = Substitute.For<ISystemContainer>();
            _prototypeSystem = Substitute.For<IPrototypeSystem>();
            _systemContainer.PrototypeSystem = _prototypeSystem;
            _executor = new EntityCommandExecutor();
            _defaultCell = Substitute.For<IEntity>();
            _defaultCell.Has<Physical>().ReturnsForAnyArgs(true);
            _defaultCell.Has<Appearance>().ReturnsForAnyArgs(true);
            _map = new Map(MAP_KEY, _defaultCell);
        }

        [Test]
        public void Execute_WithEntityName_CreatesEntity()
        {
            var entityName = "Banana";
            var command = new MapGenCommand
            {
                MapGenCommandType = MapGenCommandType.Entity,
                Parameters = entityName,
                Vector = new Vector(0, 0)
            };

            _executor.Execute(_systemContainer, _map, command, command.Vector);

            _prototypeSystem.Received().CreateAt(entityName, new MapCoordinate(MAP_KEY, 0, 0));
        }

        [Test]
        public void Execute_WithEntityNameAndProperty_CreatesEntityAndSetsProperty()
        {
            var entityName = "Sign";
            var entity = new Entity(0, entityName, new[] { new Dialog() });

            _prototypeSystem.CreateAt(entityName, new MapCoordinate(MAP_KEY, 0, 0)).Returns(entity);

            
            var command = new MapGenCommand
            {
                MapGenCommandType = MapGenCommandType.Entity,
                Parameters = $"{entityName}|Dialog.Text=I am a sign!",
                Vector = new Vector(0, 0)
            };

            _executor.Execute(_systemContainer, _map, command, command.Vector);

            _prototypeSystem.Received().CreateAt(entityName, new MapCoordinate(MAP_KEY, 0, 0));

            entity.Get<Dialog>().Text.Should().Be("I am a sign!");
        }
    }
}
