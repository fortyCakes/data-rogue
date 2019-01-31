using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
