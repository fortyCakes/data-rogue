using System.Collections.Generic;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Data
{
    public interface IEntityDataParser
    {
        IEnumerable<IEntity> Parse(IEnumerable<string> lines);
    }
}