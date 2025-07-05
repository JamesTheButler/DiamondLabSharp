using System.Windows;

namespace DiamondLab.UI;

public partial class IntInput
{
    public IntInput()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnInputChanged(object? sender, RoutedPropertyChangedEventArgs<object> eventArgs)
    {
        var old = Value;
        if (eventArgs.NewValue is not int newValue)
            return;
        Value = newValue;

        RaiseEvent(new RoutedPropertyChangedEventArgs<object>(old, newValue, ValueChangedEvent));
    }

    // Event exposed to consumers
    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(ValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<object>),
            typeof(IntInput));

    public event RoutedPropertyChangedEventHandler<object> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(IntInput),
            new PropertyMetadata(null, OnValueChanged));

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    // Min
    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register(nameof(Min), typeof(int), typeof(IntInput), new PropertyMetadata(0));

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    // Max
    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register(nameof(Max), typeof(int), typeof(IntInput), new PropertyMetadata(1000));

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    // Unit Text
    public static readonly DependencyProperty UnitProperty =
        DependencyProperty.Register(nameof(Unit), typeof(string), typeof(IntInput),
            new PropertyMetadata("unit"));

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(IntInput));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}