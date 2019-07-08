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
    /// group entity
    /// entity container
    /// </summary>
    public class Group
        : Entity
        , IList<Entity>
    {
        private List<Entity> entities;

        public Group(Layer layer)
           : base("Group", layer)
        {
            this.entities = new List<Entity>();
        }

        public Entity this[int index]
        {
            get { return this.entities[index]; }
            set { this.entities[index] = value; }
        }

        public int Count
        {
            get { return this.entities.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Add(Entity item)
        {
            this.entities.Add(item);
        }

        public void Clear()
        {
            this.entities.Clear();
        }

        public bool Contains(Entity item)
        {
            return this.entities.Contains(item);
        }

        public void CopyTo(Entity[] array, int arrayIndex)
        {
            this.entities.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }

        public int IndexOf(Entity item)
        {
            return this.entities.IndexOf(item);
        }

        public void Insert(int index, Entity item)
        {
            this.entities.Insert(index, item);
        }

        public bool Remove(Entity item)
        {
            return this.entities.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.entities.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.entities.GetEnumerator();
        }

        /// <summary>
        /// laser processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public override bool Mark(IRtc rtc)
        {
            bool success = true;
            foreach (var entity in entities)
            {
                success &= entity.Mark(rtc);
                if (!success)
                    break;
            }            
            return success;
        }
    }
}
