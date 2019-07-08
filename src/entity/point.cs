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
        public double X { get; set; }
        public double Y { get; set; }
        /// <summary>
        /// msec
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Point(Layer layer)
            : base(layer, "Point")
        {
        }
        public Point(Layer layer, double x, double y)
            : this(layer)
        {
            this.X = x;
            this.Y = y;
        }
        public Point(Layer layer, double x, double y, double timeMsec)
            : this(layer, x, y)
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
            rtc.Matrix.Push(this.X, this.Y);
            ///jump to start pos
            success &= rtc.ListJump(new Vector2(0, 0));
            ///dwell during assgined time
            success &= rtc.ListWait(this.Time);
            rtc.Matrix.Pop();
            return success;
        }
    }
}
