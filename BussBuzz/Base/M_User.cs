using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Base
{
    public class M_User: ObservableObject
    {
        public string _name;
        public string _color;
        public string _image;
        public UserStatus _status;
    }
}
