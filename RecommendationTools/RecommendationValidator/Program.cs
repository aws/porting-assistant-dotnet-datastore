using System;
using System.IO;
using CommandLine;
using Newtonsoft.Json;

namespace RecommendationValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       try
                       {

                           if (!o.FilePath.EndsWith(".recommendation.json"))
                           {
                               Console.WriteLine("the recommendation file name should end with .recommendation.json");
                               Environment.Exit(-1);
                           }
                           string recommendationText = File.ReadAllText(o.FilePath);
                           JsonConvert.DeserializeObject<RecommendationPOCO>(recommendationText);

                       }
                       catch (Exception e)
                       {
                           Console.WriteLine("failed to parse recommendation file: " + o.FilePath + ", " + e);
                           Environment.Exit(-1);
                       }

                       Console.WriteLine(o.FilePath + " has passed validation.");
                   });
        }
    }

    public class Options
    {
        [Option('p', "path", Required = true, HelpText = "file path of recommendation.json")]
        public string FilePath { get; set; }
    }
}
