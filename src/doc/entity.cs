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
        public string Name { get; }

        /// <summary>
        /// self contained layer
        /// </summary>
        public Layer Layer { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name"></param>
        public Entity(string name)
        {
            this.Name = name;
        }

        public Entity(string name, Layer layer)
        {
            this.Name = name;
            if (null != layer)
            {
                this.Layer = layer;
                layer.Add(this);
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
