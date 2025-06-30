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
        BindInputs();

        ApplyColorInputs();
        ApplyDimensionInputs();
        ApplyDisplayInputs();
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
        if (value >= _model.SizeSettings.DiamondWidth)
        {
            OffsetXInput.Value = 0;
            return;
        }

        if (value <= -_model.SizeSettings.DiamondWidth)
        {
            OffsetXInput.Value = 0;
            return;
        }

        OnAnyDimensionInputChanged(sender, changeArgs);
    }

    private void OnOffsetYChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        var value = changeArgs.NewValue as int?;
        if (value >= _model.SizeSettings.DiamondHeight)
        {
            OffsetXInput.Value = 0;
            return;
        }

        if (value <= -_model.SizeSettings.DiamondHeight)
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
        _model.ColorSettings = new ColorSettings(
            BackgroundColorInput.SelectedColor ?? defaults.BackgroundColor,
            DiamondColorInput.SelectedColor ?? defaults.DiamondColor,
            CanvasRimColorInput.SelectedColor ?? defaults.CanvasRimColor,
            MountingRimColorInput.SelectedColor ?? defaults.MountingRimColor);

        ReDraw();
    }

    private void ApplyDimensionInputs()
    {
        var defaults = SizeSettings.Defaults;
        _model.SizeSettings = new SizeSettings(
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
        _model.DisplaySettings = new DisplaySettings(
            ShowScalesInput.IsChecked ?? defaults.ShowScales,
            OnlyPaintingInput.IsChecked ?? defaults.OnlyPattern
        );

        ReDraw();
    }

    private void RefreshColorInputs()
    {
        var colorSettings = _model.ColorSettings;
        BackgroundColorInput.SelectedColor = colorSettings.BackgroundColor;
        DiamondColorInput.SelectedColor = colorSettings.DiamondColor;
        CanvasRimColorInput.SelectedColor = colorSettings.CanvasRimColor;
        MountingRimColorInput.SelectedColor = colorSettings.MountingRimColor;
    }

    private void RefreshDimensionInputs()
    {
        var sizeSettings = _model.SizeSettings;
        DiamondWidthInput.Value = sizeSettings.DiamondWidth;
        DiamondHeightInput.Value = sizeSettings.DiamondHeight;
        ColumnsInput.Value = sizeSettings.GridColumns;
        RowsInput.Value = sizeSettings.GridRows;
        PaintingMarginInput.Value = sizeSettings.PaintingMargin;
        MountingRimSizeInput.Value = sizeSettings.MountingRimSize;
        OffsetXInput.Value = sizeSettings.OffsetX;
        OffsetYInput.Value = sizeSettings.OffsetY;
    }

    private void RefreshDisplayInputs()
    {
        var displaySettings = _model.DisplaySettings;
        ShowScalesInput.IsChecked = displaySettings.ShowScales;
        OnlyPaintingInput.IsChecked = displaySettings.OnlyPattern;
    }

    private void ResetColorInputs()
    {
        _model.ResetColors();
        _model.ResetHighlights();

        RefreshColorInputs();
    }

    private void ResetSizeInputs()
    {
        _model.ResetSizes();
        var sizeSettings = _model.SizeSettings;

        DiamondWidthInput.Text = sizeSettings.DiamondWidth.ToString(CultureInfo.CurrentCulture);
        DiamondHeightInput.Text = sizeSettings.DiamondHeight.ToString(CultureInfo.CurrentCulture);
        ColumnsInput.Text = sizeSettings.GridColumns.ToString();
        RowsInput.Text = sizeSettings.GridRows.ToString();
        PaintingMarginInput.Text = sizeSettings.PaintingMargin.ToString(CultureInfo.CurrentCulture);
        MountingRimSizeInput.Text = sizeSettings.MountingRimSize.ToString(CultureInfo.CurrentCulture);
        OffsetXInput.Text = sizeSettings.OffsetX.ToString(CultureInfo.CurrentCulture);
        OffsetYInput.Text = sizeSettings.OffsetY.ToString(CultureInfo.CurrentCulture);
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
        _fileManager.SaveToPng(MainCanvas);
    }

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        _fileManager.Save();
    }

    private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
    {
        var loadResult = _fileManager.Load();

        if (!loadResult)
            return;

        UnbindInputs();
        RefreshColorInputs();
        RefreshDimensionInputs();
        RefreshDisplayInputs();
        BindInputs();
        ReDraw();
    }

    private void OnShortcutSave(object sender, ExecutedRoutedEventArgs e)
    {
        _fileManager.QuickSave();
    }
}