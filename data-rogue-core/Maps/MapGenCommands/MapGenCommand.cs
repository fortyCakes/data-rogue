using System;

namespace data_rogue_core.Maps
{
    public class MapGenCommand : ICloneable
    {
        public string MapGenCommandType { get; set; }

        public string Parameters { get; set; }
        public Vector Vector { get; set; }

        public override string ToString()
        {
            return $"{Vector.X},{Vector.Y}: {MapGenCommandType}({Parameters})";
        }

        public object Clone()
        {
            return new MapGenCommand
            {
                MapGenCommandType = MapGenCommandType,
                Parameters = Parameters,
                Vector = new Vector(Vector.X, Vector.Y)
            };
        }
    }
}