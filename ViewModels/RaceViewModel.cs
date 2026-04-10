using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RaceSimulator.Models;
using RaceSimulator.Services;

namespace RaceSimulator.ViewModels;

public class RaceViewModel : ViewModelBase
{
    private readonly Race _race;
    private readonly RaceSimulationService _simulationService;

    private string _raceName = string.Empty;
    private string _status = "Ready";
    private string _winnerText = string.Empty;
    private bool _isRunning;
    private bool _isFinished;
    private string _reflectionInfo = string.Empty;
    private int _pitStopCount;

    public ObservableCollection<RaceCarViewModel> Cars { get; } = new();

    public string RaceName
    {
        get => _raceName;
        private set => SetProperty(ref _raceName, value);
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public string WinnerText
    {
        get => _winnerText;
        private set => SetProperty(ref _winnerText, value);
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    public bool IsFinished
    {
        get => _isFinished;
        private set => SetProperty(ref _isFinished, value);
    }

    public string ReflectionInfo
    {
        get => _reflectionInfo;
        private set => SetProperty(ref _reflectionInfo, value);
    }

    public int PitStopCount
    {
        get => _pitStopCount;
        private set => SetProperty(ref _pitStopCount, value);
    }

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ShowReflectionCommand { get; }

    public event Action<RaceViewModel>? RemoveRequested;

    public RaceViewModel(Race race)
    {
        _race = race;
        _simulationService = new RaceSimulationService(race);

        RaceName = race.Name;

        int idx = 0;
        foreach (var car in race.Cars)
        {
            car.TiresWorn += OnTiresWorn;
            car.CollisionOccurred += OnCollision;
            Cars.Add(new RaceCarViewModel(car, race.Track.Length, race.Track.Laps, idx++));
        }

        race.RaceStarted += r => SetStatus("Racing...");
        race.RaceFinished += OnRaceFinished;
        race.LapCompleted += OnLapCompleted;
        _simulationService.RaceUpdated += OnRaceUpdated;

        foreach (var mechanic in race.Mechanics)
        {
            mechanic.TireChangeStarted    += (m, c) => SetStatus($"Mechanic {m.Id}: changing tires on {c.Name}");
            mechanic.TireChangeCompleted  += (m, c) => SetStatus($"Mechanic {m.Id}: done — {c.Name} back on track");
        }

        foreach (var loader in race.Loaders)
        {
            loader.LoaderArrived   += (l, c) => SetStatus($"Loader {l.Id}: arrived at {c.Name} collision site");
            loader.LoaderCompleted += (l, c) => SetStatus($"Loader {l.Id}: {c.Name} delivered to pit");
        }

        StartCommand = new AsyncRelayCommand(StartRaceAsync, () => !IsRunning && !IsFinished);
        StopCommand = new RelayCommand(StopRace, () => IsRunning);
        ShowReflectionCommand = new RelayCommand(ShowReflection);
    }

    public async Task StartRaceAsync()
    {
        IsRunning = true;
        ((AsyncRelayCommand)StartCommand).NotifyCanExecuteChanged();
        ((RelayCommand)StopCommand).NotifyCanExecuteChanged();
        await _simulationService.StartAsync();
    }

    public void StopRace()
    {
        _simulationService.Stop();
        IsRunning = false;
        Status = "Stopped";
        ((AsyncRelayCommand)StartCommand).NotifyCanExecuteChanged();
        ((RelayCommand)StopCommand).NotifyCanExecuteChanged();
    }

    public void RequestRemove() => RemoveRequested?.Invoke(this);

    private void ShowReflection()
    {
        if (_race.Cars.Count == 0) return;
        var info = ReflectionService.GetCarInfo(_race.Cars[0]);
        var propNames = ReflectionService.GetPropertyNames<RaceCar>();
        ReflectionInfo = $"[Reflection: {typeof(RaceCar).Name}]\n" +
                         $"Properties: {string.Join(", ", propNames)}\n\n" +
                         info;
    }

    private void OnRaceUpdated(Race race)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            foreach (var vm in Cars)
                vm.Sync();
            PitStopCount = race.PitStops.Count;
        });
    }

    private void OnRaceFinished(Race race, RaceCar winner)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            IsFinished = true;
            IsRunning = false;
            WinnerText = $"WINNER: {winner.Name}";
            Status = $"Finished! Winner: {winner.Name}";
            ((AsyncRelayCommand)StartCommand).NotifyCanExecuteChanged();
            ((RelayCommand)StopCommand).NotifyCanExecuteChanged();
        });
    }

    private void OnLapCompleted(Race race, RaceCar car, int lap)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Status = $"Lap {lap}/{race.Track.Laps} — {car.Name}";
        });
    }

    private void OnTiresWorn(RaceCar car)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Status = $"{car.Name}: Tires worn! Going to pit.";
        });
    }

    private void OnCollision(RaceCar car)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Status = $"BARRIER COLLISION: {car.Name}!";
        });
    }

    private void SetStatus(string s)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => Status = s);
    }
}
