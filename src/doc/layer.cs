using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// Layer container
    /// </summary>
    public class Layer
        : List<Entity>
    {
        public string Name { get; set; }

        public Layer(string name)
        {
            this.Name = name;
        }
    }
}
