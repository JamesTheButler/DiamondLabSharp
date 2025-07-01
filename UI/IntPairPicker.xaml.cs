using System.Windows;
using Diamonds.Operation;

namespace Diamonds.UI;

public partial class IntPairPicker
{
    public IntPairPicker()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnInputChanged(object? sender, RoutedPropertyChangedEventArgs<object> e)
    {
        var newSize = new IntPair(WidthInput.Value ?? 0, HeightInput.Value ?? 0);
        Value = newSize;
        RaiseEvent(new RoutedPropertyChangedEventArgs<IntPair>(default, newSize, ValueChangedEvent));
    }

    // Event exposed to consumers
    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<IntPair>),
            typeof(IntPairPicker));

    public event RoutedPropertyChangedEventHandler<IntPair> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(IntPair),
            typeof(IntPairPicker),
            new PropertyMetadata(new IntPair(1, 1), OnValueChanged));

    public IntPair Value
    {
        get => (IntPair)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is IntPairPicker picker && e.NewValue is IntPair size)
        {
            picker.WidthInput.Value = size.X;
            picker.HeightInput.Value = size.Y;
        }
    }

    // WidthMin
    public static readonly DependencyProperty WidthMinProperty =
        DependencyProperty.Register(nameof(WidthMin), typeof(int), typeof(IntPairPicker), new PropertyMetadata(0));

    public int WidthMin
    {
        get => (int)GetValue(WidthMinProperty);
        set => SetValue(WidthMinProperty, value);
    }

    // WidthMax
    public static readonly DependencyProperty WidthMaxProperty =
        DependencyProperty.Register(nameof(WidthMax), typeof(int), typeof(IntPairPicker), new PropertyMetadata(1000));

    public int WidthMax
    {
        get => (int)GetValue(WidthMaxProperty);
        set => SetValue(WidthMaxProperty, value);
    }

    // HeightMin
    public static readonly DependencyProperty HeightMinProperty =
        DependencyProperty.Register(nameof(HeightMin), typeof(int), typeof(IntPairPicker), new PropertyMetadata(0));

    public int HeightMin
    {
        get => (int)GetValue(HeightMinProperty);
        set => SetValue(HeightMinProperty, value);
    }

    // HeightMax
    public static readonly DependencyProperty HeightMaxProperty =
        DependencyProperty.Register(nameof(HeightMax), typeof(int), typeof(IntPairPicker), new PropertyMetadata(1000));

    public int HeightMax
    {
        get => (int)GetValue(HeightMaxProperty);
        set => SetValue(HeightMaxProperty, value);
    }

    // Unit Text
    public static readonly DependencyProperty UnitProperty =
        DependencyProperty.Register(nameof(Unit), typeof(string), typeof(IntPairPicker),
            new PropertyMetadata("unit"));

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(IntPairPicker));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty XLabelProperty =
        DependencyProperty.Register(nameof(XLabel), typeof(string), typeof(IntPairPicker),
            new PropertyMetadata("X"));

    public string XLabel
    {
        get => (string)GetValue(XLabelProperty);
        set => SetValue(XLabelProperty, value);
    }

    public static readonly DependencyProperty YLabelProperty =
        DependencyProperty.Register(nameof(YLabel), typeof(string), typeof(IntPairPicker),
            new PropertyMetadata("Y"));

    public string YLabel
    {
        get => (string)GetValue(YLabelProperty);
        set => SetValue(YLabelProperty, value);
    }
}