using data_rogue_core.Systems.Interfaces;
using System.IO;
using System.Reflection;

namespace data_rogue_core.EntityEngineSystem
{
    public abstract class BaseStaticEntityLoader : IStaticEntityLoader
    {
        public abstract void Load(ISystemContainer systemContainer);

        protected static void Load(ISystemContainer systemContainer, string subdirectory)
        {
                var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), subdirectory);

                var edtFiles = Directory.EnumerateFiles(basePath, "*.edt", SearchOption.AllDirectories);

                foreach (var file in edtFiles)
                {
                    var text = File.ReadAllText(file);
                    EntitySerializer.DeserializeMultiple(systemContainer, text);
                }

        }
    }

    public class NullStaticEntityLoader: BaseStaticEntityLoader
    {
        public override void Load(ISystemContainer systemContainer)
        {
            return;
        }
    }
}