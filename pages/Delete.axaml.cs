using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Project1.Viewmodels;
using System.Runtime;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using Avalonia.Data;
using static System.Environment;
using static System.Environment.SpecialFolder;


namespace Project1;

public partial class Delete : Window
{
    public Delete()
    {
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        File.Delete( MainViewmodel.PPresetPath + @"\" + MainViewmodel.SelectedListItem);
        MainViewmodel.deleted = true;
        Project1.MainWindow.DeleteSucces();
        MainViewmodel.openwindow1 = false;
        this.Close();
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        MainViewmodel.openwindow1 = false;
        this.Close();
    }

    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        MainViewmodel.openwindow1 = false;
        
    }
}