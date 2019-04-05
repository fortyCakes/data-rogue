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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Try_OneRule_ReturnsSameAsRule(bool ruleSucceeds)
        {
            IEventRule rule = GetTestRule(ruleSucceeds);

            _eventSystem.RegisterRule(rule);

            var result = _eventSystem.Try(EventType.Action, _entity, null);

            result.Should().Be(ruleSucceeds);
        }

        [Test]
        public void Try_RuleWithHigherOrder_HappensFirst()
        {
            var rule1 = GetTestRule(ruleOrder: 1);
            var rule2 = GetTestRule(ruleOrder: 2);

            _eventSystem.RegisterRules(rule1, rule2);

            _eventSystem.Try(EventType.Action, _entity, null);

            Received.InOrder(() =>
            {
                rule2.Apply(Arg.Any<EventType>(), Arg.Any<IEntity>(), Arg.Any<object>());
                rule1.Apply(Arg.Any<EventType>(), Arg.Any<IEntity>(), Arg.Any<object>());
            });
        }

        [Test]
        public void Try_HigherOrderRuleFails_DoesNotExecuteLowerRules()
        {
            var rule1 = GetTestRule(ruleOrder: 1);

            var rule2 = GetTestRule(ruleSucceeds:false, ruleOrder: 2);

            _eventSystem.RegisterRules(rule1, rule2);

            _eventSystem.Try(EventType.Action, _entity, null);

            rule1.DidNotReceiveWithAnyArgs().Apply(Arg.Any<EventType>(), null, null);
        }

        private static IEventRule GetTestRule(bool ruleSucceeds = true, uint ruleOrder = 0)
        {
            var rule = Substitute.For<IEventRule>();
            rule.Apply(Arg.Any<EventType>(), null, null).ReturnsForAnyArgs(ruleSucceeds);
            rule.EventTypes.ReturnsForAnyArgs(new EventTypeList { EventType.Action });
            rule.RuleOrder.ReturnsForAnyArgs(ruleOrder);
            return rule;
        }
    }
}
