using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using NLua;
using System.Collections.Generic;
using data_rogue_core.Components;
using FluentAssertions;
using data_rogue_core.EventSystem;
using data_rogue_core.EventSystem.EventData;
using data_rogue_core.Data;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class ScriptExecutorTests
    {
        private IScriptExecutor ScriptExecutor;
        private ISystemContainer SystemContainer;
        private IEntity TestEntity;
        private IEntity TestSkill;
        private IEntity TestMapCell;

        [SetUp]
        public void SetUp()
        {
            SystemContainer = new SystemContainer();
            SystemContainer.CreateSystems("TEST SEED");

            ScriptExecutor = SystemContainer.ScriptExecutor;

            TestEntity = SystemContainer.EntityEngine.New("Test entity");

            TestSkill = SystemContainer.EntityEngine.New("Test skill");

            SystemContainer.MapSystem.Initialise();
            TestMapCell = SystemContainer.EntityEngine.New("Test map cell", new Physical(), new Appearance(), new Position());
            SystemContainer.MapSystem.MapCollection.AddMap(new Map("test map key", TestMapCell));
        }

        [Test]
        public void NLua_Works()
        {
            var lua = new Lua();
        }

        [Test]
        public void Execute_EmptyScript_Works()
        {
            
            ScriptExecutor.Execute(TestEntity, "", null);
        }

        [Test]
        public void Execute_MessageScript_WritesMessage()
        {
            SystemContainer.MessageSystem = Substitute.For<IMessageSystem>();

            ScriptExecutor.Execute(TestEntity, "SystemContainer.MessageSystem:Write('test')", null);

            SystemContainer.MessageSystem.Received(1).Write("test");
        }

        [Test]
        public void Execute_ScriptWithTarget_GetsCallback()
        {
            SystemContainer.MessageSystem = Substitute.For<IMessageSystem>();
            SystemContainer.TargetingSystem = Substitute.For<ITargetingSystem>();

            SystemContainer.EntityEngine.AddComponent(TestSkill, new Targeting { Range = 25 });

            var mapCoordinate = new MapCoordinate(new MapKey("test map key"), new Vector(0, 0));
            Targeting targetingData = null;

            SystemContainer.TargetingSystem
                .WhenForAnyArgs(a => a.GetTarget(Arg.Any<IEntity>(), Arg.Any<Targeting>(), Arg.Any<Action<MapCoordinate>>()))
                .Do(a => {

                    a.ArgAt<Action<MapCoordinate>>(2).Invoke(mapCoordinate);
                    targetingData = a.ArgAt<Targeting>(1);
                    }); 


            var script = @"
                withTarget(
                    function(arg)
                        SystemContainer.MessageSystem:Write('Recieved target at ' .. arg:ToString())
                end)

                requestTarget(User, Entity)
            ";

            ScriptExecutor.Execute(TestEntity, script, TestSkill);

            SystemContainer.MessageSystem.Received(1).Write("Recieved target at " + mapCoordinate.ToString());
            targetingData.Range.Should().Be(25);
        }

        [Test]
        public void Execute_ScriptWithMakeAttack_MakesAttack()
        {
            SystemContainer.TargetingSystem = Substitute.For<ITargetingSystem>();
            var eventSystem = Substitute.For<IEventSystem>();
            SystemContainer.EventSystem = eventSystem;

            var mapCoordinate = new MapCoordinate(new MapKey("test map key"), new Vector(0, 0));
            SystemContainer.EntityEngine.AddComponent(TestEntity, new Position { MapCoordinate = mapCoordinate });
            SystemContainer.EntityEngine.AddComponent(TestEntity, new Health { HP = new Counter { Max = 100, Current = 100 } });


            SystemContainer.EntityEngine.AddComponent(TestSkill, new Targeting());


            SystemContainer.EntityEngine.AddComponent(TestSkill, new AttackDefinition());

            Targeting targetingData = null;

            SystemContainer.TargetingSystem
                .WhenForAnyArgs(a => a.GetTarget(Arg.Any<IEntity>(), Arg.Any<Targeting>(), Arg.Any<Action<MapCoordinate>>()))
                .Do(a => {

                    a.ArgAt<Action<MapCoordinate>>(2).Invoke(mapCoordinate);
                    targetingData = a.ArgAt<Targeting>(1);
                });

            var script = @"
                withTarget(
                    function(target)
                        targetEntities = SystemContainer.PositionSystem:EntitiesAt(target)
                        for defender in each(targetEntities) do
                            
			                makeAttack(User, defender, Entity)

                        end
                end)

                requestTarget(User, Entity)
            ";

            ScriptExecutor.Execute(TestEntity, script, TestSkill);

            eventSystem.Received(1).Try(EventType.Attack, TestEntity, Arg.Any<AttackEventData>());
        }

    }
}
