using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    internal class DataStaticEntityLoader : IStaticEntityLoader
    {
        public void Load(IEntityEngineSystem engine)
        {
            LoadMapCells(engine);
        }

        private static void LoadMapCells(IEntityEngineSystem engine)
        {
            EntitySerializer.DeserializeMultiple(DataFileLoader.LoadFile(@"Entities\MapCells\GenericCells.edt"), engine);
        }
    }
}