using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public static class PythonScriptHandler
    {
        public static object PythonRunner(string scriptPath, string[] outVariables)
        {
            var engine = Python.CreateEngine(); // Extract Python language engine 
            var scope = engine.CreateScope(); // Introduce Python namespace 
            
            var source = engine.CreateScriptSourceFromFile(scriptPath); // Load the script
            object result = "";
            try
            {
                source.Execute(scope);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            foreach (var param in outVariables)
            {
                result += $" {param} = {scope.GetVariable<int>(param).ToString()} ";
            }
            return result;
        }
        public static object PythonRunner (Dictionary<string, object> dictionary, string scriptPath, string[] outVariables)
        {
            var engine = Python.CreateEngine(); // Extract Python language engine 
            var scope = engine.CreateScope(); // Introduce Python namespace 
            
            scope.SetVariable("params", dictionary); 

            var source = engine.CreateScriptSourceFromFile(scriptPath); // Load the script
            object result;
            try
            {
                 result = source.Execute(scope);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            foreach (var param in outVariables)
            {
                result +=$" {param} = {scope.GetVariable<int>(param).ToString()} ";
            }
            return result;
        }
    }    
}
