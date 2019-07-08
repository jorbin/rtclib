using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// block insert vertex parameter
    /// </summary>
    public struct BlockInsertVertex
    {
        /// <summary>
        /// Dx
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Dy
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// degree
        /// </summary>
        public double Angle { get; set; }
        /// <summary>
        /// Scale X
        /// </summary>
        public double ScaleX { get; set; }
        /// <summary>
        /// Scale Y
        /// </summary>
        public double ScaleY { get; set; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="angle"></param>
        /// <param name="sx"></param>
        /// <param name="sy"></param>
        public BlockInsertVertex(double x, double y, double angle=0.0, double sx=1.0, double sy=1.0)
        {
            this.X = x;
            this.Y = y;
            this.Angle = angle;
            this.ScaleX = sx;
            this.ScaleY = sy;
        }
        /// <summary>
        /// convert to 3x3 matrix
        /// </summary>
        public Matrix3x2 Matrix
        {
            get
            {
                return
                    Matrix3x2.CreateTranslation((float)this.X, (float)this.Y) *
                    Matrix3x2.CreateRotation((float)(this.Angle * Math.PI / 180.0)) *
                    Matrix3x2.CreateScale((float)this.ScaleX, (float)this.ScaleY);
            }
        }
    }
    /// <summary>
    /// block insert entity
    /// </summary>
    public class BlockInsert
        : Entity
        , IList<BlockInsertVertex>
    {
        private List<BlockInsertVertex> list;
        private Block block;

        /// <summary>
        /// owner master block
        /// </summary>
        public Block Master
        {
            get { return this.block; }
            set { this.block = value; }
        }

        public BlockInsert(Layer layer, Block masterBlock)
           : base(layer, "BlockInsert")
        {
            this.block = masterBlock;
            this.list = new List<BlockInsertVertex>();
        }
        public BlockInsert(Layer layer, Block masterBlock, double x, double y)
            : this(layer, masterBlock)
        {
            this.Add(new BlockInsertVertex(x, y, 0, 1.0, 1.0));  
        }
        public BlockInsert(Layer layer, Block masterBlock, BlockInsertVertex vertex)
            : this(layer, masterBlock)
        {
            this.Add(vertex);
        }

        public BlockInsertVertex this[int index]
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
        public void Add(BlockInsertVertex item)
        {
            this.list.Add(item);
        }
        public void Clear()
        {
            this.list.Clear();
        }
        public bool Contains(BlockInsertVertex item)
        {
            return this.list.Contains(item);
        }
        public void CopyTo(BlockInsertVertex[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }
        public IEnumerator<BlockInsertVertex> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
        public int IndexOf(BlockInsertVertex item)
        {
            return this.list.IndexOf(item);
        }
        public void Insert(int index, BlockInsertVertex item)
        {
            this.list.Insert(index, item);
        }
        public bool Remove(BlockInsertVertex item)
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

        public override bool Mark(IRtc rtc)
        {
            bool success = true;
            ///jump to start pos
            foreach (var item in this)
            {
                rtc.Matrix.Push(item.Matrix);
                success &= this.block.Mark(rtc);
                rtc.Matrix.Pop();
                if (!success)
                    break;
            }
            return success;
        }
    }
}
