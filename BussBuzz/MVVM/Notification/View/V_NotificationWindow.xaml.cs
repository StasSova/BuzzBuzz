using BussBuzz.MVVM.Notification.ViewModel;

namespace BussBuzz.MVVM.Notification.View;

public partial class V_NotificationWindow : ContentView
{
    public V_NotificationWindow()
    {
        VM_NotificationWindow viewModel = VM_NotificationWindow.Instance;
        BindingContext = viewModel;
        InitializeComponent();
    }
}