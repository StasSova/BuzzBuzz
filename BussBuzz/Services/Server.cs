using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using BussBuzz.Base;
using BussBuzz.MVVM.MAP.ViewModell;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System;
using BussBuzz.MVVM.Menu.ViewModel;
using BussBuzz.MVVM.Friends.ViewModel;
using BussBuzz.MVVM.Notification.Model;
using BussBuzz.MVVM.Notification.ViewModel;

namespace BussBuzz.Services
{
    public class TCPClient
    {
        public TCPClient() 
        {
            EventService.ImLocationChangedEvent += SendMessagaLocationChangeCurrentUser;
            EventService.ImChangedColorEvent += SendMessageChangedColor;
            EventService.ImChangedPhotoEvent += SendMessageImChangedPhoto;
            EventService.ImHasLeftAppEvent += SendMessageImHasLeftApp;
            EventService.ImRequestAddFriendEvent += SendMessageImRequestAddFriend;
            EventService.ImRemoveFriendEvent += SendMessageImRemoveFriend;
            EventService.ImCancelAddFriendEvent += SendMessageImCancelAddFriend;
            EventService.ImConfirmAddFriendEvent += SendMessageImConfirmAddFriend;
        }
        TcpClient client;

        string serverIP = "YOUR_KEY";
        int serverPort = -1;
        private static TCPClient _instance;
        public static TCPClient Instance
        {
            get
            {
                _instance ??= new TCPClient();
                return _instance;
            }
        }
   
        [Serializable]
        [JsonObject]
        public class User
        {
            [JsonProperty(nameof(Name))]
            public string Name { get; set; }
            [JsonProperty(nameof(Cords))]
            public string Cords { get; set; }
            [JsonProperty(nameof(Friends))]
            public List<string> Friends { get; set; }

            [JsonProperty(nameof(Code))]
            public int Code {  get; set; }

            // Добавьте конструктор без параметров, который может быть использован для десериализации
            public User()
            {
            }

            public User(string name, string cords, List<string> friends, int code)
            {
                Name = name;
                Cords = cords;
                Friends = friends;
                Code = code;
            }
        }

        // Я ЗАХОЖУ В ПРИЛОЖЕНИЕ
        public async Task Start()
        {
            client= new TcpClient();
            try
            {
                // дожидаюсь подключение к Серверу
                await client.ConnectAsync(serverIP, serverPort);
                if (client.Connected)
                {
                    var dataToSend = new User(VM_CurrentUser.Instance.Name, null,
                        FriendCollection.Instance.ConnectedUsers.Select(x => x.Name).ToList(), 0);

                    NetworkStream stream = client.GetStream();

                    // Serialize the user to JSON
                    var json = JsonConvert.SerializeObject(dataToSend);
                    byte[] data = Encoding.UTF8.GetBytes(json);

                    await stream.WriteAsync(data);
                    //await stream.WriteAsync(data, 0, data.Length);

                    // Start a separate task to receive and display messages from the server
                    Task receiveTask = ReceiveMessagesAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        // ПЕРЕИМЕНОВАТЬ КЛАСС USER НА MESSAGE (ДЛЯ МЕНЬШЕГО ЗАПУТЫВАНИЯ)
        /*
         * СООБЩЕНИЯ ПОЛНОСТЬЮ ГЕНЕРИРУЮТСЯ ПРИ ОТПРАВКЕ.
         */ 
        public async Task SendData(User message)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    //var user = new User(Name, location, friends,code);
                    var json = JsonConvert.SerializeObject(message);
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    //await stream.WriteAsync(data, 0, data.Length);
                    await stream.WriteAsync(data);
                    Console.WriteLine("Sent the User to the server");
                }
                else
                {
                    Console.WriteLine("Client is not connected or has been disposed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending data: " + ex.Message);
            }
        }

        // ПОЛУЧЕНИЯ СООБЩЕНИЙ С СЕРВЕРА
        public async Task ReceiveMessagesAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                if (client.Connected)
                {
                    try
                    {
                        // считываем байтовый поток.
                        int bytesRead = await stream.ReadAsync(buffer);
                        if (bytesRead == 0)
                            client.Close(); 
                        // преобразаем байтовый поток в строку.
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Split the string into individual JSON objects
                        string[] responseArray = response.Split(new[] { "{", "}{", "}" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string responseObject in responseArray)
                        {
                            try
                            {
                                // форматируем в json формат для дессериалаизации объекта.
                                string formattedObject = $"{{{responseObject}}}";
                                User reciveMessage = JsonConvert.DeserializeObject<User>(formattedObject);
                                // обрабатываем сообщение.
                                switch (reciveMessage.Code)
                                {
                                    // ИЗМЕНЕНИЕ ЛОКАЦИИ ДРУГА - КОД 0
                                    case 0:
                                        {
                                            var str = reciveMessage.Cords.Split("|");       // разбиение координат через "|"
                                            double Latitude = Convert.ToDouble(str[0]);     // получение широты
                                            double Longitude = Convert.ToDouble(str[1]);    // получение долготы
                                            EventService.RaiseFrLocationChangedEvent(reciveMessage.Name, new Location(Latitude, Longitude));
                                            break;
                                        }
                                    // ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА - КОД 1
                                    case 1:
                                        {
                                            // передаю имя друга, который хочет мне отправить запрос в друзья
                                            _ = EventService.RaiseFrRequestAddFriendEventAsync(reciveMessage.Name);
                                            break;
                                        }
                                    // ОТКЛОНЕНИЯ ЗАПРОСА НА ДОБАВЛЕНИЯ В ДРУЗЬЯ - КОД 2
                                    case 2:
                                        {
                                            // передаю друга, который отклонил запрос на добавления в друзья.
                                            EventService.RaiseFrCancelAddFriendEventAsync(reciveMessage.Name);
                                            break;
                                        }
                                    // ПОДТВЕРЖДЕНИЕ ЗАПРОСА НА ДОБАВЛЕНИЯ В ДРУЗЬЯ - КОД 3
                                    case 3:
                                        {
                                            // передаю друга, который принял запрос на добавления в друзья.
                                            EventService.RaiseFrConfirmAddFriendEventAsync(reciveMessage.Name);
                                            break;
                                        }
                                    // УДАЛЕНИЕ ДРУГА - КОД 4
                                    case 4:
                                        {
                                            // передаю имя друга, который меня удалил.
                                            EventService.RaiseFrRemoveFriendEventAsync(reciveMessage.Name);
                                            break;
                                        }
                                    // ИЗМЕНЕНИЕ ФОТО ДРУГА - КОД 5
                                    case 5:
                                        {
                                            // reciveMessage.Cords - содержит путь к картинке
                                            EventService.RaiseFrChangedPhotoEventAsync(reciveMessage.Name, reciveMessage.Cords);
                                            break;
                                        }
                                    // ИЗМЕНЕНИЯ ЦВЕТА ДРУГА - КОД 6
                                    case 6:
                                        {
                                            // reciveMessage.Cords - содержит цвет
                                            EventService.RaiseFrChangedColorEventAsync(reciveMessage.Name, reciveMessage.Cords);
                                            break;
                                        }
                                    // ДРУГ ВЫХОДИТ ИЗ ПРИЛОЖЕНИЯ - КОД 7
                                    case 7:
                                        {
                                            EventService.RaiseFrFriendHasLeftAppEvent(reciveMessage.Name);
                                            break;
                                        }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch
                    {
                        // Handle exceptions as needed
                    }
                }
                else
                {
                    try
                    {
                        await client.ConnectAsync(serverIP, serverPort);
                    }
                    catch { break; }
                }
            }
        }



        // ПОДПИСКА НА СОБЫТИЯ
        // Я ИЗМЕНЯЮ ЛОКАЦИЮ - КОД 0
        private async Task SendMessagaLocationChangeCurrentUser(Location location)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 0,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = location.Latitude.ToString() + "|" + location.Longitude.ToString()
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 0,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = location.Latitude.ToString() + "|" + location.Longitude.ToString()
                });
            }
        }
        
        //Я ОТПРАВЛЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА - КОД 1
        private async Task SendMessageImRequestAddFriend(string FriendName)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 1,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { FriendName },
                    Cords = null
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 1,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { FriendName },
                    Cords = null
                });
            }
        }

        //Я ОТКЛОНЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 2
        private async Task SendMessageImCancelAddFriend(VM_Notification Notification)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 2,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { Notification.User.Name },
                    Cords = null
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 2,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { Notification.User.Name },
                    Cords = null
                });
            }
        }

        // Я ПОДТВЕРЖДАЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 3 
        private async Task SendMessageImConfirmAddFriend(VM_Notification Notification)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 3,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { Notification.User.Name },
                    Cords = null
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 3,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { Notification.User.Name },
                    Cords = null
                });
            }
        }
        // Я ХОЧУ УДАЛИТЬ ДРУГА ИЗ ДРУЗЕЙ - КОД 4
        
        private async Task SendMessageImRemoveFriend(string FriendName)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 4,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { FriendName },
                    Cords = null
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 4,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = new List<string>() { FriendName },
                    Cords = null
                });
            }
        }
        // Я ИЗМЕНЯЮ ФОТО СВОЕГО ПРОФИЛЯ И ПОДТВЕРЖДАЮ ЭТО КНОПКОЙ - КОД 5
        private async Task SendMessageImChangedPhoto(string pathToPhoto)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 5,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = pathToPhoto
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 5,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = pathToPhoto
                });
            }
        }
        // Я ИЗМЕНЯЮ ЦВЕТ - КОД 6
        private async Task SendMessageChangedColor(string color)
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 6,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = color
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 6,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = color
                });
            }
        }
        // Я ВЫХОЖУ ИЗ ПРИЛОЖЕНИЯ - КОД 7
        private async Task SendMessageImHasLeftApp()
        {
            if (client.Connected)
            {
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 7,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = null
                });
            }
            else
            {
                await TCPClient.Instance.Start();
                _ = TCPClient.Instance.SendData(new TCPClient.User
                {
                    Code = 7,
                    Name = VM_CurrentUser.Instance.Name,
                    Friends = null,
                    Cords = null
                });
            }
        }
    }
}
