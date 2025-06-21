using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Diamonds.Rendering;
using Diamonds.Rendering.AxisScale;
using Diamonds.Utilities;
using Microsoft.Win32;

namespace Diamonds;

public partial class MainWindow
{
    private const int InfoBarThickness = 50;
    
    private readonly FileManager _fileManager = FileManager.Instance;
    
    private SizeSettings _currentSizeSettings = SizeSettings.Defaults;
    private ColorSettings _currentColorSettings = ColorSettings.Defaults;
    
    private readonly Thickness _paintingMargin = new(10);
    private readonly Thickness _canvasMargin = new(10,10, 0, 0);
    
    public MainWindow()
    {
        InitializeComponent();
        SetUpUi();
        ReDraw();
    }

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

    private void ReDraw()
    {
        MainCanvas.Children.Clear();
        
        var paintingSizeSettings = _currentSizeSettings;
        var colorSettings = _currentColorSettings;

        var canvasSize = paintingSizeSettings.PaintingSize
            .Add(_canvasMargin)
            .Add(_paintingMargin)
            .Add(InfoBarThickness);
        
        MainCanvas.Width = canvasSize.Width;
        MainCanvas.Height = canvasSize.Height;
        MainCanvas.Background = new SolidColorBrush(MyColors.Background);

        var paintingOrigin = new Point(
            _canvasMargin.Left + InfoBarThickness + _paintingMargin.Left, 
            _canvasMargin.Top + InfoBarThickness + _paintingMargin.Top);

        DrawCanvasBackground(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawScales(paintingOrigin, paintingSizeSettings);
        DrawMountingRim(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawPaintingBackground(paintingOrigin, paintingSizeSettings, colorSettings);
        DrawDiamondPattern(paintingOrigin, paintingSizeSettings, colorSettings);
    }

    private void DrawScales(Point paintingOrigin, SizeSettings sizeSettings)
    {
        GetDiamondTicks(sizeSettings, 
            out var horizontalTickPositions, 
            out var verticalTickPositions);

        var rim = sizeSettings.MountingRimSize;
        var totalMargin = rim + sizeSettings.PaintingMargin;
        
        var horizontalTicks = horizontalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]\n{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - totalMargin, new FormatterLabel(p => $"[R]\n{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Width - rim, new FormatterLabel(p => $"[M]\n{p:0,#}")))
            .ToArray();

        var horizontalBar = new AxisScale(sizeSettings.PaintingSize.Width, Orientation.Horizontal)
        {
            Ticks = horizontalTicks,
        };
        Canvas.SetLeft(horizontalBar, paintingOrigin.X);
        Canvas.SetTop(horizontalBar, 0);
        MainCanvas.Children.Add(horizontalBar);
        
        var verticalTicks = verticalTickPositions
            .Select(pos => new AxisScaleTick(pos, new FormatterLabel(p => $"[D]{p:0,#}")))
            .Append(new AxisScaleTick(rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .Append(new AxisScaleTick(totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - totalMargin, new FormatterLabel(p => $"[R]{p:0,#}")))
            .Append(new AxisScaleTick(sizeSettings.PaintingSize.Height - rim, new FormatterLabel(p => $"[M]{p:0,#}")))
            .ToArray();
        var verticalBar = new AxisScale(sizeSettings.PaintingSize.Height, Orientation.Vertical)
        {
            Ticks = verticalTicks,
        };
        
        Canvas.SetLeft(verticalBar, 0);
        Canvas.SetTop(verticalBar, paintingOrigin.Y);
        MainCanvas.Children.Add(verticalBar);
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

    private void DrawCanvasBackground(Point origin, SizeSettings canvasSize, ColorSettings colors)
    {
        var canvasBackground = new Rectangle
        {
            Width = canvasSize.PaintingSize.Width,
            Height = canvasSize.PaintingSize.Height,
            Fill = new SolidColorBrush(colors.CanvasRimColor),
            Stroke = new SolidColorBrush(colors.MountingRimColor)
        };
        Canvas.SetLeft(canvasBackground, origin.X);
        Canvas.SetTop(canvasBackground, origin.Y);
        MainCanvas.Children.Add(canvasBackground);
    }

    private void DrawMountingRim(Point origin, SizeSettings size, ColorSettings colors)
    {
        var paintingSize = size.PaintingSize;
        var actualMountingRimColor = colors.MountingRimColor;
        actualMountingRimColor.A = 128;
        var mountingRim = new Polygon
        {
            Points =
            [
                new Point(0, 0),
                new Point(0, paintingSize.Height),
                new Point(paintingSize.Width, paintingSize.Height),
                new Point(paintingSize.Width, 0),
                new Point(size.MountingRimSize, 0),
                new Point(size.MountingRimSize, size.MountingRimSize),
                new Point(paintingSize.Width - size.MountingRimSize, size.MountingRimSize),
                new Point(paintingSize.Width - size.MountingRimSize, paintingSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, paintingSize.Height - size.MountingRimSize),
                new Point(size.MountingRimSize, 0),
                new Point(0, 0),
                
            ],
            Fill = new SolidColorBrush(actualMountingRimColor),
        };
        Canvas.SetLeft(mountingRim, origin.X);
        Canvas.SetTop(mountingRim, origin.Y);
        MainCanvas.Children.Add(mountingRim);
        
        var mountingRimOutline = new Rectangle
        {
            Width = paintingSize.Width - 2 * size.MountingRimSize,
            Height = paintingSize.Height - 2 * size.MountingRimSize,
            Stroke = new SolidColorBrush(colors.MountingRimColor),
            StrokeThickness = 1,
        };
        Canvas.SetLeft(mountingRimOutline, origin.X + size.MountingRimSize);
        Canvas.SetTop(mountingRimOutline, origin.Y + size.MountingRimSize);
        MainCanvas.Children.Add(mountingRimOutline);
    }

    private void DrawPaintingBackground(Point origin, SizeSettings size, ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.PaintingMargin;
        var paintedBackground = new Rectangle
        {
            Width = size.GridColumns * size.DiamondWidth,
            Height = size.GridRows * size.DiamondHeight,
            Fill = new SolidColorBrush(colors.BackgroundColor)
        };
        Canvas.SetLeft(paintedBackground, origin.X + offset);
        Canvas.SetTop(paintedBackground, origin.Y + offset);
        MainCanvas.Children.Add(paintedBackground);
    }
    
    private void DrawDiamondPattern(Point origin, SizeSettings size,  ColorSettings colors)
    {
        var offset = size.MountingRimSize + size.PaintingMargin;
        for (var row = 0; row < size.GridRows; row++)
        {
            for (var col = 0; col < size.GridColumns; col++)
            {
                var cx = origin.X  + offset + col * size.DiamondWidth + size.DiamondWidth * .5;
                var cy = origin.Y  + offset + row * size.DiamondHeight + size.DiamondHeight * .5;

                var diamond = new Polygon
                {
                    Points =
                    [
                        new Point(cx, cy - size.DiamondHeight * .5) ,
                        new Point(cx + size.DiamondWidth * .5, cy) ,
                        new Point(cx, cy + size.DiamondHeight * .5) ,
                        new Point(cx - size.DiamondWidth * .5, cy) 
                    ],
                    Fill = new SolidColorBrush(colors.DiamondColor),
                };
                MainCanvas.Children.Add(diamond);
            }
        }
    }

    private void GetDiamondTicks(SizeSettings size, out double[] horizontalTicks, out double[] verticalTicks)
    {
        verticalTicks = new double[size.GridRows];
        horizontalTicks = new double[size.GridColumns];
        
        var offset = size.MountingRimSize + size.PaintingMargin;
        for (var row = 0; row < size.GridRows; row++)
        {
            verticalTicks[row] = offset + size.DiamondHeight * .5 + size.DiamondHeight * row;
        }
        for (var col = 0; col < size.GridColumns; col++)
        {
            horizontalTicks[col] = offset + size.DiamondWidth * .5 + size.DiamondWidth * col;
        }
    }
    
    #region Event Handlers

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
        var dialogFileName = _fileManager.GetActiveFileName() ?? FileManagementDefaults.FileName;
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;
        
        var dialog = new SaveFileDialog
        {
            Filter = "PNG Files (*.png)|*.png",
            DefaultExt = ".png",
            FileName = $"{dialogFileName}.png",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return;
        
        FileOperations.SaveAsPng(MainCanvas, dialog.FileName);
    }

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        var dialogFileName = _fileManager.GetActiveFileName() ?? FileManagementDefaults.FileName;
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;

        const string extension = FileManagementDefaults.FileExtension;
        var dialog = new SaveFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            FileName = $"{dialogFileName}.{extension}",
            DefaultDirectory = dialogDirectory
        };

        if (dialog.ShowDialog() != true)
            return;

        var isSaveSuccess = FileOperations.Save(new FileFormat(_currentSizeSettings, _currentColorSettings), dialog.FileName);

        if (isSaveSuccess)
        {
            _fileManager.ActiveFilePath = dialog.FileName;
        }
    }

    private void OnLoadButtonClicked(object sender, RoutedEventArgs e)
    {
        var dialogDirectory = _fileManager.GetActiveFileLocation() ?? FileManagementDefaults.DefaultLocation;
        
        const string extension = FileManagementDefaults.FileExtension;
        var dialog = new OpenFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            DefaultDirectory = dialogDirectory,
        };
        
        if (dialog.ShowDialog() != true)
            return;

        var data = FileOperations.Load(dialog.FileName);
        
        if (data is null)
        {
            Console.WriteLine($"Failed to load {dialog.FileName}");
            return;
        }
        
        _fileManager.ActiveFilePath = dialog.FileName;
        _currentSizeSettings = data.Value.SizeSettings;
        _currentColorSettings = data.Value.ColorSettings;
        
        UnbindInputs();
        RefreshColorInputs();
        RefreshDimensionInputs();
        BindInputs();
        ReDraw();
    }

    #endregion

    private void OnShortcutSave(object sender, ExecutedRoutedEventArgs e)
    {
        if (_fileManager.ActiveFilePath is not null)
        {
            FileOperations.Save(new FileFormat(_currentSizeSettings, _currentColorSettings), _fileManager.ActiveFilePath);
            return;
        }

        const string extension = FileManagementDefaults.FileExtension;
        
        var dialog = new SaveFileDialog
        {
            Filter = $"{extension} Files (*.{extension})|*.{extension}",
            DefaultExt = $".{extension}",
            FileName = $"{FileManagementDefaults.FileName}.{extension}",
            DefaultDirectory = FileManagementDefaults.DefaultLocation
        };

        if (dialog.ShowDialog() != true)
            return;

        var isSaveSuccess = FileOperations.Save(new FileFormat(_currentSizeSettings, _currentColorSettings), dialog.FileName);

        if (isSaveSuccess)
        {
            _fileManager.ActiveFilePath = dialog.FileName;
        }
    }
}