using data_rogue_core.EntitySystem;

namespace data_rogue_core.Maps.MapGenCommands
{
    public interface IMapGenCommandExecutor
    {
        void Execute(Map map, IEntityEngineSystem entityEngineSystem, MapGenCommand command, Vector offset);
    }
}
