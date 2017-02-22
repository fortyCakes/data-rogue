using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Display;
using Newtonsoft.Json;

namespace data_rogue_core.Map.Vaults
{
    public class VaultDataParser : IVaultDataParser
    {
        public DungeonMap ParseVault(string json)
        {
            json = json.Replace("\r", "");
            json = json.Replace("\n", "\\n");
            VaultData data = JsonConvert.DeserializeObject<VaultData>(json);

            var mapRows = data.Map.Split('\n').ToList();
            if (string.IsNullOrWhiteSpace(mapRows.First()))
            {
                mapRows.RemoveAt(0);
            }
            if (string.IsNullOrWhiteSpace(mapRows.Last()))
            {
                mapRows.RemoveAt(mapRows.Count-1);
            }

            var height = mapRows.Count;
            var width = mapRows.Max(m => m.Length);

            DungeonMap map = new DungeonMap();
            map.Initialize(width, height, '#',Colors.Wall);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x <= mapRows[y].Length)
                    {
                        char vaultChar = mapRows[y][x];
                        DungeonCell cell = ParseVaultSymbol(vaultChar);
                        map.SetCellProperties(x, y, cell.IsTransparent, cell.IsWalkable, cell.Symbol, cell.Color);
                    }
                }
            }

            return map;
        }

        private DungeonCell ParseVaultSymbol(char vaultChar)
        {
            switch (vaultChar)
            {
                case '#':
                    return new DungeonCell(0,0,'#',Colors.Wall, false, false, false);
                case '.':
                    return new DungeonCell(0,0,'.',Colors.Floor,true, true, false);
                case '~':
                    return new DungeonCell(0, 0, '.', Colors.Wall, false, true, false);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [JsonObject]
    internal struct VaultData
    {
        public string Map;
    }
}
