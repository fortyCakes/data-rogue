using data_rogue_core.Entities;

namespace data_rogue_core.Monsters
{
    public interface IMonsterFactory:ITaggable

    {
    Monster GetMonster();

    }
}