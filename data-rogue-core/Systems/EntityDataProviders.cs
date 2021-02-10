using data_rogue_core.EntityEngineSystem;

namespace data_rogue_core.Systems
{
    public class EntityDataProviders
    {
        public static EntityDataProviders Default => new EntityDataProviders
        {
            PrototypeEntityDataProvider = new StaticEntityDataProvider(),
            KeyBindingsDataProvider = new KeyBindingsDataProvider(),
            WorldEntityDataProvider = new WorldEntityDataProvider(),
            PlayerEntityDataProvider = new PlayerEntityDataProvider(),
            GraphicsDataProvider = new SpriteSheetDataProvider(),
            VaultDataProvider = new VaultDataProvider()

        };

        public IEntityDataProvider PrototypeEntityDataProvider { get; set; }
        public IEntityDataProvider KeyBindingsDataProvider { get; set; }
        public IEntityDataProvider WorldEntityDataProvider { get; set; }
        public IEntityDataProvider PlayerEntityDataProvider { get; set; }
        public IEntityDataProvider GraphicsDataProvider { get; set; }
        public IEntityDataProvider VaultDataProvider { get; set; }
    }
}