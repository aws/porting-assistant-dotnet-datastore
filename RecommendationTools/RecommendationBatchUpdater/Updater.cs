using System;
using System.Collections.Generic;
using System.Linq;
using RecommendationValidator;
using RulePOCO = Namespaces;

namespace RecommendationBatchUpdater
{
    public class Updater
    {
        public static void BatchAddFramework(Dictionary<string, RecommendationPOCO> recommendations, string frameworkToAdd, string mostRecentFrameworkAdded)
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
                                && recommendedAction.TargetFrameworks.Contains(mostRecentFrameworkAdded))
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

        public static void BatchAddFramework(Dictionary<string, RulePOCO> recommendations, string frameworkToAdd, string mostRecentFrameworkAdded)
        {
            foreach (var (filePath, recommendationRoot) in recommendations)
            {
                try
                {
                    foreach (var recommendation in recommendationRoot.Recommendations)
                    {
                        foreach (var recommendedAction in recommendation.RecommendedActions)
                        {
                            // Assuming frameworkToAdd > net6.0,
                            // only add it if net6.0 is already in TargetFrameworks
                            if (recommendedAction.TargetFrameworks.All(f => f.Name != frameworkToAdd) 
                                && recommendedAction.TargetFrameworks.Any(f => f.Name == mostRecentFrameworkAdded))
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
