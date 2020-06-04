using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class SoundSystem : BaseSystem, ISoundSystem
    {
        private Dictionary<IEntity, Sound> _soundCache;

        public void Initialise()
        {
            base.Initialise();
            _soundCache = new Dictionary<IEntity, Sound>();
        }

        public void AddEntity(IEntity entity)
        {
            _soundCache.Add(entity, entity.Get<Sound>());
        }

        public void RemoveEntity(IEntity entity)
        {
            _soundCache.Remove(entity);
        }

        public bool HasEntity(IEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Sound)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public void Play(Sound sound)
        {
            throw new System.NotImplementedException();
        }

        public void Play(IEntity soundEntity)
        {
            throw new System.NotImplementedException();
        }
    }
}