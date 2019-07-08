using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sepwind
{
    /// <summary>
    /// 엔티티 추상 객체
    /// </summary>
    public abstract class Entity
    {
        public string Name { get; }

        /// <summary>
        /// 자신이 속한 레이어 
        /// </summary>
        public Layer Layer { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name"></param>
        public Entity(string name)
        {
            this.Name = name;
        }

        public Entity(string name, Layer layer)
        {
            this.Name = name;
            this.Layer = layer;
            layer.Add(this);
        }

        /// <summary>
        /// 실제 레이저및 스캐너 가공을 하는 추상 함수
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        public abstract bool Mark(IRtc rtc);


    }
}
