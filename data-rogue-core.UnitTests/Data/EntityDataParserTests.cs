using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Data
{
	[TestFixture]
    class EntityDataParserTests
    {
        private EntityDataParser _entityDataParser;
        private IEntityEngineSystem _EntityEngineSystem;

        [SetUp]
		public void SetUp()
        {
            _EntityEngineSystem = Substitute.For<IEntityEngineSystem>();
            _EntityEngineSystem.New(Arg.Any<string>(), Arg.Any<IEntityComponent[]>()).ReturnsForAnyArgs(callInfo =>
            {
				var entity = new Entity(0, callInfo.ArgAt<IEntityComponent[]>(1));
                entity.Name = callInfo.ArgAt<string>(0);
                return entity;
            });

			_entityDataParser = new EntityDataParser(new List<Type>() {typeof(Appearance)}, _EntityEngineSystem);
        }

        [Test]
		public void Load_TestEntity_Loads()
        {
            List<string> testData = new List<string>()
            {
                "\"TestEntity\"",
                "[Appearance]",
                "    Glyph: £"
            };

            var result = _entityDataParser.Parse(testData);

            var entity = result.Single();

            entity.Name.Should().Be("TestEntity");

            var appearance = entity.Get<Appearance>();

            appearance.Glyph.Should().Be('£');
        }

        [Test]
        public void Load_TestEntity_LoadsColor()
        {
            List<string> testData = new List<string>()
            {
                "\"TestEntity\"",
                "[Appearance]",
                "    Glyph: £",
				"    Color: #FF0000",
            };

            var result = _entityDataParser.Parse(testData);

            var entity = result.Single();

            entity.Name.Should().Be("TestEntity");

            var appearance = entity.Get<Appearance>();

            appearance.Color.R.Should().Be(255);
            appearance.Color.G.Should().Be(0);
            appearance.Color.B.Should().Be(0);
        }
    }
}
