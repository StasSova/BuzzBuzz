﻿using PositionChangedEventArgs = Windows.Devices.Geolocation.PositionChangedEventArgs;

namespace BussBuzz;

public class GeolocatorImplementation : BussBuzz.MVVM.MAP.Model.IGeolocator
{
    readonly Windows.Devices.Geolocation.Geolocator locator = new();

    public async Task StartListening(IProgress<Location> positionChangedProgress, CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource();
        cancellationToken.Register(() =>
        {
            locator.PositionChanged -= PositionChanged;
            taskCompletionSource.TrySetResult();
        });
        locator.PositionChanged += PositionChanged;

        void PositionChanged(Windows.Devices.Geolocation.Geolocator sender, PositionChangedEventArgs args)
        {
            positionChangedProgress.Report(new Location(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude));
        }

        await taskCompletionSource.Task;
    }
}