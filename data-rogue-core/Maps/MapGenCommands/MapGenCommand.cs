using System;

namespace data_rogue_core.Maps
{
    public class MapGenCommand : ICloneable
    {
        public MapGenCommandType MapGenCommandType { get; set; }

        public string Parameters { get; set; }
        public Vector Vector { get; set; }

        public override string ToString()
        {
            return $"{Vector.X},{Vector.Y}: {MapGenCommandType}({Parameters})";
        }

        internal MapGenCommand Clone()
        {
            throw new NotImplementedException();
        }

        object ICloneable.Clone()
        {
            return new MapGenCommand
            {
                MapGenCommandType = MapGenCommandType,
                Parameters = Parameters,
                Vector = new Vector(Vector.X, Vector.Y)
            };
        }
    }

    public enum MapGenCommandType
    {
        Null,
        Entity,
        EntityStack
    }
}