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
using Sodevlog.Cryptographie;

namespace Registration.ViewModels;

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

    private static string _clefText;
    public string ClefText
    {
        set { SetProperty(ref _clefText, value); }
        get { return _clefText; }
    }

    
    private static string _clefPlaceholder;
    public string ClefPlaceholder
    {
        set { SetProperty(ref _clefPlaceholder, value); }
        get { return _clefPlaceholder; }
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
        _textBoxFileName = "MonFichier.cyp"; // nom du fichier de l'utilisateur par défaut
        _textBoxFileNamePlaceholder = "Nom du fichier :";
        _clefText = "";
        _clefPlaceholder = "Entrez la clef.";
        _editorFileText = "Contenu du fichier à décrypter...";
        _checkBoxSupprimerBakChecked = true;

        SendConsole("Application prête à fonctionner.");
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
        // 1 - Otherwise it's not worth going any further
        //

        if (ClefText == "")
        {
            afficherTextBoxConsole("Vous devez enter une clef de décryptage.");
            ClefPlaceholder = "Vous devez entrez une Clef, la Clef ne peut pas être vide...";
            return;
        }

        long clef = 0;

        try
        {
            clef = long.Parse(ClefText);
        }
        catch
        {
            afficherTextBoxConsole("La clef n'est pas un long compris entre 1 et 2 147 483 647");
            ClefText = "";
            ClefPlaceholder = "Clef incorrecte...";
            return;
        }

        //
        // 2 - Open Dialog Box to read the File 
        //

        CancellationToken cancellationToken = CancellationToken.None;

        var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/comics", "*/*" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".cyp", ".bak", "*" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
                });

        PickOptions options = new()
        {
            PickerTitle = "Please select a comic file",
            FileTypes = customFileType,
        };

        // 2.1 - Open Dialog Box

        FileResult result = await FilePicker.Default.PickAsync(options);

        if (result != null)
        {
            TextBoxFileName = result.FullPath;
            FileOpenned = true;
        }
        else
        {
            SendConsole("No file selected!");
            TextBoxFileName = "";
            TextBoxFileNamePlaceholder = "Erreur sur le fichier.";
            return;
        }

        afficherTextBoxConsole(String.Format("End - Try OpenFile : {0}", TextBoxFileName));

        //
        // 3 - Try to read the file
        //

        FileStream fs;

        try
        {
            fs = new FileStream(TextBoxFileName, FileMode.Open, FileAccess.Read);
        }
        catch
        {
            afficherTextBoxConsole("Impossible de lire le fichier.");
            TextBoxFileName = "";
            TextBoxFileNamePlaceholder = "Lecture du fichier imposssible.";
            return;
        }

        //
        // 4 - Decrypt the File
        //

        byte[] fichierCrypteByte = new byte[fs.Length];
        fs.Read(fichierCrypteByte, 0, (int)fs.Length);
        fs.Close();

        fichierCrypteByte = Crypte.ByteArray(fichierCrypteByte, clef);
        string text = Crypte.ByteArrayToString(fichierCrypteByte);
        EditorFileText = text;

        // Afficher la scrollbar si le texte dépasse
        // le nombre de lignes du textBox
        //
        if (EditorFileText.Length > 21 * 50) // Comptées à la main dans le textBox ...
        {
            // TODO: textBoxFichier.ScrollBars = ScrollBars.Vertical;
        }

        afficherTextBoxConsole("Lecture et décryptage du fichier \"" + TextBoxFileName + "\"");
//        afficherLabelMessageVert("Lecture et décryptage du fichier.");
    }

    private async void SaveFile()
    {
        //
        // 1 - Otherwise it's not worth going any further
        //

        long clef = 0;

        try
        {
            clef = long.Parse(ClefText);
        }
        catch
        {
            afficherTextBoxConsole("La clef n'est pas un long compris entre 1 et 2 147 483 647");
            ClefText = "";
            ClefPlaceholder = "Clef incorrecte.";
            return;
        }

        if (TextBoxFileName == "")
        {
            afficherTextBoxConsole("Vous devez entrer le nom d'un fichier.");
            ClefText = "";
            ClefPlaceholder = "Entrez un nom de fichier.";
            return;
        }

        if (EditorFileText == "")
        {
            afficherTextBoxConsole("Aucun texte à sauvegarder.");
            ClefText = "";
            ClefPlaceholder = "Entrez du contenu à sauvegarder.";
            return;
        }

        //
        // 2 - Creating a new file
        //

        // Si le fichier a ete ouvert par la dialogue Box Open, il contient le repertoire complet
        if (FileOpenned == true)
        {
            // On sait que le fichier existe puisque l'on vient de l'ouvrir
            // Si un ancien fichier .bak existe on le supprime
            if (File.Exists(TextBoxFileName + ".bak"))
            {
                File.Delete(TextBoxFileName + ".bak");
                afficherTextBoxConsole("Suppression du fichier \"" + TextBoxFileName + "\"");
            }
            File.Move(TextBoxFileName, TextBoxFileName + ".bak");
            afficherTextBoxConsole("Sauvegarde du fichier .bak");
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
                    afficherTextBoxConsole(messageFileAlreadyExist);
                    ClefText = "";
                    ClefPlaceholder = messageFileAlreadyExist;
                    return;
                }
            }

            // Sauvegarde du fichier en .bak
            // Mais si le fichier n'existe pas encore, alors on ne fait pas de .bak
            if (File.Exists(TextBoxFileName))
            {
                if (File.Exists(TextBoxFileName + ".bak"))
                {
                    File.Delete(TextBoxFileName + ".bak");
                    afficherTextBoxConsole("Suppression du fichier \"" + TextBoxFileName + "\"");
                }
                File.Move(TextBoxFileName, TextBoxFileName + ".bak");
                afficherTextBoxConsole("Sauvegarde du fichier .bak");
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
            afficherTextBoxConsole("Impossible de créer le fichier.");
            return;
        }

        afficherTextBoxConsole("Fichier créé correctement.");

        //
        // 3 - Cryptographie en phase gazeuse
        //

        byte[] fichierCrypteByte = new byte[EditorFileText.Length];
        fichierCrypteByte = Crypte.StringToByteArray(EditorFileText);
        fichierCrypteByte = Crypte.ByteArray(fichierCrypteByte, clef);

        fs.Write(fichierCrypteByte, 0, EditorFileText.Length);
        fs.Close();

        afficherTextBoxConsole("Cryptage et sauvegarde du fichier \"" + TextBoxFileName + "\"");

        if ( CheckBoxSupprimerBakChecked == true)
        {
            if (!File.Exists(TextBoxFileName + ".bak"))
            {
                afficherTextBoxConsole("Case est cochée mais le fichier \"" + TextBoxFileName + ".bak" + "\" n'existe pas.");
                return;
            }

            File.Delete(TextBoxFileName + ".bak");
            afficherTextBoxConsole("Fichier : " + TextBoxFileName + ".bak" + " supprimé.");
        }
    }

    // -------------------------------------------

    public void afficherTextBoxConsole(string message)
    {
        SendConsole(message);
    }
}
