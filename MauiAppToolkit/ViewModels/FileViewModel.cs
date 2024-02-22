//
// https://learn.microsoft.com/fr-fr/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-7.0&tabs=android
// https://learn.microsoft.com/en-us/xamarin/android/platform/files/external-storage?tabs=windows
//
// The External memory application space : FileSystem.Current.AppDataDirectory
//
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoDevLog;
using System.Text;
using System.Windows.Input;

// FilePickerFileType

namespace MauiAppToolkit.ViewModels;

public sealed class FileViewModel : BaseViewModel
{
    private FileResult resultFilePicker = null;

    // All what I saw in doc does not work!
    private string externalStorageDirectory = null; // mean we are on Windows

#region View_Binding_properties

    private static string _textBoxFileName;
    public string TextBoxFileName
    {
        set { SetProperty(ref _textBoxFileName, value); }
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

    public FileViewModel()
    {
        _textBoxFileName = "FileName.txt"; // nom du fichier de l'utilisateur par défaut
        _textBoxFileNamePlaceholder = "File Name :";
        _textToUser = "";
        _placeholderTextToUser = "Text for user altert";
        _editorFileText = "File's Content that may be saved...";
        _checkBoxSupprimerBakChecked = true;

        SendConsole("Application started and ready.");
        SendConsole(string.Format("Running Plateform: {0}", DeviceInfo.Current.Platform.ToString()));

        SendConsoleSeparator();

        //
        //
        // To know where we are...
        //
        //
        string filePersonalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        SendConsole(String.Format("SpecialFolder.Personal: {0}", filePersonalPath), false);

        string fileApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        SendConsole(String.Format("SpecialFolder.ApplicationData: {0}", fileApplicationDataPath), false);

        if (Directory.Exists(filePersonalPath))
        {
            SendConsole("SpecialFolder.Personal: Ok", false);
        }
        else
        {
            SendConsole("SpecialFolder.Personal: NO-Ok", false);
        }

        if (Directory.Exists(fileApplicationDataPath))
        {
            SendConsole("SpecialFolder.ApplicationData: Ok", false);
        }
        else
        {
            SendConsole("SpecialFolder.ApplicationData: NO-Ok", false);
        }

        SendConsoleSeparator();

        //
        // Find nothing else that works
        //
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/";
            SendConsole(String.Format("ExternalStorageDirectory: {0}", externalStorageDirectory), false);

            // Can't use these! "files" directory does not exist. Should I create it ... pfff
            //externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/files/";
            //externalStorageDirectory = "/data/com.sodevlog.mauiapptoolkit/files/";

            if (Directory.Exists(externalStorageDirectory)) 
            {
                SendConsole("ExternalStorageDirectory: Ok", false);
            }
            else
            {
                SendConsole("ExternalStorageDirectory: NO-Ok", false);
            }

            if (Directory.Exists(externalStorageDirectory + "files"))
            {
                SendConsole($"ExternalStorageDirectory.Files: Ok", false);
            }
            else
            {
                SendConsole("ExternalStorageDirectory.Files: NO-Ok", false);
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
            SendConsole(string.Format("FilePicker.FullPath: {0}", TextBoxFileName));
        }
        catch
        {
            TextBoxFileName = "";
            TextBoxFileNamePlaceholder = "Impossible to read the File.";
            SendConsole("Impossible to read the File.");
            return;
        }

        //
        // 3 - Read in Byte the File
        //

        byte[] fileInBytes = new byte[fs.Length];
        fs.Read(fileInBytes, 0, (int)fs.Length);
        fs.Close();

        EditorFileText = Encoding.UTF8.GetString(fileInBytes);

        // Display scroll bar if text is to long
        // le nombre de lignes du textBox
        //
        if (EditorFileText.Length > 21 * 50) // Comptées à la main dans le textBox ...
        {
            // TODO: textBoxFichier.ScrollBars = ScrollBars.Vertical;
        }

        SendConsole("Read and display file \"" + TextBoxFileName + "\"");
        SendConsole("End - OpenFile");
    }

    private async void SaveFile()
    {
        //
        // 1 - Otherwise nothing to be done
        //

        if (TextBoxFileName == "")
        {
            SendConsole("There must be a File name.");
            TextToUser = "";
            PlaceholderTextToUser = "Enter File name.";
            return;
        }

        if (EditorFileText == "")
        {
            SendConsole("No Text to save.");
            TextToUser = "";
            PlaceholderTextToUser = "Enter text to save.";
            return;
        }

        //
        // 2 - Creating a new file
        //

        // User want to save a New File writing his name by hand in TextBoxFileName
        string fileName = TextBoxFileName;

        // There is no path in TextBoxFileName
        // we put the path instead of the user
        //
        if (containPath(fileName) == false) 
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            {
            string filePersonalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            TextBoxFileName = Path.Combine(filePersonalPath, TextBoxFileName);
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                TextBoxFileName = externalStorageDirectory + TextBoxFileName;
            }
            else 
            {
                SendConsole("ERROR: Plateform not implemented!");
            };
        }
        else // containPath(fileName) == true
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                // But on Android The file may come from very far away
                // and needed to be saved in : externalStorageDirectory
                //
                char[] delimiterChars = { '/', '\\' };
                string[] name = TextBoxFileName.Split(delimiterChars);
                TextBoxFileName = externalStorageDirectory + name[name.Length - 1];
            }
        }

        // Warning the user overwrites an existing file
        if (File.Exists(TextBoxFileName))
        {
            // DialogResult
            bool answer = false;
            string messageFileAlreadyExist = "The file \"" + fileName + "\" exist.";
            answer = await Application.Current.MainPage.DisplayAlert(
                    messageFileAlreadyExist,
                    "Would you like to upate ?",
                    "Yes",
                    "No");

            if (answer == false)
            {
                SendConsole(messageFileAlreadyExist);
                SendConsole("File not saved");
                TextToUser = "";
                PlaceholderTextToUser = messageFileAlreadyExist;
                return;
            }
        }

        // 2.0 - Process File .bak

        if (File.Exists(TextBoxFileName + ".bak"))
        {
            File.Delete(TextBoxFileName + ".bak");
            SendConsole("File bak : " + TextBoxFileName + ".bak" + " supressed.");
        }

        if (CheckBoxSupprimerBakChecked == false)
        {
            File.Copy(TextBoxFileName, TextBoxFileName + ".bak");
            SendConsole("File .bak saved");
        }

        // 2.1 - Open the right file

        FileStream fs;
        try
        {
            fs = new FileStream(TextBoxFileName, FileMode.Create, FileAccess.Write);
        }
        catch (Exception ex)
        {
            SendConsole(String.Format("Exception: {0}", ex.Message));
            return;
        }

        PlaceholderTextToUser = "File created.";
        SendConsole("File created.");

        //
        // 3 - Write the file in Bytes
        //

        byte[] fileInBytes = new byte[EditorFileText.Length];
        fileInBytes = Strings.StringToByteArray(EditorFileText);

        // 3.1 - Real Creation File
        
        fs.Write(fileInBytes, 0, EditorFileText.Length);
        fs.Close();

        SendConsole("File in Byte read \"" + TextBoxFileName + "\"");

        if ( CheckBoxSupprimerBakChecked == true )
        {
            if (!File.Exists(TextBoxFileName + ".bak"))
            {
                SendConsole("Checkbox checked but the file \"" + TextBoxFileName + ".bak" + "\" does not already exist.");
                return;
            }

            File.Delete(TextBoxFileName + ".bak");
            SendConsole("File : " + TextBoxFileName + ".bak" + " deleted.");
        }
    }

    // -------------------------------------------

    bool containPath(string fileName)
    {
        bool result = false;

        if ( fileName.Contains('/') || fileName.Contains('\\') )
            result = true;

        return result;
    }
}
