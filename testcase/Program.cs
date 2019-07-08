/*
 *
 * Testcase program for rtclib
 * Author : hong chan, choi / sepwind @gmail.com(https://sepwind.blogspot.com)
 * 
 */


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
        static Doc doc;
        static void Main(string[] args)
        {
            #region RTC 초기화
            IRtc rtc = new Rtc5(0);
            double fov = 60;
            rtc.Initialize(Math.Pow(2, 20) / fov, LaserMode.Yag1, "cor_1to1.ct5");
            rtc.CtlFrequency(50 * 1000, 2);    ///freq : 50KHz, pulse width : 2usec
            rtc.CtlSpeed(100, 100); /// 100mm/s
            rtc.CtlDelay(10, 100, 200, 200, 0);    ///delays
            #endregion


            #region 개별 객체(entity) 를 생성하여 추후 스캐너 보정에 쓰는 방식 예제
            doc = new Doc("entities for field correction");
            /// 나선 모양을 9개 만들어 doc 에 저장
            Vector2[] examplePos = new Vector2[9];
            examplePos[0] = new Vector2(-20.0f, 20.0f);
            examplePos[1] = new Vector2(0.0f, 20.0f);
            examplePos[2] = new Vector2(20.0f, 20.0f);
            examplePos[3] = new Vector2(-20.0f, 0.0f);
            examplePos[4] = new Vector2(0.0f, 0.0f);
            examplePos[5] = new Vector2(20.0f, 0.0f);
            examplePos[6] = new Vector2(-20.0f, -20.0f);
            examplePos[7] = new Vector2(0.0f, -20.0f);
            examplePos[8] = new Vector2(20.0f, -20.0f);

            Layer layer = doc.Layers.Active;    ///현재 활성 레이어
            layer.Clear();
            for (int i = 0; i < examplePos.Length; i++)
                layer.Add(new Spiral(
                        doc.Layers.Active,
                        0.5, //안의 공백 0.5mm
                        2.0, //외곽 크기 2mm
                        5,  //5 바퀴회전
                        true) //폐곡선
                    );
            #endregion

            ConsoleKeyInfo key;
            do
            {
                Console.WriteLine("Testcase for rtclib. powered by sepwind@gmail.com");                
                Console.WriteLine("select your target :  (Q : quit)");
                Console.WriteLine("'C' : draw circle");
                Console.WriteLine("'R' : draw rectangle");
                Console.WriteLine("'D' : draw circle with dots");
                Console.WriteLine("'L' : draw lines with rotate");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("'F' : draw entities for field correction");
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
                    case ConsoleKey.F:
                        DrawForFieldCorrection(rtc, doc);
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

        private static void DrawForFieldCorrection(IRtc rtc, Doc doc)
        {
            rtc.ListBegin();

            ///doc 에 있는 레이어의 모든 엔티티를 가공
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
