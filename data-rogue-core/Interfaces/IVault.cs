using RogueSharp;

namespace data_rogue_core.Map.Vaults
{
    public interface IVault
    {
        int Height { get; }
        int Width { get; }

        void PutVaultOnMap(IMap map, int x, int y);
        void PlaceMonsters(IMap map);
        
    }
}