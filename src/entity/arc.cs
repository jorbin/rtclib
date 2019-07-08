using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// arc entity
    /// </summary>
    public class Arc
        : Entity
    {
        /// <summary>
        /// radius (mm)
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// center X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// center Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// start angle (degree)
        /// </summary>
        public double StartAngle { get; set; }
        /// <summary>
        /// sweep angle (degree)
        /// + : CCW, - : CW
        /// </summary>
        public double SweepAngle { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Arc(Layer layer)
            : base(layer, "Arc")
        {
        }

        public Arc(Layer layer, double x, double y, double radius, double startAngle, double sweepAngle)
            : this(layer)
        {
            this.X = x;
            this.Y = y;
            this.Radius = radius;
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }

        /// <summary>
        /// laser processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public override bool Mark(IRtc rtc)
        {
            if (this.Radius <= 0)
                return false;

            bool success = true;
            ///translate arc's center
            rtc.Matrix.Push(this.X, this.Y);
            ///jump to start pos
            double x = this.Radius * Math.Sin(this.StartAngle * Math.PI / 180.0);
            double y = this.Radius * Math.Cos(this.SweepAngle * Math.PI / 180.0);
            success &= rtc.ListJump(new Vector2((float)x, (float)y));
            success &= rtc.ListArc(new Vector2(0.0f, 0.0f), this.SweepAngle);
            ///revert matrix
            rtc.Matrix.Pop();
            return success;
        }
    }
}
