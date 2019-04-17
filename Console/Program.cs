using System;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Security.Policy;
using BoolangEmbedded;

class Program: MarshalByRefObject
{
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
    emb.RunInAppDomain(source)
";

        string combined = string.Format(source, source2);
        Console.WriteLine(combined);
 
        EmbeddedBoo embBoo = new EmbeddedBoo();
        embBoo.Run(source);

        //while (true)
        //    Thread.Sleep(1);
    }
}