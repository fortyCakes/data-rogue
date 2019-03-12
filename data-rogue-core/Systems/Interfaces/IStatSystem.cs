using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Forms.StaticForms;

namespace data_rogue_core
{

    public interface IStatSystem
    {
        int GetStat(IEntity entity, string statName);

        void SetStat(IEntity entity, string statName, int value);
        void IncreaseStat(IEntity entity, string statName, int value);
    }
}