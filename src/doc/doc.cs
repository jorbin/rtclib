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
        /// <summary>
        /// container for block
        /// </summary>
        public Blocks Blocks { get; }

        public Doc(string name)
        {
            this.Name = name;
            this.Layers = new Layers(this);
            this.Blocks = new Blocks(this);
        }

        #region serialize/deserialize
        public bool Open(string fileName)
        {
            return false;
        }
        public bool Save(string fileName)
        {
            return false;
        }
        #endregion
    }
}
