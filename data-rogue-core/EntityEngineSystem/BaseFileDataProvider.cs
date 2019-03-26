using data_rogue_core.Systems.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace data_rogue_core.EntityEngineSystem
{

    public abstract class BaseFileDataProvider : IEntityDataProvider
    {
        protected static IEnumerable<string> Load(string fileName)
        {
            var file = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

            yield return File.ReadAllText(file);
        }

        public abstract List<string> GetData();
    }
}