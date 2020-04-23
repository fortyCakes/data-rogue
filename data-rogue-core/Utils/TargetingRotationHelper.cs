using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Utils
{
    public static class TargetingRotationHelper
    {
        public static Matrix GetSkillRotation(IEntity user, MapCoordinate target)
        {
            // We assume the unrotated target of a skill is to the right i.e. in the positive x-direction.

            var origin = user.Get<Position>()?.MapCoordinate;

            if (origin == null || target.Key != origin.Key)
            {
                return Matrix.Identity;
            }

            var dx = target.X - origin.X;
            var dy = target.Y - origin.Y;

            return GetSkillRotation(dy, dx);
        }

        public static Matrix GetSkillRotation(int dy, int dx)
        {
            var vertical = Math.Abs(dy) > Math.Abs(dx);

            if (vertical)
            {
                if (dy < 0) return Matrix.Rotate270;
                else return Matrix.Rotate90;
            }
            else
            {
                if (dx < 0) return Matrix.Rotate180;
                else return Matrix.Identity;
            }
        }
    }
}
