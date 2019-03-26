using System.Collections.Generic;
using System.IO;
using System.Linq;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core
{
    public class FolderDataProvider : BaseFolderDataProvider
    {
        public override List<string> GetData()
        {
            return LoadFolderFiles().ToList();
        }

        private static IEnumerable<string> LoadFolderFiles()
        {
            string basePath;
#if DEBUG
            basePath = @"C:\Users\Lex\source\repos\fortyCakes\data-rogue\data-rogue-one\Data\Entities\StaticEntities";
#else
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    basePath = Path.Combine(dialog.SelectedPath, "Data/Entities/StaticEntities");

                    
                }
            }
#endif

            var edtFiles = Directory.EnumerateFiles(basePath, "*.edt", SearchOption.AllDirectories);

            foreach (var file in edtFiles)
            {
                var text = File.ReadAllText(file);
                yield return text;
            }
        }
    }
}