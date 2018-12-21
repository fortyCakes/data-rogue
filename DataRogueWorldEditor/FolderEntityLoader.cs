using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using data_rogue_core.Data;
using data_rogue_core.EntityEngine;

namespace data_rogue_core
{
    public class FolderEntityLoader : BaseStaticEntityLoader
    {
        public override void Load(IEntityEngine engine)
        {
            string basePath;
#if DEBUG
            basePath = @"C:\Users\Lex\source\repos\fortyCakes\data-rogue\data-rogue-core\Data\Entities\StaticEntities";
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
                EntitySerializer.DeserializeMultiple(text, engine);
            }
        }
    }
}