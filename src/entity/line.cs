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
    public class Line
        : Entity
    {
        /// <summary>
        /// start position
        /// </summary>
        public Vector2 Start { get; set; }
        /// <summary>
        /// end position
        /// </summary>
        public Vector2 End { get; set; }        

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Line(Layer layer)
            : base("Line", layer)
        {
        }

        public Line(Layer layer, Vector2 start, Vector2 end)
            : this (layer)
        {
            this.Start = start;
            this.End = end;
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
            success &= rtc.ListJump(this.Start);
            ///mark to end pos
            success &= rtc.ListMark(this.End);
            return success;
        }
    }
}
