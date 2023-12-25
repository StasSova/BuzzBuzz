using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BussBuzz.Services.InterfaceValidation
{
    public interface IEmailValidation
    {
        void ValidateEmail(string email);
    }

    public class DefaultEmailValidation : IEmailValidation
    {
        //readonly string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        public void ValidateEmail(string email)
        {
            // Проверка, что email не является пустым или null
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email cannot be empty.");
            }

            try
            {
                // Нормализация домена в email (если домен содержит Unicode)
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Обработка части домена электронной почты и его нормализация.
                static string DomainMapper(Match match)
                {
                    // Используйте класс IdnMapping для преобразования доменных имен Unicode.
                    var idn = new IdnMapping();

                    // Извлечение и обработка доменного имени (генерирует исключение ArgumentException при неверном)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                // Если произошел таймаут при выполнении регулярного выражения, выбросить исключение
                throw new ArgumentException("Email validation timeout.", nameof(email), e);
            }
            catch (ArgumentException e)
            {
                // Если произошла ошибка при нормализации домена, выбросить исключение
                throw new ArgumentException("Invalid email domain.", nameof(email), e);
            }

            // Проверка с использованием регулярного выражения на основные форматные требования email
            try
            {
                if (!Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                {
                    // Если email не соответствует ожидаемому формату, выбросить исключение
                    throw new Exception("Invalid email format.");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                // Если произошел таймаут при выполнении регулярного выражения, выбросить исключение
                throw new Exception("Email validation timeout.");
            }
        }
    }
}
