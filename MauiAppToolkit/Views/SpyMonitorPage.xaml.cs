using Camera.MAUI;

namespace MauiAppToolkit.Views;

public partial class SpyMonitorPage : ContentPage
{
    bool playing = false;

    public SpyMonitorPage()
    {
        InitializeComponent();

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
                        // Do something with UI controlButton.Text = "Stop";
                        playing = true;
                    }
                } 
            );
        }
    }

    private async void ButtonStart_Clicked( object sender, EventArgs e )
    {
        if ( await cameraView.StartCameraAsync() == CameraResult.Success )
        {
            playing = true;
        }
    }

    private async void ButtonStop_Clicked( object sender, EventArgs e )
    {
        if ( await cameraView.StopCameraAsync() == CameraResult.Success )
        {
            playing = false;
        }
    }


}