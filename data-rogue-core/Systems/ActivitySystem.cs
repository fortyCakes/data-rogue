using data_rogue_core.Activities;
using data_rogue_core.Systems.Interfaces;

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

        public void Push(IActivity activity) => ActivityStack.Push(activity);
    }
}