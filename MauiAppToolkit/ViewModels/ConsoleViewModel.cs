using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppToolkit.ViewModels;

public class ConsoleViewModel : BaseViewModel
{
    public ConsoleViewModel() 
    {
    }

    public ConsoleViewModel(string msg)
    {
        base.MessageText = msg;
    }
}
