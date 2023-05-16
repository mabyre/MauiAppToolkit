using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppToolkit.ViewModels;

public class ConsoleViewModel : BaseViewModel
{
    public ConsoleViewModel() 
    {
        SendConsole("Console ViewModel");
    }
}
