using BussBuzz.MVVM.Friends.ViewModel;

namespace BussBuzz.MVVM.Friends.View;

public partial class V_Friends : ContentView
{
	public V_Friends()
	{
        VM_Friends viewModel = VM_Friends.Instance;
        BindingContext = viewModel;
        InitializeComponent();
	}
}