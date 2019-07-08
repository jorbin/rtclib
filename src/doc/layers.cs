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

        /// <summary>
        /// current activated layer
        /// </summary>
        public Layer Active { get { return this.active; } }

        public Layers()
        {
            /// there's one default layer 
            Layer l = new Layer("default");
            base.Add(l);
            this.active = l;
        }
    }
}
