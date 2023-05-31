//
// https://learn.microsoft.com/fr-fr/dotnet/maui/platform-integration/storage/file-picker?view=net-maui-7.0&tabs=android
//
// https://learn.microsoft.com/en-us/xamarin/android/platform/files/external-storage?tabs=windows
//
// The External memory application space : FileSystem.Current.AppDataDirectory
//
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Storage;
using SoDevLog;
using System.Text;
using System.Windows.Input;

namespace MauiAppToolkit.ViewModels;

public sealed partial class MainViewModel : BaseViewModel
{
    private FileResult resultFilePicker = null;

    private static bool fileGettedByDialogBox = false; // Open by Dialog Box

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

    // Due to the use of CommunityToolkit.Maui.Core and [RelayCommand] no more needed to declare ICommand

    public ICommand SaveFileCommand { private set; get; }

    public MainViewModel()
    {
        _textBoxFileName = "FileName.txt"; // nom du fichier de l'utilisateur par défaut
        _textBoxFileNamePlaceholder = "File Name :";
        _textToUser = "";
        _placeholderTextToUser = "Text for user altert";
        _editorFileText = "File's Content that may be saved..." + Environment.NewLine;
        _checkBoxSupprimerBakChecked = true;

        base.SendConsole("Application started and ready.");
        base.SendConsole(string.Format("Running Plateform: {0}", DeviceInfo.Current.Platform.ToString()));

        SendConsoleSeparator();

        //
        //
        // To know where we are...
        //
        //
        string filePersonalPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        SendConsole(String.Format("Environment.SpecialFolder.Personal: {0}", filePersonalPath), false);

        string fileApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        SendConsole(String.Format("Environment.SpecialFolder.ApplicationData: {0}", fileApplicationDataPath), false);

        string folder1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        SendConsole(String.Format("Environment.SpecialFolder.LocalApplicationData): {0}", folder1), false);

        string cacheDir = FileSystem.Current.CacheDirectory;
        SendConsole(String.Format("FileSystem.Current.CacheDirectory: {0}", cacheDir), false);

        string mainDir = FileSystem.Current.AppDataDirectory;
        SendConsole(String.Format("FileSystem.Current.AppDataDirectory: {0}", mainDir), false);


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
        SaveFileCommand = new RelayCommand(SaveFile);
    }

    [RelayCommand]
    public async Task UseFolderPicker() // should be named UseOfFolderPicker in xaml
    {
        WriteToEditorFileText("Begin - UseOfPickerFoler");

        // FileSystem.Current.AppDataDirectory:
        // C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState

        //
        // Choice of Initial Path
        //
        // - On my smartphoe
        //string initialPath = "DCIM"; 
        //string initialPath = "ANE-LX1";
        // - Try on Android emulator
        //sdk_gphone64_x86_64
        //string initialPath = "/data/user/0/com.sodevlog.mauiapptoolkit/";
        string initialPath = externalStorageDirectory;
        // - On windows
        // FileSystem.Current.AppDataDirectory

        WriteToEditorFileText(String.Format("InitialPath:{0}", initialPath));

        var folderResult = await FolderPicker.PickAsync(initialPath, CancellationToken.None);
        if (folderResult.IsSuccessful)
        {
            var filesCount = Directory.EnumerateFiles(folderResult.Folder.Path).Count();
            WriteToEditorFileText(String.Format("Folder.Name:{0}", folderResult.Folder.Name));
            WriteToEditorFileText(String.Format("Folder.Path: {0}", folderResult.Folder.Path));
            WriteToEditorFileText(String.Format("filecount: {0}", filesCount));
        }
        else
        {
            WriteToEditorFileText(String.Format("Folder choice unsucessful: {0}", folderResult.Exception.Message));
        }

        //
        // Is there a file where we an write some TEXT
        //

        // Write the file content to the app data directory
        //System.IO.Path.Combine targetFile:
        //C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState\FileName.txt
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "FileName.txt");
        WriteToEditorFileText(String.Format("System.IO.Path.Combine targetFile: {0}", targetFile));

        // Windows
        //2023-05-12 11:31:14.90 : Application started and ready.
        //2023-05-12 11:31:14.91 : External memory application space: FileSystem.Current.AppDataDirectory
        //2023-05-12 11:31:14.97 : C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState
        //2023-05-12 11:31:14.97 : AppDomain.CurrentDomain.BaseDirectory:
        //2023-05-12 11:31:14.97 : C:\Users\Mabyre\Documents\Visual Studio 2022\Samples\MAUI\MauiAppToolkit\MauiAppToolkit\bin\Debug\net7.0-windows10.0.19041.0\win10-x64\AppX\
        //2023-05-12 11:31:21.82 : Begin - OpenFile
        //2023-05-12 11:31:21.82 : SpecialFolder.Personal): C:\Users\Mabyre\Documents
        //2023-05-12 11:31:21.82 : SpecialFolder.ApplicationData): C:\Users\Mabyre\AppData\Roaming
        //2023-05-12 11:31:21.82 : SpecialFolder.LocalApplicationData): C:\Users\Mabyre\AppData\Local
        //2023-05-12 11:31:26.20 : FileSystem.Current.CacheDirectory: C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalCache
        //2023-05-12 11:31:26.20 : FileSystem.Current.AppDataDirectory: C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState
        //2023-05-12 11:31:26.21 : FileSystem.AppDataDirectory: C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState/MyFile.txt
        //2023-05-12 11:31:26.21 : System.IO.Path.Combine targetFile: C:\Users\Mabyre\AppData\Local\Packages\d761835c-b769-4b75-815a-8516e7766911_9zz4h110yvjzm\LocalState\FileName.txt

        // Android
        //2023-05-12 11:36:15.11 : Application started and ready.
        //2023-05-12 11:36:15.12 : External memory application space: FileSystem.Current.AppDataDirectory
        //2023-05-12 11:36:15.13 : /data/user/0/com.sodevlog.mauiapptoolkit/files
        //2023-05-12 11:36:15.13 : AppDomain.CurrentDomain.BaseDirectory:
        //2023-05-12 11:36:15.13 : /data/user/0/com.sodevlog.mauiapptoolkit/files
        //2023-05-12 11:36:20.07 : Begin - OpenFile
        //2023-05-12 11:36:20.09 : SpecialFolder.Personal): /data/user/0/com.sodevlog.mauiapptoolkit/files
        //2023-05-12 11:36:20.09 : SpecialFolder.ApplicationData): /data/user/0/com.sodevlog.mauiapptoolkit/files/.config
        //2023-05-12 11:36:20.09 : SpecialFolder.LocalApplicationData): /data/user/0/com.sodevlog.mauiapptoolkit/files
        //2023-05-12 11:36:20.10 : FileSystem.Current.CacheDirectory: /data/user/0/com.sodevlog.mauiapptoolkit/cache
        //2023-05-12 11:36:20.10 : FileSystem.Current.AppDataDirectory: /data/user/0/com.sodevlog.mauiapptoolkit/files
        //2023-05-12 11:36:20.10 : FileSystem.AppDataDirectory: /data/user/0/com.sodevlog.mauiapptoolkit/files/MyFile.txt
        //2023-05-12 11:36:20.10 : System.IO.Path.Combine targetFile: /data/user/0/com.sodevlog.mauiapptoolkit/files/FileName.txt

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
    }

    public void WriteToEditorFileText(string message)
    {
        EditorFileText += message + Environment.NewLine;
    }

    [RelayCommand]
    public async Task OpenFile() // should be named OpenFileCommand in xaml
    {
        base.SendConsole("Begin - OpenFile");
        EditorFileText = string.Empty;

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
            base.SendConsole("No file selected!");
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
            base.SendConsole(string.Format("FilePicker.FullPath: {0}", TextBoxFileName));
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

        // There is no path in the file name
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
                base.SendConsole("ERROR: Plateforme non implémentée!");
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

