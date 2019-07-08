using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// Document 
    /// </summary>
    public class Doc
    {
        /// <summary>
        /// Doc's Name 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// container for layer
        /// </summary>
        public Layers Layers { get; }


        public Doc(string name)
        {
            this.Layers = new Layers();
        }

    }
}
