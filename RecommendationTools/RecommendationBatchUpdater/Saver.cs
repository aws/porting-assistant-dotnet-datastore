using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RecommendationValidator;
using RulePOCO = Namespaces;

namespace RecommendationBatchUpdater
{
    public class Saver
    {
        public static void SaveBatch(Dictionary<string, RecommendationPOCO> recommendations)
        {
            foreach (var (filePath, recommendation) in recommendations)
            {
                try
                {
                    SaveSingle(filePath, recommendation);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save recommendation to {filePath}: {e}");
                }
            }
        }

        public static void SaveBatch(Dictionary<string, RulePOCO> recommendations)
        {
            foreach (var (filePath, recommendation) in recommendations)
            {
                try
                {
                    SaveSingle(filePath, recommendation, 2);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save recommendation to {filePath}: {e}");
                }
            }
        }

        public static void SaveSingle(string savePath, object recommendation, int indentation = 4)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            var recommendationText = JsonConvert.SerializeObject(recommendation, jsonSerializerSettings);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using var writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.Indented,
                IndentChar = ' ',
                Indentation = indentation
            };
            writer.WriteRaw(recommendationText);

            File.WriteAllText(savePath, sb.ToString());
        }
    }
}