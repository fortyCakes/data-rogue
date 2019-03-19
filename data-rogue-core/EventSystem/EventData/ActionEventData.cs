using data_rogue_core.Components;
using data_rogue_core.EventSystem.EventData;

namespace data_rogue_core.Systems
{
    public class ActionEventData
    {
        public ActionType Action { get; set; }
        public string Parameters { get; set; }
        public KeyCombination KeyPress { get; set; }
        public int? Speed { get; set; }
    }
}