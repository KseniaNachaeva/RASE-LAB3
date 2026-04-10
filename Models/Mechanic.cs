using System;
using System.Threading.Tasks;

namespace RaceSimulator.Models;

public delegate void TireChangeHandler(Mechanic mechanic, RaceCar car);

public class Mechanic
{
    private static readonly Random _random = new();

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsBusy { get; set; }

    public event TireChangeHandler? TireChangeStarted;
    public event TireChangeHandler? TireChangeCompleted;

    public async Task ChangeTiresAsync(RaceCar car, PitStop pitStop)
    {
        IsBusy = true;
        TireChangeStarted?.Invoke(this, car);

        await Task.Delay(2000 + _random.Next(1000));

        car.RepairAfterPit();
        pitStop.ExitTime = DateTime.Now;

        IsBusy = false;
        TireChangeCompleted?.Invoke(this, car);
    }
}
