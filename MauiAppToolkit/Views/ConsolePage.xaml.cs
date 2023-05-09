using Microsoft.Maui.Controls.Xaml;
using MauiAppToolkit.ViewModels;

namespace MauiAppToolkit.Views;

public partial class ConsolePage : ContentPage
{
	public ConsolePage(MainViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
}