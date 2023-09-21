using Microsoft.Maui.Controls.Xaml;
using MauiAppToolkit.ViewModels;

namespace MauiAppToolkit.Views;

public partial class ConsolePage : ContentPage
{
    // Keep a backup of the current viewmodel
    ConsoleViewModel _viewModel;

	public ConsolePage(ConsoleViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    //<event> NavigatedTo
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        ConsoleViewModel viewModel = new ConsoleViewModel(_viewModel.MessageText);
        BindingContext = viewModel;
    }
    //</event>
}