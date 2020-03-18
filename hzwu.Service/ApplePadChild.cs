using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hzwu.Service
{
    public class ApplePadChild : ApplePad
    {
        public override void Show()
        {
            base.Show();
            Console.WriteLine($"This is {nameof(ApplePadChild)}");
        }
    }
}
