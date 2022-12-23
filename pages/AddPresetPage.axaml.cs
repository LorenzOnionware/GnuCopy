using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;
using Project1.Viewmodels;
using Newtonsoft.Json;

namespace Project1.pages;

public partial class AddPreset : Window
{
    public AddPreset()
    {
        DataContext = Presetvievmodel.Default;
        InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        MainViewmodel.openwindow = false;
        this.Close();
    }

    private async  void ApplyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        await Dataformats(Presetvievmodel.Default.Textboxtext);
        MainViewmodel.openwindow = false;
        MainWindow.AddSuccess();
        this.Close();
    }

    private static List<string>  list1 = new();
    private async Task Dataformats(string a)
    {
        var Dataformatlist1= Presetvievmodel.Default.Dataformatlist;
        var folderlist1 = Presetvievmodel.Default.Folderlist;
        foreach (var value in Dataformatlist1)
        {
            string value2 = new string(@"." + value);
            list1.Add(value2);
        }

        foreach (var value in folderlist1)
        {
            list1.Add(value);
        }
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\GnuCopy"+ @"\" +  Presetvievmodel.Default.Textbox2text + ".json";
        using (StreamWriter file = File.CreateText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            
            string json = "[";
            foreach (string str in list1)
            {
                json += $"\"{str}\",";
            }
            json += "]";
            await file.WriteAsync(json);
        }
    }

    private void Remove_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void Add_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataformats = Presetvievmodel.Default.dataformatstext;
        var folders = Presetvievmodel.Default.folderstext;
        if (string.IsNullOrEmpty(dataformats) != true)
        {
            Presetvievmodel.Default.Dataformatlist.Add(dataformats);
        }

        if (string.IsNullOrEmpty(folders) != true)
        {
            Presetvievmodel.Default.Folderlist.Add(folders);
        }
    }
}
