using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.Generators
{
    public class VaultBasedDungeonMapGenerator : IMapGenerator
    {
        private ISystemContainer _systemContainer;
        private IEntity _floorCell;
        private IEntity _wallCell;
        private readonly int _numberOfVaults;
        private readonly IEntity _branch;

        public VaultBasedDungeonMapGenerator(ISystemContainer systemContainer, string floorCell, string wallCell, int numberOfVaults, IEntity branch)
        {
            _systemContainer = systemContainer;
            _floorCell = systemContainer.PrototypeSystem.Get(floorCell);
            _wallCell = systemContainer.PrototypeSystem.Get(wallCell);
            _numberOfVaults = numberOfVaults;
            _branch = branch;
        }

        public IMap Generate(string mapName, IRandom random)
        {
            IMap map = new Map(mapName, _wallCell);
            var vaultPlacements = new List<VaultPlacement>();

            List<IMap> validVaults = GetValidVaults();

            for (int i = 0; i <= _numberOfVaults; i++)
            {
                var selectedVault = SelectVault(validVaults, random);

                var vaultPlacement = PlaceVault(map, selectedVault);

                vaultPlacements.Add(vaultPlacement);
            }

            while (!map.IsFullyConnected())
            {
                var possibleVaultConnections = GetPossibleVaultConnections(map, vaultPlacements).ToList();

                var vaultConnection = SelectVaultConnection(possibleVaultConnections, random);

                AddVaultConnectionToMap(map, vaultConnection);
            }

            map.Vaults = vaultPlacements.Select(v => v.Vault.MapKey);

            return map;
        }

        private List<IMap> GetValidVaults()
        {
            var biomes = _branch.Components.OfType<Biome>();
            var validVaults = _systemContainer.MapSystem.Vaults
                .Where(v => BiomesMatch(v, biomes))
                .ToList();

            if (!validVaults.Any())
            {
                throw new ApplicationException($"No valid vaults found for branch {_branch.Name}");
            }

            return validVaults;
        }

        private static bool BiomesMatch(IMap vault, IEnumerable<Biome> branchBiomes)
        {
            return !vault.Biomes.Any() || vault.Biomes.Any(b => branchBiomes.Any(biome => biome.Name == b.Name));
        }

        private void AddVaultConnectionToMap(IMap map, object vaultConnection)
        {
            throw new NotImplementedException();
        }

        private object SelectVaultConnection(IList<VaultConnection> possibleVaultConnections, IRandom random)
        {
            return random.PickOne(possibleVaultConnections);
        }

        private IEnumerable<VaultConnection> GetPossibleVaultConnections(IMap map, List<VaultPlacement> vaults)
        {
            throw new NotImplementedException();
        }

        private VaultPlacement PlaceVault(IMap map, IMap selectedVault)
        {
            var placedVault = selectedVault.Clone();
            placedVault.Spin();

            var position = FindPosition(map, selectedVault);

            map.PlaceSubMap(position, selectedVault);

            return new VaultPlacement { Vault = placedVault, Position = position };
        }

        private Rectangle FindPosition(IMap map, IMap selectedVault)
        {
            throw new NotImplementedException();
        }

        private IMap SelectVault(List<IMap> validVaults, IRandom random)
        {
            return random.PickOne<IMap>(validVaults);
        }
    }

    internal class VaultConnection
    {
        public MapCoordinate First;
        public MapCoordinate Second;
    }

    internal class VaultPlacement
    {
        public IMap Vault;
        public Rectangle Position;
    }
}
