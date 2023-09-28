//
// https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-7.0
//
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
//using Windows.Media.Capture;

namespace MauiAppToolkit.ViewModels;

public class OpticalRecogViewModel : BaseViewModel
{
    private CancellationTokenSource _cancelTokenSource;
    private Location location;
    private bool _isCheckingLocation;
    private string format = "F8";

    public ICommand CheckCameraStatusCommand { private set; get; }
    public ICommand CheckMicrophoneStatusCommand { private set; get; }
    public ICommand StartCameraCommand { private set; get; }
    public ICommand StopCameraCommand { private set; get; }


    public OpticalRecogViewModel()
    {
        StartCameraCommand = new RelayCommand( StartCamera );
        StopCameraCommand = new RelayCommand( StopCamera );
    }

    private async void StartCamera()
    {
        // Interact with UI
        SendConsole( "OpticalRecog: StartCamera" );
    }

    private async void StopCamera()
    {
        // Interact with UI
        SendConsole( "OpticalRecog: StopCamera" );
    }
}