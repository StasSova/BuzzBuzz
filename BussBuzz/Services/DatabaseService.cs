using BussBuzz.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Data.Common;
using System.Xml.Linq;
using BussBuzz.MVVM.Friends.ViewModel;
using BussBuzz.MVVM.Notification.Model;
using BussBuzz.MVVM.Notification.ViewModel;
using System.Drawing;
using static SkiaSharp.HarfBuzz.SKShaper;
using BussBuzz.MVVM.Menu.ViewModel;
using AutoFixture;
using System.Collections.ObjectModel;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews.Details;
//using Foundation;
//using Windows.System;

namespace BussBuzz.Services
{
    // Подключение БД
    public class DatabaseConnection
    {
        //firebase connection Settings
        public IFirebaseConfig fc = new FirebaseConfig()
        {
            AuthSecret = "YOUR_KEY",
            BasePath = "YOUR_KEY"
        };

        public IFirebaseClient client;
        //Code to warn console if class cannot connect when called.

        public string path = "users/";

        public DatabaseConnection()
        {
            try
            {
                client = new FireSharp.FirebaseClient(fc);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
    }

    // Данные для БД
    public class DatabaseUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public string Salt { get; set; }
        public List<M_Notification> Notifications { get; set; } = new();
        public List<string> FriendsUsernames { get; set; }
    }


    public class DatabaseService
    {
        static DatabaseService _instance;
        public static DatabaseService Instance
        {
            get
            {
                // если будет _instance == null, ему присвоится значения
                _instance ??= new DatabaseService();
                return _instance;
            }
        }
        public bool IsConnect { get; private set; } = false;

        DatabaseConnection dbConnection = new();
        private static DatabaseUser me { get; set; }


        private DatabaseService()
        {
            //Я ОТПРАВЛЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА - КОД 1
            EventService.ImRequestAddFriendEvent += OnImRequestAddFriendEvent;

            // Я ХОЧУ УДАЛИТЬ ДРУГА ИЗ ДРУЗЕЙ - КОД 4
            EventService.ImRemoveFriendEvent += OnImRemoveFriendEvent;

            //Я ОТКЛОНЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 2 
            EventService.ImCancelAddFriendEvent += OnImCancelAddFriendEvent;
            //Я ПОДТВЕРЖДАЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 3 
            EventService.ImConfirmAddFriendEvent += OnImConfirmAddFriendEvent;
            //Я ОЗНАКОМИЛСЯ С УВЕДОМЛЕНИЕ - КОД 8
            EventService.ImReadTheNotifyEvent += OnImReadTheNotifyEvent;


            EventService.ImChangedColorEvent += OnImChangedColorEvent;
            EventService.ImChangedPhotoEvent += OnImChangedPhotoEvent;


            EventService.FrConfirmAddFriendEvent += OnFrConfirmAddFriendEvent;
            EventService.FrRemoveFriendEvent += OnFrRemoveFriendEvent;
        }

        private Task OnFrRemoveFriendEvent(string username)
        {
            lock (me.FriendsUsernames)
            {
                me.FriendsUsernames.Remove(username);
            }
            return Task.CompletedTask;
        }

        private Task OnFrConfirmAddFriendEvent(string username)
        {
            lock (me.FriendsUsernames)
            {
                me.FriendsUsernames.Add(username);
            }
            return Task.CompletedTask;
        }

        private void AddUser(DatabaseUser user)
        {
            try
            {
                dbConnection.client.Set(dbConnection.path + user.Username, user);
            }
            catch (Exception)
            {

                throw new Exception("Ошибка в методе AddUser");
            }
        }

        private void UpdateUser(DatabaseUser user)
        {
            dbConnection.client.Update(dbConnection.path + user.Username, user);
        }

        private int GetNextIDNum(DatabaseUser user)
        {
            int num = -1;

            foreach (M_Notification notification in user.Notifications)
            {
                num = notification.ID;
            }

            return num + 1;
        }

        //Cуществует ли пользователь с таким именем
        private bool DoesNameExist(string name)
        {
            FirebaseResponse response = dbConnection.client.Get(dbConnection.path + name);

            if (response.Body == "null") return false;
            else return true;
        }

        // Получает конкретного пользователя из БД
        public DatabaseUser GetDbUser(string name)
        {
            FirebaseResponse response = dbConnection.client.Get(dbConnection.path + name);
            if (response.Body == "null") return null;

            DatabaseUser user = JsonConvert.DeserializeObject<DatabaseUser>(response.Body);
            return user;
        }

        // Получить список имён друзей
        private List<string> GetFriendsName(string name)
        {
            DatabaseUser user = GetDbUser(name);

            List<string> lst = user.FriendsUsernames;
            if (lst.Contains("empty"))
                lst.Remove("empty");
            return lst;
        }

        private Dictionary<string, DatabaseUser> GetUsers()
        {
            FirebaseResponse response = dbConnection.client.Get("users/");
            Dictionary<string, DatabaseUser> users = JsonConvert.DeserializeObject<Dictionary<string, DatabaseUser>>(response.Body);
            return users;
        }

        // Регистрация
        private void Registration(DatabaseUser user)
        {
            AddUser(user);
        }

        public List<VM_Friend> GetFriends(string name)
        {
            List<VM_Friend> friends = new();
            //DatabaseUser recievedUser = GetUser(name);

            if (me.FriendsUsernames.Count > 1)
            {
                foreach (string friendName in me.FriendsUsernames)
                {
                    if (friendName == "empty") continue;
                    DatabaseUser userFriend = GetDbUser(friendName);

                    VM_Friend m_Friend = new()
                    {
                        Name = userFriend.Username,
                        Color = userFriend.Color,
                        Image = userFriend.Image,
                    };
                    friends.Add(m_Friend);
                }
            }


            return friends;
        }

        // Подключение к БД
        public async Task ConnectAsync()
        {
            int maxAttempts = 3;
            int currentAttempt = 1;

            while (currentAttempt <= maxAttempts)
            {
                try
                {
                    dbConnection = new DatabaseConnection();
                    IsConnect = true;
                    return; // Успешное подключение, выходим из метода
                }
                catch (Exception ex)
                {
                    // Обработка исключения, например, вывод в консоль или логирование.
                    //Console.WriteLine($"Error connecting to database on attempt {currentAttempt}: {ex.Message}");

                    // Увеличиваем номер текущей попытки
                    currentAttempt++;

                    // Пауза перед следующей попыткой, если это не последняя
                    if (currentAttempt <= maxAttempts)
                    {
                        await Task.Delay(1000); // Подождать 1 секунду (или другое подходящее время)
                    }
                }
            }
            // Если мы дошли до этой точки, значит, все три попытки неудачны
            throw new Exception("Error connecting to database after multiple attempts");
        }
        // Отключение от БД                        
        public static async Task DisconnectAsync()
        {
            await Task.Run(() =>
            {

            });
        }
        // Генерация соли
        static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16]; // 16 байт для соли (можно выбрать другой размер)
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
        // Хеширование пароля с использованием соли
        static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Конкатенация пароля и соли
                string saltedPassword = password + salt;

                // Преобразование строки пароля и соли в массив байтов
                byte[] passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);

                byte[] hashedBytes = SHA256.HashData(passwordBytes);

                // Преобразование массива байтов в строку в формате HEX
                StringBuilder stringBuilder = new();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    stringBuilder.Append(hashedBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
        // Проверка пароля
        static bool VerifyPassword(string userInput, string salt, string hashedPassword)
        {
            if (!DatabaseService.Instance.IsConnect)
                DatabaseService.Instance.ConnectAsync().Wait();
            // Захешировать введенный пользователем пароль с использованием сохраненной соли и сравнить с сохраненным захешированным паролем
            return StringComparer.OrdinalIgnoreCase.Compare(HashPassword(userInput, salt), hashedPassword) == 0;
        }
        // Вход
        public Task CheckingLoginAndPassword(string username, string password)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();

                if (!DoesNameExist(username)) { throw new Exception("Пользователь с таким именем не найден"); }

                DatabaseUser user = GetDbUser(username);

                if (!VerifyPassword(password, user.Salt, user.Password)) { throw new Exception("Пароль неправильный"); }

                return Task.CompletedTask;
            }
            catch { throw new Exception("Login or password is incorrect"); }
        }
        // Регистрация              
        public Task RegisteringAndInitializingCurrentUserFields(string username, string email, string password)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();

                if (DoesNameExist(username))
                {
                    throw new Exception("A user with the same name already exists");
                }
                // генерация соли в пароль
                string salt = GenerateSalt();
                // получение хешированного пароля
                string hashedPassword = HashPassword(password, salt);

                // Случайное число картинки
                Random random = new();
                int randomNumber = random.Next(1, 10);

                // Случайный цвет
                // Генерируем случайные значения для компонентов цвета
                int red = random.Next(256);
                int green = random.Next(256);
                int blue = random.Next(256);

                // Создаем новый цвет
                System.Drawing.Color randomColor = System.Drawing.Color.FromArgb(red, green, blue);

                // Преобразуем цвет в формат HEX
                string hexColor = "#" + randomColor.R.ToString("X2") + randomColor.G.ToString("X2") + randomColor.B.ToString("X2");

                M_Notification emptyNotification = new M_Notification
                {
                    Code = NotificationCode.Empty,
                    DateAndTime = "empty",
                    UserName = "empty",
                    ID = 0,
                };

                // регистрация пользоватея БД
                me = new()
                {
                    Username = username,
                    Email = email,
                    Password = hashedPassword,
                    Color = hexColor,
                    Image = $"emoji{randomNumber}.png",
                    Salt = salt,
                    FriendsUsernames = new List<string> { "empty" },
                    Notifications = new List<M_Notification> { emptyNotification }
                };

                // Иициализация текущего пользователя
                M_CurrentUser us = M_CurrentUser.Instance;
                us._name = me.Username;
                us._email = me.Email;
                us._password = me.Password;
                us._color = me.Color;
                us._image = me.Image;

                // создание пользователя в БД
                Registration(me);

                return Task.CompletedTask;
            }
            catch { throw; }
        }
        // получение данных пользователя                   
        public async Task InitializingCurrentUser(string username)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();


                // Забираем пользователя с БД.
                me = GetDbUser(username);

                // Инициализация текущего пользователя
                M_CurrentUser us = M_CurrentUser.Instance;
                us._name = me.Username;
                us._email = me.Email;
                us._password = me.Password;
                us._color = me.Color;
                us._image = me.Image;

                InitConnectedUsers();
                //await FriendCollection.Instance.Initialization(await GetFriends());
            }
            catch { throw; }
        }

        public void InitConnectedUsers()
        {
            // То, что будем возвращать
            List<M_User> users = new List<M_User>();

            // Получаем всех пользователей из БД
            Dictionary<string, DatabaseUser> allDatabaseUsers = GetUsers();

            // Перебираю каждого пользователя из БД
            foreach (DatabaseUser user in allDatabaseUsers.Values)
            {

                M_User newUser = new M_User();

                newUser._name = user.Username;
                newUser._image = user.Image;
                newUser._color = user.Color;
                newUser._status = UserStatus.RegularUser;   // Задаю по умолчаню, что юзер обычный

                // Если имя пользователя это имя текущего пользователя
                if (user.Username == me.Username)
                {
                    newUser._status = UserStatus.CurrentUser;

                    continue;
                }

                // Если имя пользователя в списке друзей текущего пользователя
                if (me.FriendsUsernames.Contains(user.Username))
                {
                    newUser._status = UserStatus.Friend;
                    users.Add(newUser);

                    continue;
                }

                // Если это пользователь, отправивший мне запрос в друзья
                foreach (M_Notification notification in me.Notifications)
                {
                    // Если уведомление от пользователя с кодом того, что он отправил мне запрос в друзья
                    if (notification.UserName == user.Username &&
                        notification.Code == NotificationCode.FriendRequestReceived)
                    {
                        newUser._status = UserStatus.FriendRequestSender;
                        users.Add(newUser);
                        continue;
                    }
                }

                // Пользователь, который отправил отклонение
                foreach (M_Notification notification in me.Notifications)
                {
                    // Если уведомление от пользователя с кодом того, что он отправил мне запрос в друзья
                    if (notification.UserName == user.Username && notification.Code == NotificationCode.FriendRequestRejected)
                    {
                        newUser._status = UserStatus.RegularUser;
                        users.Add(newUser);
                        continue;
                    }
                }

                // Если пользователь - человек, которому отправлен запрос в друзья
                foreach (M_Notification notification in user.Notifications)
                {
                    // Если в уведомлениях пользователя моё имя
                    if (notification.UserName == me.Username &&
                        notification.Code == NotificationCode.FriendRequestReceived)
                    {
                        newUser._status = UserStatus.FriendRequestRecipient;
                        users.Add(newUser);
                        continue;
                    }
                }

                //if (newUser._status != UserStatus.RegularUser) users.Add(newUser);
            }

            FriendCollection.Instance.Initialization(users);
        }
        // Проверка пользователя                   
        public Task IsUserNameExist(string username)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();

                return DoesNameExist(username) == true ? Task.CompletedTask : throw new Exception("This user is not registered. Check if your input is correct");
            }
            catch { throw; }
        }
        // Получение M_Friend, введя имя друга
        public Task<VM_Friend> GetFriend(string friendName)
        {
            if (GetDbUser(friendName) == null) return null;
            DatabaseUser friend = GetDbUser(friendName);
            VM_Friend m_friend = new()
            {
                Name = friend.Username,
                Color = friend.Color,
                Image = friend.Image,
            };

            return Task.FromResult(m_friend);
        }
        // Изменение пароля
        public Task SetPassword(string username, string newPassword)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();

                me = GetDbUser(username);
                string salt = GenerateSalt();
                me.Password = HashPassword(newPassword, salt);
                me.Salt = salt;

                UpdateUser(me);

                return Task.CompletedTask;
            }
            catch { throw; }
        }
        // Возращает пользователей с таким же схождением по имени
        public Task<List<VM_Friend>> GetСoincidences(string searchString)
        {
            List<VM_Friend> friends = new();

            // если строка пустая
            if (string.IsNullOrEmpty(searchString))
            {
                //List<string> friendsName = GetFriendsName(M_CurrentUser.Instance.Name);
                if (me.FriendsUsernames.Contains("empty")) return Task.FromResult(friends);
                foreach (string friendName in me.FriendsUsernames)
                {
                    DatabaseUser friend = GetDbUser(friendName);
                    VM_Friend m_friend = new()
                    {
                        Name = friend.Username,
                        Color = friend.Color,
                        Image = friend.Image,
                    };
                    friends.Add(m_friend);
                }
            }

            else
            {
                // Cначала добавляем друзей с совпадениями в список
                //List<string> friendsName = GetFriendsName(M_CurrentUser.Instance.Name);
                if (!me.FriendsUsernames.Contains("empty"))
                {
                    foreach (string str in me.FriendsUsernames)
                    {
                        if (str.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            DatabaseUser friend = GetDbUser(str);
                            VM_Friend m_friend = new()
                            {
                                Name = friend.Username,
                                Color = friend.Color,
                                Image = friend.Image,
                            };
                            friends.Add(m_friend);
                        }
                    }
                }


                Dictionary<string, DatabaseUser> users = GetUsers();

                foreach (string userName in users.Keys)
                {
                    // Если userName нет в friendsName
                    if (me.FriendsUsernames.Contains(userName) || userName == me.Username) continue;
                    else
                    {
                        if (userName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            DatabaseUser friend = GetDbUser(userName);
                            VM_Friend m_friend = new()
                            {
                                Name = users[userName].Username,
                                Color = users[userName].Color,
                                Image = users[userName].Image,
                            };
                            friends.Add(m_friend);
                        }
                    }
                }
            }
            return Task.FromResult(friends);
        }
        // Добавление пользователю друга
        public Task AddFriend(string username, string friendName)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();

                //DatabaseUser user = GetUser(username);

                if (!me.FriendsUsernames.Contains(friendName))
                {
                    me.FriendsUsernames.Add(friendName);
                    if (me.FriendsUsernames.Count == 2)
                    {
                        me.FriendsUsernames.Remove("empty");
                    }
                    UpdateUser(me);
                }



                return Task.CompletedTask;
            }
            catch { throw; }
        }
        // Удаление друга у пользователя
        public Task RemoveFriend(string username, string friendName)
        {
            try
            {
                if (!IsConnect)
                    ConnectAsync().Wait();
                //DatabaseUser user = GetUser(username);

                if (me.FriendsUsernames.Contains(friendName))
                {
                    if (me.FriendsUsernames.Count == 1)
                    {
                        me.FriendsUsernames.Add("empty");
                    }
                    me.FriendsUsernames.Remove(friendName);
                    UpdateUser(me);
                }

                return Task.CompletedTask;
            }
            catch { throw; }
        }

        // ПРОВЕРИТЬ
        // Проверка почты                                                       // DONE  
        public Task<bool> IsEmailExist(string email)
        {
            bool result = false;
            Dictionary<string, DatabaseUser> usersDictionary = GetUsers();

            bool containsEmail1 = usersDictionary.Any(kv => kv.Value.Email == email);

            return Task.FromResult(result);
        }

        // Получение списка друзей пользователя (зачем на M_Friend)???
        public async Task<List<M_User>> GetFriends()
        {
            return await Task.Run(() =>
            {
                List<M_User> friends = new();

                //DatabaseUser recievedUser = GetUser(M_CurrentUser.Instance.Name);
                if (me.FriendsUsernames.Contains("empty")) return new List<M_User>();

                foreach (string friendName in me.FriendsUsernames)
                {
                    DatabaseUser userFriend = GetDbUser(friendName);

                    M_User m_Friend = new()
                    {
                        _name = userFriend.Username,
                        _color = userFriend.Color,
                        _image = userFriend.Image,
                        _status = UserStatus.Friend
                    };
                    friends.Add(m_Friend);
                }
                return friends;
            });
        }

        public Task<List<M_Notification>> GetNotifications()
        {
            me = GetDbUser(VM_CurrentUser.Instance.Name);
            List<M_Notification> notificationList = new();

            foreach (var notification in me.Notifications)
            {
                if (notification.DateAndTime != "empty")
                {
                    notificationList.Add(notification);
                }
            }
            return Task.FromResult(notificationList);
        }

        public Task<M_Notification> GetLastNotificationFrom(string userName)
        {
            me = GetDbUser(VM_CurrentUser.Instance.Name);
            M_Notification notification = null;

            notification = me.Notifications.LastOrDefault(x => x.UserName == userName);

            if (notification != null)
            {
                return Task.FromResult(notification);
            }
            else
            {
                throw new Exception("No notifications found for the user.");
            }
        }


        // Является ли пользователь другом                                      
        public Task<bool> IsUserFriend(string friendName)
        {
            bool result = false;
            //DatabaseUser user = GetUser(M_CurrentUser.Instance.Name);  

            if (me.FriendsUsernames.Contains(friendName))
            {
                result = true;
            }
            return Task.FromResult(result);
        }


        //Я ОТПРАВЛЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА  (VM_FRIENDS)
        //Сделал
        private Task OnImRequestAddFriendEvent(string friendName)
        {
            // Я отправляю запрос в друзья, ДРУГУ должно в список его уведомлений добавиться новое уведомление с определенным кодом
            DatabaseUser friend = GetDbUser(friendName);

            M_Notification newFriendNotification = new M_Notification
            {
                ID = GetNextIDNum(friend),
                Code = NotificationCode.FriendRequestReceived,
                DateAndTime = DateTime.Now.ToString("MM.dd HH:mm"),
                UserName = me.Username,
            };

            //// Проверка, чтобы нельзя было отправить много одинаковых уведомлений
            //foreach (M_Notification notification in friend.Notifications)
            //{
            //    if (notification.Code == newFriendNotification.Code && notification.UserName == newFriendNotification.UserName)
            //    {
            //        return Task.CompletedTask;
            //    }
            //}

            lock (friend.Notifications)
            {
                friend.Notifications.Add(newFriendNotification);
            }
            UpdateUser(friend);

            return Task.CompletedTask;
        }


        //Я ОТКЛОНЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ
        //Сделал
        private Task OnImCancelAddFriendEvent(VM_Notification Notification)
        {
            //1.Изменить уведомление, которое весит у меня в списке моих уведомлений на обработанное(УДАЛИТЬ)
            me = GetDbUser(VM_CurrentUser.Instance.Name);

            lock (me.Notifications)
            {
                //me.Notifications.Remove(Notification.Model);
                me.Notifications.RemoveAll(notification => notification.Code == Notification.Model.Code && notification.UserName == Notification.Model.UserName);
                //me.Notifications.Remove(me.Notifications.First(x => x.Code == Notification.Model.Code));
                //foreach (M_Notification notification in me.Notifications)
                //{
                //    if (notification == Notification.Model)
                //    {
                //        me.Notifications.Remove(notification);
                //    }
                //}
            }
            UpdateUser(me);


            // 2. Другу, сделать уведомление о том, что я отменяю его запрос на добавления в друзья.
            //DatabaseUser friend = GetUser(Notification.Friend._name);
            DatabaseUser friend = GetDbUser(Notification.Model.UserName);

            M_Notification newFriendNotification = new M_Notification
            {
                ID = GetNextIDNum(friend),
                Code = NotificationCode.FriendRequestRejected,
                DateAndTime = DateTime.Now.ToString("MM.dd HH:mm"),
                UserName = me.Username,
            };

            // Проверка, чтобы нельзя было отправить много одинаковых уведомлений
            //foreach (M_Notification notification in friend.Notifications)
            //{
            //    if (notification.Code == newFriendNotification.Code && notification.UserName == newFriendNotification.UserName)
            //    {
            //        return Task.CompletedTask;
            //    }
            //}

            lock (friend.Notifications)
            {
                friend.Notifications.Add(newFriendNotification);
            }
            UpdateUser(friend);


            return Task.CompletedTask;
        }


        // Я ПОДТВЕРЖДАЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ
        // Сделал
        private Task OnImConfirmAddFriendEvent(VM_Notification Notification)
        {
            // 1. Изменить уведомление, которое весит у меня в списке моих уведомлений на обработанное (УДАЛИТЬ)
            me = GetDbUser(VM_CurrentUser.Instance.Name);
            try
            {
                lock (me.Notifications)
                {
                    //me.Notifications.Remove(Notification.Model);
                    me.Notifications.RemoveAll(notification => notification.Code == Notification.Model.Code && notification.UserName == Notification.Model.UserName);
                }

                if (!me.FriendsUsernames.Contains(Notification.Model.UserName))
                {
                    me.FriendsUsernames.Add(Notification.Model.UserName);
                    if (me.FriendsUsernames.Count == 2)
                    {
                        me.FriendsUsernames.Remove("empty");
                    }
                    UpdateUser(me);
                }

                //me.FriendsUsernames.Add(Notification.Model.UserName);
                //if (me.FriendsUsernames.Contains("empty"))
                //{
                //    me.FriendsUsernames.Remove("empty");
                //}

                UpdateUser(me);
                // 2. Другу, сделать уведомление о том, что я принимаю его запрос на добавления в друзья.
                DatabaseUser friend = GetDbUser(Notification.Model.UserName);


                M_Notification newFriendNotification = new M_Notification
                {
                    ID = GetNextIDNum(friend),
                    Code = NotificationCode.FriendRequestAccepted,
                    DateAndTime = DateTime.Now.ToString("MM.dd HH:mm"),
                    UserName = me.Username
                };


                lock (friend.Notifications)
                {
                    friend.Notifications.Add(newFriendNotification);
                }
                //friend.FriendsUsernames.Add(VM_CurrentUser.Instance.Name);
                //if (friend.FriendsUsernames.Contains("empty"))
                //    friend.FriendsUsernames.Remove("empty");
                //UpdateUser(friend);

                if (!friend.FriendsUsernames.Contains(VM_CurrentUser.Instance.Name))
                {
                    friend.FriendsUsernames.Add(VM_CurrentUser.Instance.Name);
                    if (friend.FriendsUsernames.Count == 2)
                    {
                        friend.FriendsUsernames.Remove("empty");
                    }
                    UpdateUser(friend);
                }
            }
            catch { }
            return Task.CompletedTask;
        }

        // Я ОЗНАКОМИЛСЯ С УВЕДОМЛЕНИЕ
        // Сделал
        private Task OnImReadTheNotifyEvent(VM_Notification Notification)
        {
            // Удалить уведомление из списка моих уведомлений
            me = GetDbUser(VM_CurrentUser.Instance.Name);
            lock (me.Notifications)
            {
                //me.Notifications.Remove(Notification.Model);
                me.Notifications.RemoveAll(notification => notification.Code == Notification.Model.Code && notification.UserName == Notification.Model.UserName);
                //foreach (M_Notification notification in me.Notifications)
                //{
                //    if (notification == Notification.Model)
                //    {
                //        me.Notifications.Remove(notification);
                //    }
                //}
            }
            UpdateUser(me);

            return Task.CompletedTask;
        }

        // Я ХОЧУ УДАЛИТЬ ДРУГА ИЗ ДРУЗЕЙ  (VM_FRIENDS)
        // Сделал
        private Task OnImRemoveFriendEvent(string friendName)
        {
            // 1. Удалить у меня из друзей этого друга
            me = GetDbUser(VM_CurrentUser.Instance.Name);
            me.FriendsUsernames.Remove(friendName);
            if (me.FriendsUsernames.Count == 0)
                me.FriendsUsernames.Add("empty");
            UpdateUser(me);

            // 2. Удалить у друга меня из друзей.
            DatabaseUser friend = GetDbUser(friendName);
            friend.FriendsUsernames.Remove(VM_CurrentUser.Instance.Name);
            if (friend.FriendsUsernames.Count == 0)
                friend.FriendsUsernames.Add("empty");
            // 3. Отправить другу сообщение о том, что я его удалил из друзей.
            M_Notification newFriendNotification = new M_Notification
            {
                ID = GetNextIDNum(friend),
                Code = NotificationCode.FriendRemoved,
                DateAndTime = DateTime.Now.ToString("MM.dd HH:mm"),
                UserName = VM_CurrentUser.Instance.Name
            };
            lock (friend.Notifications)
            {
                friend.Notifications.Add(newFriendNotification);
            }
            UpdateUser(friend);


            return Task.CompletedTask;
        }

        // Я ИЗМЕНЯЮ ФОТО СВОЕГО ПРОФИЛЯ И ПОДТВЕРЖДАЮ ЭТО КНОПКОЙ 
        private Task OnImChangedPhotoEvent(string newPathPhoto)
        {
            // 1. Изменить в БД путь к моему изображению
            me.Image = newPathPhoto;
            UpdateUser(me);
            return Task.CompletedTask;
        }

        // Я ИЗМЕНЯЮ ЦВЕТ 
        private Task OnImChangedColorEvent(string newColor)
        {
            // 1. Изменить в БД мой цвет фона
            me.Color = newColor;
            UpdateUser(me);
            return Task.CompletedTask;
        }


        public M_User GetUser(string username)
        {
            DatabaseUser databaseUser = GetDbUser(username);
            M_User user = new M_User();
            if (databaseUser != null)
            {
                // вернуть данные пользователя, с таким именем
                user._image = databaseUser.Image;
                user._name = databaseUser.Username;
                user._color = databaseUser.Color;

                // Статус не надо инициализировать
                //user._status = UserStatus.RegularUser;
            }
            return user;
        }

        public List<M_User> GetСoincidences(string seacrch, List<string> alreadyExists)
        {
            List<M_User> users = new List<M_User>();
            Dictionary<string, DatabaseUser> allDbUsers = GetUsers();

            // Проходим по каждому пользователю
            foreach (var user in allDbUsers.Values)
            {
                if (user.Username == me.Username)
                {
                    continue;
                }
                if (!alreadyExists.Contains(user.Username))
                {
                    if (user.Username.Contains(seacrch, StringComparison.OrdinalIgnoreCase))
                    {
                        M_User newUser = new M_User();
                        newUser._name = user.Username;
                        newUser._color = user.Color;
                        newUser._image = user.Image;
                        newUser._status = UserStatus.RegularUser;

                        users.Add(newUser);
                    }
                }
            }
            return users;
        }

        
        public class NewsDB : M_OfficialNew
        {
            public string shortDescription {  get; set; }
        } 

        // NEWS
        public M_OfficialNewDetails GetDetailsNew(VM_OfficialNew News)
        {
            try
            {
                FirebaseResponse response = dbConnection.client.Get("news/" + News.Title);
                if (response.Body == "null") return null;

                NewsDB detailNewsDb = JsonConvert.DeserializeObject<NewsDB>(response.Body);

                M_OfficialNewDetails detail = new M_OfficialNewDetails
                {
                    Title = detailNewsDb.Title,
                    Date = detailNewsDb.Date,
                    Image = detailNewsDb.Image,
                    Description = detailNewsDb.Description,
                };

                return detail;
            }
            catch { }
            return null;
        }
        public List<M_OfficialNew> GetOfficialNews()
        {
            try
            {
                FirebaseResponse response = dbConnection.client.Get("news/");
                if (response.Body == "null") return null;

                Dictionary<string, NewsDB> newsDbDictionary = JsonConvert.DeserializeObject<Dictionary<string, NewsDB>>(response.Body);

                // Преобразование словаря в список
                //List<NewsDB> newsDbList = new List<NewsDB>(newsDbDictionary.Values);


                // Удаление LongDescription
                List<M_OfficialNew> shortNews = new List<M_OfficialNew>();
                foreach (var newDb in newsDbDictionary.Values)
                {
                    M_OfficialNew newShortNew = new M_OfficialNew
                    {
                        Title = newDb.Title,
                        Date = newDb.Date,
                        Image = newDb.Image,
                        Description = newDb.shortDescription,
                    };
                    shortNews.Add(newShortNew);
                }


                // Сортировка списка по полю Date от старых к новым
                List<M_OfficialNew> sortedNews = shortNews.OrderBy(news => news.Date).ToList();

                return sortedNews;
            }
            catch { }
            return null;
        }
    }
}
