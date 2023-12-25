using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BussBuzz.Services.InterfaceValidation
{
    public interface IPasswordValidation
    {
        void ValidatePassword(string Password);
        void RepitedValidatePassword(string Password, string RepeatedPassword);
    }
    public class DefaultPasswordCheck : IPasswordValidation
    {
        readonly string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*_])[A-Za-z0-9_!@#$%^&*]{8,}$";
        const int minLength = 8;
        public void ValidatePassword(string Password)
        {
            if (!Regex.IsMatch(Password, passwordPattern))
            {
                // Проверка на null
                if (string.IsNullOrEmpty(Password))
                {
                    throw new Exception("Password cannot be empty");
                }
                // Проверка на наличие заглавных букв
                if (!Password.Any(char.IsUpper))
                {
                    throw new Exception("Password should contain at least one uppercase letter.");
                }

                // Проверка на наличие строчных букв
                if (!Password.Any(char.IsLower))
                {
                    throw new Exception("Password should contain at least one lowercase letter.");
                }

                // Проверка на наличие цифр
                if (!Password.Any(char.IsDigit))
                {
                    throw new Exception("Password should contain at least one digit.");
                }

                // Проверка на наличие специальных символов
                if (!Password.Any(c => "_!@#$%^&*".Contains(c)))
                {
                    throw new Exception("Password should contain at least one of the following special characters: _!@#$%^&*");
                }

                // Проверка на длину пароля (пример: минимум 8 символов)
                if (Password.Length < minLength)
                {
                    throw new Exception($"Password should be at least {minLength} characters long.");
                }

                // Проверка на наличие только разрешенных символов
                if (Password.Any(c => !char.IsLetterOrDigit(c) && "_!@#$%^&*".Contains(c)))
                {
                    throw new Exception("Password contains invalid characters. Only letters, digits, and the following special characters are allowed: !@#$%^&*_");
                }

            }
        }
        public void RepitedValidatePassword(string Password, string RepeatedPassword)
        {
            // Базовая проверка на сравнение паролей
            if (Password != RepeatedPassword)
            {
                throw new Exception("Passwords do not match.");
            }
        }
    }
}
