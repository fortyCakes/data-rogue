using System;
using data_rogue_core.Monsters;
using data_rogue_core.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using RogueSharp.DiceNotation;

namespace data_rogue_core.UnitTests.Monsters
{
    [TestFixture]
    public class MonsterFactoryDataParserTests
    {
        private MonsterFactoryDataParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MonsterFactoryDataParser();
        }

        [Test]
        public void GetMonsterFactory_WithTestData_ReturnsDefaultMonsterFactory()
        {
            var testJson = GetTestJson();
            var result = _parser.GetMonsterFactory(testJson);

            result.Should().BeOfType<DefaultMonsterFactory>();
        }

        [Test]
        public void GetMonsterFactory_WithTestData_HasCorrectStats()
        {
            var testJson = GetTestJson();
            DefaultMonsterFactory result = _parser.GetMonsterFactory(testJson) as DefaultMonsterFactory;

            result.Attack.StatShouldBe( "1d3");
            result.AttackChance.StatShouldBe("25d3");
            result.Defense.StatShouldBe("1d3");
            result.DefenseChance.StatShouldBe("10d4");
            result.Awareness.StatShouldBe("10");
            result.Speed.StatShouldBe("14");
            result.Gold.StatShouldBe("5d5");
            result.Health.StatShouldBe("2d5");
        }

        

        private string GetTestJson()
        {
            return 
@"{
  'name': 'TestMonster',
  'Attack':  '1d3',
  'AttackChance' : '25d3',
  'Awareness' : 10,
  'Color' : 'KoboldColor',
  'Defense':  '1d3',
  'DefenseChance': '10d4',
  'Gold' : '5d5',
  'Health' : '2d5',
  'Speed' : 14,
  'Symbol' : 'k'
}";
        }

    }
}
