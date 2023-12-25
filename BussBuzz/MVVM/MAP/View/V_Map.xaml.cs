namespace BussBuzz.MVVM.MAP.View;

using AutoFixture;
using BussBuzz.MVVM.MAP.ViewModell;
using BussBuzz.Services;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Maps;
public partial class V_Map : ContentView
{
    public V_Map()
    {
        VM_Map viewModel = VM_Map.Instance;
        BindingContext = viewModel;
        InitializeComponent();
        MyMap.MoveToRegion(new MapSpan(new Location(46.4, 30.7), 0.1, 0.2));
    }
	public V_Map(VM_Map viewModel)
	{
        BindingContext = viewModel;
        InitializeComponent();
    }

    private void IconButton_Clicked(object sender, SkiaSharp.Views.Maui.SKTouchEventArgs e)
    {
        try
        {
            if (VM_Map.Instance.GetCurrentLocation() != null)
                MyMap.MoveToRegion(new MapSpan(VM_Map.Instance.GetCurrentLocation(), 0.01, 0.02));
        }
        catch
        {

        }
    }
    private void ContentView_Loaded(object sender, EventArgs e)
    {
       // MyMap.MoveToRegion(new MapSpan(new Location(46.4, 30.7), 0.1, 0.2));
    }
}