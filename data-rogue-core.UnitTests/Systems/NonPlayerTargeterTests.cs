using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class NonPlayerTargeterTests
    {
        private ISystemContainer _systemContainer;
        private IPositionSystem _positionSystem;
        private IEntity _sender;
        private TargetingData _data;

        [SetUp]
        public void SetUp()
        {
            _systemContainer = Substitute.For<ISystemContainer>();
            _positionSystem = Substitute.For<IPositionSystem>();
            _systemContainer.PositionSystem.ReturnsForAnyArgs(_positionSystem);

            _sender = CreateSkillUser();

            _data = CreateTargetingData();
        }

        private TargetingData CreateTargetingData()
        {
            return new TargetingData();
        }

        private static IEntity CreateSkillUser()
        {
            return Substitute.For<IEntity>();
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_HostileAndPlayerInRange_TargetsPlayer()
        {
            NonPlayerTargeter.GetTargetForNonPlayer(_systemContainer, _sender, _data, null);

            throw new NotImplementedException();
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_MeleeRangeSkill_HostileAndPlayerInMelee_TargetsPlayer()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void NonPlayerTargeter_GetTargetForNonPlayer_HostileAndPlayerNotInRange_DoesNotCallBack()
        {
            throw new NotImplementedException();
        }
    }
}
