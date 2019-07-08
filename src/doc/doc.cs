using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    public class Doc
    {
        /// <summary>
        /// 식별 이름
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 레이어 컨테이너
        /// </summary>
        public Layers Layers { get; }


        public Doc(string name)
        {
            this.Layers = new Layers();
        }

    }
}
