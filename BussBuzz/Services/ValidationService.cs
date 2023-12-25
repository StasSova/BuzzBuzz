using BussBuzz.Services.InterfaceValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public class ValidationService
    {
        public ILoginValidation loginValidation {  get; set; }
        public IPasswordValidation passwordValidation { get; set; }
        public IEmailValidation emailValidation { get; set; }
        private static ValidationService _instance;
        public static ValidationService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ValidationService();
                return _instance;
            }
        }
        private ValidationService() 
        {
            loginValidation = new DefaultLoginCheck();
            passwordValidation = new DefaultPasswordCheck();
            emailValidation = new DefaultEmailValidation();
        }


        public void ValidateLogin(string UserName) 
        {
            loginValidation.ValidateLogin(UserName); 
        }
        public void ValidateLoginSearched(string UserName)
        {
            loginValidation.ValidateLoginSearched(UserName);
        }
        public void ValidateEmail(string Email) 
        {
            emailValidation.ValidateEmail(Email);
        }

        public void ValidatePassword(string Password) 
        {  
            passwordValidation.ValidatePassword(Password); 
        }
        public void IsRepeatPasswordValid(string Password, string RepeatPassword) 
        {  
            passwordValidation.RepitedValidatePassword(Password, RepeatPassword); 
        }


    }
}
