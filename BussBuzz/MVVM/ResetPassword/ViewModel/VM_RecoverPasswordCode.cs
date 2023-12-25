using BussBuzz.Base;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PINView.Maui.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BussBuzz.MVVM.ResetPassword.Model;
using BussBuzz.MVVM.ResetPassword.View;

namespace BussBuzz.MVVM.ResetPassword.ViewModel
{
    public partial class VM_RecoverPasswordCode : ObservableValidator, VM_Base
    {
        public event EventHandler FormSubmissionCompleted;
        public event EventHandler FormSubmissionFailed;

        private DatabaseUser me;
        private int random_code;

        // Введенный код
        private string _code = "";

        public string Code
        {
            get => this._code;
            set => SetProperty(ref this._code, value);
        }
        private void OnNavigateToPassword(object sender, EventArgs e)
        {
            try
            {
                _ = NavigationService.Instance.NavigateToAsync<VM_RecoverPasswordNewPassword>();
            }
            catch { }
        }


        // COMMANDS
        [RelayCommand]
        private async Task Verify()
        {
            try
            {
                // Проверка введенных данных ()
                ValidateAllProperties();

                if (HasErrors)
                {
                    // УВЕДОМЛЕНИЕ ОБ ОШИБКЕ
                    string message = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));
                    throw new Exception(message);
                }
                else
                {
                    bool isValid = Code == random_code.ToString();

                    if (isValid)
                    {
                        FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        // Обработка неудачного ввода, например, показ всплывающего окна
                        throw new Exception("Incorrect code");
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }

        }

        [RelayCommand]
        private static void Back()
        {
            try
            {
                _ = NavigationService.Instance.NavigateBackAsync();
            }
            catch
            {

            }
        }

        private static async Task ShowError(string error, ToastDuration duration = ToastDuration.Long)
        {
            try
            {
                await Toast.Make(error, duration).Show();
            }
            catch { }
        }





        private const int startTimer = 59;
        private int secondsRemaining;

        // текст 
        private string numSec;
        public string NumSec
        {
            get { return numSec; }
            set { SetProperty(ref numSec, value); }
        }

        // активность кнопки
        private bool isResEneble;
        public bool IsResEneble
        {
            get { return isResEneble; }
            set { SetProperty(ref isResEneble, value); }
        }

        public VM_RecoverPasswordCode()
        {
            FormSubmissionCompleted += OnNavigateToPassword;

            me = DatabaseService.Instance.GetDbUser(VM_CurrentUser.Instance.Name);
            if (me == null) throw new Exception("Can't find username in database");
            SendCode();
            ResendCodeStartTimer();
        }

        private void SendCode()
        {
            M_EmailVerification m_EmailVerification = new M_EmailVerification(me.Email);
            Random random = new Random();
            random_code = random.Next(10000, 100000);
            m_EmailVerification.SendResetPasswordMessage(me.Username, random_code.ToString());
        }





        [RelayCommand]
        private async Task ResendCode()
        {
            try
            {
                Code = string.Empty;
                SendCode();
                ResendCodeStartTimer();
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
                FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
            }
        }



        private void ResendCodeStartTimer()
        {
            IsResEneble = false;               // Выключаю кнопку
            secondsRemaining = startTimer;     // Устанавливаем отсчёт
            NumSec = $"{secondsRemaining}s";   // Первоначально показываем откуда отсчёт
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }

        private bool OnTimerTick()
        {
            if (secondsRemaining > 0)
            {
                secondsRemaining--;
                NumSec = $"{secondsRemaining}s";
                return true; // Continue the timer
            }
            else
            {
                NumSec = "";
                IsResEneble = true;  // Включаю кнопку
                return false; // Stop the timer
            }
        }





    }
}
