using InRule.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;

namespace InRule.Authoring.AWS
{
	public static class InRuleJavaScriptDistributionService
    {
		public static string GetJavaScriptRules(RuleApplicationDef ruleAppDef)
        {
			var subscriptionKey = Properties.Settings.Default.InRuleDistKey;

			string returnContent;

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://api.distribution.inrule.com");
                var uri = new Uri("https://api.distribution.inrule.com/package?logOption=None&subscription-key=" + subscriptionKey);

                using (var mpfdc = new MultipartFormDataContent())
                {
                    var byteArray = System.Text.Encoding.UTF8.GetBytes(ruleAppDef.GetXml());
                    var stream = new MemoryStream(byteArray);
                    var content = new StreamContent(stream);
                    mpfdc.Add(content, "ruleApplication", ruleAppDef.Name + ".ruleapp");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("multipart/form-data"));

					var result = client.PostAsync(uri, mpfdc).Result;
                    dynamic resource = result.Content.ReadAsAsync<JObject>().Result;
                    var resultDownload = client.GetAsync((string)resource.PackagedApplicationDownloadUrl + "?subscription-key=" + subscriptionKey).Result;

                    returnContent = resultDownload.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                returnContent = e.Message;
            }

            return returnContent;
        }

        public static void GenerateIndexFile(AwsServiceInfo info)
        {
            // this generates an index page that will set up the node handler and create the 
            // inrule SDK code to invoke the engine when the service is called

            // the template file has placeholders that will be replaced by specific
            // names for this rule application

            // 0: rule app file name (e.g. rectangle.min.js)
            // 1: entity name
            // 2: return value, object name is entity (e.g. entity.Customer)

            var fileText = File.ReadAllText(Properties.Settings.Default.IndexFileTemplatePath)
                .Replace("{0}", "rules.js")
                .Replace("{1}", info.TopEntityName)
                .Replace("{2}", info.ReturnDefinition);

            File.WriteAllText(Properties.Settings.Default.IndexFilePath, fileText);
        }
    }
}
