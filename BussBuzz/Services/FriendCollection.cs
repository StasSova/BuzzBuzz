using BussBuzz.Base;
using BussBuzz.MVVM.Notification.ViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public partial class FriendCollection: ObservableObject
    {
        private static FriendCollection _instance;
        public static FriendCollection Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FriendCollection();
                return _instance;
            }
        }
        private FriendCollection()
        {
            EventService.FrChangedPhotoEvent += EventService_FrChangedPhotoEvent;
            EventService.FrChangedColorEvent += EventService_FrChangedColorEvent;

            EventService.FrRemoveFriendEvent += EventService_FrRemoveFriendEvent;
            EventService.FrRequestAddFriendEvent += EventService_FrRequestAddFriendEvent;
            EventService.FrConfirmAddFriendEvent += EventService_FrConfirmAddFriendEvent;
            EventService.FrCancelAddFriendEvent += EventService_FrCancelAddFriendEvent;


            EventService.ImRequestAddFriendEvent += EventService_ImRequestAddFriendEvent;
            EventService.ImRemoveFriendEvent += EventService_ImRemoveFriendEvent;
            EventService.ImConfirmAddFriendEvent += EventService_ImConfirmAddFriendEvent;
            EventService.ImCancelAddFriendEvent += EventService_ImCancelAddFriendEvent;

            EventService.ImHasLeftAppEvent += Dispose;
        }

        private Task Dispose()
        {
            lock(ConnectedUsers)
            {
                ConnectedUsers.Clear();
            }
            return Task.CompletedTask;
        }

        public void AddUser(string UserName, UserStatus status)
        {
            try
            {
                if (null != ConnectedUsers.FirstOrDefault(u => u.Name == UserName))
                    return;
                M_User item = DatabaseService.Instance.GetUser(UserName);
                item._status = status;
                ConnectedUsers.Add(new VM_User(item));
            }
            catch { }
        }

        public Task Initialization(List<M_User> users)
        {
            try
            {
                ConnectedUsers.Clear();
                foreach (M_User item in users)
                {
                    ConnectedUsers.Add(new VM_User(item));
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_ImRequestAddFriendEvent(string friendname)
        {
            // Когда пользователь, хочет добавиться ко мне в друзья
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User user = ConnectedUsers.FirstOrDefault(x => x.Name == friendname);
                    // если нету такого пользователя - создаем
                    if (user == null)
                    {
                        // 1. С БД забрать информацию о пользователе.
                        M_User m_user = DatabaseService.Instance.GetUser(friendname);
                        // 2. Добавить его в список со статусом
                        user = new VM_User(m_user);
                        user.Status = UserStatus.FriendRequestRecipient;
                        ConnectedUsers.Add(user);
                    }
                    else
                    {
                        // если есть, просто меняем статус
                        user.Status = UserStatus.FriendRequestRecipient;
                    }
                }
            }
            catch (Exception e)
            {
                
            }
            return Task.CompletedTask;
        }
        private Task EventService_ImRemoveFriendEvent(string friendname)
        {
            // Когда пользователь, хочет добавиться ко мне в друзья
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User user = ConnectedUsers.FirstOrDefault(x => x.Name == friendname);
                    // если нету такого пользователя - создаем
                    if (user == null)
                    {
                        // 1. С БД забрать информацию о пользователе.
                        M_User m_user = DatabaseService.Instance.GetUser(friendname);
                        // 2. Добавить его в список со статусом
                        user = new VM_User(m_user);
                        user.Status = UserStatus.RegularUser;
                        ConnectedUsers.Add(user);
                    }
                    else
                    {
                        // если есть, просто меняем статус
                        user.Status = UserStatus.RegularUser;
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_ImConfirmAddFriendEvent(VM_Notification Notification)
        {
            try
            {
                lock(ConnectedUsers)
                {
                    Notification.User.Status = UserStatus.Friend;
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_ImCancelAddFriendEvent(VM_Notification Notification)
        {
            try
            {
                lock (ConnectedUsers)
                {
                    Notification.User.Status = UserStatus.RegularUser;
                }
            }
            catch { }
            return Task.CompletedTask;
        }


        private Task EventService_FrChangedColorEvent(string username, string color)
        {
            try
            {
                VM_User friendToChange;
                lock (ConnectedUsers)
                {
                    friendToChange = ConnectedUsers.FirstOrDefault(f => f.Name == username);
                }

                if (friendToChange != null)
                {
                    lock (friendToChange)
                    {
                        friendToChange.Color = color;
                    }
                }
            }
            catch { }

            return Task.CompletedTask;
        }
        private Task EventService_FrChangedPhotoEvent(string username, string pathPhoto)
        {
            try
            {
                VM_User friendToChange;
                lock (ConnectedUsers)
                {
                    friendToChange = ConnectedUsers.FirstOrDefault(f => f.Name == username);
                }

                if (friendToChange != null)
                {
                    friendToChange.Image = pathPhoto;
                }
            }
            catch { }

            return Task.CompletedTask;
        }
        private Task EventService_FrRequestAddFriendEvent(string username)
        {
            // Когда пользователь, хочет добавиться ко мне в друзья
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User user = ConnectedUsers.FirstOrDefault(x => x.Name == username);
                    // если нету такого пользователя - создаем
                    if (user == null)
                    {
                        // 1. С БД забрать информацию о пользователе.
                        M_User m_user = DatabaseService.Instance.GetUser(username);
                        // 2. Добавить его в список со статусом
                        user = new VM_User(m_user);
                        user.Status = UserStatus.FriendRequestSender;
                        ConnectedUsers.Add(user);
                    }
                    else
                    {
                        // если есть, просто меняем статус
                        user.Status = UserStatus.FriendRequestSender;
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_FrRemoveFriendEvent(string username)
        {
            // просто меняем статус с друг на обычный пользователь
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User friendToRemove = ConnectedUsers.FirstOrDefault(f => f.Name == username);

                    if (friendToRemove != null)
                    {
                        friendToRemove.Status = UserStatus.RegularUser;
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_FrConfirmAddFriendEvent(string username)
        {
            // Когда пользователь, подтвердил запрос в друзья
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User user = ConnectedUsers.FirstOrDefault(x => x.Name == username);
                    // если нету такого пользователя - создаем
                    if (user == null)
                    {
                        // 1. С БД забрать информацию о пользователе.
                        M_User m_user = DatabaseService.Instance.GetUser(username);
                        // 2. Добавить его в список со статусом
                        user = new VM_User(m_user);
                        user.Status = UserStatus.Friend;
                        ConnectedUsers.Add(user);
                    }
                    else
                    {
                        // если есть, просто меняем статус
                        user.Status = UserStatus.Friend;
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task EventService_FrCancelAddFriendEvent(string username)
        {
            // Когда пользователь, подтвердил запрос в друзья
            try
            {
                lock (ConnectedUsers)
                {
                    VM_User user = ConnectedUsers.FirstOrDefault(x => x.Name == username);
                    // если нету такого пользователя - создаем
                    if (user == null)
                    {
                        // 1. С БД забрать информацию о пользователе.
                        M_User m_user = DatabaseService.Instance.GetUser(username);
                        // 2. Добавить его в список со статусом
                        user = new VM_User(m_user);
                        user.Status = UserStatus.RegularUser;
                        ConnectedUsers.Add(user);
                    }
                    else
                    {
                        // если есть, просто меняем статус
                        user.Status = UserStatus.RegularUser;
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        // Коллекция для моих друзей, она всегда содержит коллекцию моих друзей в полном составе
        public ObservableCollection<VM_User> ConnectedUsers { get; } = new();
        public Task ClearAllRegularUser()
        {
            // Когда я нажимаю на кнопку поиска, мне нету смысла больше хранить тех пользователей, 
            // с которыми у меня нету связи и я их удаляю из этой коллекции
            try
            {
                lock (ConnectedUsers)
                {
                    List<VM_User> usersToRemove = ConnectedUsers.Where(user => user.Status == UserStatus.RegularUser).ToList();
                    foreach (VM_User userToRemove in usersToRemove)
                    {
                        try
                        {
                            ConnectedUsers.Remove(userToRemove);
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
