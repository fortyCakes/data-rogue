using System;
using data_rogue_core.Entities;
using NSubstitute;
using NUnit.Framework;
using RLNET;
using RogueSharp;

namespace data_rogue_core.UnitTests.Entities
{
    [TestFixture]
    public class ActorTests
    {
        private IMap _map;

        [SetUp]
        public void Setup()
        {
            _map = Substitute.For<IMap>();
        }

        [Test]
        public void Draw_WhenCellIsNotExplored_DoesNotSetConsole()
        {
            RLConsole console = Substitute.For<RLConsole>(1, 1);
            console.Set(1, 1, RLColor.Black, RLColor.White, 'x');

            Cell cell = new Cell(1,1,false,false,false,false);

            _map.GetCell(1, 1).Returns(cell);

            Actor actor = new Actor {X = 1, Y = 1};

            actor.Draw(console, _map);

            throw new NotImplementedException();
        }
    }
}