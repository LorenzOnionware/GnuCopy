using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
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
        DataContext = MainViewmodel.Default;
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
        await Dataformats(MainViewmodel.Default.Textboxtext);
        this.Close();
    }

    private static List<string>  list1 = new();
    private async Task Dataformats(string a)
    {
        
        string value = "";
        int lengt = a.Length;
        int b = 1;
        foreach (char c in a)
        {
            if (c.ToString() != ",")
            {
                value += c;
            }
            if (c.ToString() == ",")
            {
                list1.Add(value);
                value = "";
            }
            else if(b==lengt)
            {
                list1.Add(value);
                value = "";
            }
            b++;
        }
        string path = @System.Reflection.Assembly.GetEntryAssembly().Location + @"\..\Presets\" + MainViewmodel.Default.Textbox2text + ".json";
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
}
