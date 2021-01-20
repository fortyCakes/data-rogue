using System.Collections.Generic;
using System.Drawing;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class ParticleSystem: BaseSystem, IParticleSystem
    {
        private IAnimationSystem _animationSystem;
        private IEntityEngine _entityEngine;

        public override SystemComponents RequiredComponents => new SystemComponents { typeof(TextParticle) };
        public override SystemComponents ForbiddenComponents => new SystemComponents { typeof(Prototype) };

        public ParticleSystem(IAnimationSystem animationSystem, IEntityEngine entityEngine)
        {
            _animationSystem = animationSystem;
            _entityEngine = entityEngine;
        }

        public List<IEntity> Particles => Entities;

        public void CreateTextParticle(MapCoordinate location, List<AnimationMovement> movements, string text, Color color)
        {
            _entityEngine.New("particle",
                new TextParticle { Text = text, Color = color },
                new Position { MapCoordinate = location },
                new Moving { Movements = movements }
                );
        }

        public void Tick()
        {
            var entitiesAtStartOfTick = new List<IEntity>(Entities);

            foreach(var entity in entitiesAtStartOfTick)
            {
                if (!entity.Has<Moving>())
                {
                    _entityEngine.Destroy(entity);
                }
            }
        }
    }
}

       