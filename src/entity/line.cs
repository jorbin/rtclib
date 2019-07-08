using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// line entity
    /// </summary>
    public class Line
        : Entity
    {
        /// <summary>
        /// start position
        /// </summary>
        public double StartX { get; set; }
        public double StartY { get; set; }
        /// <summary>
        /// end position
        /// </summary>
        public double EndX { get; set; }
        public double EndY { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Line(Layer layer)
            : base(layer, "Line")
        {
        }

        public Line(Layer layer, double startX, double startY, double endX, double endY)
            : this (layer)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.EndX = endX;
            this.EndY = endY;
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
            success &= rtc.ListJump(new Vector2((float)this.StartX, (float)this.StartY));
            ///mark to end pos
            success &= rtc.ListMark(new Vector2((float)this.EndX, (float)this.EndY));
            return success;
        }
    }
}
