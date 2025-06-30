using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Diamonds.Rendering;

public sealed class Diamond
{
    public Color Color { get; }
    public Point Center { get; }
    public Size Size { get; }

    public event Action? Clicked;
    public event Action? RightClicked;
    public event Action? WheelClicked;
    
    private readonly Polygon _shape;
    public Shape Shape => _shape;

    public Geometry Clip
    {
        get => _shape.Clip;
        set => _shape.Clip = value;
    }

    public Diamond(Point center, Size size, Color color)
    {
        Color = color;
        Center = center;
        Size = size;

        var halfWidth = Size.Width / 2;
        var halfHeight = Size.Height / 2;
        
        _shape = new Polygon
            { Points= [
            Center with { Y = Center.Y - halfHeight },
            Center with { X = Center.X + halfWidth },
            Center with { Y = Center.Y + halfHeight },
            Center with { X = Center.X - halfWidth }
            ],
        

        Fill = new SolidColorBrush(color),
            };
        
        _shape.MouseEnter += (_, _) =>
        {
            _shape.Stroke = new SolidColorBrush(MyColors.Darkest);
            _shape.StrokeThickness = 3;
        };
        _shape.MouseLeave += (_, _) =>
        {
            _shape.StrokeThickness = 0;
        };
        
        _shape.MouseUp += (_, clickEvent) =>
        {
            switch (clickEvent.ChangedButton)
            {
                case MouseButton.Right: RightClicked?.Invoke(); break;
                case MouseButton.Middle: WheelClicked?.Invoke(); break;
                case MouseButton.Left: Clicked?.Invoke(); break;
            }
        };
    }
}