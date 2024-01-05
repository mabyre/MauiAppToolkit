using Camera.MAUI;
using MauiAppToolkit.ViewModels;
//using Windows.Media.Ocr;

namespace MauiAppToolkit.Views;

public partial class OpticalRecogPage : ContentPage
{
    OpticalRecogViewModel _viewModel;

    public OpticalRecogPage( OpticalRecogViewModel viewModel )
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
        cameraView.CamerasLoaded += CameraView_CamerasLoaded;
    }

    private void CameraView_CamerasLoaded( object sender, EventArgs e )
    {
        if ( cameraView.NumCamerasDetected > 0 )
        {
            if ( cameraView.NumMicrophonesDetected > 0 )
                cameraView.Microphone = cameraView.Microphones.First();

            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread( async () =>
                {
                    if ( await cameraView.StartCameraAsync() == CameraResult.Success )
                    {
                        _viewModel.IsCameraPlaying = true;
                    }
                } 
            );
        }
    }

    private async void ButtonStart_Clicked( object sender, EventArgs e )
    {
        if ( await cameraView.StartCameraAsync() == CameraResult.Success )
        {
            _viewModel.IsCameraPlaying = true;
        }
    }

    private async void ButtonStop_Clicked( object sender, EventArgs e )
    {
        if ( await cameraView.StopCameraAsync() == CameraResult.Success )
        {
            _viewModel.IsCameraPlaying = false;
        }
    }

    private async void ButtonExtract_Clicked( object sender, EventArgs e )
    {
        //OcrEngine ocrEngine = null;
    }
}