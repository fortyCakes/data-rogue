using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems.Interfaces
{
    public interface IParticleSystem : ISystem, IInitialisableSystem
    {
        void CreateTextParticle(MapCoordinate location, List<AnimationMovement> movement, string text, Color color);

        void Tick();

        List<IEntity> Particles { get; }
    }
}
