using data_rogue_core.Data;
using System.Collections.Generic;

namespace data_rogue_core
{
    public class SaveMap
    {
        public string MapKey { get; set; }
        public uint DefaultCellId { get; set; }
        public List<MapSaveCell> Cells { get; set; }
    }
}