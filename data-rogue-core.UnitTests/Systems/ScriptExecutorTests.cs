using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using NLua;
using System.Collections.Generic;

namespace data_rogue_core.UnitTests.Systems
{
    [TestFixture]
    public class ScriptExecutorTests
    {
        private IScriptExecutor ScriptExecutor;
        private ISystemContainer SystemContainer;
        private IEntity TestEntity;

        [SetUp]
        public void SetUp()
        {
            SystemContainer = new SystemContainer();
            SystemContainer.CreateSystems("TEST SEED");

            ScriptExecutor = SystemContainer.ScriptExecutor;

            TestEntity = SystemContainer.EntityEngine.New("Test entity");
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

            var mapCoordinate = new MapCoordinate(new MapKey("test map key"), new Vector(0, 0));

            SystemContainer.TargetingSystem
                .WhenForAnyArgs(a => a.GetTarget(Arg.Any<IEntity>(), Arg.Any<TargetingData>(), Arg.Any<Action<MapCoordinate>>()))
                .Do(a => a.ArgAt<Action<MapCoordinate>>(2).Invoke(mapCoordinate));


            var script = @"
                withTarget(
                    function(arg)
                        SystemContainer.MessageSystem:Write('Recieved target at ' .. arg:ToString())
                end)

                requestTarget(User, TargetingData())
            ";

            ScriptExecutor.Execute(TestEntity, script, null);

            SystemContainer.MessageSystem.Received(1).Write("Recieved target at " + mapCoordinate.ToString());
        }

    }
}
