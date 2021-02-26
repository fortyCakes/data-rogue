using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Maps.Generators
{
    public class VaultPlacement
    {
        public IMap Vault;
        public MapCoordinate Position;
        public List<VaultConnectionPoint> VaultConnectionPoints { get; private set; }

        public VaultPlacement(IMap placedVault, MapCoordinate position)
        {
            Vault = placedVault;
            Position = position;
            InitialiseVaultConnectionPoints();
        }


        private void InitialiseVaultConnectionPoints()
        {
            VaultConnectionPoints = Vault.MapGenCommands
                .Where(c => c.MapGenCommandType == MapGenCommandType.VaultConnection)
                .Select(c => new VaultConnectionPoint(Position, c.Vector, Vault))
                .ToList();
        }
    }
}
