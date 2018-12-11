using System;
using System.IO;
using System.Reflection;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core
{
    internal class DataStaticEntityLoader : IStaticEntityLoader
    {
        public void Load(IEntityEngineSystem engine)
        {
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data/Entities/StaticEntities");

            var edtFiles = Directory.EnumerateFiles(basePath, "*.edt", SearchOption.AllDirectories);

            foreach(var file in edtFiles)
            {
                var text = File.ReadAllText(file);
                EntitySerializer.DeserializeMultiple(text, engine);
            }
        }
    }
}