using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Systems;
using FluentAssertions;
using NUnit.Framework;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class PlayerControlSystemTests
    {
        [Test]
        public void ExtractActionType_NoParameters_Extracts()
        {
            var result = PlayerControlSystem.ExtractActionType("InventoryMenu");

            result.Should().Be(ActionType.InventoryMenu);
        }

        [Test]
        public void ExtractActionType_WithParameters_Extracts()
        {
            var result = PlayerControlSystem.ExtractActionType("Move(1,0)");

            result.Should().Be(ActionType.Move);
        }

        [Test]
        public void ExtractParameters_NoParameters_Extracts()
        {
            var result = PlayerControlSystem.ExtractParameters("InventoryMenu");

            result.Should().Be(null);
        }

        [Test]
        public void ExtractParameters_WithParameters_Extracts()
        {
            var result = PlayerControlSystem.ExtractParameters("Move(1,0)");

            result.Should().Be("1,0");
        }
    }
}
