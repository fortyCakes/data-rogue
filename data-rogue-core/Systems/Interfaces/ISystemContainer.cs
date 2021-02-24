using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.EventSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ISystemContainer
    {

        string Seed { get; set; }

        IEntityEngine EntityEngine {get; set;}
        IEventSystem EventSystem {get; set;}
        IPositionSystem PositionSystem {get; set;}
        IControlSystem ControlSystem {get; set;}
        IPrototypeSystem PrototypeSystem {get; set;}
        IFighterSystem FighterSystem {get; set;}
        IMessageSystem MessageSystem {get; set;}
        ITimeSystem TimeSystem {get; set;}
        IRandom Random {get; set;}
        IScriptExecutor ScriptExecutor {get; set;}
        ISkillSystem SkillSystem {get; set;}
        ITargetingSystem TargetingSystem { get; set; }
        IItemSystem ItemSystem { get; set; }
        IEquipmentSystem EquipmentSystem { get; set; }
        IPlayerSystem PlayerSystem { get; set; }
        IActivitySystem ActivitySystem { get; set; }
        IRendererSystem RendererSystem { get; set; }
        IAnimationSystem AnimationSystem { get; set; }
        IAnimatedMovementSystem AnimatedMovementSystem { get; set; }
        IMapSystem MapSystem { get; set; }
        ISaveSystem SaveSystem {get;set;}
        IStatSystem StatSystem { get; }
        ISoundSystem SoundSystem { get; }
        IParticleSystem ParticleSystem { get; }
        IInteractionSystem InteractableSystem { get; }
        IFactionSystem FactionSystem { get; }

        void Verify();
        void CreateSystems(string rngSeed);
    }
}
