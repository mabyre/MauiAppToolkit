//
// https://learn.microsoft.com/fr-fr/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-7.0&tabs=android
//
// The External memory application space : FileSystem.Current.AppDataDirectory
//
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoDevLog;
using System.Windows.Input;

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

        //
        // Find nothing else that works
        //
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {

            externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/";

            // No! Can't this directory does not exist should create it ... pfff
            //externalStorageDirectory = "/storage/emulated/0/Android/data/com.sodevlog.mauiapptoolkit/files/";
            //externalStorageDirectory = "/data/com.sodevlog.mauiapptoolkit/files/";
        }

        SendConsole(String.Format("Running Plateform: {0}", DeviceInfo.Current.Platform.ToString()));

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

        if (false)
        {
            // TODO: _BRY_
            string cacheDir = FileSystem.Current.CacheDirectory;
            // FileSystem.Current.CacheDirectory:
            // C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalCache
            SendConsole(String.Format("FileSystem.Current.CacheDirectory: {0}", cacheDir));
            // FileSystem.Current.AppDataDirectory:
            // C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState
            string mainDir = FileSystem.Current.AppDataDirectory;
            SendConsole(String.Format("FileSystem.Current.AppDataDirectory: {0}", mainDir));

            // User folder choice
            //string initialPath = "DCIM"; 
            //string initialPath = "ANE-LX1";
            //var folderResult = await FolderPicker.PickAsync(initialPath, CancellationToken.None);
            //if (folderResult.IsSuccessful)
            //{
            //    var filesCount = Directory.EnumerateFiles(folderResult.Folder.Path).Count();
            //    SendConsole(String.Format("Folder.Name:{0}", folderResult.Folder.Name));
            //    SendConsole(String.Format("Folder.Path: {0}", folderResult.Folder.Path));
            //    SendConsole(String.Format("filecount: {0}", filesCount));
            //}
            //else
            //{
            //    SendConsole("Folder choice UnSucessful");
            //}

            // Get the file path for the file you want to read/write
            string filePath = FileSystem.AppDataDirectory + "/MyFile.txt";
            SendConsole(false, String.Format("FileSystem.AppDataDirectory: {0}", filePath));

            // Write the file content to the app data directory
            //System.IO.Path.Combine targetFile:
            //C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState\FileName.txt
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "FileName");
            SendConsole(String.Format("System.IO.Path.Combine targetFile: {0}", targetFile));

            //2023-05-11 10:47:29.31 : Application started and ready.
            //2023-05-11 10:47:37.17 : Begin - OpenFile
            //2023-05-11 10:47:37.18 : FileSystem.Current.CacheDirectory: /data/user/0/com.companyname.mauiapptoolkit/cache
            //2023-05-11 10:47:37.18 : FileSystem.Current.AppDataDirectory: /data/user/0/com.companyname.mauiapptoolkit/files
            //FileSystem.AppDataDirectory: /data/user/0/com.companyname.mauiapptoolkit/files/MyFile.txt
            //2023-05-11 10:47:37.18 : System.IO.Path.Combine targetFile: /data/user/0/com.companyname.mauiapptoolkit/files/FileName
            //2023-05-11 10:47:40.00 : End - Try OpenFile : /storage/emulated/0/Android/data/com.companyname.mauiapptoolkit/cache/2203693cc04e0be7f4f024d5f9499e13/906fe8a1b2dc4b50a73e6c5b239b588a/Recruteur.cyp
            //2023-05-11 10:47:40.06 : Read and display file "/storage/emulated/0/Android/data/com.companyname.mauiapptoolkit/cache/2203693cc04e0be7f4f024d5f9499e13/906fe8a1b2dc4b50a73e6c5b239b588a/Recruteur.cyp"

            // Very bad plateforme dependant solution given by ChatGPT
            //using Android.App;
            //var path1 = Application.Context.GetExternalFilesDir(null).AbsolutePath;
            //SendConsole(String.Format("Android.App.Application: {0}", path1));

            //[Obsolete]
            //using System;
            //// ...
            //string path = null;
            //switch (Device.RuntimePlatform)
            //{
            //    case Device.Android:
            //        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //        break;
            //    case Device.iOS:
            //        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //        break;
            //    case Device.tvOS:
            //        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //        break;
            //    case Device.UWP:
            //        path = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            //        break;
            //}

        } // if (false)

        //
        // 1 - Open Dialog Box to read the File 
        //

        CancellationToken cancellationToken = CancellationToken.None;

        FilePickerFileType customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "*/*" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { "*", "*.txt", "*.odt" } }, // file extension
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

        if (fileGettedByDialogBox == false)
        {
            // User want to save a New File writing his name by hand in TextBoxFileName
            string fileName = TextBoxFileName;
            if (containPath(fileName) == false)
            {
                if (externalStorageDirectory == null)
                    TextBoxFileName = Path.Combine(FileSystem.Current.AppDataDirectory, TextBoxFileName);
                else
                    TextBoxFileName = externalStorageDirectory + TextBoxFileName;
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

    bool containPath(string path)
    {
        bool result = false;

        if ( path.Contains('/') || path.Contains('\\') )
            result = true;

        return result;
    }
}
