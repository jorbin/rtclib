using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// 레이어를 가지는 컨테이너
    /// </summary>
    public class Layers
        : List<Layer>
    {
        private Layer active;

        /// <summary>
        /// 현재 활성화된 레이어
        /// </summary>
        public Layer Active { get { return this.active; } }

        public Layers()
        {
            /// 기본적으로 레이어를 하나 가지고 있다
            Layer l = new Layer("default");
            base.Add(l);
            this.active = l;
        }
    }
}
