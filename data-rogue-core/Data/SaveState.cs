using data_rogue_core.EntitySystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace data_rogue_core
{
    public class SaveState
    {
        public string CurrentMapKey { get; set; }

        public List<string> Entities { get; set; }
        public List<string> Maps { get; set; }

        internal string Serialize()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"CurrentMapKey:\"{CurrentMapKey}\"");
            stringBuilder.AppendLine($"===== ENTITIES =====");
            foreach(var entity in Entities)
            {
                stringBuilder.AppendLine(entity);
            }

            stringBuilder.AppendLine($"===== MAPS =====");
            foreach (var map in Maps)
            {
                stringBuilder.AppendLine(map);
            }

            return stringBuilder.ToString();
        }

        internal static SaveState Deserialize(string serialized)
        {
            throw new NotImplementedException();
        }
    }
}