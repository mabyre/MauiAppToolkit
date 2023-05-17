using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppToolkit.ViewModels;

public class ConsoleViewModel : BaseViewModel
{
    private static string _consoleMessage;

    public string ConsoleMessage 
    {
        get { return _consoleMessage; }
        set { SetProperty(ref _consoleMessage, value); }
    }

    public override void OnMessageTextPropertyChanged()
    {
        ConsoleMessage = base.MessageText;
    }

    public ConsoleViewModel() 
    {
        SendConsole("Console ViewModel");
    }
}
