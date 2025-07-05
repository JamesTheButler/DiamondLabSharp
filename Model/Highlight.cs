using System.Windows;
using System.Windows.Media;

namespace DiamondLab.Model;

public readonly record struct Highlight(Point Position, Color Color, bool IsBackground);