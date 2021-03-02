using data_rogue_core.Systems.Interfaces;
using System.Drawing;
using System.Windows.Forms;

namespace data_rogue_core.Activities
{
    public class LoadingScreenActivity : StaticTextActivity
    {
        public LoadingScreenActivity(Rectangle position, Padding padding, IActivitySystem activitySystem, string staticText) : base(position, padding, activitySystem, staticText, false)
        {

        }

        public LoadingScreenActivity(IActivitySystem activitySystem, string staticText) : base(activitySystem, staticText)
        {

        }
    }
}
