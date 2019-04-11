using data_rogue_core.Activities;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Linq;

namespace data_rogue_core.Systems
{
    public class ActivitySystem: IActivitySystem
    {
        public IRendererFactory _rendererFactory;

        public ActivitySystem(IRendererFactory rendererFactory)
        {
            _rendererFactory = rendererFactory;
        }

        public ActivityStack ActivityStack { get; private set; }

        public void Initialise()
        {
            ActivityStack = new ActivityStack(_rendererFactory);
        }

        public IActivity Peek() => ActivityStack.Peek();

        public IActivity Pop() => ActivityStack.Pop();

        public void Push(IActivity activity) => ActivityStack.PushAndInitialise(activity);

        public void RemoveActivity(IActivity activity)
        {
            var list = ActivityStack.ToList();
            list.Remove(activity);
            list.Reverse();

            ActivityStack = new ActivityStack(_rendererFactory, list);
        }

        public Action QuitAction { get; set; }
    }
}