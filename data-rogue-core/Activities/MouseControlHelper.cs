using System;
using System.Collections.Generic;
using data_rogue_core.IOSystems;

namespace data_rogue_core.Activities
{
    public static class MouseControlHelper
    {
        public static IEnumerable<IDataRogueControl> GetControlsUnderMouse(MouseData mouse, IEnumerable<IDataRogueControl> controls)
        {
            foreach(var control in controls)
            {
                bool mouseIsOver = control.Position.Contains(mouse.X, mouse.Y);

                if (mouseIsOver)
                {
                    yield return control;
                }

                if (control is IDataRogueParentControl)
                {
                    foreach(var subControl in GetControlsUnderMouse(mouse, (control as IDataRogueParentControl).Controls))
                    {
                        yield return subControl;
                    }
                }
            }
        }
    }
}