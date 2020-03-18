using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hzwu.FrameWork
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class InjectionConstructorAttribute : Attribute
    {
        public InjectionConstructorAttribute()
        { }
    }
}
