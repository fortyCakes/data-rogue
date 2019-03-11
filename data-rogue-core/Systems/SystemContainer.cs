using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
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
        public IItemSystem ItemSystem { get; set; }
        public IEquipmentSystem EquipmentSystem { get; set; }
        public IPlayerSystem PlayerSystem { get; set; }
        public IActivitySystem ActivitySystem { get; set; }
        public IMapSystem MapSystem { get; set; }
        public IRendererSystem RendererSystem { get; set; }

        public string Seed { get; set; }

        public void CreateSystems(string rngSeed)
        {
            ScriptExecutor = new ScriptExecutor(this);

            MessageSystem = new MessageSystem();
            PlayerSystem = new PlayerSystem();
            MapSystem = new MapSystem();
            ActivitySystem = new ActivitySystem();

            EntityEngine = new EntityEngine(new DataStaticEntityLoader());

            EventSystem = new EventSystem.EventSystem();


            PositionSystem = new PositionSystem(MapSystem);
            EntityEngine.Register(PositionSystem);

            Random = new RNG(rngSeed);

            TimeSystem = new TimeSystem(BehaviourFactory, EventSystem, PlayerSystem);
            EntityEngine.Register(TimeSystem);

            FighterSystem = new FighterSystem(EntityEngine, MessageSystem, EventSystem, TimeSystem);
            EntityEngine.Register(FighterSystem);

            PlayerControlSystem = new PlayerControlSystem(this);

            BehaviourFactory = new BehaviourFactory(PositionSystem, EventSystem, Random, MessageSystem);

            PrototypeSystem = new PrototypeSystem(EntityEngine, PositionSystem, BehaviourFactory);
            EntityEngine.Register(PrototypeSystem);

            ItemSystem = new ItemSystem(EntityEngine, PrototypeSystem, ScriptExecutor, MessageSystem, EventSystem);
            EntityEngine.Register(ItemSystem);

            SkillSystem = new SkillSystem(this);
            EntityEngine.Register(SkillSystem);

            Seed = rngSeed;

            TargetingSystem = new TargetingSystem(this);

            EquipmentSystem = new EquipmentSystem(this);

            RendererSystem = new RendererSystem(PlayerSystem);

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
            Check(ItemSystem, "ItemSystem", msg, ref valid);
            Check(EquipmentSystem, "EquipmentSystem", msg, ref valid);
            Check(ActivitySystem, "ActivitySystem", msg, ref valid);
            Check(PlayerSystem, "PlayerSystem", msg, ref valid);

            if (!valid)
                throw new ContainerNotValidException(msg.ToString());
        }

        public void Check(object toCheck, string fieldName, StringBuilder msg, ref bool valid)
        {
            if (toCheck == null)
            {
                valid = false;
                msg.AppendLine($"SystemContainer not valid: {fieldName} is null");
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

