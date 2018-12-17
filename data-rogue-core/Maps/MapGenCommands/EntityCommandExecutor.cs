using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntitySystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    class EntityCommandExecutor : ICommandExecutor
    {
        public MapGenCommandType CommandType => MapGenCommandType.Entity;

        public void Execute(Map map, IEntityEngineSystem entityEngineSystem, IPrototypeSystem prototypeSystem, MapGenCommand command, Vector offset)
        {
            throw new NotImplementedException();
        }
    }
}
