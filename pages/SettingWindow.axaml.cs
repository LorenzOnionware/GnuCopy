using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Project1.Viewmodels;

namespace Project1.pages;

public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        DataContext = SettingsViewmodel.Default;
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        MainViewmodel.openwindow1 = false;
        
    }

    #region  Labels
    private void Contributors_OnTapped(object? sender, TappedEventArgs e)
    {
        
    }

    private void Contact_OnTapped(object? sender, TappedEventArgs e)
    {
        
    }
    private void Github_OnTapped(object? sender, TappedEventArgs e)
    {
        
    }
    private void License_OnTapped(object? sender, TappedEventArgs e)
    {
        
    }
    #endregion
}