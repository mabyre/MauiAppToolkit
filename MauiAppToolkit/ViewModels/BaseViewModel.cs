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

    public virtual void OnMessageTextPropertyChanged() { }

    public void SendConsole(string message)
    {
        // Add time to console message
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
        MessageText += time + message + Environment.NewLine;
    }

    public void SendConsole(bool date, string message)
    {
        MessageText += message + Environment.NewLine;

        if (date == true)
        {
            // Add time to console message
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
            String.Concat(time, MessageText);
        }
    }

    public BaseViewModel() 
    {
        _message = Message.Instance;
    }
}
