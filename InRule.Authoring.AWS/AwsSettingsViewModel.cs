using System.Collections.Generic;
using InRule.Authoring.ComponentModel;
using InRule.Repository;

namespace InRule.Authoring.AWS
{
	public class AwsSettingsViewModel : ObservableObject
	{
		public IEnumerable<EntityDef> Entities { get; }
		public string AwsFunctionName { get; set; }
		public string ReturnDefinition { get; set; }

		public string EntityName => _selectedEntity.Name;

		private EntityDef _selectedEntity;
		
		public AwsSettingsViewModel(RuleApplicationDef ruleAppDef)
		{
			Entities = ruleAppDef.Entities;
			SelectedEntity = ruleAppDef.Entities[0];
		}

		public EntityDef SelectedEntity
		{
			get { return _selectedEntity; }
			set
			{
				if (value != _selectedEntity)
				{
					_selectedEntity = value;

					OnPropertyChanged(nameof(SelectedEntity));
				}
			}
		}
	}
}
