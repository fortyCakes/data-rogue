using System.Collections.Generic;
using data_rogue_core.Entities;

namespace data_rogue_core.Map
{
    public interface IMonsterGenerator
    {
        Monster GetNewMonster();

        Monster GetNewMonsterWithTag(List<string> tag);
    }
}