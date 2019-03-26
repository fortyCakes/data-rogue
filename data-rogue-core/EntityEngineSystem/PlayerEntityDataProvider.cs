using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{

    public class PlayerEntityDataProvider : BaseFileDataProvider
    {
        public override List<string> GetData()
        {
            return Load("Data/Entities/player.edt").ToList();
        }
    }
}