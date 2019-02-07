using System.Collections.Generic;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Data
{
    public interface IEntityDataParser
    {
        List<IEntity> Parse(IEnumerable<string> lines);
    }
}