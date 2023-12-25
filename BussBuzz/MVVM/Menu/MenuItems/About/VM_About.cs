using BussBuzz.Base;
using BussBuzz.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussBuzz.MVVM.Menu.MenuItems.About
{
    public partial class VM_About: ObservableObject, VM_Base
    {
        [RelayCommand]
        private async Task Back()
        {
            try
            {
                await NavigationService.Instance.NavigateBackAsync();
            }
            catch { }
        }
    }
}
