using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_one.Utils
{
    public interface IItemPricer
    {
        Price Price(IEntity item);
    }
}