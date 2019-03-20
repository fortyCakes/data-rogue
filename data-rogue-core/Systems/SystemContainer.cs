using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Text;
using data_rogue_core.Utils;

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
        public IStatSystem StatSystem { get; set; }
        public ISaveSystem SaveSystem { get; set; }

        public string Seed { get; set; }

        public void CreateSystems(string rngSeed)
        {
            ScriptExecutor = new ScriptExecutor(this);
            EventSystem = new EventSystem.EventSystem();

            MessageSystem = new MessageSystem();
            ActivitySystem = new ActivitySystem();
            PlayerSystem = new PlayerSystem(ActivitySystem);
            MapSystem = new MapSystem();


            RendererSystem = new RendererSystem(PlayerSystem);

            EntityEngine = new EntityEngine(new DataStaticEntityLoader());

            StatSystem = new StatSystem(EntityEngine);

            PositionSystem = new PositionSystem(MapSystem, EntityEngine, new AStarPathfindingAlgorithm());
            EntityEngine.Register(PositionSystem);

            Random = new RNG(rngSeed);

            TimeSystem = new TimeSystem(BehaviourFactory, EventSystem, PlayerSystem);
            EntityEngine.Register(TimeSystem);

            FighterSystem = new FighterSystem(EntityEngine, MessageSystem, EventSystem, TimeSystem, StatSystem);
            EntityEngine.Register(FighterSystem);

            BehaviourFactory = new BehaviourFactory(PositionSystem, EventSystem, Random, MessageSystem, PlayerSystem, MapSystem);

            PrototypeSystem = new PrototypeSystem(EntityEngine, PositionSystem, BehaviourFactory);
            EntityEngine.Register(PrototypeSystem);

            ItemSystem = new ItemSystem(EntityEngine, PrototypeSystem, ScriptExecutor, MessageSystem, EventSystem);
            EntityEngine.Register(ItemSystem);

            SkillSystem = new SkillSystem(this);
            EntityEngine.Register(SkillSystem);

            Seed = rngSeed;

            EquipmentSystem = new EquipmentSystem(this);

            TargetingSystem = new TargetingSystem(this);

            PlayerControlSystem = new PlayerControlSystem(this);

            SaveSystem = new SaveSystem(this);

            EntityEngine.Initialise(this);
            PlayerControlSystem.Initialise();

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
            Check(MapSystem, "MapSystem", msg, ref valid);
            Check(RendererSystem, "RendererSystem", msg, ref valid);
            Check(SaveSystem, "SaveSystem", msg, ref valid);
            Check(StatSystem, "StatSystem", msg, ref valid);

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

