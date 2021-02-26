using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.MapGenCommands
{
    public class VaultConnectionCommandExecutor : ICommandExecutor
    {
        public string CommandType => MapGenCommandType.VaultConnection;

        public void Execute(ISystemContainer systemContainer, Map map, MapGenCommand command, Vector offset)
        {
            // These are just to indicate to the generator where connections should go.
        }
    }
}
