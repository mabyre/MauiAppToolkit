using CommunityToolkit.Mvvm.ComponentModel;
using MauiAppToolkit.Model;

namespace MauiAppToolkit.ViewModels;

public class BaseViewModel : ObservableObject
{
    private Message _message;

    public string MessageText
    {
        get { return _message.Text; }
        set
        {
            if (_message.Text != value)
            {
                _message.Text = value;
                OnPropertyChanged(nameof(MessageText));
                OnMessageTextPropertyChanged();
            }
        }
    }

    // In case we want to do something in derived ViewModel
    public virtual void OnMessageTextPropertyChanged() { }

    public void SendConsole(string message, bool date = true)
    {
        string time = string.Empty;

        if (date == true)
        {
            // Add time to console message
            time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
        }

        MessageText += time + message + Environment.NewLine;
    }

    public void SendConsoleSeparator()
    {
        MessageText += Environment.NewLine;
    }

    public BaseViewModel() 
    {
        _message = Message.Instance;
    }
}
