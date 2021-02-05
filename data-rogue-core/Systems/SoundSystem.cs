using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{
    public class SoundSystem : BaseSystem, ISoundSystem
    {
        private Dictionary<IEntity, SoundPlayer> _entityCache;

        public override void Initialise()
        {
            base.Initialise();
            _entityCache = new Dictionary<IEntity, SoundPlayer>();
        }

        public override void AddEntity(IEntity entity)
        {
            var sound = entity.Get<Sound>();
            var soundPlayer = LoadSound(sound);
            _entityCache.Add(entity, soundPlayer);
        }

        private SoundPlayer LoadSound(Sound sound)
        {
            return new SoundPlayer(sound.Path);
        }

        public override void RemoveEntity(IEntity entity)
        {
            _entityCache.Remove(entity);
        }

        public override bool HasEntity(IEntity entity)
        {
            return _entityCache.ContainsKey(entity);
        }

        public override SystemComponents RequiredComponents => new SystemComponents {typeof(Sound)};
        public override SystemComponents ForbiddenComponents => new SystemComponents();

        public void PlaySound(IEntity entity)
        {
            _entityCache[entity].Play();
        }
    }
}