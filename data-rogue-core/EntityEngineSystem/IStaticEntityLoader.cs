using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.EntityEngineSystem
{
    public interface IStaticEntityLoader
    {
        void Load(ISystemContainer systemContainer);
    }
}