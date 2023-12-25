using BussBuzz.Base;
using BussBuzz.MVVM.Main.ViewModel;
using BussBuzz.MVVM.ResetPassword.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using BussBuzz.MVVM.Authorization.Model;
using IconPacks.Material;

namespace BussBuzz.MVVM.Authorization.ViewModel
{
    public partial class VM_SignIn : ObservableValidator, VM_Base
    {
        public VM_SignIn()
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
        [CustomValidation(typeof(VM_SignIn), nameof(ValidateName))]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public static ValidationResult ValidateName(string name, ValidationContext context)
        {
            try
            {
                VM_SignIn instance = (VM_SignIn)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidateLogin(name);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            {
                return new(ex.Message);
            }

        }


        // USER PASSWORD
        private string _password = "";
        [CustomValidation(typeof(VM_SignIn), nameof(ValidatePassword))]
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public static ValidationResult ValidatePassword(string name, ValidationContext context)
        {
            try
            {
                VM_SignIn instance = (VM_SignIn)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidatePassword(name);
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

        // Асинхронный метод для автоизации
        [RelayCommand]
        private async Task SignIn()
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
                    // Асинхронная проверка логина и пароля в базе данных
                    await _DB_SERVICE.CheckingLoginAndPassword(Name, Password);
                    
                    if (Remember) UserSettings.SaveToFile(Name);

                    // Инициализируем все поля пользователя
                    await _DB_SERVICE.InitializingCurrentUser(Name);

                    FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                await ShowErrorService.ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        [RelayCommand]
        private static void SignUp(object parameter)
        {
            _ = NavigationService.Instance.NavigateToAsync<VM_SignUp>(parameter);
        }

        [RelayCommand]
        private static void ForgotPassword()
        {
            _ = NavigationService.Instance.NavigateToAsync<VM_RecoverPasswordEmail>();
        }

        // переход на главную страничку
        private void ToMainPage(object sender, EventArgs e)
        {
            _ = NavigationService.Instance.NavigateToAsync<VM_Main>();
        }
    }
}
