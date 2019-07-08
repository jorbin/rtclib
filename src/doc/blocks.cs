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
    public class Blocks
        : List<Block>
    {
        private Doc owner;

        public Doc Owner { get { return this.owner; } }

        public Blocks(Doc owner)
        {
            this.owner = owner;           
        }
    }
}
