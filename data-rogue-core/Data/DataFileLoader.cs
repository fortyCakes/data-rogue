using System.IO;
using System.Reflection;

namespace data_rogue_core.Data
{
    public class DataFileLoader
    {
        public static string LoadFile(string file)
        {
            var fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data", file);

            return File.ReadAllText(fullPath);
        }
    }
}
