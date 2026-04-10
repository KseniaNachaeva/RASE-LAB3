using System;

namespace RaceSimulator.Models;

public class PitStop
{
    public int CarId { get; set; }
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string Reason { get; set; } = string.Empty;

    public double DurationSeconds =>
        ExitTime.HasValue ? (ExitTime.Value - EntryTime).TotalSeconds : 0;
}
