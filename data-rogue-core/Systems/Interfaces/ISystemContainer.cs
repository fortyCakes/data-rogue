using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;
using data_rogue_core.Renderers;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISystemContainer
    {
        IEntityEngine EntityEngine {get; set;}
        IEventSystem EventSystem {get; set;}
        IPositionSystem PositionSystem {get; set;}
        IPlayerControlSystem PlayerControlSystem {get; set;}
        IPrototypeSystem PrototypeSystem {get; set;}
        IFighterSystem FighterSystem {get; set;}
        IMessageSystem MessageSystem {get; set;}
        ITimeSystem TimeSystem {get; set;}
        IBehaviourFactory BehaviourFactory {get; set;}
        IRandom Random {get; set;}
        IScriptExecutor ScriptExecutor {get; set;}
        ISkillSystem SkillSystem {get; set;}
        ITargetingSystem TargetingSystem { get; set; }
        IItemSystem ItemSystem { get; set; }
        IEquipmentSystem EquipmentSystem { get; set; }
        IPlayerSystem PlayerSystem { get; set; }
        IActivitySystem ActivitySystem { get; set; }
        string Seed { get; set; }
        IRendererSystem RendererSystem { get; set; }
        IMapSystem MapSystem { get; set; }

        void Verify();
        void CreateSystems(string rngSeed);
    }
}
