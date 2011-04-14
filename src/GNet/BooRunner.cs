using System;
using System.Text;
using System.Reflection;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

using GNet.Lib;

namespace GNet
{
    public class BooRunner
    {
        public void Run(string name, string contents)
        {
            BooCompiler compiler = new BooCompiler();
            compiler.Parameters.Input.Add(new StringInput(name, contents)); //( new FileInput(path));
            compiler.Parameters.Pipeline = new CompileToMemory();
            compiler.Parameters.Ducky = true;
            compiler.Parameters.AddAssembly(typeof(G13Device).Assembly);

            var context = compiler.Run();

            //Note that the following code might throw an error if the Boo script had bugs.
            //Poke context.Errors to make sure.
            if (context.GeneratedAssembly != null)
            {
                Type scriptModule = context.GeneratedAssembly.GetType(name + "Module");

                foreach (var type in context.GeneratedAssembly.GetTypes())
                    Console.WriteLine("Type: " + type.Name);

                foreach (var method in scriptModule.GetMembers())
                    Console.WriteLine("Method: " + method.Name);

                var ctors = scriptModule.GetConstructors();

                //var instance = Activator.CreateInstance(scriptModule);

                //MethodInfo stringManip = scriptModule.GetMethod("stringManip");
                //string output = (string)stringManip.Invoke(null, new object[] { "Tag" });
                //Console.WriteLine(output);
            }
            else
            {
                foreach (CompilerError error in context.Errors)
                    Console.WriteLine(error);
            }
        }
    }
}