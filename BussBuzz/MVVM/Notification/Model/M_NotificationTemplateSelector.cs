using BussBuzz.Base;
using BussBuzz.MVVM.Notification.ViewModel;

namespace BussBuzz.MVVM.Notification.Model
{
    public class M_NotificationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FriendRequestReceivedTemplate { get; set; }
        public DataTemplate FriendRemovedTemplate { get; set; }
        public DataTemplate FriendRequestAcceptedTemplate { get; set; }
        public DataTemplate FriendRequestRejectedTemplate { get; set; }
        public DataTemplate Unknown { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is VM_Notification viewModel)
            {
                switch (viewModel.NotificationCode) 
                {
                    case NotificationCode.FriendRequestReceived:
                        {
                            return FriendRequestReceivedTemplate;
                        }
                    case NotificationCode.FriendRemoved:
                        {
                            return FriendRemovedTemplate;
                        }
                    case NotificationCode.FriendRequestAccepted:
                        {
                            return FriendRequestAcceptedTemplate;
                        }
                    case NotificationCode.FriendRequestRejected:
                        {
                            return FriendRequestRejectedTemplate;
                        }
                    default: return Unknown;
                }
            }
            return Unknown;
        }
    }

}
