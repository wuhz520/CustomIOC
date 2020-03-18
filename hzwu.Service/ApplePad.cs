using hzwu.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hzwu.Service
{
    public class ApplePad : AbstractPad
    {
        public override void Show()
        {
            Console.WriteLine($"This is {nameof(ApplePad)}");
        }
    }
}
