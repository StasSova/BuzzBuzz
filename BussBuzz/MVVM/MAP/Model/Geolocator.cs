using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.MAP.Model
{
    public static class Geolocator
    {
        public static IGeolocator Default = new GeolocatorImplementation();
    }
}
