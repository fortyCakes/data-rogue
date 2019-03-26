using System.Collections.Generic;
using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class StaticEntityDataProvider : BaseFolderDataProvider
    {
        public override List<string> GetData()
        {
            return Load("Data/Entities/StaticEntities").ToList();
        }
    }

    public class KeyBindingsDataProvider : BaseFileDataProvider
    {
        public override List<string> GetData()
        {
            return Load("Data/Entities/keybindings.edt").ToList();
        }
    }
}