using BussBuzz.Base;
using BussBuzz.MVVM.Notification.Model;
using BussBuzz.MVVM.Notification.ViewModel;
using Material.Components.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public class EventService
    {
        // ИЗМЕНЕНИЕ ЛОКАЦИИ ДРУГА - КОД 0
        /*
         * 1. Мой друг переместился.
         * 2. Мне приходит сообщение с тем, что мой друг переместился
         * 3. Срабатывает данное событие. Данное событие вызывается в классе VM_Map UpdateLocation
         * 4. Я получаю имя друга, и его обновленное местоположение
         */
        public delegate Task FrLocationChangedEventHandler(string username, Location location);
        public static event FrLocationChangedEventHandler FrLocationChangedEvent;
        public static void RaiseFrLocationChangedEvent(string username, Location location)
        {
            try
            {
                FrLocationChangedEvent?.Invoke(username, location);
            }
            catch { }
        }


        // ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА - КОД 1
        /*
         * 1. Кто-то хочет добавить меня в друзья.
         * 2. Нажимает на кнопку "+" возле моей иконки.
         * 3. Мне приходит сообщение о том, что меня хотят добавить в друзья.
         * 3.1. В параметрах мне приходит: имя человека, который хочет добавить меня в друзья.
         * 4. Я в главном меню, в разделе Notification должен добавить это уведомление.
         */
        public delegate Task FrRequestAddFriendEventHandler(string username);
        public static event FrRequestAddFriendEventHandler FrRequestAddFriendEvent;
        public static async Task RaiseFrRequestAddFriendEventAsync(string username)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrRequestAddFriendEvent;

                // Создаем список задач
                var tasks = new List<Task>();

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrRequestAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Если есть метод из FriendCollection, выполняем его
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username));
                }




                // Вызываем все остальные обработчики, кроме метода из FriendCollection
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username)));
                    }
                }

                // Дожидаемся завершения всех задач, кроме метода из FriendCollection
                //await Task.WhenAll(tasks);
            }
            catch { }
        }

        // ОТКЛОНЕНИЯ ЗАПРОСА НА ДОБАВЛЕНИЯ В ДРУЗЬЯ - КОД 2
        /*
         * 1. Друг заходит в уведомления и видит запрос на добавления в друзья.
         * 2. Нажимает на крестик. Посылает сообщения на клиент-сервер, мне, результат запроса.
         * 3. Мне приходит отрицательный ответ.
         * 3.1 В параметрах я получаю имя друга, который отказался со мной дружить.
         * 4. Я должен добавить уведомления в Notification о том, что друг отклонил мой запрос.
         * 4.1. Возможно я просто изменяю кол-во новых уведомлений, а сами уведомления подгружаются, когда я перехожу на вкладку
         *      с уведомлениями. 
         */
        public delegate Task FrCancelAddFriendEventHandler(string username);
        public static event FrCancelAddFriendEventHandler FrCancelAddFriendEvent;
        public static async void RaiseFrCancelAddFriendEventAsync(string username)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrCancelAddFriendEvent;

                // Создаем список задач
                var tasks = new List<Task>();

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrCancelAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));


                // Если есть метод из FriendCollection, выполняем его
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username));
                }


                // Вызываем все остальные обработчики, кроме метода из FriendCollection
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username)));
                    }
                }

                //// Дожидаемся завершения всех задач, кроме метода из FriendCollection
                //await Task.WhenAll(tasks);
            }
            catch { }
        }

        // ПОДТВЕРЖДЕНИЕ ЗАПРОСА НА ДОБАВЛЕНИЯ В ДРУЗЬЯ - КОД 3
        /*
         * 1. Друг заходит в уведомления и видит запрос на добавления в друзья.
         * 2. Нажимает на плюсик. Посылает сообщения на клиент-сервер, мне, результат запроса.
         * 3. Мне приходит положительный ответ.
         * 3.1. В параметрах я получаю имя друга, который подтвердил дружбу со мной.
         * 4. Я должен добавить уведомления в Notification о том, что друг принял мой запрос.
         * 4.1. Возможно я просто изменяю кол-во новых уведомлений, а сами уведомления подгружаются, когда я перехожу на вкладку
         *      с уведомлениями.
         */
        public delegate Task FrConfirmAddFriendEventHandler(string username);
        public static event FrConfirmAddFriendEventHandler FrConfirmAddFriendEvent;
        public static async void RaiseFrConfirmAddFriendEventAsync(string username)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrConfirmAddFriendEvent;

                // Создаем список задач
                var tasks = new List<Task>();

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrConfirmAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Если есть метод из FriendCollection, выполняем его
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username));
                }

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var database = handlers?.GetInvocationList().OfType<FrConfirmAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Если есть метод из FriendCollection, выполняем его
                if (database != null)
                {
                    await Task.Run(() => database(username));
                }

                // Вызываем все остальные обработчики, кроме метода из FriendCollection
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != database))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username)));
                    }
                }

                // Дожидаемся завершения всех задач, кроме метода из FriendCollection
                await Task.WhenAll(tasks);
            }
            catch { }
        }



        // УДАЛЕНИЕ ДРУГА - КОД 4
        /*
         * 1. Мой друг удаляет меня из друзей.
         * 2. Сообщение идет ко мне.
         * 3. Срабатывает данное событие.
         * 4. Я должен удалить маркер данного друга из карты.
         * 5. Сделать в главном меню в разделе Notification сообщение о том, что друг мне больше не друг
         */
        public delegate Task FrRemoveFriendEventHandler(string username);
        public static event FrRemoveFriendEventHandler FrRemoveFriendEvent;
        public static async void RaiseFrRemoveFriendEventAsync(string username)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrRemoveFriendEvent;

                // Создаем список задач
                var tasks = new List<Task>();

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrRemoveFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Если есть метод из FriendCollection, выполняем его
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username));
                }

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var databaseHandler = handlers?.GetInvocationList().OfType<FrRemoveFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Если есть метод из FriendCollection, выполняем его
                if (databaseHandler != null)
                {
                    await Task.Run(() => databaseHandler(username));
                }


                // Вызываем все остальные обработчики, кроме метода из FriendCollection
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != databaseHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username)));
                    }
                }
                // Дожидаемся завершения всех задач, кроме метода из FriendCollection
                await Task.WhenAll(tasks);
            }
            catch { }
        }



        // ИЗМЕНЕНИЕ ФОТО ДРУГА - КОД 5
        /*
         * 1. Друг изменяет свою фотографию.
         * 2. Мне приходит уведомление о том, что фото измененно и срабатывает данное событие.
         * 3. Я должен изменить фото на маркере на карте на соответсвующую картинку.
         * 
         * 4. (ПЕРЕДЕЛАТЬ)Чат инициализируется при входе на соответствующую вкладку с БД. 
         *    Инициализируется следующее: Имя пользователя с кем я веду переписку
         *    его картинка, его фон, последнее сообщение с беседы. При клике, подгружается с БД остальные сообщения. При закрытии они должны удаляться чтобы не занимать память.
         *
         * 5. Изменение картинки у чата
         * 6. Должно сгенерироваться уведомление о том что друг изменил свою картинку.
         * 6.1. Возможно изменить кол-во уведомлений, сами уведомления подгружаются при переходе на соответствующую вкладку.
         */
        public delegate Task FrChangedPhotoEventHandler(string username, string pathPhoto);
        public static event FrChangedPhotoEventHandler FrChangedPhotoEvent;
        public static async void RaiseFrChangedPhotoEventAsync(string username, string path)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrChangedPhotoEvent;

                // Находим метод из FriendCollection
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrChangedPhotoEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Вызываем метод из FriendCollection асинхронно с ожиданием завершения
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username, path));
                }

                // Находим метод из FriendCollection
                var DatabaseHandler = handlers?.GetInvocationList().OfType<FrChangedPhotoEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Вызываем метод из FriendCollection асинхронно с ожиданием завершения
                if (DatabaseHandler != null)
                {
                    await Task.Run(() => DatabaseHandler(username, path));
                }

                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != DatabaseHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username, path)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        // ИЗМЕНЕНИЯ ЦВЕТА ДРУГА - КОД 6
        /*
         * 1. Друг изменяет свой цвет.
         * 2. Мне приходит код сообщения о том, что друг изменил фото, и срабатывает данное событие.
         * 3. Я должен изменить цвет друга во вкладке сообщения. (в БД уже цвет изменился)
         * 4. Должно сгенерироваться уведомление о том что друг изменил свой цвет.
         * 5. В БД должно добавиться уведомление.
         */
        public delegate Task FrChangedColorEventHandler(string username, string color);
        public static event FrChangedColorEventHandler FrChangedColorEvent;
        public static async void RaiseFrChangedColorEventAsync(string username, string color)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = FrChangedColorEvent;

                // Находим метод из FriendCollection
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<FrChangedColorEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));


                // Вызываем метод из FriendCollection и ожидаем его завершения
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(username, color));
                }

                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(username, color)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }
        
        // ДРУГ ВЫХОДИТ ИЗ ПРИЛОЖЕНИЯ - КОД 7
        /*
         * 1. Пользователь закрывает или выходит из приложения
         * 2. Сервер посылает мне сообщение с кодом и именем человека, который вышел.
         * 3. Я должен удалить метку данного пользователя на карте либо отметить ее как отсутсвующего (изменить картинку).
         */
        public delegate Task FriendHasLeftAppEventHandler(string username);
        public static event FriendHasLeftAppEventHandler FrFriendHasLeftAppEvent;
        public static void RaiseFrFriendHasLeftAppEvent(string username)
        {
            try
            {
                FrFriendHasLeftAppEvent?.Invoke(username);
            }
            catch { }
        }



        // ИЗМЕНЕНИЕ МЕСТОПОЛОЖЕНИЯ ПОЛЬЗОВАТЕЛЯ - КОД 0
        public delegate Task ImLocationChangedEventHandler(Location location);
        public static event ImLocationChangedEventHandler ImLocationChangedEvent;
        public static void RaiseImLocationChangedEvent(Location location)
        {
            try
            {
                ImLocationChangedEvent?.Invoke(location);
            }
            catch { }
        }

        
        //Я ОТПРАВЛЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ ДРУГА - КОД 1
        public delegate Task ImRequestAddFriendEventHandler(string friendname);
        public static event ImRequestAddFriendEventHandler ImRequestAddFriendEvent;
        public static async void RaiseImRequestAddFriendEventAsync(string friendname)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImRequestAddFriendEvent;

                // Находим метод из FriendCollection
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<ImRequestAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Вызываем метод из FriendCollection асинхронно с ожиданием
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(friendname));
                }

                // Находим метод из FriendCollection
                var databaseHandler = handlers?.GetInvocationList().OfType<ImRequestAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Вызываем метод из FriendCollection асинхронно с ожиданием
                if (databaseHandler != null)
                {
                    await Task.Run(() => databaseHandler(friendname));
                }


                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != databaseHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(friendname)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        //Я ОТКЛОНЯЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 2
        public delegate Task ImCancelAddFriendEventHandler(VM_Notification Notification);
        public static event ImCancelAddFriendEventHandler ImCancelAddFriendEvent;
        public static async void RaiseImCancelAddFriendEventAsync(VM_Notification notification)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImCancelAddFriendEvent;

                // Находим метод из FriendCollection
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<ImCancelAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Вызываем метод из FriendCollection асинхронно без ожидания
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(notification));
                }

                // Находим метод из FriendCollection
                var databaseHandler = handlers?.GetInvocationList().OfType<ImCancelAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Вызываем метод из FriendCollection асинхронно без ожидания
                if (databaseHandler != null)
                {
                    await Task.Run(() => databaseHandler(notification));
                }


                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != databaseHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(notification)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        //Я ПОДТВЕРЖДАЮ ЗАПРОС НА ДОБАВЛЕНИЕ В ДРУЗЬЯ - КОД 3 
        public delegate Task ImConfirmAddFriendEventHandler(VM_Notification Notification);
        public static event ImConfirmAddFriendEventHandler ImConfirmAddFriendEvent;
        public static async void RaiseImConfirmAddFriendEventAsync(VM_Notification notification)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImConfirmAddFriendEvent;

                // Находим метод из FriendCollection
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<ImConfirmAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Вызываем метод из FriendCollection асинхронно без ожидания
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(notification));
                }

                // Находим метод из FriendCollection
                var databaseHandler = handlers?.GetInvocationList().OfType<ImConfirmAddFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Вызываем метод из FriendCollection асинхронно без ожидания
                if (databaseHandler != null)
                {
                    await Task.Run(() => databaseHandler(notification));
                }


                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != databaseHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(notification)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        
        //Я ОЗНАКОМИЛСЯ С УВЕДОМЛЕНИЕ - КОД 8 
        public delegate Task ImReadTheNotifyEventHandler(VM_Notification Notification);
        public static event ImReadTheNotifyEventHandler ImReadTheNotifyEvent;
        public static void RaiseImReadTheNotifyEvent(VM_Notification Notification)
        {
            try
            {
                ImReadTheNotifyEvent?.Invoke(Notification);
            }
            catch { }
        }


        // Я ХОЧУ УДАЛИТЬ ДРУГА ИЗ ДРУЗЕЙ - КОД 4
        public delegate Task ImRemoveFriendEventHandler(string friendname);
        public static event ImRemoveFriendEventHandler ImRemoveFriendEvent;
        public static async void RaiseImRemoveFriendEventAsync(string friendname)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImRemoveFriendEvent;

                // Создаем список задач
                var tasks = new List<Task>();

                // Если есть метод из FriendCollection, добавляем его задачу в список
                var friendCollectionHandler = handlers?.GetInvocationList().OfType<ImRemoveFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(FriendCollection));

                // Если есть метод из FriendCollection, выполняем его
                if (friendCollectionHandler != null)
                {
                    await Task.Run(() => friendCollectionHandler(friendname));
                }

                
                // Если есть метод из FriendCollection, добавляем его задачу в список
                var dbHandler = handlers?.GetInvocationList().OfType<ImRemoveFriendEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(DatabaseService));

                // Если есть метод из FriendCollection, выполняем его
                if (dbHandler != null)
                {
                    await Task.Run(() => dbHandler(friendname));
                }


                // Вызываем все остальные обработчики, кроме метода из FriendCollection
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != friendCollectionHandler && handler != dbHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(friendname)));
                    }
                }
            }
            catch { }
        }



        // Я ИЗМЕНЯЮ ФОТО СВОЕГО ПРОФИЛЯ И ПОДТВЕРЖДАЮ ЭТО КНОПКОЙ - КОД 5
        public delegate Task ImChangedPhotoEventHandler(string pathPhoto);
        public static event ImChangedPhotoEventHandler ImChangedPhotoEvent;
        public static async void RaiseImChangedPhotoEventAsync(string path)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImChangedPhotoEvent;

                // Находим метод из VM_CurrentUser
                var vmCurrentUserHandler = handlers?.GetInvocationList().OfType<ImChangedPhotoEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(VM_CurrentUser));

                // Вызываем метод из VM_CurrentUser асинхронно с ожиданием
                if (vmCurrentUserHandler != null)
                {
                    await Task.Run(() => vmCurrentUserHandler(path));
                }

                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != vmCurrentUserHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(path)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }

        // Я ИЗМЕНЯЮ ЦВЕТ - КОД 6
        public delegate Task ImChangedColorEventHandler(string color);
        public static event ImChangedColorEventHandler ImChangedColorEvent;
        public static async void RaiseImChangedColorEventAsync(string color)
        {
            try
            {
                // Получаем все обработчики события
                var handlers = ImChangedColorEvent;

                // Находим метод из VM_CurrentUser
                var vmCurrentUserHandler = handlers?.GetInvocationList().OfType<ImChangedColorEventHandler>()
                    .FirstOrDefault(handler => handler.Method.DeclaringType == typeof(VM_CurrentUser));

                // Вызываем метод из VM_CurrentUser асинхронно с ожиданием
                if (vmCurrentUserHandler != null)
                {
                    await Task.Run(() => vmCurrentUserHandler(color));
                }

                // Создаем список задач
                var tasks = new List<Task>();

                // Вызываем все остальные обработчики параллельно
                if (handlers != null)
                {
                    foreach (var handler in handlers.GetInvocationList().Where(handler => handler != vmCurrentUserHandler))
                    {
                        // Запускаем задачу параллельно и добавляем её в список задач
                        tasks.Add(Task.Run(() => handler.DynamicInvoke(color)));
                    }
                }

                // Дожидаемся завершения всех задач
                await Task.WhenAll(tasks);
            }
            catch { }
        }


        // Я ВЫХОЖУ ИЗ ПРИЛОЖЕНИЯ - КОД 7
        public delegate Task ImHasLeftAppEventHandler();
        public static event ImHasLeftAppEventHandler ImHasLeftAppEvent;
        public static void RaiseImHasLeftAppEvent()
        {
            try
            {
                ImHasLeftAppEvent?.Invoke();
            }
            catch { }
        }

        // Я ВХОЖУ В ПРИЛОЖЕНИЯ - КОД 8
        public delegate Task ImEnterAppEventHandler();
        public static event ImEnterAppEventHandler ImEnterAppEvent;
        public static void RaiseImEnterAppEvent()
        {
            try
            {
                ImEnterAppEvent?.Invoke();
            }
            catch { }
        }

    }
}
