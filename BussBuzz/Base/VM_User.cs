using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Base
{
    public partial class VM_User: M_User
    {
        protected M_User user;
        public string Name
        {
            get { return user._name; }
            set
            {
                SetProperty<string>(ref user._name, value);
            }
        }
        public string Color
        {
            get { return user._color; }
            set
            {
                SetProperty<string>(ref user._color, value);
            }
        }
        public string Image
        {
            get { return user._image; }
            set
            {
                SetProperty<string>(ref user._image, value);
            }
        }
        public UserStatus Status
        {
            get { return user._status; }
            set { SetProperty<UserStatus>(ref user._status, value); }
        }
        public VM_User() 
        {
            user = new M_User();
        }
        public VM_User(M_User user) 
        {
            this.user = user;
        }
    }
}
