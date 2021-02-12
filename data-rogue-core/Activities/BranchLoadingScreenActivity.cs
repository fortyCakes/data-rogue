using data_rogue_core.Components;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class BranchLoadingScreenActivity : StaticTextActivity
    {
        public BranchLoadingScreenActivity(IActivitySystem activitySystem, string staticText) : base(activitySystem, staticText, false)
        {
        }

        public override ActivityType Type => base.Type;

        public override bool RendersEntireSpace => base.RendersEntireSpace;

        public override bool AcceptsInput => base.AcceptsInput;

        public override string Text {
            get  {
                return _displayEntity.Get<Description>().Detail;
            }
            set {
            }
        }
    }
}
