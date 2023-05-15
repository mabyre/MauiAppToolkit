//
// https://learn.microsoft.com/fr-fr/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-7.0&tabs=android
// https://learn.microsoft.com/en-us/xamarin/android/platform/files/external-storage?tabs=windows
//
// The External memory application space : FileSystem.Current.AppDataDirectory
//
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoDevLog;
using System.Windows.Input;
using static System.Environment;

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

    public void SendConsole(bool date, string message)
    {
        ConsoleText += message + Environment.NewLine;

        if (date == true)
        {
            // Add time to console message
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " : ";
            String.Concat(time, ConsoleText);
        }
    }

    //--------------------------------------------

    private FileResult resultFilePicker = null;

    private static bool fileGettedByDialogBox = false; // Open by Dialog Box

    // All what I saw in doc does not work!
    private string externalStorageDirectory = null; // mean we are on Windows

#region UI_Binding_properties

    private static string _textBoxFileName;
    public string TextBoxFileName
    {
        set
        {
            if (SetProperty(ref _textBoxFileName, value))
            {
                fileGettedByDialogBox = false;
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

#endregion

    // Command Binding properties

    public ICommand OpenFileCommand { private set; get; }

    public ICommand SaveFileCommand { private set; get; }

    public MainViewModel()
    {
        _textBoxFileName = "FileName.txt"; // nom du fichier de l'utilisateur par défaut
        _textBoxFileNamePlaceholder = "File Name :";
        _textToUser = "";
        _placeholderTextToUser = "Text for user altert";
        _editorFileText = "File's Content that may be saved...";
        _checkBoxSupprimerBakChecked = true;

        SendConsole("Application started and ready.");
        SendConsole(String.Format("Running Plateform: {0}", DeviceInfo.Current.Platform.ToString()));

        SendConsole(false, ""); // separator

        //
        //
        // To know where we are...
        //
        //
        string filePersonalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        SendConsole(false, String.Format("SpecialFolder.Personal: {0}", filePersonalPath));

        string fileApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        SendConsole(false, String.Format("SpecialFolder.ApplicationData: {0}", fileApplicationDataPath));

        if (Directory.Exists(filePersonalPath))
        {
            SendConsole(false, "SpecialFolder.Personal: Ok");
        }
        else
        {
            SendConsole(false, "SpecialFolder.Personal: NO-Ok");
        }

        if (Directory.Exists(fileApplicationDataPath))
        {
            SendConsole(false, "SpecialFolder.ApplicationData: Ok");
        }
        else
        {
            SendConsole(false, "SpecialFolder.ApplicationData: NO-Ok");
        }

        //
        // Find nothing else that works
        //
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/";
            SendConsole(false, String.Format("ExternalStorageDirectory: {0}", externalStorageDirectory));

            // Can't use these! "files" directory does not exist. Should I create it ... pfff
            //externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/files/";
            //externalStorageDirectory = "/data/com.sodevlog.mauiapptoolkit/files/";

            if (Directory.Exists(externalStorageDirectory)) 
            {
                SendConsole(false, "ExternalStorageDirectory: Ok");
            }
            else
            {
                SendConsole(false, "ExternalStorageDirectory: NO-Ok");
            }

            if (Directory.Exists(externalStorageDirectory + "files"))
            {
                SendConsole(false, "ExternalStorageDirectory.files: Ok");
            }
            else
            {
                SendConsole(false, "ExternalStorageDirectory.fles: NO-Ok");
            }
        }

        //if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        //{
        //}

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

        //
        // 1 - Open Dialog Box to read the File 
        //

        FilePickerFileType customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "*/*" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { "" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
                });

        PickOptions options = new()
        {
            PickerTitle = "Please select a file",
            FileTypes = customFileType,
        };

        // 1.1 - Open Dialog Box

        resultFilePicker = await FilePicker.Default.PickAsync(options);

        if (resultFilePicker != null)
        {
            TextBoxFileName = resultFilePicker.FullPath;
            fileGettedByDialogBox = true;
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
        //
        // 1 - Otherwise nothing to be done
        //

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

        // If the file was opened by the Box Open dialog then it contains the complete directory
        if (fileGettedByDialogBox == false)
        {
            // User want to save a New File writing his name by hand in TextBoxFileName
            string fileName = TextBoxFileName;

            // There is no path in the file name
            // we put the path instead of the user
            //
            if (containPath(fileName) == false) 
            {
                if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
                {
                    TextBoxFileName = Path.Combine(FileSystem.Current.AppDataDirectory, TextBoxFileName);
                }
                else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                {
                    TextBoxFileName = externalStorageDirectory + TextBoxFileName;
                }
                else 
                {
                    SendConsole("ERROR: Plateforme non implémentée!");
                };
            }

            // Warning the user overwrites an existing file
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

                if (answer == false)
                {
                    displayToConsole(messageFileAlreadyExist);
                    TextToUser = "";
                    PlaceholderTextToUser = messageFileAlreadyExist;
                    return;
                }
            }

            // Save the File .bak
            // if file not already exist, don't create .bak
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
        else // The File had been opened by Dialog Box Open there is a the path in it
        {
            // The file may come from very far away needed to be saved in : externalStorageDirectory
            if (externalStorageDirectory != null)
            {
                char[] delimiterChars = { '/', '\\' };
                string[] name = TextBoxFileName.Split( delimiterChars );
                TextBoxFileName = externalStorageDirectory + name[ name.Length - 1];
            }

            // On sait que le fichier existe puisque l'on vient de l'ouvrir
            // Si un ancien fichier .bak existe on le supprime
            if (File.Exists(TextBoxFileName + ".bak"))
            {
                displayToConsole(String.Format("Old File Exist: {0}", TextBoxFileName + ".bak"));

                File.Delete(TextBoxFileName + ".bak");
                displayToConsole(String.Format("Old File deleted: {0}", TextBoxFileName + ".bak"));

                File.Move(TextBoxFileName, TextBoxFileName + ".bak");
                displayToConsole("New File .bak saved");
            }
        }

        // 2.1 - Maintenant on peut ouvrir le bon fichier

        FileStream fs;
        try
        {
            fs = new FileStream(TextBoxFileName, FileMode.Create, FileAccess.Write);
        }
        catch (Exception ex)
        {
            displayToConsole(String.Format("Exception: {0}", ex.Message));
            return;
        }

        PlaceholderTextToUser = "File created.";
        displayToConsole("File created.");

        //
        // 3 - Cryptographie en phase gazeuse
        //

        byte[] fileInBytes = new byte[EditorFileText.Length];
        fileInBytes = Strings.StringToByteArray(EditorFileText);

        // 3.1 - Real Creation File
        
        fs.Write(fileInBytes, 0, EditorFileText.Length);
        fs.Close();

        displayToConsole("File in Byte read \"" + TextBoxFileName + "\"");

        if ( CheckBoxSupprimerBakChecked == true )
        {
            if (!File.Exists(TextBoxFileName + ".bak"))
            {
                displayToConsole("Checkbox checked but the file \"" + TextBoxFileName + ".bak" + "\" does not already exist.");
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

    bool containPath(string fileName)
    {
        bool result = false;

        if ( fileName.Contains('/') || fileName.Contains('\\') )
            result = true;

        return result;
    }
}
