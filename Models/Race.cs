using System;
using System.Collections.Generic;

namespace RaceSimulator.Models;

public delegate void RaceStartedHandler(Race race);
public delegate void RaceFinishedHandler(Race race, RaceCar winner);
public delegate void LapCompletedHandler(Race race, RaceCar car, int lap);

public class Race
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RaceTrack Track { get; set; } = new();
    public List<RaceCar> Cars { get; set; } = new();
    public List<PitStop> PitStops { get; set; } = new();
    public List<Mechanic> Mechanics { get; set; } = new();
    public List<ILoader> Loaders { get; set; } = new();
    public bool IsRunning { get; set; }
    public bool IsFinished { get; set; }
    public DateTime StartTime { get; set; }
    public RaceCar? Winner { get; set; }

    public event RaceStartedHandler? RaceStarted;
    public event RaceFinishedHandler? RaceFinished;
    public event LapCompletedHandler? LapCompleted;

    public void Start()
    {
        IsRunning = true;
        StartTime = DateTime.Now;
        RaceStarted?.Invoke(this);
    }

    public void Finish(RaceCar winner)
    {
        IsRunning = false;
        IsFinished = true;
        Winner = winner;
        RaceFinished?.Invoke(this, winner);
    }

    public void NotifyLapCompleted(RaceCar car, int lap)
    {
        LapCompleted?.Invoke(this, car, lap);
    }
}
