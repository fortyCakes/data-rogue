using data_rogue_core.EntityEngineSystem;
using System.Drawing;

namespace data_rogue_core.Components
{
    public class Description : IEntityComponent
    {
        public string Name;
        public string Detail;
        public Color Color = Color.White;
    }
}
