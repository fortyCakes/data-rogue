using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class SystemContainerTests
    {
        private SystemContainer SystemContainer;

        [SetUp]
        public void SetUp()
        {
            SystemContainer = new SystemContainer();

            SystemContainer.CreateSystems("TEST SEED");
        }

        [Test]
        public void Verify_StandardSetup_Passes()
        {
            SystemContainer.Verify();
        }

        [Test]
        public void Verify_SystemMissing_Throws()
        {
            SystemContainer.EntityEngine = null;

            Action action = () => SystemContainer.Verify();

            action.Should().Throw<ContainerNotValidException>();
        }
    }
}
