namespace data_rogue_core.Monsters
{
    public interface IMonsterFactoryDataParser
    {
        IMonsterFactory GetMonsterFactory(string monsterData);
    }
}