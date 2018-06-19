using System;
using System.Collections.Generic;
using System.Diagnostics;
using data_rogue_core.Entities;
using data_rogue_core.Monsters;
using data_rogue_core.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NUnit.Framework;
using RogueSharp.DiceNotation;
using RogueSharp.Random;

namespace data_rogue_core.UnitTests.Monsters
{
    [TestFixture]
    public class RandomMonsterGeneratorTests
    {

        private RandomMonsterGenerator _generator;
        private IMonsterFactory _factoryA;
        private IMonsterFactory _factoryB;
        private IRandom _random;

        [SetUp]
        public void Setup()
        {
            _factoryA = Substitute.For<IMonsterFactory>();
            _factoryA.Is("testTag").Returns(false);
            _factoryA.GetMonster().Returns(new Monster() {Name = "A"});

            _factoryB = Substitute.For<IMonsterFactory>();
            _factoryB.Is("testTag").Returns(true);
            _factoryB.GetMonster().Returns(new Monster() { Name = "B" });

            _random = Substitute.For<IRandom>();
            _random.Next(1).Returns(0);
            _generator = new RandomMonsterGenerator(new List<IMonsterFactory>{_factoryA, _factoryB}, _random);
        }



        [Test]
        public void GetNewMonster_ReturnsMonster()
        {
            var monster = _generator.GetNewMonster();
            monster.Name.Should().Be("A");
        }

        [Test]
        public void GetNewMonsterWithTag_ReturnsMonster()
        {
            var monster = _generator.GetNewMonsterWithTag(new List<string> {"testTag"});
            monster.Name.Should().Be("B");
        }

        [Test]
        public void GetNewMonsterWithTag_DoesNotExist_ReturnsNull()
        {
            var monster = _generator.GetNewMonsterWithTag(new List<string> { "testTagDoesNotExist" });
            monster.Should().BeNull();
        }


    }
}
