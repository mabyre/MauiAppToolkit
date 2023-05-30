// Accéder à l'instance unique de SingletonString
// SingletonString singleton = SingletonString.Instance;
//
// Accéder ou modifier la chaîne de caractères stockée
// string myString = singleton.MyString;
// singleton.MyString = "Bonjour le monde!";

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppToolkit.Model;

public class Message : ObservableObject
{
    private static Message _instance;
    private string _myProperty;

    private Message()
    {
        // Private constructor to prevent external instantiation
    }

    public static Message Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Message();
            }
            return _instance;
        }
    }

    public string Text
    {
        get => _myProperty;
        set => SetProperty(ref _myProperty, value);
    }
}

