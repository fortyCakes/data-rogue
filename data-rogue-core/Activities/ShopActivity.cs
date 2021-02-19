using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.IOSystems;
using data_rogue_core.Maps;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Activities
{
    public class ShopActivity : BaseActivity
    {
        private ActivitySystem activitySystem;
        private IEntity shop;

        public ShopActivity(ActivitySystem activitySystem, IEntity shop)
        {
            this.activitySystem = activitySystem;
            this.shop = shop;
        }

        public override ActivityType Type => ActivityType.Shop;

        public override bool RendersEntireSpace => true;

        public override bool AcceptsInput => true;

        public override IEnumerable<IDataRogueControl> GetLayout(IUnifiedRenderer renderer, ISystemContainer systemContainer, object rendererHandle, List<IDataRogueControlRenderer> controlRenderers, List<MapCoordinate> playerFov, int width, int height)
        {
            throw new NotImplementedException();
        }

        public override void HandleAction(ISystemContainer systemContainer, ActionEventData action)
        {
            throw new NotImplementedException();
        }

        public override void HandleKeyboard(ISystemContainer systemContainer, KeyCombination keyboard)
        {
            throw new NotImplementedException();
        }
    }
}
