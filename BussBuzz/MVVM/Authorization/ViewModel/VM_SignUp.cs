using BussBuzz.Base;
using System.Windows.Input;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CommunityToolkit.Maui.Core;
using BussBuzz.MVVM.Main.ViewModel;
using Newtonsoft.Json;
using BussBuzz.MVVM.Authorization.Model;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using IconPacks.Material;
using BussBuzz.MVVM.ResetPassword.Model;


namespace BussBuzz.MVVM.Authorization.ViewModel
{
    public class AuthorizationManager
    {
        public async Task<UserCredential> AuthorizeUserAsync()
        {
            var clientId = "479035108972-35qre8fl2htvistgd7hbat95dqa18rpq.apps.googleusercontent.com";
            var clientSecret = "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";
            var scopes = new[] { GmailService.Scope.GmailReadonly };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                },
                Scopes = scopes,
            });

            var credential = await new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver())
                .AuthorizeAsync("user", CancellationToken.None);

            return credential;
        }
    }
    public partial class VM_SignUp : ObservableValidator, VM_Base
    {
        public VM_SignUp()
        {
            _VALIDATION_SERVICE = ValidationService.Instance;
            _DB_SERVICE = DatabaseService.Instance;
            Kind = IconKind.Visibility;


            FormSubmissionCompleted += ToMainPage;
        }
        private readonly ValidationService _VALIDATION_SERVICE;
        private readonly DatabaseService _DB_SERVICE;
       
        public event EventHandler FormSubmissionCompleted;
        public event EventHandler FormSubmissionFailed;

        // USER NAME
        private string _name = "";
        [CustomValidation(typeof(VM_SignUp), nameof(ValidateName))]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public static ValidationResult ValidateName(string name, ValidationContext context)
        {
            try
            {
                VM_SignUp instance = (VM_SignUp)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidateLogin(name);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            {
                return new(ex.Message);
            }
        }

        // USER EMAIL
        private string _email = "";
        [CustomValidation(typeof(VM_SignUp), nameof(ValidateEmail))]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public static ValidationResult ValidateEmail(string email, ValidationContext context)
        {
            try
            {
                VM_SignUp instance = (VM_SignUp)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidateEmail(email);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            {
                return new(ex.Message);
            }
        }

        private bool secret = true;
        public bool Secret
        {
            get { return secret; }
            set { SetProperty<bool>(ref secret, value); }
        }
        [RelayCommand]
        private void ChangeVisiblePassword()
        {
            Secret = !Secret;
            Kind = Kind == IconKind.Visibility ? IconKind.VisibilityOff : IconKind.Visibility;
        }

        private IconKind kind;
        public IconKind Kind
        {
            get { return kind; }
            set { SetProperty(ref kind, value); }
        }

        // USER PASSWORD
        private string _password;
        [CustomValidation(typeof(VM_SignUp), nameof(ValidatePassword))]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public static ValidationResult ValidatePassword(string password, ValidationContext context)
        {
            try
            {
                VM_SignUp instance = (VM_SignUp)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidatePassword(password);
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new(ex.Message);
            }
        }

        private bool remember;
        public bool Remember
        {
            get { return remember; }
            set { SetProperty<bool>(ref remember, value); }
        }


        // Асинхронный метод для регистрации
        [RelayCommand]
        private async Task SignUp()
        {
            try
            {
                // Проверка всех свойств на валидность
                ValidateAllProperties();

                // Если есть ошибки валидации, вывести сообщение (например, всплывающее окно)
                if (HasErrors)
                {
                    // УВЕДОМЛЕНИЕ ОБ ОШИБКЕ
                    string message = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
                    throw new Exception(message);
                }
                else
                {
                    // Создание пользователя
                    await _DB_SERVICE.RegisteringAndInitializingCurrentUserFields(Name, Email, Password);

                    if (Remember) UserSettings.SaveToFile(Name);

                    UserSettings.SaveToFile(Name);

                    M_EmailVerification email = new M_EmailVerification(Email);
                    email.SendRegistrationMessage(Name);



                    FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch(Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }
        }


        [RelayCommand]
        private async Task GoogleReg()
        {
            var authorizationManager = new AuthorizationManager();
            var credentials = await authorizationManager.AuthorizeUserAsync();

            // Создаем службу Gmail с использованием учетных данных
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials
            });

            // Выводим сообщение об успешной аутентификации
            //Console.WriteLine("Authentification Successfully!");

            // Получаем и выводим профиль текущего пользователя
            var profile = await service.Users.GetProfile("me").ExecuteAsync();
            //Console.WriteLine(profile.EmailAddress);
        }

        // на кнопку Sign In => BACK
        [RelayCommand]
        private static void SignIn()
        {
            try
            {
                _ = NavigationService.Instance.NavigateBackAsync();
            }
            catch { }
        }

        // На главную страничку
        private async void ToMainPage(object sender, EventArgs e)
        {
            try
            {
                await NavigationService.Instance.NavigateToAsync<VM_Main>();
            }
            catch { }
        }
    }
}
