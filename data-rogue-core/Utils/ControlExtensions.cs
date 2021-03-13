using data_rogue_core.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Utils
{
    public static class ControlExtensions
    {
        public static IList<IDataRogueControl> GetControlsRecursive(this IList<IDataRogueControl> controls)
        {
            var toReturn = new List<IDataRogueControl>();

            foreach (var control in controls)
            {
                toReturn.Add(control);

                if (control is IDataRogueParentControl)
                {
                    var subControls = GetControlsRecursive((control as IDataRogueParentControl).Controls);

                    toReturn.AddRange(subControls);
                }
            }

            return toReturn;
        }
    }
}
