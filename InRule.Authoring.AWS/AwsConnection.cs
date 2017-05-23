using Amazon.Lambda;
using Amazon.Lambda.Model;
using System;
using System.IO;
using System.IO.Compression;

namespace InRule.Authoring.AWS
{
    public static class AwsConnection
    {
		public static string UpdateLambdaFunction(string functionName, string javaScript)
		{
			var ms = new MemoryStream();

			File.WriteAllText(Properties.Settings.Default.RuleFilePath, javaScript);

			using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
			{
				zipArchive.CreateEntryFromFile(Properties.Settings.Default.IndexFilePath, Path.GetFileName(Properties.Settings.Default.IndexFilePath), CompressionLevel.Fastest);
				zipArchive.CreateEntryFromFile(Properties.Settings.Default.RuleFilePath, Path.GetFileName(Properties.Settings.Default.RuleFilePath), CompressionLevel.Fastest);
			}

			string responseText;

			try
			{
				var client = new AmazonLambdaClient(Properties.Settings.Default.AwsAccessKeyId, Properties.Settings.Default.AwsSecretKey, Amazon.RegionEndpoint.USEast1);

				var response = client.UpdateFunctionCode(new UpdateFunctionCodeRequest
				{
					FunctionName = functionName,
					Publish = false,
					ZipFile = ms
				});

				responseText = response.ToString();
			}
			catch (Exception ex)
			{
				responseText = ex.ToString();
			}

			return responseText;
		}
	}
}

