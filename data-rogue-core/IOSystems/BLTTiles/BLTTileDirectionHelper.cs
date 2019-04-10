using System.Collections.Generic;
using BLTWrapper;
using data_rogue_core.Components;
using data_rogue_core.Maps;

namespace data_rogue_core.IOSystems.BLTTiles
{
    internal class BLTTileDirectionHelper
    {
        private static readonly Dictionary<TileDirections, TileDirections> OppositeDirections = new Dictionary<TileDirections, TileDirections>
        {
            {TileDirections.Left, TileDirections.Right},
            {TileDirections.Right, TileDirections.Left},
            {TileDirections.Up, TileDirections.Down},
            {TileDirections.Down, TileDirections.Up}
        };

        private static readonly Dictionary<TileDirections, Vector> OppositeCornerAnticlockwise = new Dictionary<TileDirections, Vector>
        {
            {TileDirections.Down, new Vector(-1, -1)},
            {TileDirections.Right, new Vector(-1, +1)},
            {TileDirections.Up, new Vector(+1, +1)},
            {TileDirections.Left, new Vector(+1, -1)}
        };

        private static readonly Dictionary<TileDirections, Vector> OppositeCornerClockwise = new Dictionary<TileDirections, Vector>
        {
            {TileDirections.Down, new Vector(+1, -1)},
            {TileDirections.Right, new Vector(-1, -1)},
            {TileDirections.Up, new Vector(-1, +1)},
            {TileDirections.Left, new Vector(+1, +1)}
        };

        private static readonly List<TileDirections> AllDirections = new List<TileDirections>
        {
            TileDirections.Up,
            TileDirections.Down,
            TileDirections.Right,
            TileDirections.Left
        };

        public static TileDirections GetDirections(SpriteAppearance[,,] tilesTracker, int x, int y, int z, bool top)
        {
            SpriteAppearance appearance = tilesTracker[x, y, z];
            string connect = GetConnect(appearance, top);
            SpriteConnectType connectType = GetConnectType(appearance, top);
            TileDirections directions = TileDirections.None;
            if (appearance == null || connect == null) return directions;

            SpriteAppearance above = tilesTracker[x, y - 1, z];
            SpriteAppearance below = tilesTracker[x, y + 1, z];
            SpriteAppearance left = tilesTracker[x - 1, y, z];
            SpriteAppearance right = tilesTracker[x + 1, y, z];

            bool aboveConnect = GetConnect(above, top) == connect;
            bool belowConnect = GetConnect(below, top) == connect;
            bool leftConnect = GetConnect(left, top) == connect;
            bool rightConnect = GetConnect(right, top) == connect;

            if (aboveConnect) directions |= TileDirections.Up;
            if (belowConnect) directions |= TileDirections.Down;
            if (leftConnect) directions |= TileDirections.Left;
            if (rightConnect) directions |= TileDirections.Right;

            if (connectType == SpriteConnectType.Wall) directions = ApplyWallTypeConnectionRules(tilesTracker, x, y, z, top, directions, connect);

            return directions;
        }

        private static TileDirections ApplyWallTypeConnectionRules(SpriteAppearance[,,] tilesTracker, int x, int y, int z, bool top, TileDirections directions, string connect)
        {
            if (IsTripleSide(directions))
            {
                TileDirections fromDirection = (TileDirections) (TileDirections.All - directions);
                directions = ApplyWallCornerCheck(tilesTracker, x, y, z, top, directions, connect, fromDirection);
            }

            if (directions == TileDirections.All)
            {
                foreach (var fromDirection in AllDirections)
                {
                    directions = ApplyWallCornerCheck(tilesTracker, x, y, z, top, directions, connect, fromDirection);
                }
            }

            return directions;
        }

        private static TileDirections ApplyWallCornerCheck(SpriteAppearance[,,] tilesTracker, int x, int y, int z, bool top, TileDirections directions, string connect, TileDirections fromDirection)
        {
            if (OppositeCornersConnect(tilesTracker, x, y, z, top, fromDirection, connect)) directions = (TileDirections) (directions - OppositeDirection(fromDirection));
            return directions;
        }

        private static TileDirections OppositeDirection(TileDirections fromDirection)
        {
            return OppositeDirections[fromDirection];
        }

        private static bool OppositeCornersConnect(SpriteAppearance[,,] tilesTracker, int x, int y, int z, bool top, TileDirections fromDirection, string connect)
        {
            Vector clockwiseVector = OppositeCornerClockwise[fromDirection];
            SpriteAppearance clockwiseAppearance = tilesTracker[x + clockwiseVector.X, y + clockwiseVector.Y, z];
            string clockwiseConnect = top ? clockwiseAppearance.TopConnect : clockwiseAppearance.BottomConnect;
            if (connect != clockwiseConnect) return false;

            Vector anticlockwiseVector = OppositeCornerAnticlockwise[fromDirection];
            SpriteAppearance anticlockwiseAppearance = tilesTracker[x + anticlockwiseVector.X, y + anticlockwiseVector.Y, z];
            string anticlockwiseConnect = top ? anticlockwiseAppearance.TopConnect : anticlockwiseAppearance.BottomConnect;
            if (connect != anticlockwiseConnect) return false;

            return true;
        }

        private static bool IsTripleSide(TileDirections directions)
        {
            return
                directions == (TileDirections) (TileDirections.All - TileDirections.Down) ||
                directions == (TileDirections) (TileDirections.All - TileDirections.Left) ||
                directions == (TileDirections) (TileDirections.All - TileDirections.Right) ||
                directions == (TileDirections) (TileDirections.All - TileDirections.Up);
        }

        private static SpriteConnectType GetConnectType(SpriteAppearance appearance, bool top)
        {
            return top ? appearance.TopConnectType : appearance.BottomConnectType;
        }

        private static string GetConnect(SpriteAppearance appearance, bool top)
        {
            if (appearance == null) return null;
            return !top ? appearance.BottomConnect : appearance.TopConnect;
        }
    }
}