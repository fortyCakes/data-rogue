using System.Drawing;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Components
{

    public class SpriteAppearance : IEntityComponent
    {
        public string Bottom;
        public string BottomConnect = null;
        public SpriteConnectType BottomConnectType = SpriteConnectType.FourDirection;
        public string Top;
        public string TopConnect = null;
        public SpriteConnectType TopConnectType = SpriteConnectType.FourDirection;
    }

    public enum SpriteConnectType
    {
        FourDirection,
        Wall
    }
}
