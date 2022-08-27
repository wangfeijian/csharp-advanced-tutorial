using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Prototype
{
    [Serializable]
    public class NormalActor : NormalActorBase
    {
        public NormalActor(int id, Gan gan)
        {
            ID = id;
            NormalGan = gan;
        }

        /// <summary>
        /// 这个方法需要实现深拷贝，因为对象里面有引用类型的
        /// 类需要标记[Serializable]特性
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            object retObj;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                retObj = bf.Deserialize(ms);
                ms.Close();
            }

            return retObj as NormalActorBase;
        }
    }

    public class FlyActor : FlyActorBase
    {
        public FlyActor(int id, Gan gan)
        {
            ID= id;
            FlyGan = gan;
        }

        /// <summary>
        /// 这里使用的是浅拷贝，因为对象中只有值类型
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class WaterActor : WaterActorBase
    {
        public WaterActor(int id)
        {
            ID = id;
        }

        /// <summary>
        /// 这里使用的是浅拷贝，因为对象中只有值类型
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}
