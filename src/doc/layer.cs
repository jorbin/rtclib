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
        private Layers owner;

        public string Name { get; set; }

        public bool Selected { get; set; }

        public bool Visible { get; set; }

        public bool Mark { get; set; }

        public Layers Owner { get { return this.owner; } }

        public Layer(Layers owner, string name)
        {
            this.owner = owner;
            this.Name = name;
            this.Visible = true;
            this.Mark = true;
        }
        public Layer(Layers owner, string name, bool visible, bool mark)
        {
            this.owner = owner;
            this.Name = name;
            this.Visible = visible;
            this.Mark = mark;
        }
    }
}
