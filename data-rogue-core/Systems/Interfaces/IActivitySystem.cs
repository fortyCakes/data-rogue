using data_rogue_core.Activities;

namespace data_rogue_core.Systems.Interfaces
{

    public interface IActivitySystem
    {
        ActivityStack ActivityStack { get; }

        void Initialise();
        void Push(IActivity activity);
        IActivity Pop();
        IActivity Peek();
    }
}
