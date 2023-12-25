using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.Services
{
    public class ShowErrorService
    {
        public static async Task ShowError(string error, ToastDuration duration = ToastDuration.Long)
        {
            await Toast.Make(error, duration).Show();
        }
    }
}
