using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IAnimationSystem : ISystem
    {
        void Tick();

        int GetFrame(IEntity entity);
        void SetFrame(IEntity entity, int frame);
    }
}
