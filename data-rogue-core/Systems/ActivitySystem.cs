using data_rogue_core.Activities;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Renderers;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Linq;

namespace data_rogue_core.Systems
{
    public class ActivitySystem: IActivitySystem
    {
        public ActivityStack ActivityStack { get; private set; }

        public void Initialise()
        {
            ActivityStack = new ActivityStack();
        }

        public IActivity Peek() => ActivityStack.Peek();

        public IActivity Pop() => ActivityStack.Pop();

        public void Push(IActivity activity) => ActivityStack.PushAndInitialise(activity);

        public void RemoveActivity(IActivity activity)
        {
            var list = ActivityStack.ToList();
            list.Remove(activity);
            list.Reverse();

            ActivityStack = new ActivityStack(list);
        }

        public Action QuitAction { get; set; }

        public GameplayActivity GameplayActivity => ActivityStack.OfType<GameplayActivity>().Single();
        public MapEditorActivity MapEditorActivity => ActivityStack.OfType<MapEditorActivity>().Single();

        public IActivity GetActivityAcceptingInput()
        {
            foreach (var activity in ActivityStack)
            {
                if (activity.AcceptsInput)
                    return activity;
            }

            return null;
        }

        public IMapActivity GetMapActivity()
        {
            foreach(var activity in ActivityStack)
            {
                if (activity is IMapActivity)
                {
                    return (IMapActivity)activity;
                }
            }

            return null;
        }

        public void OpenShop(ISystemContainer systemContainer, IEntity shop)
        {
            var shoppingActivity = new ShopActivity(systemContainer, shop);
            ActivityStack.Push(shoppingActivity);
        }
    }
}