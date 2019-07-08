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
    /// block entity
    /// entity container
    /// </summary>
    public class Block
        : Entity
        , IList<Entity>
    {
        private List<Entity> list;

        public Block()
           : base(null, "Block")
        {
            this.list = new List<Entity>();
        }

        public Entity this[int index]
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
        public void Add(Entity item)
        {
            this.list.Add(item);
        }
        public void Clear()
        {
            this.list.Clear();
        }
        public bool Contains(Entity item)
        {
            return this.list.Contains(item);
        }
        public void CopyTo(Entity[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }
        public IEnumerator<Entity> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
        public int IndexOf(Entity item)
        {
            return this.list.IndexOf(item);
        }
        public void Insert(int index, Entity item)
        {
            this.list.Insert(index, item);
        }
        public bool Remove(Entity item)
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
            bool success = true;
            foreach (var entity in this)
            {
                success &= entity.Mark(rtc);
                if (!success)
                    break;
            }
            return success;
        }
    }
}
