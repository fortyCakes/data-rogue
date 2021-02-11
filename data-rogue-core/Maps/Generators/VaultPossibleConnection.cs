using System;
using BLTWrapper;

namespace data_rogue_core.Maps.Generators
{
    public class VaultPossibleConnection
    {
        public VaultPlacement FirstVault;
        public VaultConnectionPoint First;
        public VaultPlacement SecondVault;
        public VaultConnectionPoint Second;

        public bool IsRightAngle {
            get {
                var vToH = IsVertical(First) && !IsVertical(Second);
                var hToV = IsVertical(Second) && !IsVertical(First);

                return vToH || hToV;
            }
        }

        public bool IsVertical(VaultConnectionPoint point)
        {
            return point.Facing == TileDirections.Down || point.Facing == TileDirections.Up;
        }

        internal void RemovePossibleConnectionPoints()
        {
            FirstVault.VaultConnectionPoints.Remove(First);
            SecondVault.VaultConnectionPoints.Remove(Second);
        }

        public MapCoordinate GetCorner()
        {
            if (!IsRightAngle) throw new ApplicationException("Can only get corner of connection if it's a right angle");

            var x = !IsVertical(First) ? Second.Coordinate.X : First.Coordinate.X;
            var y = IsVertical(First) ? Second.Coordinate.Y : First.Coordinate.Y;

            return new MapCoordinate(First.Coordinate.Key, x, y);
        }

        public (MapCoordinate, MapCoordinate) GetCorners()
        {
            if (IsRightAngle) throw new ApplicationException("Can only get double corner if it's not a right angle");

            if (IsVertical(First))
            {
                var y = (First.Coordinate.Y + Second.Coordinate.Y) / 2;

                return (
                    new MapCoordinate(First.Coordinate.Key, First.Coordinate.X, y),
                    new MapCoordinate(Second.Coordinate.Key, Second.Coordinate.X, y));
            }
            else
            {
                var x = (First.Coordinate.X + Second.Coordinate.X) / 2;

                return (
                    new MapCoordinate(First.Coordinate.Key, x, First.Coordinate.Y),
                    new MapCoordinate(Second.Coordinate.Key, x, Second.Coordinate.Y));
            }
        }
    }
}
