using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntitySystem;
using RLNET;

namespace data_rogue_core.Systems
{
    public interface IPlayerControlSystem : ISystem
    {
        void HandleKeyPress(RLKeyPress keyPress);
    }
}
