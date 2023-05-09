namespace MauiAppToolkit;

public partial class MainPage : ContentPage
{
    MainViewModel mainViewModel;

    public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();

        mainViewModel = viewModel; 

        BindingContext = viewModel;
    }
}

