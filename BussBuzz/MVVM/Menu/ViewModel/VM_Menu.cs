using BussBuzz.Base;
using BussBuzz.MVVM.Authorization.ViewModel;
using BussBuzz.MVVM.ImageColor;
using BussBuzz.MVVM.MAP.ViewModell;
using BussBuzz.MVVM.Menu.MenuItems.About;
using BussBuzz.MVVM.Menu.MenuItems.Feedback;
using BussBuzz.MVVM.Menu.MenuItems.MainFunc;
using BussBuzz.MVVM.Menu.MenuItems.Settings;
using BussBuzz.MVVM.Menu.MenuItems.Share;
using BussBuzz.MVVM.Menu.Model;
using BussBuzz.Services;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Material;
using System.Windows.Input;

namespace BussBuzz.MVVM.Menu.ViewModel
{
    public partial class VM_Menu : ObservableValidator, VM_Base
    {
        public VM_Menu()
        {
            _model = new M_Menu();
            CurrentUser = VM_CurrentUser.Instance;
            this.ThemeIcon = Application.Current.RequestedTheme is AppTheme.Dark
                ? IconKind.DarkMode
                : IconKind.LightMode;

            Application.Current.RequestedThemeChanged += (s, e) =>
            {
                this.ThemeIcon = Application.Current.RequestedTheme is AppTheme.Dark
                     ? IconKind.DarkMode
                     : IconKind.LightMode;
            };
            EventService.ImHasLeftAppEvent += MoveToSignIn;
        }

        private M_Menu _model;

        [ObservableProperty]
        private IconKind themeIcon;

        [RelayCommand]
        public static void ThemeChanged()
        {
            Application.Current.UserAppTheme =
                Application.Current.RequestedTheme is AppTheme.Dark
                    ? AppTheme.Light
                    : AppTheme.Dark;
        }

        [ObservableProperty]
        private VM_CurrentUser currentUser;
        [RelayCommand]
        public Task Exit()
        {
            EventService.RaiseImHasLeftAppEvent();
            return Task.CompletedTask;
        }
        private Task MoveToSignIn()
        {
            NavigationService.Instance.NavigateToAsync<VM_SignIn>();
            return Task.CompletedTask;
        }
        [RelayCommand]
        private Task ShowPopupImageAndColorChange()
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_ImageColor>();
            }
            catch (Exception ex) { ShowErrorService.ShowError(ex.Message).Wait(); }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async void MoveToAbout()
        {
            try
            {
                await NavigationService.Instance.NavigateToAsync<VM_About>();
            }
            catch (Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
            }
        }
        [RelayCommand]
        private void MoveToFeedback()
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_Feedback>();
            }
            catch { }
        }
        [RelayCommand]
        private void MoveToMainFunc()
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_MainFunc>();
            }
            catch { }
        }
        [RelayCommand]
        private void MoveToSettings()
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_Settings>();
            }
            catch { }
        }
        [RelayCommand]
        private async void MoveToShare()
        {
            try
            {
                await ShareText("https://github.com/StasSova/BuzzBus/releases/download/Release/BuzzBuzz-v1.0.apk");
            }
            catch { }
        }
        // Вызов метода ShareText для передачи текста
        async Task ShareText(string text)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Поделиться ссылкой",
            });
        }

        [RelayCommand]
        private void MoveToOfficialNews()
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_OfficialNews>();
            }
            catch { }
        }
    }
}
