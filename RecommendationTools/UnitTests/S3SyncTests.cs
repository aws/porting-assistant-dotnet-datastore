using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace PostSyncTests
{
    /// <summary>
    /// These tests are made to be run from the main branch after a merge and sync to the public S3 bucket. This is needed
    /// in order to validate that the public S3 bucket is up to date with the repository and that the sync is working.
    /// If running these tests in a feature branch with changes to the recommendation files, the tests should fail.
    /// </summary>
    public class S3SyncTests
    {
        private const string S3UrlPrefix = "https://s3.us-west-2.amazonaws.com/aws.portingassistant.dotnet.datastore/recommendationsync/recommendation";

        [Test]
        public void ValidateS3RecommendationContentMatchesRepository()
        {
            var pathToRepoRecs = FindDirectoryUpwards("recommendation");
            var allRepoRecs = Directory.EnumerateFiles(pathToRepoRecs).ToList();

            using var httpClient = new HttpClient();
            Assert.Multiple(async () =>
            {
                foreach(var file in allRepoRecs)
                {
                    var s3ObjectPath = $"{S3UrlPrefix}/{file.Split('\\').Last()}";
                    var s3Content = await httpClient.GetStringAsync(s3ObjectPath);
                    var localContent = await File.ReadAllTextAsync(file);
                    localContent = localContent.Replace("\r\n", "\n", StringComparison.Ordinal);
                    s3Content = s3Content.Replace("\r\n", "\n", StringComparison.Ordinal);
                    Assert.AreEqual(localContent, s3Content, $"Content didn't match for file {file}");
                }
            });
        }

        private static string FindDirectoryUpwards(string searchPattern)
        {
            var fullPath = string.Empty;
            var currentDir = Directory.GetCurrentDirectory();
            while (string.IsNullOrEmpty(fullPath) && currentDir != null)
            {
                var matchingDir = Directory.GetDirectories(currentDir, "recommendation").FirstOrDefault();

                if (matchingDir != null)
                {
                    return matchingDir;
                }
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            throw new InvalidOperationException($"Couldn't find directory with name {searchPattern} when looking upward from current directory");
        }
    }
}