namespace RaceSimulator.Models;

public delegate void TiresWornHandler(RaceCar car);
public delegate void CollisionHandler(RaceCar car);

public class RaceCar
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Speed { get; set; }
    public double Position { get; set; }
    public double TireWear { get; set; }
    public bool IsInPit { get; set; }
    public bool HasCollided { get; set; }
    public int LapsCompleted { get; set; }
    public string Color { get; set; } = "#FF0000";
    public double CollisionRisk { get; set; }

    public event TiresWornHandler? TiresWorn;
    public event CollisionHandler? CollisionOccurred;

    public void UpdateTireWear(double amount)
    {
        TireWear += amount;
        if (TireWear >= 100)
        {
            TireWear = 100;
            TiresWorn?.Invoke(this);
        }
    }

    public void TriggerBarrierCollision()
    {
        HasCollided = true;
        CollisionOccurred?.Invoke(this);
    }

    public void RepairAfterPit()
    {
        TireWear = 0;
        HasCollided = false;
        IsInPit = false;
    }
}
