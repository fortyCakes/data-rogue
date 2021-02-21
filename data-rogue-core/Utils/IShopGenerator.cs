using data_rogue_core;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_one.Utils
{
    public interface IShopGenerator
    {
        IEnumerable<IEntity> GenerateShopItems(ISystemContainer systemContainer, int numberOfItems, int itemLevel, IEnumerable<IEntity> itemList, IRandom random);

        IEntity GenerateShop(ISystemContainer systemContainer, int numberOfItems, int itemLevel, IEnumerable<IEntity> itemList, IRandom random);
    }
}
