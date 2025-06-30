using System.Windows;
using System.Windows.Media;

namespace Diamonds.Model;

public readonly record struct Highlight(Point Position, Color Color, bool IsBackground);