using data_rogue_core.EventSystem.EventData;
using data_rogue_core.IOSystems;

namespace data_rogue_core.Systems
{
    public class ActionEventData
    {
        public ActionType Action { get; set; }
        public string Parameters { get; set; }
        public KeyCombination KeyPress { get; set; }
        public ulong? Speed { get; set; }
        public bool IsAction { get; internal set; }
    }
}