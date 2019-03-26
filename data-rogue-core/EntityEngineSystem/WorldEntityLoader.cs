using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core
{
    public class WorldEntityLoader : BaseFolderDataProvider
    {
        public override List<string> GetData()
        {
            return Load("Data/Entities/World").ToList();
        }

    }
}