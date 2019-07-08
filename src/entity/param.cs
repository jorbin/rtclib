using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// laser parameter
    /// </summary>
    public class Param 
        : Entity
    {
        /// <summary>
        /// Hz
        /// </summary>
        double Frequency { get; set; }    
        /// <summary>
        /// usec
        /// </summary>
        double PulseWidth { get; set; }
        /// <summary>
        /// usec
        /// </summary>
        double LaserOnDelay { get; set; }
        /// <summary>
        /// usec
        /// </summary>
        double LaserOffDelay { get; set; }
        /// <summary>
        /// usec
        /// </summary>
        double ScannerJumpDelay { get; set; }
        /// <summary>
        /// usec
        /// </summary>
        double ScannerMarkDelay { get; set; }
        /// <summary>
        /// usec
        /// </summary>
        double ScannerPolygonDelay { get; set; }
        /// <summary>
        /// mm/s
        /// </summary>
        double JumpSpeed { get; set; }
        /// <summary>
        /// mm/s
        /// </summary>
        double MarkSpeed { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="layer"></param>
        public Param(Layer layer)
            : base("Param", layer)
        {
        }

        /// <summary>
        /// laser processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public override bool Mark(IRtc rtc)
        {
            bool success = true;
            success &= rtc.ListSpeed(this.JumpSpeed, this.MarkSpeed);
            success &= rtc.ListFrequency(this.Frequency, this.PulseWidth);
            success &= rtc.ListDelay(this.LaserOnDelay, this.LaserOffDelay, this.ScannerJumpDelay, this.ScannerMarkDelay, this.ScannerPolygonDelay);
            return success;
        }

    }
}
