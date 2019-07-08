using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// abstract class for Entity
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// entity name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// selected or not
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// self contained layer
        /// </summary>
        public Layer Owner { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name"></param>
        public Entity(string name)
        {
            this.Name = name;
        }

        public Entity(Layer owner, string name)
        {
            this.Name = name;
            if (null != owner)
            {
                this.Owner = owner;
                owner.Add(this);
            }
        }

        /// <summary>
        /// abstract method for RTC processing
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public abstract bool Mark(IRtc rtc);


    }
}
