using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// 레이어 
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
