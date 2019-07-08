using System;
using System.Numerics;

namespace sepwind
{
    public class MathUtil
    {
        internal static double AngleByRad(Vector2 a, Vector2 b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
        internal static double AngleByRad(Vector2 a)
        {
            return Math.Atan2(a.Y, a.X);
        }
    }
}
