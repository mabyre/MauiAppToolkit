using Microsoft.Maui.Controls.Xaml;
using MauiAppToolkit.ViewModels;

namespace MauiAppToolkit.Views;

public partial class ConsolePage : ContentPage
{
	public ConsolePage(ConsoleViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
	}
}