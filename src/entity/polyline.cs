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
    }

    /// <summary>
    /// LW polyline 
    /// polyline vertex container
    /// </summary>
    public class Polyline 
        : Entity
        , IList<PolyLineVertex>
    {
        private List<PolyLineVertex> vertices;

        /// <summary>
        /// whethher closed figure or not
        /// </summary>
        public bool Closed { get; set; }

        public Polyline(Layer layer)
           : base("Polyline", layer)
        {
            this.vertices = new List<PolyLineVertex>();
        }

        public PolyLineVertex this[int index]
        {
            get { return this.vertices[index]; }
            set { this.vertices[index] = value; }
        }

        public int Count
        {
            get { return this.vertices.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Add(PolyLineVertex item)
        {
            this.vertices.Add(item);
        }

        public void Clear()
        {
            this.vertices.Clear();
        }

        public bool Contains(PolyLineVertex item)
        {
            return this.vertices.Contains(item);
        }

        public void CopyTo(PolyLineVertex[] array, int arrayIndex)
        {
            this.vertices.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PolyLineVertex> GetEnumerator()
        {
            return this.vertices.GetEnumerator();
        }

        public int IndexOf(PolyLineVertex item)
        {
            return this.vertices.IndexOf(item);
        }

        public void Insert(int index, PolyLineVertex item)
        {
            this.vertices.Insert(index, item);
        }

        public bool Remove(PolyLineVertex item)
        {
            return this.vertices.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.vertices.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.vertices.GetEnumerator();
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
            //incorrect way to create continous paths
            //foreach (var entity in entities)
            //{
            //    success &= entity.Mark(rtc);
            //    if (!success)
            //        break;
            //}
            bool first = false;
            foreach (var entity in entities)
            {
                if (entity is Line)
                {
                    Line line = entity as Line;
                    if (!first)
                    {
                        rtc.ListJump(new Vector2((float)line.StartX, (float)line.StartY));
                        first = true;
                    }
                    rtc.ListMark(new Vector2((float)line.EndX, (float)line.EndY));
                }
                else
                {
                    Arc arc = entity as Arc;
                    if (!first)
                    {
                        double x = arc.Radius * Math.Sin(arc.StartAngle * Math.PI / 180.0);
                        double y = arc.Radius * Math.Cos(arc.SweepAngle * Math.PI / 180.0);
                        success &= rtc.ListJump(new Vector2((float)(x + arc.X), (float)(y + arc.Y)));
                        first = true;
                    }
                    success &= rtc.ListArc(new Vector2((float)arc.X, (float)arc.Y), arc.SweepAngle);
                }
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
            List<Entity> entities = new List<Entity>(this.vertices.Count);
            int index = 0;
            foreach (var vertex in this.vertices)
            {
                double bulge = vertex.Bulge;
                Vector2 p1;
                Vector2 p2;

                if (index == this.vertices.Count - 1)
                {
                    if (!this.Closed)
                        break;
                    p1 = new Vector2((float)vertex.X, (float)vertex.Y);
                    p2 = new Vector2((float)this[0].X, (float)this[0].Y);
                }
                else
                {
                    p1 = new Vector2((float)vertex.X, (float)vertex.Y);
                    p2 = new Vector2((float)this[index + 1].X, (float)this[index + 1].Y);
                }

                if (Math.Abs(bulge) < 0.001)   //isZero
                {
                    // the polyline edge is a line
                    entities.Add(new Line(null, p1.X, p1.Y, p2.X, p2.Y));
                }
                else
                {
                    // the polyline edge is an arc
                    double theta = 4 * Math.Atan(Math.Abs(bulge));
                    double c = Vector2.Distance(p1, p2);
                    double r = (c / 2) / Math.Sin(theta / 2);

                    // avoid arcs with very small radius, draw a line instead
                    if (Math.Abs(r) < 0.001)    //is Zero
                    {
                        // the polyline edge is a line
                        entities.Add(new Line(null, p1.X, p1.Y, p2.X, p2.Y));
                    }
                    else
                    {
                        double gamma = (Math.PI - theta) / 2.0;
                        double phi = MathUtil.AngleByRad(p1, p2) + Math.Sign(bulge) * gamma;
                        Vector2 center = new Vector2((float)(p1.X + r * Math.Cos(phi)), (float)(p1.Y + r * Math.Sin(phi)));
                        double startAngle;
                        double endAngle;
                        if (bulge > 0)
                        {
                            startAngle = 180.0 / Math.PI * MathUtil.AngleByRad(p1 - center);
                            endAngle = startAngle + 180.0 / Math.PI * theta;
                        }
                        else
                        {
                            endAngle = 180.0 / Math.PI * MathUtil.AngleByRad(p1 - center);
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
