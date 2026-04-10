using System;
using System.Threading.Tasks;

namespace RaceSimulator.Models;

public class Loader : ILoader
{
    private static readonly Random _random = new();

    public int Id { get; set; }
    public bool IsAvailable { get; set; } = true;

    public event LoaderEventHandler? LoaderArrived;
    public event LoaderEventHandler? LoaderCompleted;

    public async Task HandleCollisionAsync(RaceCar car, PitStop pitStop)
    {
        IsAvailable = false;

        await Task.Delay(1000 + _random.Next(1000));
        LoaderArrived?.Invoke(this, car);

        await Task.Delay(2000 + _random.Next(2000));
        car.RepairAfterPit();
        pitStop.ExitTime = DateTime.Now;

        IsAvailable = true;
        LoaderCompleted?.Invoke(this, car);
    }
}
