//
// https://learn.microsoft.com/fr-fr/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-7.0&tabs=android
//
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows.Input;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using Microsoft.Maui.Storage;
using SoDevLog;

namespace MauiAppToolkit.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    //--------------------------------------------
    // Console ViewModel
    //--------------------------------------------

    private static string _consoleText;
    public string ConsoleText
    {
        set { SetProperty(ref _consoleText, value); }
        get { return _consoleText; }
    }

    public void SendConsole(string message)
    {
        // Add time to console message
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
        ConsoleText += time + message + Environment.NewLine;
    }

    //--------------------------------------------

    private static bool FileOpenned = false; // by Dialog Box

    // UI Binding properties

    private static string _textBoxFileName;
    public string TextBoxFileName
    {
        set 
        { 
            if (SetProperty(ref _textBoxFileName, value) )
            {
                FileOpenned = false;
            }
        }
        get { return _textBoxFileName; }
    }

    
    private static string _textBoxFileNamePlaceholder;
    public string TextBoxFileNamePlaceholder
    {
        set { SetProperty(ref _textBoxFileNamePlaceholder, value); }
        get { return _textBoxFileNamePlaceholder; }
    }

    private static string _textToUser;
    public string TextToUser
    {
        set { SetProperty(ref _textToUser, value); }
        get { return _textToUser; }
    }

    
    private static string _placeholderTextToUser;
    public string PlaceholderTextToUser
    {
        set { SetProperty(ref _placeholderTextToUser, value); }
        get { return _placeholderTextToUser; }
    }

    private static string _editorFileText;
    public string EditorFileText
    {
        set { SetProperty(ref _editorFileText, value); }
        get { return _editorFileText; }
    }

    private static bool _checkBoxSupprimerBakChecked;
    public bool CheckBoxSupprimerBakChecked
    {
        set { SetProperty(ref _checkBoxSupprimerBakChecked, value); }
        get { return _checkBoxSupprimerBakChecked; }
    }

    // Command Binding properties

    public ICommand OpenFileCommand { private set; get; }

    public ICommand SaveFileCommand { private set; get; }

    public MainViewModel()
    {
        _textBoxFileName = "FileName.txt"; // nom du fichier de l'utilisateur par défaut
        _textBoxFileNamePlaceholder = "File Name :";
        _textToUser = "";
        _placeholderTextToUser = "Text for user alter";
        _editorFileText = "File's Content that may be saved...";
        _checkBoxSupprimerBakChecked = true;

        SendConsole("Application started and ready.");
        SetupCommands();
    }

    private void SetupCommands()
    {
        OpenFileCommand = new RelayCommand(OpenFile);
        SaveFileCommand = new RelayCommand(SaveFile);
    }

    public async void OpenFile()
    {
        SendConsole("Begin - OpenFile");

        // _BRY_
        string cacheDir = FileSystem.Current.CacheDirectory;
        SendConsole(String.Format("FileSystem.Current.CacheDirectory: {0}", cacheDir));
        string mainDir = FileSystem.Current.AppDataDirectory;
        SendConsole(String.Format("FileSystem.Current.AppDataDirectory: {0}", mainDir));

        //
        // 1 - Open Dialog Box to read the File 
        //

        CancellationToken cancellationToken = CancellationToken.None;

        var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/comics", "*/*" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".txt", ".bak", "*" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
                });

        PickOptions options = new()
        {
            PickerTitle = "Please select a comic file",
            FileTypes = customFileType,
        };

        // 1.1 - Open Dialog Box

        FileResult result = await FilePicker.Default.PickAsync(options);

        if (result != null)
        {
            TextBoxFileName = result.FullPath;
            FileOpenned = true;
            SendConsole(String.Format("End - Try OpenFile : {0}", TextBoxFileName));
        }
        else
        {
            SendConsole("No file selected!");
            TextBoxFileName = "";
            TextBoxFileNamePlaceholder = "Error on picking File.";
            return;
        }

        //
        // 2 - Try to read the file
        //

        FileStream fs;

        try
        {
            fs = new FileStream(TextBoxFileName, FileMode.Open, FileAccess.Read);
        }
        catch
        {
            displayToConsole("Impossible to read the File.");
            TextBoxFileName = "";
            TextBoxFileNamePlaceholder = "Impossible to read the File.";
            return;
        }

        //
        // 3 - Read in Byte the File
        //

        byte[] fileInBytes = new byte[fs.Length];
        fs.Read(fileInBytes, 0, (int)fs.Length);
        fs.Close();

        EditorFileText = Strings.ByteArrayToString(fileInBytes);

        // Display scroll bar if text is to long
        // le nombre de lignes du textBox
        //
        if (EditorFileText.Length > 21 * 50) // Comptées à la main dans le textBox ...
        {
            // TODO: textBoxFichier.ScrollBars = ScrollBars.Vertical;
        }

        displayToConsole("Read and display file \"" + TextBoxFileName + "\"");
    }

    private async void SaveFile()
    {
        if (TextBoxFileName == "")
        {
            displayToConsole("There must be a File name.");
            TextToUser = "";
            PlaceholderTextToUser = "Enter File name.";
            return;
        }

        if (EditorFileText == "")
        {
            displayToConsole("No Text to save.");
            TextToUser = "";
            PlaceholderTextToUser = "Enter text to save.";
            return;
        }

        //
        // 2 - Creating a new file
        //

        // If the File had been opened by Dialog Box Open there is the path in it
        if (FileOpenned == true)
        {
            // On sait que le fichier existe puisque l'on vient de l'ouvrir
            // Si un ancien fichier .bak existe on le supprime
            if (File.Exists(TextBoxFileName + ".bak"))
            {
                File.Delete(TextBoxFileName + ".bak");
                displayToConsole("Suppression du fichier \"" + TextBoxFileName + "\"");
            }
            File.Move(TextBoxFileName, TextBoxFileName + ".bak");
            displayToConsole("Sauvegarde du fichier .bak");
        }
        else
        {
            string fileName = TextBoxFileName;

            // Le nom du fichier est seul sans repertoire entré par l'utilisateur
            if (Strings.StringSearchWord(TextBoxFileName, "\\") == false)
            {
                // On ajoute le repertoire de base de l'application
                TextBoxFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + TextBoxFileName;
            }

            // Attention l'utilisateur écrase un fichier existant
            if (File.Exists(TextBoxFileName))
            {
                // DialogResult
                bool answer = false;
                string messageFileAlreadyExist = "Attention: le fichier \"" + fileName + "\" existe déjà.";
                answer = await Application.Current.MainPage.DisplayAlert(
                        messageFileAlreadyExist,
                        "Voulez vous l'écraser ?",
                        "Oui",
                        "Non");

                if ( answer == false )
                {
                    displayToConsole(messageFileAlreadyExist);
                    TextToUser = "";
                    PlaceholderTextToUser = messageFileAlreadyExist;
                    return;
                }
            }

            // Save the File .bak
            // but if file not already exist, don't create .bak
            if (File.Exists(TextBoxFileName))
            {
                if (File.Exists(TextBoxFileName + ".bak"))
                {
                    File.Delete(TextBoxFileName + ".bak");
                    displayToConsole("File deleted  \"" + TextBoxFileName + "\"");
                }
                File.Move(TextBoxFileName, TextBoxFileName + ".bak");
                displayToConsole("Save file .bak");
            }
        }

        // 2.1 - Maintenant on peut sauver le bon fichier

        FileStream fs;
        try
        {
            fs = new FileStream(TextBoxFileName, FileMode.Create, FileAccess.Write);
        }
        catch
        {
            displayToConsole("Impossible to create the File.");
            return;
        }

        displayToConsole("File created.");

        //
        // 3 - Cryptographie en phase gazeuse
        //

        byte[] fileInBytes = new byte[EditorFileText.Length];
        fileInBytes = Strings.StringToByteArray(EditorFileText);

        fs.Write(fileInBytes, 0, EditorFileText.Length);
        fs.Close();

        displayToConsole("File in Byte read \"" + TextBoxFileName + "\"");

        if ( CheckBoxSupprimerBakChecked == true)
        {
            if (!File.Exists(TextBoxFileName + ".bak"))
            {
                displayToConsole("Checkbox checked but the file \"" + TextBoxFileName + ".bak" + "\" does not exist.");
                return;
            }

            File.Delete(TextBoxFileName + ".bak");
            displayToConsole("File : " + TextBoxFileName + ".bak" + " deleted.");
        }
    }

    // -------------------------------------------

    public void displayToConsole(string message)
    {
        SendConsole(message);
    }
}
