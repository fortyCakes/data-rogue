using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class LoadingScreenActivity : StaticTextActivity
    {
        public LoadingScreenActivity(IActivitySystem activitySystem, string staticText) : base(activitySystem, staticText, false)
        {
        }
    }
}
