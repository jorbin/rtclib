using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// Layers container
    /// </summary>
    public class Layers
        : List<Layer>
    {
        private Layer active;
        private Doc owner;

        /// <summary>
        /// current activated layer
        /// </summary>
        public Layer Active { get { return this.active; } }

        public Doc Owner { get { return this.owner; } }

        public Layers(Doc owner)
        {
            this.owner = owner;
            /// there's one default layer 
            Layer l = new Layer(this, "default");
            base.Add(l);
            this.active = l;
        }
    }
}
