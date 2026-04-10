using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RaceSimulator.Models;

namespace RaceSimulator.Services;

public class RaceSimulationService
{
    private static readonly Random _random = new();
    private readonly Race _race;
    private CancellationTokenSource? _cts;

    public event Action<Race>? RaceUpdated;

    public RaceSimulationService(Race race)
    {
        _race = race;
    }

    public async Task StartAsync()
    {
        _cts = new CancellationTokenSource();
        _race.Start();
        await SimulateAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task SimulateAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && !_race.IsFinished)
        {
            try
            {
                await Task.Delay(50, token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            UpdateCars();
            CheckCollisions();
            HandlePitStops();
            CheckFinish();
            RaceUpdated?.Invoke(_race);
        }
    }

    private void UpdateCars()
    {
        double trackTotal = _race.Track.Length * _race.Track.Laps;

        foreach (var car in _race.Cars)
        {
            if (car.IsInPit || car.Position >= trackTotal) continue;

            double speedFactor = Math.Max(0.2, (100.0 - car.TireWear) / 100.0);
            double delta = car.Speed * speedFactor * 0.05;

            double prevLapPos = car.Position / _race.Track.Length;
            car.Position += delta;
            double newLapPos = car.Position / _race.Track.Length;

            int prevLap = (int)prevLapPos;
            int newLap = (int)newLapPos;
            if (newLap > prevLap && newLap <= _race.Track.Laps)
            {
                car.LapsCompleted = newLap;
                _race.NotifyLapCompleted(car, newLap);
            }

            double wearRate = (0.1 + _random.NextDouble() * 0.3) * _race.Track.Difficulty;
            car.UpdateTireWear(wearRate);
        }
    }

    private void CheckCollisions()
    {
        foreach (var car in _race.Cars)
        {
            if (car.IsInPit || car.HasCollided) continue;
            if (_random.NextDouble() < car.CollisionRisk)
                car.TriggerBarrierCollision();
        }
    }

    private void HandlePitStops()
    {
        foreach (var car in _race.Cars)
        {
            bool needsPit = car.TireWear >= 100 || car.HasCollided;
            if (car.IsInPit || !needsPit) continue;

            car.IsInPit = true;
            car.TireWear = 100;

            var pitStop = new PitStop
            {
                CarId = car.Id,
                EntryTime = DateTime.Now,
                Reason = car.HasCollided ? "Collision Repair" : "Tire Change"
            };
            _race.PitStops.Add(pitStop);

            var capturedCar = car;
            var capturedStop = pitStop;

            if (car.HasCollided)
            {
                var loader = _race.Loaders.Find(l => l.IsAvailable);
                if (loader != null)
                {
                    Task.Run(() => loader.HandleCollisionAsync(capturedCar, capturedStop));
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(6000 + _random.Next(2000));
                        capturedCar.RepairAfterPit();
                        capturedStop.ExitTime = DateTime.Now;
                    });
                }
            }
            else
            {
                var mechanic = _race.Mechanics.Find(m => !m.IsBusy);
                if (mechanic != null)
                {
                    Task.Run(() => mechanic.ChangeTiresAsync(capturedCar, capturedStop));
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(4000 + _random.Next(2000));
                        capturedCar.RepairAfterPit();
                        capturedStop.ExitTime = DateTime.Now;
                    });
                }
            }
        }
    }

    private void CheckFinish()
    {
        double trackTotal = _race.Track.Length * _race.Track.Laps;
        var winner = _race.Cars
            .Where(c => !c.IsInPit)
            .FirstOrDefault(c => c.Position >= trackTotal);

        if (winner != null)
        {
            _race.Finish(winner);
        }
    }
}
