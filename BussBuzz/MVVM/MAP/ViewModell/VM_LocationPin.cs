using BussBuzz.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.MAP.ViewModell
{
    public partial class VM_LocationPin: ObservableObject
    {
        private Location location;
        public Location Location
        {
            get { return this.location; }
            set { SetProperty<Location>(ref this.location, value); }
        }
        private VM_User _user;
        public VM_User User
        {
            get { return _user; }
            set { SetProperty<VM_User>(ref _user, value); }
        }
    }
}
