using Avalonia.Controls;
using Avalonia.Interactivity;
using RaceSimulator.ViewModels;

namespace RaceSimulator.Views;

public partial class RaceView : UserControl
{
    public RaceView()
    {
        InitializeComponent();
    }

    private void OnRemoveClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is RaceViewModel vm)
            vm.RequestRemove();
    }
}
