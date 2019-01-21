﻿using data_rogue_core.Behaviours;
using System.IO;
using System.Reflection;

namespace data_rogue_core.EntityEngine
{
    public abstract class BaseStaticEntityLoader
    {
        public abstract void Load(IEntityEngine engine, IBehaviourFactory behaviourFactory);

        protected static void Load(IEntityEngine engine, IBehaviourFactory behaviourFactory, string subdirectory)
        {
                var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), subdirectory);

                var edtFiles = Directory.EnumerateFiles(basePath, "*.edt", SearchOption.AllDirectories);

                foreach (var file in edtFiles)
                {
                    var text = File.ReadAllText(file);
                    EntitySerializer.DeserializeMultiple(text, engine, behaviourFactory);
                }

        }
    }

    public class NullStaticEntityLoader: BaseStaticEntityLoader
    {
        public override void Load(IEntityEngine engine, IBehaviourFactory behaviourFactory)
        {
            return;
        }
    }
}