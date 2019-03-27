using data_rogue_core.Renderers;
using System;
using System.Collections.Generic;

namespace data_rogue_core.Activities
{
    public class ActivityStack : Stack<IActivity>
    {
        private readonly IRendererFactory _rendererFactory;

        public ActivityStack(IRendererFactory rendererFactory)
        {
            _rendererFactory = rendererFactory;
        }

        internal void PushAndInitialise(IActivity activity)
        {
            Push(activity);
            activity.Initialise(_rendererFactory.GetRendererFor(activity.Type));
        }
    }
}
