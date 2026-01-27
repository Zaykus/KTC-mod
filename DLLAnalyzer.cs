using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

class DLLAnalyzer
{
    static void Main()
    {
        string dllPath = @"f:\vs CODE\KingdomEnhanced - Copy\requseted feature\FasterWorker.dll";
        
        try
        {
            // Load the assembly
            Assembly assembly = Assembly.LoadFrom(dllPath);
            Console.WriteLine($"=== Analyzing {assembly.GetName().Name} ===\n");

            // Get all types in the assembly
            Type[] types = assembly.GetTypes();
            Console.WriteLine($"Found {types.Length} types\n");

            // Keywords to search for
            string[] keywords = { "speed", "work", "boost", "multiply", "time", "fast", "delay", "duration", "rate", "worker" };

            // Analyze each type
            foreach (Type type in types)
            {
                Console.WriteLine($"--- Type: {type.FullName} ---");
                
                // Get all methods
                MethodInfo[] methods = type.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.Static | BindingFlags.Instance);

                bool foundRelevantMethod = false;

                foreach (MethodInfo method in methods)
                {
                    // Skip compiler-generated and common object methods
                    if (method.Name.StartsWith("<") || 
                        method.DeclaringType == typeof(object))
                        continue;

                    string methodName = method.Name.ToLower();
                    
                    // Check if method name contains keywords
                    if (keywords.Any(k => methodName.Contains(k)))
                    {
                        foundRelevantMethod = true;
                        PrintMethod(method);
                    }
                }

                // Also check for properties and fields
                PropertyInfo[] properties = type.GetProperties(
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.Static | BindingFlags.Instance);

                foreach (PropertyInfo prop in properties)
                {
                    string propName = prop.Name.ToLower();
                    if (keywords.Any(k => propName.Contains(k)))
                    {
                        foundRelevantMethod = true;
                        Console.WriteLine($"  [PROPERTY] {prop.PropertyType.Name} {prop.Name}");
                    }
                }

                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.Static | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    string fieldName = field.Name.ToLower();
                    if (keywords.Any(k => fieldName.Contains(k)))
                    {
                        foundRelevantMethod = true;
                        Console.WriteLine($"  [FIELD] {field.FieldType.Name} {field.Name}");
                    }
                }

                if (foundRelevantMethod)
                    Console.WriteLine();
            }

            // Also list all public methods that might be entry points
            Console.WriteLine("\n=== ALL PUBLIC METHODS (potential entry points) ===\n");
            foreach (Type type in types)
            {
                MethodInfo[] publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                
                if (publicMethods.Length > 0)
                {
                    Console.WriteLine($"Type: {type.FullName}");
                    foreach (MethodInfo method in publicMethods)
                    {
                        if (!method.Name.StartsWith("<") && method.DeclaringType != typeof(object))
                        {
                            PrintMethod(method);
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static void PrintMethod(MethodInfo method)
    {
        string returnType = method.ReturnType.Name;
        string parameters = string.Join(", ", 
            method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
        
        string access = "public";
        if (method.IsPrivate) access = "private";
        else if (method.IsProtected) access = "protected";
        if (method.IsStatic) access += " static";
        
        Console.WriteLine($"  [{access}] {returnType} {method.Name}({parameters})");
    }
}
