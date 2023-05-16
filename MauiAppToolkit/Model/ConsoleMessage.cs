// Accéder à l'instance unique de SingletonString
// SingletonString singleton = SingletonString.Instance;
//
// Accéder ou modifier la chaîne de caractères stockée
// string myString = singleton.MyString;
// singleton.MyString = "Bonjour le monde!";

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppToolkit.Model;

public class ConsoleMessage : ObservableObject
{
    private static ConsoleMessage _instance;
    private string _myProperty;

    private ConsoleMessage()
    {
        // Private constructor to prevent external instantiation
    }

    public static ConsoleMessage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConsoleMessage();
            }
            return _instance;
        }
    }

    public string ConsoleMessageText
    {
        get => _myProperty;
        set => SetProperty(ref _myProperty, value);
    }
}
