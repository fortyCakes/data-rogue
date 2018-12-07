using RLNET;

namespace data_rogue_core.Activities
{
    public interface IActivity
    {
        ActivityType Type { get; }
        object Data { get; }
        void Render();
    }
}