using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;
using NUnit.Framework;
using data_rogue_core.EventSystem;
using FluentAssertions;
using NSubstitute;

namespace data_rogue_core.UnitTests.EventSystem
{
    [TestFixture]
    public class EventSystemTests
    {
        private IEntity _entity;
        public data_rogue_core.EventSystem.EventSystem _eventSystem;

        [SetUp]
        public void SetUp()
        {
            _eventSystem = new data_rogue_core.EventSystem.EventSystem();
            _eventSystem.Initialise();

            _entity = Substitute.For<IEntity>();
        }

        [Test]
        public void Try_NoRules_Succeeds()
        {
            var result = _eventSystem.Try(EventType.Action, _entity, null);

            result.Should().BeTrue();
        }
    }
}
