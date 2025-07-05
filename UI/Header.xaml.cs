using System.Windows;

namespace DiamondLab.UI;

public partial class Header
{
    public Header()
    {
        InitializeComponent();
        DataContext = this;
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(Header));

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty ButtonContentProperty =
        DependencyProperty.Register(nameof(ButtonContent), typeof(object), typeof(Header));

    public object ButtonContent
    {
        get => GetValue(ButtonContentProperty);
        set => SetValue(ButtonContentProperty, value);
    }

    public event RoutedEventHandler? ButtonClick;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ButtonClick?.Invoke(this, e);
    }

    public static readonly DependencyProperty IsButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsButtonVisible), typeof(bool), typeof(Header), new PropertyMetadata(true));

    public bool IsButtonVisible
    {
        get => (bool)GetValue(IsButtonVisibleProperty);
        set => SetValue(IsButtonVisibleProperty, value);
    }
}