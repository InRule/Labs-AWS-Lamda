using System;
using InRule.Authoring.Commanding;
using InRule.Authoring.Windows;
using System.ComponentModel;

namespace InRule.Authoring.AWS
{
    class Extension : ExtensionBase
    {
        private VisualDelegateCommand _updateRulesCommand;
	    private readonly AwsServiceInfo _awsServiceInfo = new AwsServiceInfo();


		// To make system extension that cannot be disabled, change last parm to true
		public Extension()
            : base("AWSRuleManager", "AWS Rule Manager", new Guid("{f14363c3-c320-43d4-8d40-b5b81fc4ec6c}"), false)
        {}

        public override void Enable()
        {   
	        _updateRulesCommand = new VisualDelegateCommand(UpdateRules, "Update Rules", "/Images/Amazon.png", "/Images/Amazon.png", false);

			var group = IrAuthorShell.HomeTab.AddGroup("AWS", null, "");
            group.AddButton(_updateRulesCommand);
            
            RuleApplicationService.Opened += EnableButton;
            RuleApplicationService.Closed += EnableButton;
        }

        private void EnableButton(object sender, EventArgs e)
        {
	        // if we have a rule app, enable the button
	        _updateRulesCommand.IsEnabled = RuleApplicationService.RuleApplicationDef != null;
        }
        
        private void UpdateRules(object obj)
        {
            var sendToAmazon = false;
            var viewModel = new AwsSettingsViewModel(RuleApplicationService.RuleApplicationDef);

            var window = WindowFactory.CreateWindow("Service Settings", new AwsSettingsView(viewModel), false, Strings.OK, Strings.Cancel);
            window.ButtonClicked += delegate (object sender, WindowButtonClickedEventArgs<AwsSettingsView> args)
            {
                if (args.ClickedButtonText == Strings.OK)
                {
                    _awsServiceInfo.TopEntityName = viewModel.EntityName;
                    _awsServiceInfo.AwsFunctionName = viewModel.AwsFunctionName;
                    _awsServiceInfo.ReturnDefinition = viewModel.ReturnDefinition;

                    sendToAmazon = true;
                }

                window.Close();
            };
            window.Show();

            if (sendToAmazon)
            {
                var waitWindow = new BackgroundWorkerWaitWindow("Beaming rules...", "Creating temporary rule application file...", true, false, true);
                waitWindow.DoWork += delegate
                {
	                InRuleJavaScriptDistributionService.GenerateIndexFile(_awsServiceInfo);

	                var javaScript = InRuleJavaScriptDistributionService.GetJavaScriptRules(RuleApplicationService.RuleApplicationDef);

                    AwsConnection.UpdateLambdaFunction(_awsServiceInfo.AwsFunctionName, javaScript);
                };

                waitWindow.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    var error = e.Error;

                    if (error != null)
                    {
                        var text = $"The following error occurred:\n\n{error.Message}";
                        MessageBoxFactory.Show(text, "Error getting JavaScript", MessageBoxFactoryImage.Warning);
                    }
                    else if (e.Result != null)
                    {
                        var lines = ((string)e.Result).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        WindowFactory.ShowLabeledListWindow("Output", "The following information was returned:", lines);
                    }
                };

                waitWindow.ShowDialog();
            }
        }
    }
}
