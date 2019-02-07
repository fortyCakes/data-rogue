using RLNET;

namespace data_rogue_core.EventSystem.EventData
{
    public class InputEventData
    {
        public RLKeyPress Keyboard { get; }
        public RLMouse Mouse { get; }

        public InputEventData(RLKeyPress keyboard, RLMouse mouse)
        {
            Keyboard = keyboard;
            Mouse = mouse;
        }
    }
}
