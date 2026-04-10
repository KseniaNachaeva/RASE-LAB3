using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RaceSimulator.Models;

namespace RaceSimulator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private static int _raceCounter;

    private static readonly List<string> CarColors = new()
        { "#E8002D", "#0600EF", "#FF8000", "#00D2BE", "#DC0000", "#00A651", "#900000", "#006F62" };

    private readonly Random _random = new();

    public ObservableCollection<RaceViewModel> Races { get; } = new();

    public ICommand AddRaceCommand { get; }
    public ICommand StartAllCommand { get; }
    public ICommand StopAllCommand { get; }

    public MainWindowViewModel()
    {
        AddRaceCommand = new RelayCommand(AddRace);
        StartAllCommand = new AsyncRelayCommand(StartAllAsync);
        StopAllCommand = new RelayCommand(StopAll);
    }

    private void AddRace()
    {
        _raceCounter++;
        var race = BuildRace(_raceCounter);
        var vm = new RaceViewModel(race);
        vm.RemoveRequested += r => Races.Remove(r);
        Races.Add(vm);
    }

    private async Task StartAllAsync()
    {
        var tasks = new List<Task>();
        foreach (var race in Races)
        {
            if (!race.IsRunning && !race.IsFinished)
                tasks.Add(race.StartRaceAsync());
        }
        await Task.WhenAll(tasks);
    }

    private void StopAll()
    {
        foreach (var race in Races)
            race.StopRace();
    }

    private Race BuildRace(int id)
    {
        var difficulty = 0.5 + _random.NextDouble() * 1.5;

        var track = new RaceTrack
        {
            Name = $"Track {id}",
            Length = 80 + _random.Next(60),
            Laps = 3 + _random.Next(3),
            TrackType = "Circuit",
            Difficulty = difficulty
        };

        int carCount = 3 + _random.Next(4);
        var cars = new List<RaceCar>();

        for (int i = 0; i < carCount; i++)
        {
            cars.Add(new RaceCar
            {
                Id = i + 1,
                Name = $"Car {i + 1}",
                Speed = 10 + _random.NextDouble() * 8,
                Position = 0,
                TireWear = 0,
                IsInPit = false,
                HasCollided = false,
                LapsCompleted = 0,
                Color = CarColors[i % CarColors.Count],
                CollisionRisk = 0.001 + _random.NextDouble() * 0.003
            });
        }

        var mechanics = new List<Mechanic>
        {
            new Mechanic { Id = 1, Name = $"Mechanic 1 (Race {id})", IsBusy = false },
            new Mechanic { Id = 2, Name = $"Mechanic 2 (Race {id})", IsBusy = false }
        };

        var loaders = new List<ILoader>
        {
            new Loader { Id = 1, IsAvailable = true }
        };

        return new Race
        {
            Id = id,
            Name = $"Race #{id} — Track {id}",
            Track = track,
            Cars = cars,
            Mechanics = mechanics,
            Loaders = loaders
        };
    }
}
