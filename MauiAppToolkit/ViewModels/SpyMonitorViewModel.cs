//
// https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-7.0
//
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiAppToolkit.ViewModels;

public class GeoLocViewModel : BaseViewModel
{
    private CancellationTokenSource _cancelTokenSource;
    private Location location;
    private bool _isCheckingLocation;
    private string format = "F8";

    #region View_Binding_properties

    private static string _latitude;
    public string LabelLatitude
    {
        set { SetProperty(ref _latitude, value); }
        get { return _latitude; }
    }

    private static string _longitude;

    public string LabelLongitude
    {
        set { SetProperty(ref _longitude, value); }
        get { return _longitude; }
    }

    #endregion

    public ICommand RefreshLocationCommand { private set; get; }

    public GeoLocViewModel()
    {
        _latitude = "Latitude";
        _longitude = "Longitude";

        RefreshLocationCommand = new RelayCommand(RefreshLocation);
    }

    public async void RefreshLocation()
    {
        await GetCurrentLocation();

        if (location != null)
        {
            LabelLongitude = location.Latitude.ToString(format);
            LabelLatitude = location.Longitude.ToString(format);

            SendConsole("New Location");
        }
    }

    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            //if (location != null)
            //    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (FeatureNotSupportedException)
        {
            // La fonctionnalité de géolocalisation n'est pas prise en charge sur le périphérique
            Console.WriteLine($"Feature Not Supported");
            SendConsole("Feature Not Supported");
        }
        catch (PermissionException)
        {
            // L'autorisation d'accès à la géolocalisation n'a pas été accordée
            Console.WriteLine($"Permission exception");
            SendConsole("Permission exception");
        }
        catch (Exception ex)
        {
            // Une erreur s'est produite lors de la récupération de la position GPS
            Console.WriteLine($"Erreur de géolocalisation : {ex.Message}");
            SendConsole(String.Format("Exception: {0}", ex.Message));
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }

}
