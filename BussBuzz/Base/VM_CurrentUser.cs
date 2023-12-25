using BussBuzz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Base
{
    public partial class VM_CurrentUser: VM_User
    {
        private static VM_CurrentUser _instance;
        public static VM_CurrentUser Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VM_CurrentUser();
                return _instance;
            }
        }
        private VM_CurrentUser() 
        {
            user = M_CurrentUser.Instance;
            EventService.ImChangedColorEvent += EventService_ImChangedColorEvent;
            EventService.ImChangedPhotoEvent += EventService_ImChangedPhotoEvent;
        }

        private Task EventService_ImChangedPhotoEvent(string pathPhoto)
        {
            lock (this)
            {
                Image = pathPhoto;
            }
            return Task.CompletedTask;
        }
        private Task EventService_ImChangedColorEvent(string color)
        {
            lock (this)
            {
                Color = color;
            }
            return Task.CompletedTask;
        }


        public string Email
        {
            get { return M_CurrentUser.Instance._email; }
            set { SetProperty<string>(ref M_CurrentUser.Instance._email, value); }
        }
        public string Password
        {
            get { return M_CurrentUser.Instance._password; }
            set { SetProperty<string>(ref M_CurrentUser.Instance._password, value); }
        }
    }
}
