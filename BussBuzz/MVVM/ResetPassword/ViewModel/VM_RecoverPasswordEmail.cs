using BussBuzz.Base;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace BussBuzz.MVVM.ResetPassword.ViewModel
{
    public partial class VM_RecoverPasswordEmail: ObservableValidator, VM_Base
    {
        private readonly ValidationService _VALIDATION_SERVICE;
        private readonly DatabaseService _DB_SERVICE;
        // Конструктор класса
        public VM_RecoverPasswordEmail()
        {
            _VALIDATION_SERVICE = ValidationService.Instance;
            _DB_SERVICE = DatabaseService.Instance;

            FormSubmissionCompleted += OnNavigateToCode;
        }

        public event EventHandler FormSubmissionCompleted;
        public event EventHandler FormSubmissionFailed;


        // USER NAME
        private string _name = "";
        [CustomValidation(typeof(VM_RecoverPasswordEmail), nameof(ValidateUserName))]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public static ValidationResult ValidateUserName(string name, ValidationContext context)
        {
            try
            {
                VM_RecoverPasswordEmail instance = (VM_RecoverPasswordEmail)context.ObjectInstance;
                instance._VALIDATION_SERVICE.ValidateLogin(name);
                return ValidationResult.Success;
            }
            catch (Exception ex) 
            {
                return new(ex.Message);
            }
        }


        [RelayCommand]
        private async Task ResetPassword()
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
                    // Асинхронная проверка имени пользователя в базе данных на существование
                    await _DB_SERVICE.IsUserNameExist(Name);

                    VM_CurrentUser.Instance.Name = Name;
                    FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }

        }
        private void OnNavigateToCode(object sender, EventArgs e)
        {
            try
            {
                NavigationService.Instance.NavigateToAsync<VM_RecoverPasswordCode>();
            }
            catch { }
        }

        [RelayCommand]
        private static void Back()
        {
            try
            {
                _ = NavigationService.Instance.NavigateBackAsync();
            }
            catch { }
        }

        private static async Task ShowError(string error, ToastDuration duration = ToastDuration.Long)
        {
            await Toast.Make(error, duration).Show();
        }
    }
}
