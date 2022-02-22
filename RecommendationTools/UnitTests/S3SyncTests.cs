using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PostSyncTests
{
    /// <summary>
    /// These tests are made to be run automatically after a merge and sync to the public S3 bucket. This is needed
    /// in order to validate that the public S3 bucket is up to date with the repository and that the sync is working.
    /// If running these tests in a feature branch with changes to the recommendation files, the tests should fail.
    /// </summary>
    public class S3SyncTests
    {
        private const string S3UrlPrefix = "https://s3.us-west-2.amazonaws.com/aws.portingassistant.dotnet.datastore/recommendationsync/recommendation";
        private const string TempExtractDir = "masterBranchCode";

        [TearDown]
        [SetUp]
        public static void SetupAndTearDown()
        {
            if (Directory.Exists(TempExtractDir))
            {
                Directory.Delete(TempExtractDir, true);
            }
        }

        [Test]
        public async Task ValidateS3RecommendationContentMatchesRepository()
        {
            using var httpClient = new HttpClient();
            // Use always master branch since that is the only branch that should be in sync with S3
            var zipBytes = await httpClient.GetByteArrayAsync("https://github.com/aws/porting-assistant-dotnet-datastore/archive/refs/heads/master.zip");
            await File.WriteAllBytesAsync("github.zip", zipBytes);
            Directory.CreateDirectory(TempExtractDir);
            ZipFile.ExtractToDirectory("github.zip", "masterBranchCode");
            var pathToRepoRecs = $@"{TempExtractDir}\porting-assistant-dotnet-datastore-master\recommendation";

            var allRepoRecs = Directory.EnumerateFiles(pathToRepoRecs).ToList();

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
    }
}