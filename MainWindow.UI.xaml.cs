using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Diamonds.Rendering;

namespace Diamonds;

public partial class MainWindow
{
    private void SetUpUi()
    {
        ResetColorInputs();
        ResetSizeInputs();
        BindInputs();
        ApplyColorInputs();
        ApplyDimensionInputs();
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
    }

    private void OnAnyColorInputChanged(object _, RoutedPropertyChangedEventArgs<Color?> __)
    {
        ApplyColorInputs();
    }

    private void OnAnyDimensionInputChanged(object _, RoutedPropertyChangedEventArgs<object> __)
    {
        ApplyDimensionInputs();
    }

    private void ApplyColorInputs()
    {
        _currentColorSettings = new ColorSettings(
            BackgroundColorInput.SelectedColor ?? ColorSettings.Defaults.BackgroundColor,
            DiamondColorInput.SelectedColor ?? ColorSettings.Defaults.DiamondColor,
            CanvasRimColorInput.SelectedColor ?? ColorSettings.Defaults.CanvasRimColor,
            MountingRimColorInput.SelectedColor ?? ColorSettings.Defaults.MountingRimColor);

        ReDraw();
    }

    private void ApplyDimensionInputs()
    {
        _currentSizeSettings = new SizeSettings(
            DiamondWidthInput.Value ?? SizeSettings.Defaults.DiamondWidth,
            DiamondHeightInput.Value ?? SizeSettings.Defaults.DiamondHeight,
            ColumnsInput.Value ?? SizeSettings.Defaults.GridColumns,
            RowsInput.Value ?? SizeSettings.Defaults.GridRows,
            PaintingMarginInput.Value ?? SizeSettings.Defaults.PaintingMargin,
            MountingRimSizeInput.Value ?? SizeSettings.Defaults.MountingRimSize);

        ReDraw();
    }

    private void RefreshColorInputs()
    {
        BackgroundColorInput.SelectedColor = _currentColorSettings.BackgroundColor;
        DiamondColorInput.SelectedColor = _currentColorSettings.DiamondColor;
        CanvasRimColorInput.SelectedColor = _currentColorSettings.CanvasRimColor;
        MountingRimColorInput.SelectedColor = _currentColorSettings.MountingRimColor;
    }

    private void RefreshDimensionInputs()
    {
        DiamondWidthInput.Value = _currentSizeSettings.DiamondWidth;
        DiamondHeightInput.Value = _currentSizeSettings.DiamondHeight;
        ColumnsInput.Value = _currentSizeSettings.GridColumns;
        RowsInput.Value = _currentSizeSettings.GridRows;
        PaintingMarginInput.Value = _currentSizeSettings.PaintingMargin;
        MountingRimSizeInput.Value = _currentSizeSettings.MountingRimSize;
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
        DiamondWidthInput.Text = SizeSettings.Defaults.DiamondWidth.ToString(CultureInfo.CurrentCulture);
        DiamondHeightInput.Text = SizeSettings.Defaults.DiamondHeight.ToString(CultureInfo.CurrentCulture);
        ColumnsInput.Text = SizeSettings.Defaults.GridColumns.ToString();
        RowsInput.Text = SizeSettings.Defaults.GridRows.ToString();
        PaintingMarginInput.Text = SizeSettings.Defaults.PaintingMargin.ToString(CultureInfo.CurrentCulture);
        MountingRimSizeInput.Text = SizeSettings.Defaults.MountingRimSize.ToString(CultureInfo.CurrentCulture);
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