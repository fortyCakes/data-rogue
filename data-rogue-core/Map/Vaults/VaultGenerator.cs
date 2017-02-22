using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Map.Vaults
{
    public class VaultGenerator : IVaultGenerator
    {
        List<DungeonMap> Vaults = new List<DungeonMap>();

        public VaultGenerator()
        {
            var parser = new VaultDataParser();

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\Vaults\");

            foreach (string file in Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories))
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
