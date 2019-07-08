using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// 원 엔티티
    /// </summary>
    public class Circle
        : Entity
    {
        /// <summary>
        /// 반지름 크기
        /// </summary>
        public double Radius { get; set; }       
        /// <summary>
        /// 중심 X위치
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 중심 Y위치
        /// </summary>
        public double Y { get; set; }

        public Circle(Layer layer)
            : base("Circle", layer)
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

        public override bool Mark(IRtc rtc)
        {
            if (this.Radius <= 0)
                return false;

            bool success = true;
            ///원의 중심 위치 이동
            success &= rtc.ListMatrix(Matrix3x2.CreateTranslation((float)this.X, (float)this.Y));
            ///0 도 위치의 점으로 점프
            success &= rtc.ListJump(new Vector2((float)(this.Radius), 0.0f));       
            /// 360도 시계방향 회전
            success &= rtc.ListArc(new Vector2(0.0f, 0.0f), 360.0);
            return success;
        }
    }
}
