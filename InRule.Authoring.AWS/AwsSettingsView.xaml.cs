using System.Windows.Controls;

namespace InRule.Authoring.AWS
{
    public partial class AwsSettingsView : UserControl
    {
	    public AwsSettingsView(AwsSettingsViewModel awsSettingsViewModel)
        {
            InitializeComponent();

	        DataContext = awsSettingsViewModel;
        }
    }
}
