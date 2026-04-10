namespace RaceSimulator.Models;

public class RaceTrack
{
    public string Name { get; set; } = string.Empty;
    public double Length { get; set; }
    public int Laps { get; set; }
    public string TrackType { get; set; } = string.Empty;
    public double Difficulty { get; set; }
}
