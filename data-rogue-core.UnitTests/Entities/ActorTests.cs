using System;
using data_rogue_core.Display;
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
        IRLConsoleWriter _console;

        [SetUp]
        public void Setup()
        {
            
            _console = Substitute.For<IRLConsoleWriter>();
            _map = Substitute.For<IMap>();
        }

        [Test]
        public void Draw_WhenCellIsNotExplored_DoesNotSetConsole()
        {
            Cell cell = new Cell(1,1,false,false,false,false);

            _map.GetCell(1, 1).Returns(cell);

            Actor actor = new Actor {X = 1, Y = 1};

            actor.Draw(_console, _map, 1, 1);

            _console.DidNotReceiveWithAnyArgs().Set(Arg.Any<int>(),Arg.Any<int>(), Colors.Floor, Colors.FloorBackground, Arg.Any<char>());
        }

        [Test]
        public void Draw_WhenCellIsNotInFov_ShowsFloor()
        {
            Cell cell = new Cell(1, 1, false, false, false, true);

            _map.GetCell(1, 1).Returns(cell);

            Actor actor = new Actor { X = 1, Y = 1 };

            actor.Draw(_console, _map, 0, 0);

            _console.Received().Set(1, 1, Colors.Floor, Colors.FloorBackground, '.');
        }

        [Test]
        public void Draw_WhenCellIsInFov_ShowsActor()
        {
            Cell cell = new Cell(1, 1, false, false, true, true);

            _map.GetCell(1, 1).Returns(cell);
            _map.IsInFov(1, 1).ReturnsForAnyArgs(true);

            Actor actor = new Actor { X = 1, Y = 1, Symbol = ':', Color = Colors.Player};

            actor.Draw(_console, _map, 0, 0);

            _console.Received().Set(1, 1, Colors.Player, Colors.FloorBackgroundFov, ':');
        }
    }
}