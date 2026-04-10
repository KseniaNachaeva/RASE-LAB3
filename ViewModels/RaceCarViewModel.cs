using RaceSimulator.Models;

namespace RaceSimulator.ViewModels;

public class RaceCarViewModel : ViewModelBase
{
    private readonly RaceCar _car;
    private readonly double _trackLength;
    private readonly int _totalLaps;

    
    private const double TrackDisplayWidth = 680;
    private const double TrackDisplayHeight = 120;

    private string _name = string.Empty;
    private double _speed;
    private double _position;
    private double _tireWear;
    private bool _isInPit;
    private bool _hasCollided;
    private int _lapsCompleted;
    private double _visualX;
    private double _visualY;
    private string _statusColor = "#22CC44";
    private string _statusText = "Racing";

    public string Name
    {
        get => _name;
        private set => SetProperty(ref _name, value);
    }

    public double Speed
    {
        get => _speed;
        private set => SetProperty(ref _speed, value);
    }

    public double Position
    {
        get => _position;
        private set => SetProperty(ref _position, value);
    }

    public double TireWear
    {
        get => _tireWear;
        private set => SetProperty(ref _tireWear, value);
    }

    public bool IsInPit
    {
        get => _isInPit;
        private set => SetProperty(ref _isInPit, value);
    }

    public bool HasCollided
    {
        get => _hasCollided;
        private set => SetProperty(ref _hasCollided, value);
    }

    public int LapsCompleted
    {
        get => _lapsCompleted;
        private set => SetProperty(ref _lapsCompleted, value);
    }

    public double VisualX
    {
        get => _visualX;
        private set => SetProperty(ref _visualX, value);
    }

    public double VisualY
    {
        get => _visualY;
        private set => SetProperty(ref _visualY, value);
    }

    public string StatusColor
    {
        get => _statusColor;
        private set => SetProperty(ref _statusColor, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public string CarColor => _car.Color;

    public string DriverInitial => _name.Length > 0 ? _name[0].ToString() : "?";

    public RaceCarViewModel(RaceCar car, double trackLength, int totalLaps, int index)
    {
        _car = car;
        _trackLength = trackLength;
        _totalLaps = totalLaps;
        _visualY = 48 + (index % 6) * 11;
        Sync();
    }

    public void Sync()
    {
        Name = _car.Name;
        Speed = _car.Speed;
        Position = _car.Position;
        TireWear = _car.TireWear;
        IsInPit = _car.IsInPit;
        HasCollided = _car.HasCollided;
        LapsCompleted = _car.LapsCompleted;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        double totalDistance = _trackLength * _totalLaps;
        double progress = totalDistance > 0 ? _position / totalDistance : 0;
        if (progress > 1) progress = 1;

        VisualX = progress * TrackDisplayWidth;

        if (_hasCollided)
        {
            StatusColor = "#FF3333";
            StatusText = _isInPit ? "REPAIR" : "COLLISION";
        }
        else if (_isInPit)
        {
            StatusColor = "#FFA500";
            StatusText = "PIT";
        }
        else if (_tireWear > 75)
        {
            StatusColor = "#FFFF00";
            StatusText = "TIRES WORN";
        }
        else
        {
            StatusColor = "#22CC44";
            StatusText = "Racing";
        }
    }
}
