using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Diamonds.Model;

namespace Diamonds;

public sealed class HighlightViewModel : INotifyPropertyChanged
{
    private int _x = 1;
    private int _y = 3;
    private Color _color;
    private bool _isBackground;

    public int X { get => _x; set { _x = value; OnPropertyChanged(); } }
    public int Y { get => _y; set { _y = value; OnPropertyChanged(); } }
    public Color Color { get => _color; set { _color = value; OnPropertyChanged(); } }
    public bool IsBackground { get => _isBackground; set { _isBackground = value; OnPropertyChanged(); } }
    
    public int Min => 0;
    public int Max => 10;

    public Highlight Highlight => new(new(X, Y), Color, false);
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}