using Microsoft.Maui.Controls.Xaml;
using Registration.ViewModels;

namespace Registration.Views;

public partial class ConsolePage : ContentPage
{
	public ConsolePage(MainViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
}