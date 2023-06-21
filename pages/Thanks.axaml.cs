using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;

namespace GnuCopy.pages;

public  partial class Thanks : ContentDialog,IStyleable
{
    
    Type IStyleable.StyleKey => typeof(ContentDialog);

    public Thanks()
    {
        InitializeComponent();
        ApplyGradientBackground();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void ApplyGradientBackground()
    {
        // Erstelle einen linearen Farbverlauf
        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative)
        };

        // Füge Farbstopps hinzu
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Black, 0));
        gradientBrush.GradientStops.Add(new GradientStop(Colors.DarkBlue, 1));

        // Setze den Hintergrund des Fensters
        Background = gradientBrush;
    }
}