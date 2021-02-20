using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Controls
{
    public class ShopItemControl : BaseControl
    {
        public IEntity Item { get; set; }
        public bool Selected { get; internal set; }
    }


}
