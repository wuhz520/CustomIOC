using System;
using hzwu.FrameWork;
using hzwu.Interface;
using hzwu.Service;
using System.Threading;
using System.Threading.Tasks;

namespace hzwu.CustomIOC
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                IContainer container = new Container();
                container.RegisterType<IPhone, AndroidPhone>(LifeTimeType.Singleton);
                container.RegisterType<AbstractPad, ApplePad>(LifeTimeType.PerThread);
                container.RegisterType<IHeadphone, Headphone>(LifeTimeType.Transient);
                container.RegisterType<IMicrophone, Microphone>(LifeTimeType.Singleton);
                container.RegisterType<IPower, Power>();
                IPhone pad1 = null;
                IPhone pad2 = null;
                IPhone pad3 = null;
                pad1 = container.Resolve<IPhone>();
                Action act1 = new Action(() =>
                {
                    pad1 = container.Resolve<IPhone>();
                    Console.WriteLine($"pad1由线程id={Thread.CurrentThread.ManagedThreadId}");
                });
                var result1 = Task.Run(() => act1());

                Action act2 = new Action(() =>
                {
                    pad2 = container.Resolve<IPhone>();
                    Console.WriteLine($"pad2由线程id={Thread.CurrentThread.ManagedThreadId}");
                });

                var result2 = Task.Run(() => act2()).ContinueWith(t =>
                {
                    pad3 = container.Resolve<IPhone>();
                    Console.WriteLine($"pad3由线程id={Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine($"object.ReferenceEquals(pad2, pad3)={object.ReferenceEquals(pad2, pad3)}");
                });

                Console.WriteLine($"object.ReferenceEquals(pad1, pad2)={object.ReferenceEquals(pad1, pad2)}");
            }
            Console.ReadLine ();
        }
    }
}
