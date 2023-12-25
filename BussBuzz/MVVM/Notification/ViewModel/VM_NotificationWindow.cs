using BussBuzz.Base;
using BussBuzz.MVVM.Notification.Model;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.Notification.ViewModel
{
    public partial class VM_NotificationWindow: ObservableObject
    {
        private static VM_NotificationWindow _instance;
        public static VM_NotificationWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VM_NotificationWindow();
                return _instance;
            }
        }
        private VM_NotificationWindow()
        {
            EventService.ImCancelAddFriendEvent += RemoveNotification;
            EventService.ImConfirmAddFriendEvent += RemoveNotification;
            EventService.ImReadTheNotifyEvent += RemoveNotification;

            EventService.FrRequestAddFriendEvent += UpdateNotificationList; ;
            EventService.FrConfirmAddFriendEvent += UpdateNotificationList;
            EventService.FrCancelAddFriendEvent += UpdateNotificationList;
            EventService.FrRemoveFriendEvent += UpdateNotificationList;

            EventService.ImHasLeftAppEvent += Dispose;
            EventService.ImEnterAppEvent += Init;
            Init(); // костыль
        }

        private Task Init()
        {
            Task.Run(() => InitNotification());
            return Task.CompletedTask;
        }

        private Task Dispose()
        {
            lock (DisplayedCollectionNotifications)
            {
                DisplayedCollectionNotifications.Clear();
            }
            return Task.CompletedTask;
        }

        public bool IsExistNotification()
        {
            lock(DisplayedCollectionNotifications)
            {
                return DisplayedCollectionNotifications.Count > 0;
            }
        }
        private async Task UpdateNotificationList(string friendname)
        {
            try
            {
                M_Notification notification = await DatabaseService.Instance.GetLastNotificationFrom(friendname);
                DisplayedCollectionNotifications.Insert(0, new VM_Notification(notification));
            }
            catch { }
        }

        // Визуально убираем уведомление (аля обработчик)
        private Task RemoveNotification(VM_Notification notification)
        {
            try
            {
                VM_Notification not = DisplayedCollectionNotifications.First<VM_Notification>(n => n.Model.ID == notification.Model.ID);
                if (not != null)
                    DisplayedCollectionNotifications.Remove(not);
            }
            catch { }
            return Task.CompletedTask;
        }
        
        public async Task InitNotification()
        {
            try
            {
                lock (DisplayedCollectionNotifications)
                {
                    DisplayedCollectionNotifications.Clear();
                }
                List<M_Notification> notific  = await DatabaseService.Instance.GetNotifications();
                foreach (M_Notification notification in notific)
                {
                    DisplayedCollectionNotifications.Insert(0, new VM_Notification(notification));
                }
            }
            catch(Exception ex) { }
        }
        public ObservableCollection<VM_Notification> DisplayedCollectionNotifications { get; } = new();


        // Когда я отменяю запрос на дружбу
        [RelayCommand]
        private Task ImCancelAddFriend(VM_Notification notification)
        {
            EventService.RaiseImCancelAddFriendEventAsync(notification);
            return Task.CompletedTask;
        }

        // Когда я принимаю запрос на дружбу
        [RelayCommand]
        private Task ImConfirmAddFriend(VM_Notification notification)
        {
            EventService.RaiseImConfirmAddFriendEventAsync(notification);
            return Task.CompletedTask;
        }

        // когда я ознакомлен с уведомлением
        [RelayCommand]
        private Task ImReadTheNotify(VM_Notification notification)
        {
            EventService.RaiseImReadTheNotifyEvent(notification);
            return Task.CompletedTask;
        }


        [ObservableProperty]
        private bool isRefreshing;

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                DatabaseService.Instance.InitConnectedUsers();
                await InitNotification();
                IsRefreshing = false;
            }
            catch { }
        }

    }
}
