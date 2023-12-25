namespace BussBuzz.MVVM.MAP.ViewModell;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using AutoFixture;
using BussBuzz.Base;
using BussBuzz.MVVM.MAP.Model;
using BussBuzz.MVVM.MAP.View;
using BussBuzz.MVVM.Menu.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Material;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

public partial class VM_Map : ObservableObject
{
    // SINGLETON
    private static VM_Map _instance;
    public static VM_Map Instance
    {
        get
        {
            if (_instance == null)
                _instance = new VM_Map();
            return _instance;
        }
    }
    // СЕРВИС ДЛЯ СОЗДАНИЯ ОБЪЕКТОВ
    private readonly IFixture fixture;
    private VM_Map()
    {
        fixture = new Fixture();
        // я выхожу из приложения
        EventService.ImHasLeftAppEvent += Dispose;
        // я изменяю фото (обновление маркера)
        EventService.ImChangedPhotoEvent += OnImChangedPhoto;
        // друг изменил фото (обновление маркера)
        EventService.FrChangedPhotoEvent += OnFrChangedPhoto;
        // друг изменил локацию
        EventService.FrLocationChangedEvent += UpdateLocation;
        // друг удалил меня из друзей
        EventService.FrRemoveFriendEvent += RemoveMarker;
        // друг вышел из приложения
        EventService.FrFriendHasLeftAppEvent += RemoveMarker;
        // я удалил пользователя из друзей
        EventService.ImRemoveFriendEvent += RemoveMarker;
        EventService.ImEnterAppEvent += EventService_ImEnterAppEvent;
        Kind = IconKind.LocationOff;
        IsTracking = false;
    }
    private async Task EventService_ImEnterAppEvent()
    {
        try
        {
            Kind = IconKind.LocationOff;
            IsTracking = false;
            RealTimeLocationTrackerCommand.Execute(this);
        }
        catch { }
    }

    private Task Dispose()
    {
        try
        {
            // очищаем коллекцию маркеров на карте
            lock (LocationPins)
            {
                LocationPins.Clear();
            }
            // отключаем передачу местоположения
            // проверяем, работает ли команда перед отменой
            RealTimeLocationTrackerCancelCommand.Execute(this); 
        }
        catch { }
        return Task.CompletedTask;
    }

    // МАРКЕР ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ
    [ObservableProperty]
    private VM_LocationPin currentLocationPin;

    // КОЛЛЕКЦИЯ ВСЕХ МАРКЕРОВ НА КАРТЕ
    public ObservableCollection<VM_LocationPin> LocationPins { get; } = new();

    [ObservableProperty]
    private bool isTracking;

    private IconKind kind;
    public IconKind Kind
    {
        get { return kind; }
        set { SetProperty(ref kind, value); }
    }

    // Делиться с друзьями координатами
    [RelayCommand]
    private Task ShareLocationToFriend()
    {
        try
        {
            IsTracking = !IsTracking;
            Kind = Kind == IconKind.LocationOn ? IconKind.LocationOff : IconKind.LocationOn;
        }
        catch { }
        return Task.CompletedTask;
    }
    // На кнопку, которая передвигает юзера на его местоположение
    [RelayCommand]
    private Task StartRealTimeLocationTracker()
    {
        try
        {
            if (!RealTimeLocationTrackerCommand.IsRunning)
                RealTimeLocationTrackerCommand.Execute(this);
        }
        catch { }
        return Task.CompletedTask;
    }
    public Location GetCurrentLocation()
    {
        try
        {
            if (CurrentLocationPin != null)
            {
                return CurrentLocationPin.Location;
            }
        }
        catch
        {

        }
        return null;
    }

    // ОТСЛЕЖИВАНИЕ МЕСТОПОЛОЖЕНИЯ ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ С ОБНОВЛЕНИЕМ КООРДИНАТ
    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task RealTimeLocationTracker(CancellationToken cancellationToken)
    {
        try
        {
            var progress = new Progress<Location>(location =>
            {
                if (CurrentLocationPin is null)
                {
                    CurrentLocationPin = new VM_LocationPin
                    {
                        Location = location,
                        User = VM_CurrentUser.Instance
                    };
                    lock (LocationPins)
                    {
                        LocationPins.Add(CurrentLocationPin);
                    }
                }
                else
                {
                    lock (LocationPins)
                    {
                        LocationPins.Remove(CurrentLocationPin);
                        CurrentLocationPin.Location = location;
                        LocationPins.Add(CurrentLocationPin);
                    }
                }
                if (IsTracking)
                    EventService.RaiseImLocationChangedEvent(location);
            });
            await Geolocator.Default.StartListening(progress, cancellationToken);
        }
        catch
        {
            await Toast.Make("Error reading location.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        }
        finally
        {
            lock (LocationPins)
            {
                try
                {
                    LocationPins.Remove(CurrentLocationPin);
                }
                catch { }
            }
        }
    }

    // Я СМЕНИЛ ФОТО
    [Obsolete]
    private Task OnImChangedPhoto(string PathToPhoto)
    {
        try
        {
            if (CurrentLocationPin == null) return Task.CompletedTask;
            if (!LocationPins.Contains(CurrentLocationPin)) return Task.CompletedTask;
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (LocationPins)
                {
                    LocationPins.Remove(CurrentLocationPin);
                }
                LocationPins.Add(CurrentLocationPin);
            });
        }
        catch {}
        return Task.CompletedTask;
    }
    // ИЗМЕНЕНИЕ ФОТО ДРУГА - КОД 5

    [Obsolete]
    private Task OnFrChangedPhoto(string friendName, string path)
    {
        try
        {
            VM_LocationPin pin;
            lock (LocationPins)
            {
                pin = LocationPins.First(x => x.User.Name == friendName);
            }
            if (pin != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    lock (LocationPins)
                    {
                        LocationPins.Remove(pin);
                    }
                    LocationPins.Add(pin);
                });
            }
        }
        catch (Exception ex)
        {
            try
            {
                ShowErrorService.ShowError(ex.Message).Wait();
            }
            catch { }
        }
        return Task.CompletedTask;
    }

    // ОБНОВЛЕНИЕ КООРДИНАТ ПОЛЬЗОВАТЕЛЯ
    private Task UpdateLocation(string username, Location location)
    {
        try
        {
            VM_LocationPin pin;
            lock (LocationPins)
            {
                pin = LocationPins.FirstOrDefault(x => x.User.Name == username);
            }
            // если нету создаем
            if (pin == null)
            {
                VM_User user = FriendCollection.Instance.ConnectedUsers.FirstOrDefault(u => u.Name == username);
                pin = fixture.Build<VM_LocationPin>()
                                        .With(x => x.User, user)
                                        .With(x => x.Location, location)
                                        .Create();
                lock (LocationPins)
                {
                    LocationPins.Add(pin);
                }
            }
            // если есть просто меняем координаты
            else
            {
                // Захватываем ресурс
                lock (LocationPins)
                {
                    LocationPins.Remove(pin);
                    pin.Location = location;
                    LocationPins.Add(pin);
                }
            }
        }
        catch { }
        return Task.CompletedTask;
    }
    private Task RemoveMarker(string userName)
    {
        try
        {
            VM_LocationPin pin;
            lock (LocationPins)
            {
                pin = LocationPins.FirstOrDefault(x => x.User.Name == userName);
            }
            // если есть удаляем
            if (pin != null)
            {
                // Захватываем ресурс
                lock (LocationPins)
                {
                    LocationPins.Remove(pin);
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorService.ShowError(ex.Message).Wait();
        }
        return Task.CompletedTask;
    }
}