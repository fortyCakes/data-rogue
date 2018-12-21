using System.Collections.Generic;
using data_rogue_core.EntityEngine;

namespace data_rogue_core.Data
{
    public interface IEntityDataParser
    {
        List<IEntity> Parse(IEnumerable<string> lines);
    }
}