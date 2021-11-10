using System;
using CommandLine;

namespace RecommendationBatchUpdater
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
                        ///////////////////////////////////////////////////////////////////////////
                        // ADD/MODIFY CODE BELOW TO UPDATE RECOMMENDATIONS AND/OR RULES FILES    //
                        // NOTE: We need to verify compatibility of packages added by rules with //
                        //   the target framework being added.                                   //
                        ///////////////////////////////////////////////////////////////////////////
                        
                        var recommendations = Loader.LoadBatchRecommendations(o.Directory);
                        Updater.BatchAddFramework(recommendations, "net6.0");
                        //Saver.SaveBatch(recommendations);

                        var rules = Loader.LoadBatchRules(o.Directory);
                        Updater.BatchAddFramework(rules, "net6.0");
                        //Saver.SaveBatch(rules);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to update files in {o.Directory}: {e}");
                        Environment.Exit(-1);
                    }

                    Console.WriteLine("Batch update complete.");
                });
        }
    }

    public class Options
    {
        [Option('d', "directory", Required = true, HelpText = "Directory containing recommendation json files")]
        public string Directory { get; set; }
    }
}
