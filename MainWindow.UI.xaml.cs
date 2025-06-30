using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Diamonds.Model;

namespace Diamonds;

public partial class MainWindow
{
    private void SetUpUi()
    {
        ResetColorInputs();
        ResetSizeInputs();
        ResetDisplayInputs();
        ResetHighlights();

        BindInputs();

        ApplyColorInputs();
        ApplyDimensionInputs();
        ApplyDisplayInputs();
        ApplyHighlights();
    }

    private void BindInputs()
    {
        BackgroundColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        MountingRimColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        DiamondColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        CanvasRimColorInput.SelectedColorChanged += OnAnyColorInputChanged;

        DiamondWidthInput.ValueChanged += OnAnyDimensionInputChanged;
        DiamondHeightInput.ValueChanged += OnAnyDimensionInputChanged;
        ColumnsInput.ValueChanged += OnAnyDimensionInputChanged;
        RowsInput.ValueChanged += OnAnyDimensionInputChanged;
        PaintingMarginInput.ValueChanged += OnAnyDimensionInputChanged;
        MountingRimSizeInput.ValueChanged += OnAnyDimensionInputChanged;
        OffsetXInput.ValueChanged += OnOffsetXChanged;
        OffsetYInput.ValueChanged += OnOffsetYChanged;

        ShowScalesInput.Checked += OnAnyDisplayInputChanged;
        ShowScalesInput.Unchecked += OnAnyDisplayInputChanged;
        OnlyPaintingInput.Checked += OnAnyDisplayInputChanged;
        OnlyPaintingInput.Unchecked += OnAnyDisplayInputChanged;
    }

    private void UnbindInputs()
    {
        BackgroundColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        MountingRimColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        DiamondColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        CanvasRimColorInput.SelectedColorChanged -= OnAnyColorInputChanged;

        DiamondWidthInput.ValueChanged -= OnAnyDimensionInputChanged;
        DiamondHeightInput.ValueChanged -= OnAnyDimensionInputChanged;
        ColumnsInput.ValueChanged -= OnAnyDimensionInputChanged;
        RowsInput.ValueChanged -= OnAnyDimensionInputChanged;
        PaintingMarginInput.ValueChanged -= OnAnyDimensionInputChanged;
        MountingRimSizeInput.ValueChanged -= OnAnyDimensionInputChanged;
        OffsetXInput.ValueChanged -= OnOffsetXChanged;
        OffsetYInput.ValueChanged -= OnOffsetYChanged;

        ShowScalesInput.Checked -= OnAnyDisplayInputChanged;
        ShowScalesInput.Unchecked -= OnAnyDisplayInputChanged;
        OnlyPaintingInput.Checked -= OnAnyDisplayInputChanged;
        OnlyPaintingInput.Unchecked -= OnAnyDisplayInputChanged;
    }

    private void OnOffsetXChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        var value = changeArgs.NewValue as int?;
        if (value >= _sizeSettings.DiamondWidth)
        {
            OffsetXInput.Value = 0;
            return;
        }

        if (value <= -_sizeSettings.DiamondWidth)
        {
            OffsetXInput.Value = 0;
            return;
        }

        OnAnyDimensionInputChanged(sender, changeArgs);
    }

    private void OnOffsetYChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        var value = changeArgs.NewValue as int?;
        if (value >= _sizeSettings.DiamondHeight)
        {
            OffsetXInput.Value = 0;
            return;
        }

        if (value <= -_sizeSettings.DiamondHeight)
        {
            OffsetXInput.Value = 0;
            return;
        }

        OnAnyDimensionInputChanged(sender, changeArgs);
    }

    private void OnAnyColorInputChanged(object sender, RoutedPropertyChangedEventArgs<Color?> changeArgs)
    {
        ApplyColorInputs();
    }

    private void OnAnyDimensionInputChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        ApplyDimensionInputs();
    }

    private void OnAnyDisplayInputChanged(object sender, RoutedEventArgs eventArgs)
    {
        ApplyDisplayInputs();
    }

    private void ApplyColorInputs()
    {
        var defaults = ColorSettings.Defaults;
        _colorSettings = new ColorSettings(
            BackgroundColorInput.SelectedColor ?? defaults.BackgroundColor,
            DiamondColorInput.SelectedColor ?? defaults.DiamondColor,
            CanvasRimColorInput.SelectedColor ?? defaults.CanvasRimColor,
            MountingRimColorInput.SelectedColor ?? defaults.MountingRimColor);

        ReDraw();
    }

    private void ApplyDimensionInputs()
    {
        var defaults = SizeSettings.Defaults;
        _sizeSettings = new SizeSettings(
            DiamondWidthInput.Value ?? defaults.DiamondWidth,
            DiamondHeightInput.Value ?? defaults.DiamondHeight,
            ColumnsInput.Value ?? defaults.GridColumns,
            RowsInput.Value ?? defaults.GridRows,
            PaintingMarginInput.Value ?? defaults.PaintingMargin,
            MountingRimSizeInput.Value ?? defaults.MountingRimSize,
            OffsetXInput.Value ?? defaults.OffsetX,
            OffsetYInput.Value ?? defaults.OffsetY);

        ReDraw();
    }

    private void ApplyDisplayInputs()
    {
        var defaults = DisplaySettings.Defaults;
        _displaySettings = new DisplaySettings(
            ShowScalesInput.IsChecked ?? defaults.ShowScales,
            OnlyPaintingInput.IsChecked ?? defaults.OnlyPattern
        );

        ReDraw();
    }

    private void ApplyHighlights()
    {
        ReDraw();
    }

    private void RefreshColorInputs()
    {
        BackgroundColorInput.SelectedColor = _colorSettings.BackgroundColor;
        DiamondColorInput.SelectedColor = _colorSettings.DiamondColor;
        CanvasRimColorInput.SelectedColor = _colorSettings.CanvasRimColor;
        MountingRimColorInput.SelectedColor = _colorSettings.MountingRimColor;
    }

    private void RefreshDimensionInputs()
    {
        DiamondWidthInput.Value = _sizeSettings.DiamondWidth;
        DiamondHeightInput.Value = _sizeSettings.DiamondHeight;
        ColumnsInput.Value = _sizeSettings.GridColumns;
        RowsInput.Value = _sizeSettings.GridRows;
        PaintingMarginInput.Value = _sizeSettings.PaintingMargin;
        MountingRimSizeInput.Value = _sizeSettings.MountingRimSize;
        OffsetXInput.Value = _sizeSettings.OffsetX;
        OffsetYInput.Value = _sizeSettings.OffsetY;
    }

    private void RefreshDisplayInputs()
    {
        ShowScalesInput.IsChecked = _displaySettings.ShowScales;
        OnlyPaintingInput.IsChecked = _displaySettings.OnlyPattern;
    }

    private void ResetColorInputs()
    {
        BackgroundColorInput.SelectedColor = ColorSettings.Defaults.BackgroundColor;
        MountingRimColorInput.SelectedColor = ColorSettings.Defaults.MountingRimColor;
        DiamondColorInput.SelectedColor = ColorSettings.Defaults.DiamondColor;
        CanvasRimColorInput.SelectedColor = ColorSettings.Defaults.CanvasRimColor;
    }

    private void ResetSizeInputs()
    {
        // settings the text instead of the value to not re-trigger change event
        DiamondWidthInput.Text = SizeSettings.Defaults.DiamondWidth.ToString(CultureInfo.CurrentCulture);
        DiamondHeightInput.Text = SizeSettings.Defaults.DiamondHeight.ToString(CultureInfo.CurrentCulture);
        ColumnsInput.Text = SizeSettings.Defaults.GridColumns.ToString();
        RowsInput.Text = SizeSettings.Defaults.GridRows.ToString();
        PaintingMarginInput.Text = SizeSettings.Defaults.PaintingMargin.ToString(CultureInfo.CurrentCulture);
        MountingRimSizeInput.Text = SizeSettings.Defaults.MountingRimSize.ToString(CultureInfo.CurrentCulture);
        OffsetXInput.Text = SizeSettings.Defaults.OffsetX.ToString(CultureInfo.CurrentCulture);
        OffsetYInput.Text = SizeSettings.Defaults.OffsetY.ToString(CultureInfo.CurrentCulture);
    }

    private void ResetDisplayInputs()
    {
        ShowScalesInput.IsChecked = DisplaySettings.Defaults.ShowScales;
        OnlyPaintingInput.IsChecked = DisplaySettings.Defaults.OnlyPattern;
    }

    private void ResetHighlights()
    {
        _highlightSettings.Highlights.Clear();
    }

    private void OnResetColorsButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetColorInputs();
    }

    private void OnResetSizesButtonClicked(object sender, RoutedEventArgs e)
    {
        ResetSizeInputs();
    }

    private void OnPngButtonClicked(object sender, RoutedEventArgs e)
    {
        SaveToPng();
    }

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        SaveToFile();
    }

    private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
    {
        LoadFromFile();
    }

    private void OnShortcutSave(object sender, ExecutedRoutedEventArgs e)
    {
        QuickSave();
    }
}