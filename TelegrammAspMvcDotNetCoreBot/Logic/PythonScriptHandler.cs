using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Compiler;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public static class PythonScriptHandler
    {
        public static object PythonRunner (Dictionary<string, object> dictionary, string scriptPath)
        {
            var engine = Python.CreateEngine(); // Extract Python language engine 
            var scope = engine.CreateScope(); // Introduce Python namespace 
            
            scope.SetVariable("params", dictionary); 

            var source = engine.CreateScriptSourceFromFile(scriptPath); // Load the script
            object result = source.Execute(scope);
            return result;
        }
    }    
}
