using BussBuzz.Base;
using BussBuzz.MVVM.Friends.ViewModel;
using BussBuzz.Services;

namespace BussBuzz.MVVM.Friends.Model
{
    public class M_FriendTemplateSelector : DataTemplateSelector
    {
        // Шаблон человек, отправивший запрос в друзья
        public DataTemplate FriendRequestSenderTemplate { get; set; } = new();
        // Человек, которому отправлен запрос в друзья
        public DataTemplate FriendRequestRecipientTemplate { get; set; } = new();

        // Обычный пользователь (не друг)
        public DataTemplate RegularUserTemplate { get; set; } = new();
        // Друг
        public DataTemplate FriendTemplate { get; set; } = new ();
        // Неизвестно кто
        public DataTemplate UnknownTemplate { get; set; } = new();

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is VM_FriendItem viewModel)
            {
                switch (viewModel.User.Status) 
                {
                    case (UserStatus.Friend): return FriendTemplate;
                    case (UserStatus.RegularUser): return RegularUserTemplate;
                    case (UserStatus.FriendRequestSender): return FriendRequestSenderTemplate;
                    case (UserStatus.FriendRequestRecipient): return FriendRequestRecipientTemplate;
                }
            }
            return UnknownTemplate;
        }
    }

}
