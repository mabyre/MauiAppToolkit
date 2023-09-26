using MauiAppToolkit.ViewModels;

namespace MauiAppToolkit.Views;

public partial class FilePage : ContentPage
{
    FileViewModel fileViewModel;

    public FilePage(FileViewModel viewModel)
	{
		InitializeComponent();

        fileViewModel = viewModel; 

        BindingContext = viewModel;
    }
}

