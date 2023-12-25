using BussBuzz.Base;
using BussBuzz.MVVM.Friends.ViewModel;
using BussBuzz.MVVM.Notification.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using IconPacks.Material;
using Material.Components.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.Main.ViewModel
{
    public partial class VM_Main : ObservableObject ,VM_Base
    {
        public VM_Main()
        {
            InitializeAsync();
            EventService.RaiseImEnterAppEvent();
            //EventService.FrCancelAddFriendEvent += Reload;
            //EventService.FrConfirmAddFriendEvent += Reload;
            //EventService.FrRemoveFriendEvent += Reload;
            //EventService.FrRequestAddFriendEvent += Reload;
            //EventService.ImReadTheNotifyEvent += Reload;
        }

        //private Task Reload(string username)
        //{
        //    Notifiaction = VM_NotificationWindow.Instance.IsExistNotification() ? IconKind.NotificationsActive : IconKind.Notifications;
        //    return Task.CompletedTask;
        //}
        //private Task Reload(VM_Notification Notification)
        //{
        //    Notifiaction = VM_NotificationWindow.Instance.IsExistNotification() ? IconKind.NotificationsActive : IconKind.Notifications;
        //    return Task.CompletedTask;
        //}

        //private IconKind notifiaction;
        //public IconKind Notifiaction
        //{ 
        //    get { return notifiaction; }
        //    set { SetProperty<IconKind>(ref notifiaction, value); }
        //}

        private async void InitializeAsync()
        {
            try
            {
                //Notifiaction = VM_NotificationWindow.Instance.IsExistNotification() ? IconKind.NotificationsActive : IconKind.Notifications;
                _ = TCPClient.Instance.Start();
            }
            catch
            {
                // Обработка ошибок, если не удалось выполнить какой-то из шагов
                //Console.WriteLine($"Error during initialization: {ex.Message}");
            }
        }
    }
}
