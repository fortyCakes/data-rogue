using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core
{

    public interface IStatSystem
    {
        int GetEntityStat(IEntity entity, string statName);

        void SetStat(IEntity entity, string statName, int value);
        void IncreaseStat(IEntity entity, string statName, int value);
    }
}