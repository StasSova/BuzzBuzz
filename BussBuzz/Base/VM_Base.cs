using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace BussBuzz.Base
{
    public interface VM_Base
    {
        Task InitializeAsync(object navigationData)
        {

            return Task.FromResult(false);
        }
    }
}
