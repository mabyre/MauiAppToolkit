using CommunityToolkit.Mvvm.ComponentModel;
using MauiAppToolkit.Model;

namespace MauiAppToolkit.ViewModels;

public class BaseViewModel : ObservableObject
{
    private ConsoleMessage _consoleMessage;

    public string ConsoleMessageText
    {
        get { return _consoleMessage.ConsoleMessageText; }
        set
        {
            if (_consoleMessage.ConsoleMessageText != value)
            {
                _consoleMessage.ConsoleMessageText = value;
                OnPropertyChanged("ConsoleMessageText");
            }
        }
    }

    public void SendConsole(string message)
    {
        // Add time to console message
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
        ConsoleMessageText += time + message + Environment.NewLine;
    }

    public void SendConsole(bool date, string message)
    {
        ConsoleMessageText += message + Environment.NewLine;

        if (date == true)
        {
            // Add time to console message
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
            String.Concat(time, ConsoleMessageText);
        }
    }

    public BaseViewModel() 
    {
        _consoleMessage = ConsoleMessage.Instance;
    }
}
