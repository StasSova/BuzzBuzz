using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussBuzz.Base;
using BussBuzz.MVVM.Authorization.ViewModel;
using BussBuzz.MVVM.Authorization;
using BussBuzz.MVVM.ResetPassword.ViewModel;
using BussBuzz.MVVM.ResetPassword.View;
using BussBuzz.MVVM.Friends.ViewModel;
using BussBuzz.MVVM.Friends.View;
using BussBuzz.MVVM.MAP.ViewModell;
using BussBuzz.MVVM.Main.ViewModel;
using BussBuzz.MVVM.Main.View;
using BussBuzz.MVVM.ImageColor;
using BussBuzz.MVVM.Menu.MenuItems.About;
using BussBuzz.MVVM.Menu.MenuItems.Feedback;
using BussBuzz.MVVM.Menu.MenuItems.MainFunc;
using BussBuzz.MVVM.Menu.MenuItems.Settings;
using BussBuzz.MVVM.Menu.MenuItems.Share;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews;
using BuzzBuzz.MVVM.Menu.MenuItems.OfficialNews.Details;

namespace BussBuzz.Services
{
    public class NavigationService
    {
        /*
         * Это словарь, который сопоставляет типы ViewModel (VM_Base, VM_Details) 
         * с типами представлений (страниц) в приложении (V_MainMenuNEWS, V_MainMenuNewsDetails).
         */
        protected readonly Dictionary<Type, Type> _mappings;

        static NavigationService _instance;
        public NavigationService()
        {
            _mappings = new Dictionary<Type, Type>();

            CreatePageViewModelMappings();
        }
        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NavigationService();

                return _instance;
            }
        }
        protected Application CurrentApplication
        {
            get { return Application.Current; }
        }

        /*
         *  Эти методы используются для перехода к определенному типу ViewModel. 
         *  Они вызывают InternalNavigateToAsync, передавая тип ViewModel и, возможно, 
         *  дополнительный параметр.
         */
        public Task NavigateToAsync<TViewModel>() where TViewModel : VM_Base
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }
        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : VM_Base
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }
        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }
        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        /*
         * Метод используется для возврата на предыдущую страницу в стеке навигации.
         */
        public async Task NavigateBackAsync()
        {
            await CurrentApplication.MainPage.Navigation.PopAsync();
        }

        /*
         *  Метод используется для получения типа страницы (View), связанной с определенным типом ViewModel.
         */
        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            try
            {

                if (!_mappings.ContainsKey(viewModelType))
                {
                    throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
                }
                return _mappings[viewModelType];

            }
            catch { }
            return null;
        }

        /*
         * Этот метод создает экземпляр страницы на основе типа ViewModel и возвращает его.
         */
        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"Mapping type for {viewModelType} is not a page");
            }

            Page page = Activator.CreateInstance(pageType) as Page;

            return page;
        }


        /*
         * Этот метод создает экземпляр представления (страницы) на основе типа ViewModel, 
         * затем определяет, следует ли установить созданную страницу как главную (MainPage) 
         * или добавить ее в стек навигации. Он также вызывает метод InitializeAsync виртуальной 
         * базовой модели (VM_Base), который, выполняет инициализацию данных ViewModel.
         */
        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);

            if (page is V_Main || page is V_SignIn) // page is V_MainMenuNews
            {
                try
                {
                    CurrentApplication.MainPage = new NavigationPage(page);
                }
                catch
                {
                    Console.WriteLine();
                }
            }
            else
            {
                if (CurrentApplication.MainPage is NavigationPage navigationPage)
                {
                    try
                    {
                        await (page.BindingContext as VM_Base).InitializeAsync(parameter);
                    }
                    catch { }
                    await navigationPage.PushAsync(page);
                }
            }
        }

        /*
         * Этот метод инициализирует словарь _mappings, 
         * устанавливая соответствие между типами ViewModel и типами представлений.
         */
        void CreatePageViewModelMappings()
        {
            // страница смены цвета и картинки текущего пользователя
            _mappings.Add(typeof(VM_ImageColor), typeof(V_ImageColor));
            // главная страница
            _mappings.Add(typeof(VM_Main), typeof(MVVM.Main.View.V_Main));
            // страница с друзьями
            _mappings.Add(typeof(VM_Friends), typeof(V_Friends));
            // страница с картой
            _mappings.Add(typeof(VM_Map), typeof(VM_Map));
            // страница авторизации
            _mappings.Add(typeof(VM_SignIn), typeof(V_SignIn));
            // страница регистрации
            _mappings.Add(typeof(VM_SignUp), typeof(V_SignUp));
            // страница восстановления пароля, ввод кода
            _mappings.Add(typeof(VM_RecoverPasswordCode), typeof(V_RecoverPasswordCode));
            // страница восстановления пароля, ввод почты
            _mappings.Add(typeof(VM_RecoverPasswordEmail), typeof(V_RecoverPasswordEmail));
            // страница восстановления пароля, ввод пароля
            _mappings.Add(typeof(VM_RecoverPasswordNewPassword), typeof(V_RecoverPasswordNewPassword));

            // меню
            _mappings.Add(typeof(VM_About), typeof(V_About));
            _mappings.Add(typeof(VM_Feedback), typeof(V_Feedback));
            _mappings.Add(typeof(VM_MainFunc), typeof(V_MainFunc));
            _mappings.Add(typeof(VM_Settings), typeof(V_Settings));
            _mappings.Add(typeof(VM_Share), typeof(V_Share));
            _mappings.Add(typeof(VM_OfficialNews), typeof(V_OfficialNews));
            _mappings.Add(typeof(VM_OfficialNewDetails), typeof(V_IfficialNewDetails));

        }
    }
}
