using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Project1.Viewmodels;
using System.IO;

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
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy"+ MainViewmodel.SelectedListItem);
        MainViewmodel.openwindow1 = false;
        
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        MainViewmodel.openwindow1 = false;
        this.Close();
    }
}