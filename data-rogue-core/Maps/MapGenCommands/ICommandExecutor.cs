using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    public interface ICommandExecutor
    {
        MapGenCommandType CommandType { get; }

        void Execute(Map map, IEntityEngine entityEngineSystem, IPrototypeSystem prototypeSystem, MapGenCommand command, Vector offset);
    }
}