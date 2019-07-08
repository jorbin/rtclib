using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace sepwind
{
    /// <summary>
    /// 3x3 matrix stack
    /// </summary>
    public sealed class MatrixStack :
        Stack<Matrix3x2>
    {
        /// <summary>
        /// calculate all matrices in stack
        /// </summary>
        public Matrix3x2 Calculate
        {
            get
            {

                Matrix3x2 result = Matrix3x2.Identity;
                foreach (Matrix3x2 m in this)
                    result *= m;
                return result;
            }
        }

        public MatrixStack()
        {
            this.Clear();
        }
        /// <summary>
        /// reset to 3x3 identity matrix
        /// </summary>
        public void LoadIdentity()
        {
            base.Clear();
            base.Push(Matrix3x2.Identity);
        }        
        /// <summary>
        /// push angle
        /// </summary>
        /// <param name="angle">degree</param>
        public void Push(double angle)
        {
            base.Push(Matrix3x2.CreateRotation((float)(angle * Math.PI / 180.0)));
        }
        /// <summary>
        /// push translate by dx, dy
        /// </summary>
        /// <param name="dx">mm</param>
        /// <param name="dy">mm</param>
        public void Push(double dx, double dy)
        {
            base.Push(Matrix3x2.CreateTranslation(new Vector2((float)dx, (float)dy)));
        }
        /// <summary>
        /// push rotate and translate
        /// </summary>
        /// <param name="dx">mm</param>
        /// <param name="dy">mm</param>
        /// <param name="angle">degree</param>
        public void Push(double dx, double dy, double angle)
        {
            this.Push(angle);
            this.Push(dx, dy);
        }        
    }
}