using System.Collections.Generic;

namespace data_rogue_core.EntityEngineSystem
{
    public class NullDataProvider : IEntityDataProvider
    {
        public List<string> GetData()
        {
            return new List<string>();
        }
    }
}