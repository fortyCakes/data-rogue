namespace data_rogue_core.Map.Vaults
{
    public interface IVaultDataParser
    {
        DungeonMap ParseVault(string json);
    }
}