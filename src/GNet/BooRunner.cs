using System;
using System.Text;
using System.Reflection;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

using GNet.Lib;

namespace GNet
{
    public class BooRunner : IScriptRunner
    {
        static G13Device device = new G13Device();

        string name;
        string contents;

        public BooRunner(string name, string contents)
        {
            this.name = name;
            this.contents = contents;
        }

        public IScriptRunner Run()
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
                var scriptModule = context.GeneratedAssembly.GetType(name + "Module");
                if (scriptModule == null)
                {
                    Console.WriteLine("Could not find script module " + name + "Module");
                    return null;
                }

                var runMethod = scriptModule.GetMethod("Run", new Type[] { typeof(G13Device) });

                if (runMethod == null)
                {
                    Console.WriteLine("Could not find a static function named Run that takes a G13Device as it's argument");
                    return null;
                }

                var result = runMethod.Invoke(null, new object[] { device });

                if (result != null)
                    Console.WriteLine(result.ToString());
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

            return this;
        }

        public void Stop()
        {
        }
    }
}