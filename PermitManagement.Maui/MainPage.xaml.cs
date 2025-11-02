using PermitManagement.Presentation;
using System.Linq;

namespace PermitManagement.Maui;

public partial class MainPage : ContentPage
{
	private static readonly string[] DurationOptions = new[] { "1 Day", "1 Week", "1 Month", "1 Year" };

	public MainPage(PermitViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		var zones = PermitViewModel.AvailableZones.ToList();
		ZonePicker.ItemsSource = zones;
		if (zones.Count > 0)
		{
			if (viewModel.SelectedZone is null || !zones.Contains(viewModel.SelectedZone))
			{
				viewModel.SelectedZone = zones[0];
			}

			ZonePicker.SelectedItem = viewModel.SelectedZone;
			if (ZonePicker.SelectedItem is null)
			{
				ZonePicker.SelectedIndex = 0;
			}
		}

		DurationPicker.ItemsSource = DurationOptions;
		if (DurationOptions.Length > 0)
		{
			if (viewModel.SelectedDuration is null || !DurationOptions.Contains(viewModel.SelectedDuration))
			{
				viewModel.SelectedDuration = DurationOptions[0];
			}

			DurationPicker.SelectedItem = viewModel.SelectedDuration;
			if (DurationPicker.SelectedItem is null)
			{
				DurationPicker.SelectedIndex = 0;
			}
		}
	}
}
