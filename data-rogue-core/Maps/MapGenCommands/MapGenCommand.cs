namespace data_rogue_core.Maps
{
    public class MapGenCommand
    {
        public MapGenCommandType MapGenCommandType { get; set; }

        public string Parameters { get; set; }
        public Vector Vector { get; set; }

        public override string ToString()
        {
            return $"{Vector.X},{Vector.Y}: {MapGenCommandType}({Parameters})";
        }
    }

    public enum MapGenCommandType
    {
        Null,
        Entity,
        EntityStack
    }
}