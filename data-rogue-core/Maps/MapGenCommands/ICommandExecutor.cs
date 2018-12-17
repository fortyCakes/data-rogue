using data_rogue_core.EntitySystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    public interface ICommandExecutor
    {
        MapGenCommandType CommandType { get; }

        void Execute(Map map, IEntityEngineSystem entityEngineSystem, IPrototypeSystem prototypeSystem, MapGenCommand command, Vector offset);
    }
}