using System.Collections.Generic;
using System.IO;

namespace data_rogue_core.Map.Vaults
{
    public class VaultGenerator : IVaultGenerator
    {
        List<DungeonMap> Vaults = new List<DungeonMap>();

        public VaultGenerator(IEnumerable<string> vaultData)
        {
            var parser = new VaultDataParser();

            
            foreach (string file in vaultData)
            {
                DungeonMap vault = parser.ParseVault(File.ReadAllText(file));
                Vaults.Add(vault);
            }
        }

        public DungeonMap GetVault()
        {
            var vaultsCount = Vaults.Count;
            var index = Game.Random.Next(vaultsCount - 1);

            var vault = Vaults[index];

            return vault.Clone();
        }
    }

    public interface IVaultGenerator
    {
        DungeonMap GetVault();
    }
}
