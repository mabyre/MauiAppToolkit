//
// https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-7.0
//
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
//using Windows.Media.Capture;

namespace MauiAppToolkit.ViewModels;

public class SpyMonitorViewModel : BaseViewModel
{
    private CancellationTokenSource _cancelTokenSource;
    private Location location;
    private bool _isCheckingLocation;
    private string format = "F8";

    #region View_Binding_properties

    private bool _isCameraActive;
    public bool IsCameraActive
    {
        get { return _isCameraActive; }
        set { SetProperty( ref _isCameraActive, value ); }
    }


    private bool _isMicrophoneActive;
    public bool IsMicrophoneActive
    {
        get { return _isMicrophoneActive; }
        set { SetProperty( ref _isMicrophoneActive, value ); }
    }

    #endregion

    public ICommand CheckCameraStatusCommand { private set; get; }
    public ICommand SetCameraStatusCommand { private set; get; }
    public ICommand CheckMicrophoneStatusCommand { private set; get; }

    public SpyMonitorViewModel()
    {

        CheckCameraStatusCommand = new RelayCommand( CheckCameraStatus );
        SetCameraStatusCommand = new RelayCommand( SetCameraStatus );
        CheckMicrophoneStatusCommand = new RelayCommand( CheckMicrophoneStatus );
    }

    public async Task<bool> IsCameraActiveGranted()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        return status == PermissionStatus.Granted;
    }

    private async void CheckCameraStatus()
    {
        SendConsole( "SpyMonitor: CheckCameraStatus" );

        IsCameraActive = false;

        await Task.Delay( 1000 );

        IsCameraActive = await IsCameraActiveGranted();
    }

    private async void SetCameraStatus()
    {
        // Vérification des autorisations
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if ( status != PermissionStatus.Granted )
        {
            // Gérer l'absence d'autorisation de la caméra
            return;
        }

        // Désactiver la caméra
        //await MediaCapture.StopPreviewAsync();
    }

    public async Task<bool> IsMicrophoneActiveGranted()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        return status == PermissionStatus.Granted;
    }

    private async void CheckMicrophoneStatus()
    {
        SendConsole( "SpyMonitor: CheckMicrophoneStatus" );

        IsMicrophoneActive = false;

        await Task.Delay( 1000 );

        IsMicrophoneActive = await IsMicrophoneActiveGranted();
    }
}