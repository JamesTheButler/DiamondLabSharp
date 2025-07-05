using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Diamonds.Model;
using Diamonds.Operation;

namespace Diamonds;

public partial class MainWindow
{
    private void SetUpUi()
    {
        BindInputs();

        RefreshAllInputs();

        MainCanvas.Loaded += (_, _) => { Render(); };
    }

    private void RefreshAllInputs()
    {
        RefreshColorInputs();
        RefreshSizeInputs();
        RefreshDisplayInputs();
        RefreshFrameSizeInputs();
        RefreshFrameColorInputs();
    }

    private void BindInputs()
    {
        BackgroundColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        MountingRimColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        DiamondColorInput.SelectedColorChanged += OnAnyColorInputChanged;
        CanvasRimColorInput.SelectedColorChanged += OnAnyColorInputChanged;

        PaintingMarginInput.ValueChanged += OnAnyDimensionInputChanged;
        MountingRimSizeInput.ValueChanged += OnAnyDimensionInputChanged;
        OffsetXInput.ValueChanged += OnOffsetXChanged;
        OffsetYInput.ValueChanged += OnOffsetYChanged;

        ShowScalesInput.Checked += OnAnyDisplayInputChanged;
        ShowScalesInput.Unchecked += OnAnyDisplayInputChanged;
        OnlyPaintingInput.Checked += OnAnyDisplayInputChanged;
        OnlyPaintingInput.Unchecked += OnAnyDisplayInputChanged;
        ShowDebugLinesInput.Checked += OnAnyDisplayInputChanged;
        ShowDebugLinesInput.Unchecked += OnAnyDisplayInputChanged;
        ShowFrameInput.Checked += OnAnyDisplayInputChanged;
        ShowFrameInput.Unchecked += OnAnyDisplayInputChanged;
        ShowExplodedFrameInput.Checked += OnAnyDisplayInputChanged;
        ShowExplodedFrameInput.Unchecked += OnAnyDisplayInputChanged;

        StructuralLayerColorInput.SelectedColorChanged += OnAnyFrameColorInputChanged;
        DecorativeLayer1ColorInput.SelectedColorChanged += OnAnyFrameColorInputChanged;
        DecorativeLayer2ColorInput.SelectedColorChanged += OnAnyFrameColorInputChanged;

        StructuralLayerSizeInput.ValueChanged += OnAnyFrameSizeInputChanged;
        DecorativeLayer1SizeInput.ValueChanged += OnAnyFrameSizeInputChanged;
        DecorativeLayer2SizeInput.ValueChanged += OnAnyFrameSizeInputChanged;
    }

    private void UnbindInputs()
    {
        BackgroundColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        MountingRimColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        DiamondColorInput.SelectedColorChanged -= OnAnyColorInputChanged;
        CanvasRimColorInput.SelectedColorChanged -= OnAnyColorInputChanged;

        PaintingMarginInput.ValueChanged -= OnAnyDimensionInputChanged;
        MountingRimSizeInput.ValueChanged -= OnAnyDimensionInputChanged;
        OffsetXInput.ValueChanged -= OnOffsetXChanged;
        OffsetYInput.ValueChanged -= OnOffsetYChanged;

        ShowScalesInput.Checked -= OnAnyDisplayInputChanged;
        ShowScalesInput.Unchecked -= OnAnyDisplayInputChanged;
        OnlyPaintingInput.Checked -= OnAnyDisplayInputChanged;
        OnlyPaintingInput.Unchecked -= OnAnyDisplayInputChanged;
        ShowDebugLinesInput.Checked -= OnAnyDisplayInputChanged;
        ShowDebugLinesInput.Unchecked -= OnAnyDisplayInputChanged;
        ShowFrameInput.Checked -= OnAnyDisplayInputChanged;
        ShowFrameInput.Unchecked -= OnAnyDisplayInputChanged;
        ShowExplodedFrameInput.Checked -= OnAnyDisplayInputChanged;
        ShowExplodedFrameInput.Unchecked -= OnAnyDisplayInputChanged;

        StructuralLayerColorInput.SelectedColorChanged -= OnAnyFrameColorInputChanged;
        DecorativeLayer1ColorInput.SelectedColorChanged -= OnAnyFrameColorInputChanged;
        DecorativeLayer2ColorInput.SelectedColorChanged -= OnAnyFrameColorInputChanged;

        StructuralLayerSizeInput.ValueChanged -= OnAnyFrameSizeInputChanged;
        DecorativeLayer1SizeInput.ValueChanged -= OnAnyFrameSizeInputChanged;
        DecorativeLayer2SizeInput.ValueChanged -= OnAnyFrameSizeInputChanged;
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

    private void OnAnyFrameColorInputChanged(object sender, RoutedPropertyChangedEventArgs<Color?> changeArgs)
    {
        ApplyFrameColorInputs();
    }

    private void OnAnyDimensionInputChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        ApplyDimensionInputs();
    }

    private void OnAnyDisplayInputChanged(object sender, RoutedEventArgs eventArgs)
    {
        ApplyDisplayInputs();
    }

    private void OnAnyFrameSizeInputChanged(object sender, RoutedPropertyChangedEventArgs<object> changeArgs)
    {
        ApplyFrameSizeInputs();
    }

    private void ApplyColorInputs()
    {
        var defaults = ColorSettings.Defaults;
        _model.ColorSettings = new ColorSettings(
            BackgroundColorInput.SelectedColor ?? defaults.BackgroundColor,
            DiamondColorInput.SelectedColor ?? defaults.DiamondColor,
            CanvasRimColorInput.SelectedColor ?? defaults.CanvasRimColor,
            MountingRimColorInput.SelectedColor ?? defaults.MountingRimColor);

        Render();
    }

    private void ApplyDimensionInputs()
    {
        _model.SizeSettings = new SizeSettings(
            DiamondSizeInput.Value.X,
            DiamondSizeInput.Value.Y,
            GridSizeInput.Value.X,
            GridSizeInput.Value.Y,
            PaintingMarginInput.Value,
            MountingRimSizeInput.Value,
            OffsetXInput.Value,
            OffsetYInput.Value);

        Render();
    }

    private void ApplyFrameSizeInputs()
    {
        _model.FrameSizeSettings = new FrameSizeSettings(
            StructuralLayerSizeInput.Value,
            DecorativeLayer1SizeInput.Value,
            DecorativeLayer2SizeInput.Value
        );

        Render();
    }

    private void ApplyFrameColorInputs()
    {
        var defaults = FrameColorSettings.Defaults;
        _model.FrameColorSettings = new FrameColorSettings(
            StructuralLayerColorInput.SelectedColor ?? defaults.StructuralLayerColor,
            DecorativeLayer1ColorInput.SelectedColor ?? defaults.DecorativeLayer1Color,
            DecorativeLayer2ColorInput.SelectedColor ?? defaults.DecorativeLayer2Color
        );

        Render();
    }

    private void ApplyDisplayInputs()
    {
        var defaults = DisplaySettings.Defaults;
        _model.DisplaySettings = new DisplaySettings(
            ShowScalesInput.IsChecked ?? defaults.ShowScales,
            OnlyPaintingInput.IsChecked ?? defaults.OnlyPattern,
            ShowDebugLinesInput.IsChecked ?? defaults.ShowDebugLines,
            ShowFrameInput.IsChecked ?? defaults.ShowFrame,
            ShowExplodedFrameInput.IsChecked ?? defaults.ShowExplodedFrame
        );

        Render();
    }

    private void RefreshColorInputs()
    {
        var colorSettings = _model.ColorSettings;
        BackgroundColorInput.SelectedColor = colorSettings.BackgroundColor;
        DiamondColorInput.SelectedColor = colorSettings.DiamondColor;
        CanvasRimColorInput.SelectedColor = colorSettings.CanvasRimColor;
        MountingRimColorInput.SelectedColor = colorSettings.MountingRimColor;
    }

    private void RefreshSizeInputs()
    {
        var sizeSettings = _model.SizeSettings;
        DiamondSizeInput.Value = new IntPair(sizeSettings.DiamondWidth, sizeSettings.DiamondHeight);
        GridSizeInput.Value = new IntPair(sizeSettings.GridColumns, sizeSettings.GridRows);
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
        ShowDebugLinesInput.IsChecked = displaySettings.ShowDebugLines;
        ShowFrameInput.IsChecked = displaySettings.ShowFrame;
        ShowExplodedFrameInput.IsChecked = displaySettings.ShowExplodedFrame;
    }

    private void RefreshFrameSizeInputs()
    {
        var frameSizes = _model.FrameSizeSettings;
        StructuralLayerSizeInput.Value = frameSizes.StructuralLayerWidth;
        DecorativeLayer1SizeInput.Value = frameSizes.DecorativeLayer1Width;
        DecorativeLayer2SizeInput.Value = frameSizes.DecorativeLayer2Width;
    }

    private void RefreshFrameColorInputs()
    {
        var frameColors = _model.FrameColorSettings;
        StructuralLayerColorInput.SelectedColor = frameColors.StructuralLayerColor;
        DecorativeLayer1ColorInput.SelectedColor = frameColors.DecorativeLayer1Color;
        DecorativeLayer2ColorInput.SelectedColor = frameColors.DecorativeLayer2Color;
    }

    private void OnResetColorsButtonClicked(object sender, RoutedEventArgs e)
    {
        _model.ResetColors();
        _model.ResetHighlights();

        RefreshColorInputs();
    }

    private void OnResetSizesButtonClicked(object sender, RoutedEventArgs e)
    {
        _model.ResetSizes();

        RefreshSizeInputs();
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
        RefreshAllInputs();
        BindInputs();

        Render();
    }

    private void OnShortcutSave(object sender, ExecutedRoutedEventArgs e)
    {
        _fileManager.QuickSave();
    }

    private void OnExitButtonClicked(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void OnSizePickerInputChanged(object sender, RoutedPropertyChangedEventArgs<IntPair> changeEvent)
    {
        ApplyDimensionInputs();
    }

    private void OnResetFrameColorsButtonClicked(object sender, RoutedEventArgs e)
    {
        _model.ResetFrameColors();
        RefreshFrameColorInputs();
    }

    private void OnResetFrameSizesButtonClicked(object sender, RoutedEventArgs e)
    {
        _model.ResetFrameDimensions();
        RefreshFrameSizeInputs();
    }
}