using CommandLine;
using System;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace RecommendationValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            JObject Recommendation = null;
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!o.FilePath.EndsWith(".recommendation.json"))
                       {
                           Console.WriteLine("the recommendation file name should end with .recommendation.json");
                       }
                       string recommendationText = File.ReadAllText(o.FilePath);
                       Recommendation = JObject.Parse(recommendationText);


                   });
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to read recommendation file" + e);
                Environment.Exit(-1);
            }
            
            if (Recommendation == null)
            {
                Console.WriteLine("failed to read recommendation file");
                Environment.Exit(-1);
            }
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(RecommendationPOJO));

            IList<string> messages;
            bool valid = Recommendation.IsValid(schema, out messages);
            if (!valid)
            {
                foreach (var error in messages)
                {
                    Console.WriteLine(error);
                }

            }
            else
            {
                Console.WriteLine("the file has successfully passed the validation, congratulations!");
            }

        }
    }

    public class Options
    {
        [Option('f', "file-path", Required = true, HelpText = "file path of recommendation.json")]
        public string FilePath { get; set; }
    }
}
