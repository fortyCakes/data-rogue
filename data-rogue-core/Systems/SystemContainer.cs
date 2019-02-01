using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace data_rogue_core.Systems
{
    public class SystemContainer : ISystemContainer
    {
        public IEntityEngine EntityEngine { get; set; }
        public IEventSystem EventSystem { get; set; }
        public IPositionSystem PositionSystem { get; set; }
        public IPlayerControlSystem PlayerControlSystem { get; set; }
        public IPrototypeSystem PrototypeSystem { get; set; }
        public IFighterSystem FighterSystem { get; set; }
        public IMessageSystem MessageSystem { get; set; }
        public ITimeSystem TimeSystem { get; set; }
        public IBehaviourFactory BehaviourFactory { get; set; }
        public IRandom Random { get; set; }
        public IScriptExecutor ScriptExecutor { get; set; }
        public ISkillSystem SkillSystem { get; set; }
        public ITargetingSystem TargetingSystem { get; set; }

        public string Seed { get; set; }

        public void CreateSystems(string rngSeed)
        {
            MessageSystem = new MessageSystem();

            EntityEngine = new EntityEngine.EntityEngine(new DataStaticEntityLoader());

            EventSystem = new EventSystem.EventSystem();

            PositionSystem = new PositionSystem();
            EntityEngine.Register(PositionSystem);

            Random = new RNG(rngSeed);

            TimeSystem = new TimeSystem(BehaviourFactory, EventSystem);
            EntityEngine.Register(TimeSystem);

            FighterSystem = new FighterSystem(EntityEngine, MessageSystem, EventSystem, TimeSystem);
            EntityEngine.Register(FighterSystem);

            PlayerControlSystem = new PlayerControlSystem(PositionSystem, EventSystem, TimeSystem);

            BehaviourFactory = new BehaviourFactory(PositionSystem, EventSystem, Random);

            PrototypeSystem = new PrototypeSystem(EntityEngine, PositionSystem, BehaviourFactory);
            EntityEngine.Register(PrototypeSystem);

            ScriptExecutor = new ScriptExecutor(this);

            SkillSystem = new SkillSystem(PrototypeSystem, ScriptExecutor, EventSystem);
            EntityEngine.Register(SkillSystem);

            Seed = rngSeed;

            TargetingSystem = new TargetingSystem(PositionSystem);

            Verify();
        }

        public void Verify()
        {
            var valid = true;
            var msg = new StringBuilder();

            Check(EntityEngine, "EntityEngine", msg, ref valid);
            Check(EventSystem, "EventSystem", msg, ref valid);
            Check(PositionSystem, "PositionSystem", msg, ref valid);
            Check(PlayerControlSystem, "PlayerControlSystem", msg, ref valid);
            Check(PrototypeSystem, "PrototypeSystem", msg, ref valid);
            Check(FighterSystem, "FighterSystem", msg, ref valid);
            Check(MessageSystem, "MessageSystem", msg, ref valid);
            Check(TimeSystem, "TimeSystem", msg, ref valid);
            Check(BehaviourFactory, "BehaviourFactory", msg, ref valid);
            Check(Random, "Random", msg, ref valid);
            Check(ScriptExecutor, "ScriptExecutor", msg, ref valid);
            Check(SkillSystem, "SkillSystem", msg, ref valid);
            Check(TargetingSystem, "TargetingSystem", msg, ref valid);

            if (!valid)
                throw new ContainerNotValidException(msg.ToString());
        }

        public void Check(object toCheck, string fieldName, StringBuilder msg, ref bool valid)
        {
            if (toCheck == null)
            {
                valid = false;
                msg.AppendLine($"{fieldName} is null");
            }
        }
    }

    [Serializable]
    public class ContainerNotValidException : Exception
    {
        public ContainerNotValidException()
        {
        }

        public ContainerNotValidException(string message) : base(message)
        {
        }

        public ContainerNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContainerNotValidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

