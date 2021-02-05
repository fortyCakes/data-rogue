using data_rogue_core.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Activities
{
    public class ActivityStack : Stack<IActivity>
    {
        public ActivityStack()
        {
        }

        public ActivityStack(List<IActivity> list) : base(list)
        {
        }

        internal void PushAndInitialise(IActivity activity)
        {
            Push(activity);
        }

        internal int IndexOf(IActivity activity)
        {
            return this.ToList().IndexOf(activity);
        }
    }
}
