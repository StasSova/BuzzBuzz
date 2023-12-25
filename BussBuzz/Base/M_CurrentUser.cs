using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Base
{
    public class M_CurrentUser: M_User
    {
        private static M_CurrentUser _instance;
        public static M_CurrentUser Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new M_CurrentUser();
                return _instance;
            }
        }

        public string _email;
        public string _password;

        private M_CurrentUser()
        {
            
        }
    }
}
