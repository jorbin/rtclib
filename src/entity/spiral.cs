﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// 나선 엔티티
    /// </summary>
    public class Spiral 
        : Entity
    {
        /// <summary>
        /// 안쪽 지름 크기
        /// </summary>
        public double InnerDiameter { get; set; }
        /// <summary>
        /// 바깥쪽 지름 크기
        /// </summary>
        public double OutterDiameter { get; set; }
        /// <summary>
        /// 회전수
        /// </summary>
        public int Revolutions { get; set; }
        /// <summary>
        /// 폐곡선 여부
        /// </summary>
        public bool Closed { get; set; }
        /// <summary>
        /// 중심 X위치
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 중심 Y위치
        /// </summary>
        public double Y { get; set; }

        public Spiral(Layer layer)
            : base("Spiral", layer)
        {
        }

        public Spiral(Layer layer, double innerDiameter, double outterDiameter, int revolutions, bool closed)
            : this(layer)
        {
            this.InnerDiameter = innerDiameter;
            this.OutterDiameter = outterDiameter;
            this.Revolutions = revolutions;
            this.Closed = closed;
        }

        public Spiral(Layer layer, double x, double y, double innerDiameter, double outterDiameter, int revolutions, bool closed)
            : this(layer, innerDiameter, outterDiameter, revolutions, closed)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Mark(IRtc rtc)
        {
            if (this.OutterDiameter <= 0)
                return false;
            if (this.InnerDiameter > this.OutterDiameter)
                return false;
            if (this.Revolutions <= 0)
                return false;

            bool success = true;
            
            ///나선의 중심 위치 이동
            success &= rtc.ListMatrix(Matrix3x2.CreateTranslation((float)this.X, (float)this.Y));
            ///0 도 위치의 점으로 점프
            success &= rtc.ListJump(new Vector2( (float)(this.InnerDiameter / 2.0), 0.0f));
            ///1 회전당 간격 계산
            double radialPitch = (this.OutterDiameter  - this.InnerDiameter) / 2.0 / (double)this.Revolutions;
            for (int i = 0; i < this.Revolutions; i++)
            {
                for (double t = 0; t < 360; t += 30.0)  ///360/30 = 12 개로 쪼개어서 처리
                {
                    double angle = t + 360.0 * (double)i;
                    double degInRad = angle * Math.PI / 180.0;
                    double d = InnerDiameter / 2.0 + radialPitch * (double)i + radialPitch * t / 360.0;
                    Vector2 v2;
                    v2.X = (float)(d * Math.Cos(degInRad));
                    v2.Y = (float)(d * Math.Sin(degInRad));
                    success &= rtc.ListMark(v2);
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            if (success && this.Closed)
                success &= rtc.ListArc(new Vector2(0.0f, 0.0f), 360.0);
            return success;
        }
    }
}