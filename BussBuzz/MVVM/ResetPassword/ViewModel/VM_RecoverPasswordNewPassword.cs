using BussBuzz.Base;
using BussBuzz.MVVM.Main.ViewModel;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconPacks.Material;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace BussBuzz.MVVM.ResetPassword.ViewModel
{
    public partial class VM_RecoverPasswordNewPassword: ObservableValidator, VM_Base
    {
        private readonly ValidationService _VALIDATION_SERVICE;
        private readonly DatabaseService _DB_SERVICE;

        public VM_RecoverPasswordNewPassword()
        {
            _VALIDATION_SERVICE = ValidationService.Instance;
            _DB_SERVICE = DatabaseService.Instance;

            Kind = IconKind.Visibility;

            FormSubmissionCompleted += OnNavigateToMainPage;
        }

        public event EventHandler FormSubmissionCompleted;
        public event EventHandler FormSubmissionFailed;


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
        private string password;
        [CustomValidation(typeof(VM_RecoverPasswordNewPassword), nameof(ValidatePassword))]
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        public static ValidationResult ValidatePassword(string password, ValidationContext context)
        {
            try
            {
                VM_RecoverPasswordNewPassword instance = (VM_RecoverPasswordNewPassword)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidatePassword(password);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            { 
                return new(ex.Message);
            }
        }


        // USER RECOVER PASSWORD
        private string recoverPassword;
        [CustomValidation(typeof(VM_RecoverPasswordNewPassword), nameof(ValidateRecoverPassword))]
        public string RecoverPassword
        {
            get => this.recoverPassword;
            set => SetProperty(ref this.recoverPassword, value);
        }
        public static ValidationResult ValidateRecoverPassword(string password, ValidationContext context)
        {
            try
            {
                VM_RecoverPasswordNewPassword instance = (VM_RecoverPasswordNewPassword)context.ObjectInstance;
                instance._VALIDATION_SERVICE.IsRepeatPasswordValid(password, instance.Password);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            {
                return new(ex.Message);
            }
        }


        [RelayCommand]
        private async Task LogIn()
        {
            try
            {
                ValidateAllProperties();

                if (HasErrors)
                {
                    string message = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
                    throw new Exception(message);
                }
                else
                {
                    // изменяем пароль
                    await _DB_SERVICE.SetPassword(VM_CurrentUser.Instance.Name, Password);

                    // Инициализируем все поля пользователя
                    await _DB_SERVICE.InitializingCurrentUser(VM_CurrentUser.Instance.Name);
                    
                    FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex) 
            {
                await ShowErrorService.ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }

        }
        private void OnNavigateToMainPage(object sender, EventArgs e)
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_Main>();
            }
            catch(Exception ex)
            {
                ShowErrorService.ShowError(ex.Message).Wait();
            }
        }
    }
}
