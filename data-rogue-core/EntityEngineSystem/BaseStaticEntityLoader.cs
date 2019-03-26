using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace data_rogue_core.EntityEngineSystem
{
    public abstract class BaseFolderDataProvider : IEntityDataProvider
    {
        protected static IEnumerable<string> Load(string subdirectory)
        {
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), subdirectory);
            var edtFiles = Directory.EnumerateFiles(basePath, "*.edt", SearchOption.AllDirectories);

            foreach (var file in edtFiles)
            {
                var text = File.ReadAllText(file);
                yield return text;
            }

        }

        public abstract List<string> GetData();
    }
}