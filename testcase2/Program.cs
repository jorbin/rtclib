/*
 *
 * Testcase program for rtclib
 * 
 * Using Doc/Layer/Entity and Field Correction 
 * Author : hong chan, choi / sepwind @gmail.com(https://sepwind.blogspot.com)
 * 
 */


using System;
using System.Diagnostics;
using System.Numerics;

namespace sepwind
{

    class Program
    {
        static Doc doc;
        static void Main(string[] args)
        {
            #region initialize RTC 
            IRtc rtc = new Rtc5(0); ///rtc 5 controller
            double fov = 60.0;    /// scanner field of view : 60mm                                
            double kfactor = Math.Pow(2, 20) / fov; /// k factor (bits/mm) = 2^20 / fov
            rtc.Initialize(kfactor, LaserMode.Yag1, "cor_1to1.ct5");    ///default correction file
            rtc.CtlFrequency(50 * 1000, 2); ///laser frequency : 50KHz, pulse width : 2usec
            rtc.CtlSpeed(100, 100); /// default jump and mark speed : 100mm/s
            rtc.CtlDelay(10, 100, 200, 200, 0); ///scanner and laser delays
            #endregion

            #region create entities for scanner field correction
            doc = new Doc("entities for field correction");
            Layer layer = doc.Layers.Active;    ///current active layer
            layer.Clear();
            /// 9 measured points (mm unit)
            layer.Add(new Spiral(doc.Layers.Active, -20.0f, 20.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 0.0f, 20.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 20.0f, 20.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, -20.0f, 0.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 0.0f, 0.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 20.0f, 0.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, -20.0f, -20.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 0.0f, -20.0f, 0.5, 2.0, 5, true));
            layer.Add(new Spiral(doc.Layers.Active, 20.0f, -20.0f, 0.5, 2.0, 5, true));
            #endregion

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for rtclib. powered by sepwind@gmail.com (https://sepwind.blogspot.com)");
                Console.WriteLine("");
                Console.WriteLine("'F' : draw field correction entities");
                Console.WriteLine("'C' : create new field correction for 2D");
                Console.WriteLine("'Q' : quit");
                Console.WriteLine("");
                Console.Write("select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                switch (key.Key)
                {                    
                    case ConsoleKey.F:
                        Console.WriteLine("\r\nWARNING !!! LASER IS BUSY ...");
                        var timer = new Stopwatch();
                        DrawForFieldCorrection(rtc, doc);
                        rtc.ListExecute(true);
                        Console.WriteLine($"processing time = {timer.ElapsedMilliseconds / 1000.0:F3}s");
                        break;
                    case ConsoleKey.C:
                        string result = CreateFieldCorrection();
                        Console.WriteLine("");
                        Console.WriteLine(result);
                        break;
                }               

            } while (true);

            rtc.Dispose();
        }

        private static void DrawForFieldCorrection(IRtc rtc, Doc doc)
        {
            ///laser processing 
            rtc.ListBegin();
            foreach (var layer in doc.Layers)
                foreach (var entity in layer)
                    entity.Mark(rtc);
            rtc.ListEnd();
        }
        private static string CreateFieldCorrection()
        {
            double fov = 60.0;    /// scanner field of view : 60mm 
            double kfactor = Math.Pow(2, 20) / fov; ///k factor (bits/mm) = 2^20 / fov

            IRtcCorrection correction = new RtcCorrection2D(9, kfactor, 20.0, "cor_1to1.ct5", "newfile.ct5");
            ///insert measured positions
            correction.Add(-20, 20);
            correction.Add(0, 20);
            correction.Add(20, 20);
            correction.Add(-20, 0);
            correction.Add(0, 0);
            correction.Add(20, 0);
            correction.Add(-20, -20);
            correction.Add(0, -20);
            correction.Add(20, -20);
            if (correction.Convert())
                return correction.Result();
            return "fail to convert new correction file !";
        }
    }
}
