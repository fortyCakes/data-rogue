using data_rogue_core.EntityEngineSystem;
using System.Collections.Generic;

namespace data_rogue_core.World.GenerationStrategies
{
    public interface IItemGenerator
    {
        IEntity GenerateItem(List<IEntity> itemList, int itemLevel, IRandom random);
    }
}