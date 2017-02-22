using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp;

namespace data_rogue_core.Map.Vaults
{
    /// <summary>
    /// Represents a vault - a fixed map segment to try to place in the map.
    /// </summary>
    public class Vault : IVault
    {
        public int Height { get; set; }
        public int Width { get; set; }

        public void PutVaultOnMap(IMap map, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void PlaceMonsters(IMap map)
        {
            throw new NotImplementedException();
        }
    }
}
