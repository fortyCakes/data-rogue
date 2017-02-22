using System;
using data_rogue_core.Map.Vaults;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RogueSharp.Random;

namespace data_rogue_core.UnitTests.Map.Vaults
{
    [TestFixture]
    public class VaultDataParserTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void GetVault_ReturnsDungeonMap()
        {
            var parser = new VaultDataParser();
            var testJson = GetTestJson();

            var action = new Action(() => parser.ParseVault(testJson));

            action.ShouldNotThrow();
        }

        private string GetTestJson()
        {
            return @"{""map"":""
......
......
~~~~~~
......
......
""}";
        }
    }
}
