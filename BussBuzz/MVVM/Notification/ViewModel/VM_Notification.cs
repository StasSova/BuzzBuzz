using AutoFixture;
using BussBuzz.Base;
using BussBuzz.MVVM.Notification.Model;
using BussBuzz.MVVM.Notification.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.Notification.ViewModel
{
    public partial class VM_Notification: ObservableObject, VM_Base
    {
        public M_Notification Model { get; set; }
        public VM_Notification(M_Notification model) 
        {
            Model = model;
            user = FriendCollection.Instance.ConnectedUsers.First(u => u.Name == model.UserName);
        }
        public NotificationCode NotificationCode
        {
            get { return Model.Code; }
            set
            {
                SetProperty(ref Model.Code, value);
            }
        }
        private VM_User user;
        public VM_User User
        {
            get { return user; }
        }
        public string Message
        {
            get { return GetMessageByMessageType(Model.Code); }
        }
        public string DateAndTime
        {
            get { return Model.DateAndTime; }
        }
        private static string GetMessageByMessageType(NotificationCode notificationCode)
        {
            switch (notificationCode)
            {
                case NotificationCode.FriendRequestReceived:
                    return "User sent a friend request";
                case NotificationCode.FriendRemoved:
                    return "Friendship Status Change: You've been excluded from their friends list, and your friends' list has been modified.";
                case NotificationCode.FriendRequestAccepted:
                    return "User accepted your friendship request";
                case NotificationCode.FriendRequestRejected:
                    return "User declined your friendship invitation";
                default:
                    return "Unknown Message";
            }
        }
    }
}
