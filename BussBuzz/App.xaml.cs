using BussBuzz.MVVM.Authorization.ViewModel;
using BussBuzz.MVVM.ResetPassword.ViewModel;
using BussBuzz.Services;
using BussBuzz.MVVM.MAP.View;
//using Android.Content;
//using Android.Preferences;
using Newtonsoft.Json;
using System.IO;
using BussBuzz.MVVM.Authorization.Model;
using SharpCompress.Common;

namespace BussBuzz
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            EventService.ImHasLeftAppEvent += EventService_ImHasLeftAppEvent;

            string fileName = "userdata.json";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            //if (File.Exists(filePath))
            //{
            //    // Удаляем файл
            //    File.Delete(filePath);
            //}


            // Читаем файл. Если ещё его нет, то создаём файл со значение null.  
            // null - пользователь не заходил раньше
            UserSettingsData loadedUserSettingsData = new UserSettingsData
            {
                UsernameIsLogged = UserSettings.ReadFromFile(),
            };

            if (loadedUserSettingsData.UsernameIsLogged != null)
            {

                //await _DB_SERVICE.InitializingCurrentUser(Name);
                DatabaseService databaseService = DatabaseService.Instance;
                databaseService.InitializingCurrentUser(loadedUserSettingsData.UsernameIsLogged).Wait();

                // Пользователь не вошел, выполните действия для невошедшего пользователя
                MainPage = new NavigationPage(new MVVM.Main.View.V_Main());
            }
            else
            {
                //Пользователь уже вошел, выполните необходимые действия
                MainPage = new NavigationPage(new MVVM.Authorization.V_SignIn());
            }


        }

        private Task EventService_ImHasLeftAppEvent()
        {
            try
            {
                string fileName = "userdata.json";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
