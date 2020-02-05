using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QCrawler.Sample
{
    /// <summary>
    /// 控制台程序，示例工厂
    /// </summary>
    public class DemoFactory
    {
        public static void Start(Demo demo)
        {
            if (demo == Demo.All)
                runAll();
            else
                run(demo.ToString());

            Console.WriteLine();
            Console.WriteLine("全部Demo执行完毕");
            Console.ReadKey();
        }

        const string frontNamespace = "QCrawler.Sample";
        private static void run(string demoStr)
        {
            try
            {
                var fullName = frontNamespace + "." + demoStr;
                Assembly assembly = Assembly.GetExecutingAssembly();
                var type = assembly.GetType(fullName);
                BaseDemo demo = (BaseDemo)Activator.CreateInstance(type);
                demo.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("出现异常：" + ex.Message);
            }
        }
        private static void runAll()
        {
            try
            {
                Assembly assembly = Assembly.Load("QCrawler");
                IEnumerable<TypeInfo> classInfos = assembly.DefinedTypes;
                Type interfaceType = classInfos.Where(t => t.FullName == frontNamespace + ".BaseDemo").FirstOrDefault();
                IEnumerable<TypeInfo> demoInfos = classInfos.Where(t => t.Name.StartsWith("demo", StringComparison.CurrentCultureIgnoreCase) && t.ImplementedInterfaces.FirstOrDefault() == interfaceType);
                TypeInfo[] infos = demoInfos.ToArray();
                foreach (var item in demoInfos)
                {
                    BaseDemo demo = (BaseDemo)Activator.CreateInstance(item);
                    demo.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("出现异常：" + ex.Message);
            }
        }
    }
}
