using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            rtc.CtlFrequency(50 * 1000, 2);    ///freq : 50KHz, pulse width : 2usec
            rtc.CtlSpeed(100, 100); /// 100mm/s
            rtc.CtlDelay(10, 100, 200, 200, 0);    ///delays

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Select your target :  (Q : quit)");
                Console.WriteLine("'C' : draw circle");
                Console.WriteLine("'R' : draw rectangle");
                Console.WriteLine("'D' : draw circle with dots");
                Console.WriteLine("'L' : draw lines with rotate");
                Console.WriteLine("'Q' : quit");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;

                switch (key.Key)
                {
                    case ConsoleKey.C:  //circle
                        DrawCircle(rtc, 10);
                        break;
                    case ConsoleKey.R:
                        DrawRectangle(rtc, 10, 10);
                        break;
                    case ConsoleKey.D:
                        DrawCircleWithDots(rtc, 10, 1.0);
                        break;
                    case ConsoleKey.L:
                        DrawLinesWithRotate(rtc, 0, 360);
                        break;
                }
                Console.WriteLine("press any key to fire the laser ...");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                rtc.ListExecute(true);
            } while (true);

            rtc.Dispose();
        }

        private static void DrawCircle(IRtc rtc, double radius)
        {
            rtc.ListBegin();
            rtc.ListJump(new System.Numerics.Vector2((float)radius, 0));
            rtc.ListArc(new System.Numerics.Vector2(0, 0), 360.0);
            rtc.ListEnd();
        }

        private static void DrawRectangle(IRtc rtc, double width, double height)
        {
            rtc.ListBegin();
            rtc.ListJump(new System.Numerics.Vector2((float)-width / 2, (float)height / 2));
            rtc.ListMark(new System.Numerics.Vector2((float)width / 2, (float)height / 2));
            rtc.ListMark(new System.Numerics.Vector2((float)width / 2, (float)-height / 2));
            rtc.ListMark(new System.Numerics.Vector2((float)-width / 2, (float)-height / 2));
            rtc.ListMark(new System.Numerics.Vector2((float)-width / 2, (float)height / 2));
            rtc.ListEnd();
        }

        private static void DrawCircleWithDots(IRtc rtc, double radius, double durationMsec)
        {
            rtc.ListBegin();
            for (double angle=0; angle<360; angle+=1)
            {
                double x = radius * Math.Sin(angle * Math.PI / 180.0);
                double y = radius * Math.Cos(angle * Math.PI / 180.0);
                rtc.ListJump(new System.Numerics.Vector2((float)x, (float)y));
                rtc.ListLaserOn(durationMsec);                
            }            
            rtc.ListEnd();
        }
        private static void DrawLinesWithRotate(IRtc rtc, double angleStart, double angleEnd)
        {
            rtc.ListBegin();
            for (double angle = angleStart; angle <= angleEnd; angle += 1)
            {
                rtc.ListMatrix(Matrix3x2.CreateRotation((float)(angle * Math.PI / 180.0)));
                rtc.ListJump(new System.Numerics.Vector2(-10, 0));
                rtc.ListMark(new System.Numerics.Vector2(10, 0));
            }
            rtc.ListEnd();
        }
        
    }
}
