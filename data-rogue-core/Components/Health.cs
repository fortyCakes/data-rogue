using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{
    public class Health : IEntityComponent, IHasCounter
    {
        public Counter HP;

        public Counter Counter => HP;
    }

    public interface IHasCounter
    {
        Counter Counter { get; }
    }
}
