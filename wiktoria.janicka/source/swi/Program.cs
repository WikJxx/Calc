using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace swi
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputPath = "input.json";
            var outputPath = "output.json";
            try{
                var data = Load(inputPath);
                var results = OperationsPerformance(data);
                var sorted = results.OrderBy(v => v.Value).ToList();
                SaveFile(outputPath, sorted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        
        
        static Dictionary<string,dynamic> Load(string path)
        {
            var jsonString = File.ReadAllText(path);
           if (!string.IsNullOrEmpty(jsonString))
        {

            return JsonSerializer.Deserialize<Dictionary<string, dynamic>>(jsonString);           
        }
        else
        {
            throw new FileNotFoundException("There's no file to read", path);
        }
        }
        static List<KeyValuePair<string, double>> OperationsPerformance(Dictionary<string, dynamic> operations)
        {
            var results = new List<KeyValuePair<string, double>>();
            foreach (var op in operations)
            {
                double result = 0;
                string key = op.Key;
                var value = op.Value;

                string operation = value.GetProperty("operator").GetString().ToLower();
                if (!IsValidOperation(operation))
                {
                    throw new ArgumentException($"Invalid operation '{operation}' in input JSON.");
                }

                switch (operation)
                {
                    case "add":
                        try
                        {
                            var addValue1 = value.GetProperty("value1");
                            var addValue2 = value.GetProperty("value2");
                            if (addValue1.ValueKind != JsonValueKind.Number || addValue2.ValueKind != JsonValueKind.Number)
                            {
                                throw new ArgumentException($"Missing or invalid values for operation '{operation}' in input JSON.");
                            }
                            result = addValue1.GetDouble() + addValue2.GetDouble();
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new ArgumentException($"Missing value for operation '{operation}' in input JSON.");
                        }
                        break;
                    case "sub":
                        try
                        {
                            var subValue1 = value.GetProperty("value1");
                            var subValue2 = value.GetProperty("value2");
                            if (subValue1.ValueKind != JsonValueKind.Number || subValue2.ValueKind != JsonValueKind.Number)
                            {
                                throw new ArgumentException($"Missing or invalid values for '{operation}'in input JSON");
                            }
                            result = subValue1.GetDouble() - subValue2.GetDouble();
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new ArgumentNullException($"Missing value for operation '{operation}' in input JSON.");
                        }
                        
                    break;
                    case "mul":
                        try
                        {
                            var mulValue1 = value.GetProperty("value1");
                            var mulValue2 = value.GetProperty("value2");
                            if (mulValue1.ValueKind != JsonValueKind.Number || mulValue2.ValueKind != JsonValueKind.Number)
                            {
                                throw new ArgumentException($"Missing or invalid values for '{operation}'in input JSON");
                            }
                            result = mulValue1.GetDouble() * mulValue2.GetDouble();
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new ArgumentNullException($"Missing value for operation '{operation}' in input JSON.");
                        }
                    break;
                    case "sqrt":
                        try
                            {
                                var sqrtValue1 = value.GetProperty("value1");
                            if (sqrtValue1.ValueKind != JsonValueKind.Number)
                            {
                                throw new ArgumentException($"Missing or invalid value for '{operation}'in input JSON");
                            }
                            result = Math.Sqrt(sqrtValue1.GetDouble());
                        }
                        catch (KeyNotFoundException)
                        {
                            throw new ArgumentNullException($"Missing value for operation '{operation}' in input JSON.");
                        }
                    break;
                    
                }

                results.Add(new KeyValuePair<string, double>(key,result));
            }
            return results;
        }
        static bool IsValidOperation(string operation)
        {
            List<string> validOperations = new List<string> { "add", "sub", "mul", "sqrt" };
            return validOperations.Contains(operation);
        }


        static void SaveFile(string path, List<KeyValuePair<string, double>>results)
        {
            var json = "[" + string.Join(",", results.Select(kv => $"{{\"{kv.Key}\": {kv.Value}}}")) + "]";

            File.WriteAllText(path, json);
        }
    }
}