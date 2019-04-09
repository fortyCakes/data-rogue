using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core
{

    public class SpriteSheetDataProvider : BaseFileDataProvider
    {
        public override List<string> GetData()
        {
            return Load("Data/Entities/spritesheet.edt").ToList();
        }
    }
}