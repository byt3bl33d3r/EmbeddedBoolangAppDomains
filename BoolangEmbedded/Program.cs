using System;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace BoolangEmbedded
{
    public class EmbeddedBoo : MarshalByRefObject
    {
        public void Run(string source)
        {
            Console.WriteLine("\nBoolang Compiler is executing in AppDomain \"{0}\"", AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine("Compiling...");

            BooCompiler compiler = new BooCompiler();
            compiler.Parameters.Input.Add(new StringInput("Boo", source));
            compiler.Parameters.Pipeline = new CompileToMemory();
            compiler.Parameters.Ducky = true;
            //Console.WriteLine(compiler.Parameters.LibPaths);
            //compiler.Parameters.LoadAssembly("System");
            CompilerContext context = compiler.Run();

            //Note that the following code might throw an error if the Boo script had bugs.
            //Poke context.Errors to make sure.
            if (context.GeneratedAssembly != null)
            {
                Console.WriteLine("Executing...\n");
                Type scriptModule = context.GeneratedAssembly.GetType("BooModule");
                MethodInfo mainFunction = scriptModule.GetMethod("Entry");

                string output = (string)mainFunction.Invoke(null, new object[] { new EmbeddedBoo() });
                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine("Error(s) compiling script, this probably means your Boo script has bugs\n");
                foreach (CompilerError error in context.Errors)
                    Console.WriteLine(error);
            }
        }

        public static void RunInAppDomain(string source)
        {
            /*
            AppDomainSetup setupInfo = new AppDomainSetup();
            setupInfo.ApplicationBase = Environment.CurrentDirectory;
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;
            */

            AppDomain ad = AppDomain.CreateDomain("Test");
            EmbeddedBoo remoteEmbBoo = (EmbeddedBoo)ad.CreateInstanceAndUnwrap(
                typeof(EmbeddedBoo).Assembly.FullName,
                "BoolangEmbedded.EmbeddedBoo"
            );
            remoteEmbBoo.Run(source);
        }
    }
}