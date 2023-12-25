using BussBuzz.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.Friends.ViewModel
{
    public partial class VM_FriendItem: ObservableObject
    {
        public VM_FriendItem(VM_User User) 
        {
            this.User = User;
        }
        public VM_User User { get; set; }
        public string Description
        {
            get { return GetDescription(User.Status); }
        }
        private string GetDescription(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Friend: return "Your friend";
                case UserStatus.FriendRequestSender: return "This user has sent you a friend request.";
                case UserStatus.FriendRequestRecipient: return "Waiting for a response to the friend request.";
                case UserStatus.RegularUser: return "You can send a friend request.";
                case UserStatus.BlockedUser: return "Blocked user";
                default: return "";
            }
        }
    }
}
