using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RecommendationValidator;
using RulePOCO = Namespaces;

namespace RecommendationBatchUpdater
{
    public class Loader
    {
        public static Dictionary<string, RecommendationPOCO> LoadBatchRecommendations(string directory)
        {
            var recommendations = new Dictionary<string, RecommendationPOCO>();
            foreach (var file in Directory.EnumerateFiles(directory, "*.recommendation.json", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var recommendation = LoadSingleRecommendation(file);
                    recommendations[file] = recommendation;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load recommendation file {file}: {e}");
                }
            }
            return recommendations;
        }

        public static Dictionary<string, RulePOCO> LoadBatchRules(string directory)
        {
            var recommendations = new Dictionary<string, RulePOCO>();
            var rulesFiles = Directory
                .EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly)
                .Where(path => !path.EndsWith("recommendation.json"));
            foreach (var file in rulesFiles)
            {
                try
                {
                    var recommendation = LoadSingleRule(file);
                    recommendations[file] = recommendation;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load rule file {file}: {e}");
                }
            }
            return recommendations;
        }

        private static RecommendationPOCO LoadSingleRecommendation(string recommendationPath)
        {
            var recommendationText = File.ReadAllText(recommendationPath);
            return JsonConvert.DeserializeObject<RecommendationPOCO>(recommendationText);
        }

        private static RulePOCO LoadSingleRule(string rulePath)
        {
            var recommendationText = File.ReadAllText(rulePath);
            return JsonConvert.DeserializeObject<RulePOCO>(recommendationText);
        }
    }
}