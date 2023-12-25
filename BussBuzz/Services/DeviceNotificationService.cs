using BussBuzz.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzBuzz.Services
{
    public class DeviceNotificationService
    {
        public DeviceNotificationService() 
        {
            EventService.FrCancelAddFriendEvent += VibrationDevice;
            EventService.FrConfirmAddFriendEvent += VibrationDevice;
            EventService.FrRequestAddFriendEvent += VibrationDevice;
            EventService.FrRemoveFriendEvent += VibrationDevice;
        }

        private Task VibrationDevice(string username)
        {
            try
            {
                Vibration.Default.Vibrate();
            }
            catch { }
            return Task.CompletedTask;
        }
    }
}
