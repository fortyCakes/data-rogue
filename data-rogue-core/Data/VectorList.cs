﻿using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using System.Collections.Generic;
using System.Linq;

namespace data_rogue_core.Components
{
    public class VectorList : List<Vector>, ICustomFieldSerialization
    {
        public void Deserialize(string value)
        {
            Clear();

            foreach(string split in value.Split(';'))
            {
                var vector = Vector.Parse(split);

                Add(vector);
            }
        }

        public string Serialize()
        {
            return string.Join(";", this.Select(v => v.ToString()).ToArray());
        }
    }
}