using InRule.Repository;
using InRule.Repository.RuleElements;
using System;
using System.IO;
using System.Xml.Serialization;

namespace InRule.Authoring.AWS
{
	public class AwsServiceInfo
	{
		public string ReturnDefinition { get; set; }
		public string TopEntityName { get; set; }
		public string AwsFunctionName { get; set; }
	}
}