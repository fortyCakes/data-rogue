using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace data_rogue_core
{
    public class VaultDataLoader
    {
        public static IEnumerable<string> GetVaultData()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\Vaults\");

            return Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);
        }
    }
}