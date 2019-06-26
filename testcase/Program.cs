using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sepwind;

namespace testcase
{
    class Program
    {
        static void Main(string[] args)
        {
            IRtc rtc = new Rtc5(0);

            double fov = 60;
            rtc.Initialize(Math.Pow(2, 20) / fov, LaserMode.Yag1, "cor_1to1.ct5");

            rtc.ListBegin();
            rtc.ListFrequency(50 * 1000, 2);    ///freq : 50KHz, pulse width : 2usec
            rtc.ListSpeed(100, 100); /// 100mm/s
            rtc.ListDelay(10, 100, 200, 200, 0);    ///delays

            ///draw circle
            rtc.ListJump(new System.Numerics.Vector2(10, 0));
            rtc.ListArc(new System.Numerics.Vector2(0, 0), 360.0);

            rtc.ListEnd();

            Console.WriteLine("press any key to fire the laser ...");
            Console.ReadKey(false);
            rtc.ListExecute(true);


            rtc.Dispose();
        }
    }
}
