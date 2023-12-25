using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using BussBuzz.Base;
using BussBuzz.MVVM.MAP.ViewModell;
using CommunityToolkit.Maui.Alerts;
using Location = Android.Locations.Location;
using Permissions = Microsoft.Maui.ApplicationModel.Permissions;

namespace BussBuzz;

public class GeolocatorImplementation : MVVM.MAP.Model.IGeolocator
{
    GeolocationContinuousListener locator;
    /*GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));*/
    public async Task<bool> CheckEnableLocationAsync()
    {
        try
        {
            LocationManager locationManager = Android.App.Application.Context.GetSystemService(Context.LocationService) as LocationManager;

            bool isGpsEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            bool isNetworkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

            return isGpsEnabled || isNetworkEnabled;

          /*  if (location != null)
            {
                // Обработка полученного местоположения
                // ...

                return true;
            }*/
        }
        catch (Exception ex)
        {
            
            return false;
        }
    }
    public async Task StartListening(IProgress<Microsoft.Maui.Devices.Sensors.Location> positionChangedProgress, CancellationToken cancellationToken)
    {
        
        var permission = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        if (permission != PermissionStatus.Granted)
        {
            permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
            if (permission != PermissionStatus.Granted)
            {
                await Toast.Make("No permission").Show(CancellationToken.None);
               
                return;
            }
        }
        try
        {
            if (await CheckEnableLocationAsync())
            {
                locator = new GeolocationContinuousListener();
                var taskCompletionSource = new TaskCompletionSource();
                cancellationToken.Register(() =>
                {
                    locator.Dispose();
                    locator = null;
                    taskCompletionSource.TrySetResult();
                });
                locator.OnLocationChangedAction = location =>
                    positionChangedProgress.Report(
                        new Microsoft.Maui.Devices.Sensors.Location(location.Latitude, location.Longitude));
                await taskCompletionSource.Task;
            }
            else
            {
                var result = await App.Current.MainPage.DisplayAlert("Location Permission",
                 "To access your location, you need to enable location services. Would you like to enable it now?",
                 "No", "Yes");

                if (!result)
                {
                    // Если пользователь согласился, открываем окно настроек для включения геолокации
                    var intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    intent.AddCategory(Intent.CategoryDefault);
                    intent.SetFlags(ActivityFlags.NewTask);
                    Platform.CurrentActivity.StartActivityForResult(intent, 0);

                }
                else
                {
                    
                }
            }
        }
        catch
        {

        }

    }
}


internal class GeolocationContinuousListener : Java.Lang.Object, ILocationListener
{
    public Action<Location> OnLocationChangedAction { get; set; }

    readonly LocationManager locationManager;

    public GeolocationContinuousListener()
    {
        locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.LocationService);
        // Requests location updates each second and notify if location changes more then 100 meters
        locationManager?.RequestLocationUpdates(LocationManager.GpsProvider, 5000, 0, this);
    }

    public void OnLocationChanged(Location location)
    {
        OnLocationChangedAction?.Invoke(location);
    }

    public void OnProviderDisabled(string provider)
    {
        try
        {
            VM_Map.Instance.RealTimeLocationTrackerCancelCommand.Execute(this);
        }
        catch { }
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        locationManager?.RemoveUpdates(this);
        locationManager?.Dispose();
    }
}