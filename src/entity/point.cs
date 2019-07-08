using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// point entity
    /// </summary>
    public class Point
        : Entity
    {
        /// <summary>
        /// position
        /// </summary>
        public Vector2 Pos { get; set; }
        /// <summary>
        /// msec
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Point(Layer layer)
            : base("Point", layer)
        {
        }
        public Point(Layer layer, Vector2 pos)
            : this(layer)
        {
            this.Pos = pos;
        }
        public Point(Layer layer, Vector2 pos, double timeMsec)
            : this(layer, pos)
        {
            this.Time = timeMsec;
        }
        /// <summary>
        /// laser processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public override bool Mark(IRtc rtc)
        {
            bool success = true;
            ///jump to start pos
            success &= rtc.ListJump(this.Pos);
            ///dwell during assgined time
            success &= rtc.ListWait(this.Time);
            return success;
        }
    }
}
