using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using ThreadNetwork;

namespace BussBuzz.MVVM.ResetPassword.Model
{
    public class M_EmailVerification
    {
        // Ваши данные для отправки почты
        private string smtpServer = "YOUR_KEY";
        private string smtpUsername = "YOUR_KEY"; // Пароль почты Qwerty_123
        private string smtpPassword = "YOUR_KEY";
        private int smtpPort = -1; // Обычно порт -1 используется для шифрованного соединения (TLS)
        private string toEmail;

        private SmtpClient smtpClient;

        public M_EmailVerification(string toEmail)
        {
            // Создание объекта SmtpClient
            smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true, // Включение SSL для шифрованного соединения
            };

            // Адрес получателя
            this.toEmail = toEmail;
        }

        public void SendResetPasswordMessage(string name, string number)
        {

            // Создание объекта MailMessage
            MailMessage message = new MailMessage(smtpUsername, toEmail)
            {
                Subject = "Password Reset in BuzzBuzz",
                Body = $"Password Reset Confirmation\r\n\r\nDear {name},\r\n\r\nWelcome back to BuzzBuzz! We are pleased to assist you with resetting your password.\r\n\r\nA request has been received to reset your account password. To complete the process, please enter the following five-digit code when updating your password:\r\n\r\nVerification Code: {number}\r\n\r\nThank you for choosing us! We are confident that your continued experience with our service will be secure and convenient.\r\n\r\nIf you have any questions or need further assistance, feel free to reach out to our support team.\r\n\r\nBest regards,\r\nThe BuzzBuzz Team"
            };

            try
            {
                // Отправка сообщения
                smtpClient.Send(message);
                //Console.WriteLine("Сообщение успешно отправлено");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }

        public void SendRegistrationMessage(string name)
        {
            // Создание объекта MailMessage
            MailMessage message = new MailMessage(smtpUsername, toEmail)
            {
                Subject = "Account Registration in BuzzBuzz",
                Body = $"Account Registration Confirmation\r\n\r\nDear {name},\r\n\r\nWelcome to BuzzBuzz! We're excited to confirm your successful account registration.\r\n\r\nYour account setup is now complete. Thank you for choosing BuzzBuzz, and we look forward to providing you with a seamless and enjoyable experience.\r\n\r\nIf you have any questions or need assistance, feel free to reach out to our support team.\r\n\r\nBest regards,\r\nThe BuzzBuzz Team",
            };

            try
            {
                // Отправка сообщения
                smtpClient.Send(message);
                //Console.WriteLine("Сообщение успешно отправлено");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }
    }
}
