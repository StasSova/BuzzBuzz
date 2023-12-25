using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Base
{
    // код, который используется для уведомлений
    public enum NotificationCode
    {
        // Запросы на добавление в друзья
        FriendRequestReceived = 0,

        // Уведомление о том, что друг удален из друзей
        FriendRemoved = 5,

        // Уведомление о том, что друг принял запрос в друзья
        FriendRequestAccepted = 10,

        // Уведомление о смене фото другом
        FriendChangedPhoto = 15,

        // Уведомление о смене цвета другом
        FriendChangedColor = 20,

        // Отклонение запроса на добавление в друзья
        FriendRequestRejected = 25,

        // Пустое
        Empty = 30,
    }

    public enum UserStatus
    {
        // Текущий пользователь
        CurrentUser = 0,

        // Друг
        Friend = 1,

        // Человек, отправивший запрос в друзья
        FriendRequestSender = 2,

        // Человек, которому отправлен запрос в друзья
        FriendRequestRecipient = 3,

        // Обычный пользователь (не друг)
        RegularUser = 4,

        // Пользователь из черного списка
        BlockedUser = 5,
    }

}
