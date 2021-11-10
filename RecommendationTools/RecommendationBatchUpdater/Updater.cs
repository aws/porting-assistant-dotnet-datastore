using System;
using System.Collections.Generic;
using System.Linq;
using RecommendationValidator;
using RulePOCO = Namespaces;

namespace RecommendationBatchUpdater
{
    public class Updater
    {
        public static void BatchAddFramework(Dictionary<string, RecommendationPOCO> recommendations, string frameworkToAdd)
        {
            foreach (var (filePath, recommendationRoot) in recommendations)
            {
                try
                {
                    foreach (var recommendation in recommendationRoot.Recommendations)
                    {
                        foreach (var recommendedAction in recommendation.RecommendedActions)
                        {
                            if (!recommendedAction.TargetFrameworks.Contains(frameworkToAdd) 
                                && recommendedAction.TargetFrameworks.Contains("net5.0"))
                            {
                                recommendedAction.TargetFrameworks = recommendedAction.TargetFrameworks.Append(frameworkToAdd).ToArray();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save add target framework {frameworkToAdd} to {filePath}: {e}");
                }
            }
        }

        public static void BatchAddFramework(Dictionary<string, RulePOCO> recommendations, string frameworkToAdd)
        {
            foreach (var (filePath, recommendationRoot) in recommendations)
            {
                try
                {
                    foreach (var recommendation in recommendationRoot.Recommendations)
                    {
                        foreach (var recommendedAction in recommendation.RecommendedActions)
                        {
                            // Assuming frameworkToAdd > net5.0,
                            // only add it if net5.0 is already in TargetFrameworks
                            if (recommendedAction.TargetFrameworks.All(f => f.Name != frameworkToAdd) 
                                && recommendedAction.TargetFrameworks.Any(f => f.Name == "net5.0"))
                            {
                                recommendedAction.TargetFrameworks.Add(new TargetFramework
                                {
                                    Name = frameworkToAdd,
                                    TargetCPU = Default.TargetCPU
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save add target framework {frameworkToAdd} to {filePath}: {e}");
                }
            }
        }
    }
}
