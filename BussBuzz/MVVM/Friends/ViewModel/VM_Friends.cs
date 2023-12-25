using AutoFixture;
using BussBuzz.Base;
using BussBuzz.MVVM.Notification.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
namespace BussBuzz.MVVM.Friends.ViewModel
{
    public partial class VM_Friends: ObservableValidator, VM_Base
    {
        private static VM_Friends _instance;
        public static VM_Friends Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VM_Friends();
                return _instance;
            }
        }
        private readonly ValidationService _VALIDATION_SERVICE;
        private VM_Friends()
        {
            _VALIDATION_SERVICE = ValidationService.Instance;

            SearchComamnd = new AsyncRelayCommand(Search);
            AddFriendCommand = new AsyncRelayCommand<VM_FriendItem>(async (friend) => await AddFriend(friend));
            RemoveFriendCommand = new AsyncRelayCommand<VM_FriendItem>(async (friend) => await RemoveFriend(friend));

            EventService.FrRequestAddFriendEvent += UpdateStateUser;
            EventService.FrRemoveFriendEvent += UpdateStateUser;
            EventService.FrConfirmAddFriendEvent += UpdateStateUser;
            EventService.FrCancelAddFriendEvent += UpdateStateUser;

            EventService.ImRequestAddFriendEvent += EventService_ImRequestAddFriendEvent;
            EventService.ImConfirmAddFriendEvent += UpdateStateUser;
            EventService.ImRemoveFriendEvent += UpdateStateUser;
            EventService.ImRequestAddFriendEvent += UpdateStateUser;

            EventService.ImHasLeftAppEvent += Dispose;
            EventService.ImEnterAppEvent += Init;
            Init(); // костыль
        }
        private Task Init()
        {
            GetOnlyFriend();
            return Task.CompletedTask;
        }

        private Task Dispose()
        {
            lock (DisplayedCollectionUser)
            {
                DisplayedCollectionUser.Clear();
            }
            return Task.CompletedTask;
        }
        private Task EventService_ImRequestAddFriendEvent(string friendname)
        {
            try
            {
                lock (DisplayedCollectionUser)
                {
                    VM_FriendItem user = DisplayedCollectionUser.First(x => x.User.Name == friendname);
                    if (user != null)
                    {
                        int index = DisplayedCollectionUser.IndexOf(user);
                        DisplayedCollectionUser.RemoveAt(index);
                        lock (FriendCollection.Instance.ConnectedUsers)
                            DisplayedCollectionUser.Insert(index, new VM_FriendItem(FriendCollection.Instance.ConnectedUsers.First(x => x.Name == friendname)));
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task GetOnlyFriend()
        {
            try
            {
                lock (FriendCollection.Instance.ConnectedUsers)
                {
                    lock (DisplayedCollectionUser)
                    {
                        DisplayedCollectionUser.Clear();
                        foreach (VM_FriendItem user in FriendCollection.Instance.ConnectedUsers
                            .Where(u => u.Status == UserStatus.Friend)
                            .Select(u => new VM_FriendItem(u)))
                            DisplayedCollectionUser.Add(user);
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        public ObservableCollection<VM_FriendItem> DisplayedCollectionUser { get; } = new();
        private Task UpdateStateUser(string friendname)
        {
            try
            {
                lock (DisplayedCollectionUser)
                {
                    VM_FriendItem user = DisplayedCollectionUser.First(x => x.User.Name == friendname);
                    if (user != null)
                    {
                        int index = DisplayedCollectionUser.IndexOf(user);
                        DisplayedCollectionUser.RemoveAt(index);
                        lock(FriendCollection.Instance.ConnectedUsers)
                            DisplayedCollectionUser.Insert(index, new VM_FriendItem(FriendCollection.Instance.ConnectedUsers.First(x => x.Name == friendname)));
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }
        private Task UpdateStateUser(VM_Notification friendname)
        {
            try
            {
                lock (DisplayedCollectionUser)
                {
                    DisplayedCollectionUser.Clear();
                    VM_FriendItem user = DisplayedCollectionUser.First(x => x.User.Name == friendname.User.Name);
                    if (user != null)
                    {
                        int index = DisplayedCollectionUser.IndexOf(user);
                        DisplayedCollectionUser.RemoveAt(index);
                        lock (FriendCollection.Instance.ConnectedUsers)
                            DisplayedCollectionUser.Insert(index, new VM_FriendItem(FriendCollection.Instance.ConnectedUsers.First(x => x.Name == friendname.User.Name)));
                    }
                }
            }
            catch { }
            return Task.CompletedTask;
        }


        // SEARCHED NAME
        public string _searchedName;
        [CustomValidation(typeof(VM_Friends), nameof(ValidateName))]
        public string SearchedName
        {
            get
            {
                if (_searchedName == null)
                    _searchedName = string.Empty;
                return _searchedName;
            }
            set => SetProperty(ref _searchedName, value);
        }
        public static ValidationResult ValidateName(string name, ValidationContext context)
        {
            try
            {
                Instance._VALIDATION_SERVICE.ValidateLoginSearched(name);
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new(ex.Message);
            }
        }

        // SEARCHED COMMAND
        public IAsyncRelayCommand SearchComamnd { get; }
        public async Task Search()
        {
            try
            {
                ValidateAllProperties();
                if (HasErrors)
                {
                    // УВЕДОМЛЕНИЕ ОБ ОШИБКЕ
                    string message = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
                    throw new Exception(message);
                }
                else
                {
                    // если поле пустое, получаем только друзей
                    if (string.IsNullOrWhiteSpace(SearchedName))
                    {
                        await GetOnlyFriend();
                    }
                    // если нет, получаем сначала из списка друзей, потом идем в БД
                    else
                    {
                        lock (DisplayedCollectionUser)
                        {
                            DisplayedCollectionUser.Clear();
                            // получаем список тех, с кем у нас есть связи
                            List<VM_User> us = FriendCollection.Instance.ConnectedUsers.Where(u => u.Name.Contains(SearchedName, StringComparison.OrdinalIgnoreCase)).ToList();
                            foreach (VM_User user in us)
                            {
                                DisplayedCollectionUser.Add(new VM_FriendItem(user));
                            }
                        }
                        // получаю новый список других людей
                        List<VM_User> users = 
                            DatabaseService.Instance.GetСoincidences(SearchedName,          // передаю паттерн
                            DisplayedCollectionUser.Select(x => x.User.Name).ToList())      // передаю список людей, которые уже есть
                            .Select(x => new VM_User(x)).ToList();                          // преобразую полученный список
                        lock (DisplayedCollectionUser)
                        {
                            // получаем остальную часть списка
                            foreach (VM_User user in users)
                            {
                                DisplayedCollectionUser.Add(new VM_FriendItem(user));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
            }
        }

        // ДОБАВЛЕНИЯ В ДРУЗЬЯ 
        public IAsyncRelayCommand AddFriendCommand { get; }
        private async Task AddFriend(VM_FriendItem friend)
        {
            try
            {
                EventService.RaiseImRequestAddFriendEventAsync(friend.User.Name);
            }
            catch (Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
            }
        }

        // УДАЛЕНИЕ ИЗ ДРУЗЕЙ
        public IAsyncRelayCommand RemoveFriendCommand { get; }
        private async Task RemoveFriend(VM_FriendItem friend) 
        {
            try
            {
                EventService.RaiseImRemoveFriendEventAsync(friend.User.Name);
            }
            catch(Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
            }
        }

        [ObservableProperty]
        private bool isRefreshing;

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                await GetOnlyFriend();
                SearchedName = "";
                IsRefreshing = false;
            }
            catch { }
        }
    }
}
