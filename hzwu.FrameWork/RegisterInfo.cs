using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hzwu.FrameWork
{
    public class RegisterInfo
    {
        /// <summary>
        /// 目标类型
        /// </summary>
        public Type TargetType { get; set; }
        /// <summary>
        /// 生命周期
        /// </summary>
        public LifeTimeType LifeTime { get; set; }
    }

    public enum LifeTimeType
    {
        Transient,
        Singleton,
        PerThread
    }
}
