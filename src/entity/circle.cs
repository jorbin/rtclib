using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// circle entity
    /// </summary>
    public class Circle
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
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Circle(Layer layer)
            : base(layer, "Circle")
        {
        }

        public Circle(Layer layer, double radius)
            : this(layer)
        {
            this.Radius = radius;
        }

        public Circle(Layer layer, double x, double y, double radius)
            : this(layer, radius)
        {
            this.X = x;
            this.Y = y;
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
            ///translate circle's center
            success &= rtc.ListMatrix(Matrix3x2.CreateTranslation((float)this.X, (float)this.Y));
            ///jump to start pos
            success &= rtc.ListJump(new Vector2((float)(this.Radius), 0.0f));       
            ///rotate 360 degress with CCW
            success &= rtc.ListArc(new Vector2(0.0f, 0.0f), 360.0);
            return success;
        }
    }
}
