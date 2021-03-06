﻿using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    public interface ICommandExecutor
    {
        string CommandType { get; }

        void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset);
    }
}