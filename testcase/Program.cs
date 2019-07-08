﻿/*
 *
 * Testcase program for rtclib
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
            double fov = 60;    /// scanner field of view : 60mm
            /// k factor (bits/mm) = 2^20 / fov
            double kfactor = Math.Pow(2, 20) / fov;
            rtc.Initialize(kfactor, LaserMode.Yag1, "cor_1to1.ct5");    ///default correction file
            rtc.CtlFrequency(50 * 1000, 2); ///laser frequency : 50KHz, pulse width : 2usec
            rtc.CtlSpeed(100, 100); /// default jump and mark speed : 100mm/s
            rtc.CtlDelay(10, 100, 200, 200, 0); ///scanner and laser delays
            #endregion

            #region create entities for scanner field correction
            doc = new Doc("entities for field correction");
            Layer layer = doc.Layers.Active;    ///current active layer
            layer.Clear();
            /// 9 measured points (3x3 positions by milli-meter units)
            layer.Add(new Spiral(doc.Layers.Active, -20.0f, 20.0f, 0.5, 2.0, 5, true) );
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
                Console.WriteLine("'C' : draw circle");
                Console.WriteLine("'R' : draw rectangle");
                Console.WriteLine("'D' : draw circle with dots");
                Console.WriteLine("'L' : draw lines with rotate");
                Console.WriteLine("'F' : draw entities for field correction");
                Console.WriteLine("'Q' : quit");
                Console.WriteLine("");
                Console.Write("select your target : ");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;

                var timer = new Stopwatch();
                switch (key.Key)
                {
                    case ConsoleKey.C: 
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
                    case ConsoleKey.F:
                        DrawForFieldCorrection(rtc, doc);
                        break;
                }
                Console.WriteLine("WARNING !!! press any key to fire the laser ...");
                key = Console.ReadKey(false);
                if (key.Key == ConsoleKey.Q)
                    break;
                rtc.ListExecute(true);
                Console.WriteLine($"processing time = {timer.ElapsedMilliseconds/1000.0:F3}s");                
            } while (true);

            rtc.Dispose();
        }
        private static void DrawForFieldCorrection(IRtc rtc, Doc doc)
        {
            ///laser processing with 3x3 spiral entities
            rtc.ListBegin();
            foreach(var layer in doc.Layers)
                foreach (var entity in layer)
                    entity.Mark(rtc);
            rtc.ListEnd();
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
