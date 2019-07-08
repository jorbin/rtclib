using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind.src.entity
{
    /// <summary>
    /// LW polyline vertex
    /// </summary>
    public struct PolyLineVertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Bulge { get; set; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bulge"></param>
        public PolyLineVertex(double x, double y, double bulge = 0.0)
        {
            this.X = x;
            this.Y = y;
            this.Bulge = bulge;
        }

        #region static methods
        public static PolyLineVertex operator -(PolyLineVertex left, PolyLineVertex right)
        {
            return new PolyLineVertex(left.X - right.X, left.Y - right.Y, 0.0);
        }

        internal static bool IsZero(double value)
        {
            return Math.Abs(value) < 0.001;
        }
        public static double Distance(PolyLineVertex v1, PolyLineVertex v2)
        {
            return Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
        }
        public static double Angle(PolyLineVertex v1, PolyLineVertex v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2.0) + Math.Pow(v1.Y - v2.Y, 2.0));
        }
        public static double Angle(PolyLineVertex v)
        {
            return Math.Atan2(v.Y, v.X);
        }
        #endregion
    }

    /// <summary>
    /// LW polyline 
    /// polyline vertex container
    /// </summary>
    public class Polyline 
        : Entity
        , IList<PolyLineVertex>
    {
        private List<PolyLineVertex> list;
        /// <summary>
        /// whethher closed figure or not
        /// </summary>
        public bool Closed { get; set; }

        public Polyline(Layer layer)
           : base(layer, "Polyline")
        {
            this.list = new List<PolyLineVertex>();
        }

        public PolyLineVertex this[int index]
        {
            get { return this.list[index]; }
            set { this.list[index] = value; }
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Add(PolyLineVertex item)
        {
            this.list.Add(item);
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public bool Contains(PolyLineVertex item)
        {
            return this.list.Contains(item);
        }

        public void CopyTo(PolyLineVertex[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PolyLineVertex> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        public int IndexOf(PolyLineVertex item)
        {
            return this.list.IndexOf(item);
        }

        public void Insert(int index, PolyLineVertex item)
        {
            this.list.Insert(index, item);
        }

        public bool Remove(PolyLineVertex item)
        {
            return this.list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// laser processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public override bool Mark(IRtc rtc)
        {
            ///explode entity to line and arcs
            List<Entity> entities = this.Explode();
            bool success = true;            
            foreach (var entity in entities)
            {
                success &= entity.Mark(rtc);
                if (!success)
                    break;
            }
            return success;
        }
        /// <summary>
        /// explode polyline into line and arc entities
        /// </summary>
        /// <returns></returns>
        public List<Entity> Explode()
        {
            List<Entity> entities = new List<Entity>(this.Count);
            int index = 0;
            foreach (var vertex in this)
            {
                double bulge = vertex.Bulge;
                PolyLineVertex p1;
                PolyLineVertex p2;

                if (index == this.Count - 1)
                {
                    if (!this.Closed)
                        break;
                    p1 = new PolyLineVertex(vertex.X, vertex.Y);
                    p2 = new PolyLineVertex(this[0].X, this[0].Y);
                }
                else
                {
                    p1 = new PolyLineVertex(vertex.X, vertex.Y);
                    p2 = new PolyLineVertex(this[index + 1].X, this[index + 1].Y);
                }

                if (PolyLineVertex.IsZero(bulge))
                {
                    // the polyline edge is a line
                    entities.Add(new Line(null, p1.X, p1.Y, p2.X, p2.Y));
                }
                else
                {
                    // the polyline edge is an arc
                    double theta = 4 * Math.Atan(Math.Abs(bulge));
                    double c = PolyLineVertex.Distance(p1, p2);
                    double r = (c / 2) / Math.Sin(theta / 2);

                    // avoid arcs with very small radius, draw a line instead
                    if (PolyLineVertex.IsZero(r))    
                    {
                        // the polyline edge is a line
                        entities.Add(new Line(null, p1.X, p1.Y, p2.X, p2.Y));
                    }
                    else
                    {
                        double gamma = (Math.PI - theta) / 2.0;
                        double phi = PolyLineVertex.Angle(p1, p2) + Math.Sign(bulge) * gamma;
                        PolyLineVertex center = new PolyLineVertex(p1.X + r * Math.Cos(phi), p1.Y + r * Math.Sin(phi));
                        double startAngle;
                        double endAngle;
                        if (bulge > 0)
                        {
                            startAngle = 180.0 / Math.PI * PolyLineVertex.Angle(p1 - center);
                            endAngle = startAngle + 180.0 / Math.PI * theta;
                        }
                        else
                        {
                            endAngle = 180.0 / Math.PI * PolyLineVertex.Angle(p1 - center);
                            startAngle = endAngle - 180.0 / Math.PI * theta;
                        }
                        entities.Add(new Arc(null, center.X, center.Y, r, startAngle, endAngle - startAngle));
                    }
                }
                index++;
            }
            return entities;
        }
    }
}
