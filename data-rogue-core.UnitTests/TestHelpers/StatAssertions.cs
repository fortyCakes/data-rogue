using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using RogueSharp.DiceNotation;

namespace data_rogue_core.UnitTests.TestHelpers
{
    static class StatAssertions
    {
        public static void StatShouldBe(this DiceExpression stat, string expected)
        {
            stat.ToString().ShouldBeEquivalentTo(Dice.Parse(expected).ToString());
        }
    }
}
