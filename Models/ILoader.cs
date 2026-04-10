using System.Threading.Tasks;

namespace RaceSimulator.Models;

public delegate void LoaderEventHandler(ILoader loader, RaceCar car);

public interface ILoader
{
    int Id { get; }
    bool IsAvailable { get; }

    event LoaderEventHandler? LoaderArrived;
    event LoaderEventHandler? LoaderCompleted;

    Task HandleCollisionAsync(RaceCar car, PitStop pitStop);
}
