﻿using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class StatSystemTests
    {
        private const string TEST_STAT = "TestStat";
        uint entityId;
        IEntity entity;

        private SystemContainer systemContainer;
        private IStatSystem statSystem;

        [SetUp]
        public void SetUp()
        {
            entityId = 0;

            systemContainer = new SystemContainer();

            systemContainer.CreateSystems("seed");

            systemContainer.EntityEngine.Initialise(systemContainer);

            entity = GetTestEntity();

            statSystem = systemContainer.StatSystem;
        }


        [Test]
        public void SetStat_HasStat_SetsStat()
        {
            var stat = new Stat { Name = TEST_STAT, Value = 5 };

            systemContainer.EntityEngine.AddComponent(entity, stat);

            statSystem.SetStat(entity, TEST_STAT, 10);

            stat.Value.Should().Be(10);
        }

        [Test]
        public void SetStat_NoStat_SetsStat()
        {
            statSystem.SetStat(entity, TEST_STAT, 10);

            statSystem.GetStat(entity, TEST_STAT).Should().Be(10);
        }

        [Test]
        public void GetStat_NoStat_ReturnsZero()
        {
            statSystem.GetStat(entity, TEST_STAT).Should().Be(0);
        }

        [Test]
        public void IncreaseStat_NoStat_SetsStat()
        {
            statSystem.IncreaseStat(entity, TEST_STAT, 10);

            statSystem.GetStat(entity, TEST_STAT).Should().Be(10);
        }

        [Test]
        public void IncreaseStat_HasStat_SetsStat()
        {
            var stat = new Stat { Name = TEST_STAT, Value = 5 };

            systemContainer.EntityEngine.AddComponent(entity, stat);

            statSystem.IncreaseStat(entity, TEST_STAT, 10);

            statSystem.GetStat(entity, TEST_STAT).Should().Be(15);
        }

        private IEntity GetTestEntity()
        {
            return systemContainer.EntityEngine.New("Learner");
        }
    }
}