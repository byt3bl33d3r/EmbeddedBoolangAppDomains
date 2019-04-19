using System;
using System.IO;
using System.Reflection;

namespace Wrapper
{
    public class Program
    {
        public static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            string appDomainName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("Trying to resolve {0} in AppDomain {1}", args.Name, appDomainName);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (args.Name == assembly.FullName)
                {
                    Console.WriteLine("Found {0} in AppDomain {1}", args.Name, appDomainName);
                    return assembly;
                }
            }
            return null;
        }

        public static void Main(string[] args)
        {
            string source2 = @"
public static def Entry(emb as object) as void:
    print 'In Second embedded engine'
";
            string source = @"
public static def Entry(emb as object) as void:
    print 'In first embedded engine'
    source = """"""{0}""""""
    emb.Run(source)
";

            // string combined = string.Format(source, source2);

            byte[] asmbytes = File.ReadAllBytes(@"Z:\Passthrough\Devel\BoolangEmbedded\Combined\bin\Debug\Combined.exe");
            Assembly asmBoo = Assembly.Load(asmbytes);

            AppDomain ad = AppDomain.CreateDomain("Test");
            ad.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);

            IAssemblyLoader asmLoader = (IAssemblyLoader)ad.CreateInstanceAndUnwrap(
                typeof(AssemblyLoader).Assembly.FullName,
                "AssemblyLoader"
            );

            asmLoader.Load(asmbytes);
            RunBoo(source2, asmBoo, ad);
        }

        static void RunBoo(string source, Assembly asmBoo, AppDomain ad)
        {
            var embBoo = asmBoo.GetType("Program");
            var m = embBoo.GetMethod("RunInAppDomain");

            object[] args = { source, ad };
            m.Invoke(null, args);
        }
    }
}
public interface IAssemblyLoader
{
    void Load(byte[] bytes);
}
public class AssemblyLoader : MarshalByRefObject, IAssemblyLoader
{
    public void Load(byte[] bytes)
    {
        AppDomain.CurrentDomain.Load(bytes);
    }
}