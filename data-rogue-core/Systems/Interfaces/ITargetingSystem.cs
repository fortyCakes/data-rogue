using data_rogue_core.EntityEngine;
using data_rogue_core.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Systems.Interfaces
{
    public interface ITargetingSystem
    {
        void GetTarget(IEntity sender, TargetingData data, Action<MapCoordinate> callback);
    }

    public class TargetingData
    {
        public bool Friendly = false;
        public bool MoveToCell = false;

        public int? Range = null;

        public List<Vector> CellsHit = new List<Vector> { new Vector(0, 0) };
        public List<Vector> ValidCells = new List<Vector>();
    }
}
