using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.MAP.Model
{
    public class LocationPin
    {
        public string UserName { get; set; }
        public Location Location { get; set; }
        public ImageSource ImageSource { get; set; }
    }
}
