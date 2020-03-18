using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hzwu.FrameWork
{
    public interface IContainer
    {
        void RegisterType<TFrom, TTo>(LifeTimeType lifeTimeType = LifeTimeType.Transient);
        T Resolve<T>();
    }

    /// <summary>
    /// 容器--工厂
    /// </summary>
    public class Container : IContainer
    {
        private Dictionary<string, RegisterInfo> ContainerDictionary = new Dictionary<string, RegisterInfo>();

        /// <summary>
        /// 缓存起来，类型的对象实例
        /// </summary>
        private Dictionary<Type, object> TypeObjectDictionary = new Dictionary<Type, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name="lifeTimeType">默认参数，不传递就是Transient</param>
        public void RegisterType<TFrom, TTo>(LifeTimeType lifeTimeType = LifeTimeType.Transient)
        {
            ContainerDictionary.Add(typeof(TFrom).FullName, new RegisterInfo()
            {
                TargetType = typeof(TTo),
                LifeTime = lifeTimeType
            });
        }

        public T Resolve<T>()
        {
            RegisterInfo info = ContainerDictionary[typeof(T).FullName];
            Type type = ContainerDictionary[typeof(T).FullName].TargetType;
            T result = default(T);
            switch (info.LifeTime)
            {
                case LifeTimeType.Transient:
                    result = (T)this.CreateObject(type);
                    break;
                case LifeTimeType.Singleton:
                    if (this.TypeObjectDictionary.ContainsKey(type))
                    {
                        result = (T)this.TypeObjectDictionary[type];
                    }
                    else
                    {
                        result = (T)this.CreateObject(type);
                        this.TypeObjectDictionary[type] = result;
                    }
                    break;
                case LifeTimeType.PerThread:
                    //怎么保证用线程校验呢？ 线程槽，把数据存在这里
                    {
                        string key = type.FullName;
                        object oValue = CallContext.GetData(key);
                        if (oValue == null)
                        {
                            result = (T)this.CreateObject(type);
                            CallContext.SetData(key, result);
                        }
                        else
                        {
                            result = (T)oValue;
                        }
                    }
                    break;
                default:
                    throw new Exception("wrong LifeTime");
            }
            return result;
        }
        private object CreateObject(Type type)
        {
            ConstructorInfo[] ctorArray = type.GetConstructors();
            ConstructorInfo ctor = null;
            if (ctorArray.Count(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)) > 0)
            {
                ctor = ctorArray.FirstOrDefault(c => c.IsDefined(typeof(InjectionConstructorAttribute), true));
            }
            else
            {
                ctor = ctorArray.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            }
            List<object> paraList = new List<object>();
            foreach (var parameter in ctor.GetParameters())
            {
                Type paraType = parameter.ParameterType;
                RegisterInfo info = ContainerDictionary[paraType.FullName];
                Type targetType = info.TargetType;
                //object para = this.CreateObject(targetType);
                object para = null;
                #region 
                {
                    switch (info.LifeTime)
                    {
                        case LifeTimeType.Transient:
                            para = this.CreateObject(targetType);
                            break;
                        case LifeTimeType.Singleton:
                            //需要线程安全 双if+lock
                            {
                                if (this.TypeObjectDictionary.ContainsKey(targetType))
                                {
                                    para = this.TypeObjectDictionary[targetType];
                                }
                                else
                                {
                                    para = this.CreateObject(targetType);
                                    this.TypeObjectDictionary[targetType] = para;
                                }
                            }
                            break;
                        case LifeTimeType.PerThread:
                            //怎么保证用线程校验呢？ 线程槽，把数据存在这里
                            {
                                string key = targetType.FullName;
                                object oValue = CallContext.GetData(key);
                                if (oValue == null)
                                {
                                    para = this.CreateObject(targetType);
                                    CallContext.SetData(key, para);
                                }
                                else
                                {
                                    para = oValue;
                                }
                            }
                            break;
                        default:
                            throw new Exception("wrong LifeTime");
                    }
                }
                #endregion
                //递归：隐形的跳出条件，就是GetParameters结果为空，targetType拥有无参数构造函数
                paraList.Add(para);
            }
            return Activator.CreateInstance(type, paraList.ToArray());
        }
        //属性注入+方法注入？


    }

}
