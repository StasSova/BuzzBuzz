using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BussBuzz.Services.InterfaceValidation
{
    public interface ILoginValidation
    {
        void ValidateLogin(string s);
        void ValidateLoginSearched(string Login);
    }

    public class DefaultLoginCheck : ILoginValidation
    {
        readonly string pattern = @"^[A-Za-z0-9_]+$";
        const int minLength = 8;
        const int maxLength = 20;
        readonly string searchedPattern = @"^[a-zA-Z0-9_]*$";
        public void ValidateLogin(string inputLogin)
        {
            if (!Regex.IsMatch(inputLogin, pattern))
            {
                // Проверка на null
                if (string.IsNullOrEmpty(inputLogin))
                {
                    throw new Exception("Login cannot be empty");
                }

                // Проверка на наличие букв
                if (!inputLogin.Any(char.IsLetter))
                {
                    throw new Exception("Login should contain at least one letter.");
                }

                //// Проверка на наличие цифр
                //if (!inputLogin.Any(char.IsDigit))
                //{
                //    throw new Exception("Login should contain at least one digit.");
                //}

                // Проверка на наличие символов подчеркивания
                //if (!inputLogin.Contains('_'))
                //{
                //    throw new Exception("Login should contain at least one underscore character.");
                //}

                // Проверка на наличие только разрешенных символов
                if (inputLogin.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
                {
                    throw new Exception("Login contains invalid characters. Only letters, digits, and underscores are allowed.");
                }

                // Проверка на наличие букв в начале логина
                if (!char.IsLetter(inputLogin.First()))
                {
                    throw new Exception("Login should start with a letter.");
                }

                // Проверка на длину логина
                if (inputLogin.Length < minLength || inputLogin.Length > maxLength)
                {
                    throw new Exception($"Login length should be between {minLength} and {maxLength} characters.");
                }
            }
        }
        public void ValidateLoginSearched(string login)
        {
            // Проверка на null
            if (login == null)
            {
                throw new Exception("User name cannot be empty");
            }
            // Проверка на соответствие шаблону
            if (!Regex.IsMatch(login, searchedPattern))
            {
                // Если не соответствует, определяем, какие символы являются недопустимыми

                // Проверка на наличие недопустимых символов (не букв, не цифр и не подчеркивания)
                char invalidChar = login.FirstOrDefault(c => !char.IsLetterOrDigit(c) && c != '_');

                if (invalidChar != default(char))
                {
                    // Если найден недопустимый символ, выбрасываем исключение с подробным сообщением
                    throw new Exception($"User name contains invalid character '{invalidChar}'. Only letters, digits, and underscores are allowed.");
                }
                else
                {
                    // Если не найдены недопустимые символы, но строка не соответствует шаблону, то, вероятно, есть пробелы или другие недопустимые символы
                    throw new Exception("User name is not correct.");
                }
            }
        }

    }
}
