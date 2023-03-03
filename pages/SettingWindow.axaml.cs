using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Remote.Protocol.Input;
using DynamicData;
using Newtonsoft.Json;
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

    public bool cancel = false;
    public List<string> Jsonfile => JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(SettingsViewmodel.Default.settingspath));

    private void InitializeComponent()
    { 
        SettingsOverride(Jsonfile);
        AvaloniaXamlLoader.Load(this);
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen; 
       
    }    
   
    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        MainViewmodel.openwindow1 = false;
    }

    public void SettingsOverride(List<string> settingslist)
    {
        
        foreach (var Setting in settingslist)
        {
            string input = Setting;
            string pattern = "=(.*)$";

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string value = match.Groups[1].Value;
                string pattern2 = "=.*$";
                string result = Regex.Replace(input, pattern2, "");
                switch (result)
                {
                    case "Overrite":
                        if(value == "true"){SettingsViewmodel.Default.Overritechecked = true;}else{SettingsViewmodel.Default.Overritechecked = false;}
                        break;
                    case "clearforcopy":
                        if (value == "true"){SettingsViewmodel.Default.Clearforchecked = true; }else{SettingsViewmodel.Default.Clearforchecked = false;}
                        break;
                    case "clearaftercopy":
                        if (value == "true"){SettingsViewmodel.Default.Clearafterchecked = true; }else{SettingsViewmodel.Default.Clearafterchecked = false;}
                        break;
                }
            }
        }

        cancel = false;
    }

    #region  Labels
    private void Contributors_OnTapped(object? sender, TappedEventArgs e)
    {
        var window = new Project1.pages.Contributors();
        window.Show();
    }
    private void Github_OnTapped(object? sender, TappedEventArgs e)
    {
        OpenLink("https://github.com/neutralezwiebel/");
    }
    
    private void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    private void License_OnTapped(object? sender, TappedEventArgs e)
    {
        OpenLink(Environment.CurrentDirectory + @"\pages\License.html");
    }
    private void Contact_OnTapped(object? sender, TappedEventArgs e)
    {
        OpenLink("mailto:contact.onionware@gmail.com");
    }
    #endregion
#region checkboxen
    private void Overritefiles_OnChecked(object? sender, RoutedEventArgs e)
    {
        if (cancel==true)
             return;
        var list = Jsonfile.ToArray();
        string input = list[0];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=true");
        list.Replace<string>(list[0], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }

    private void Overritefiles_OnUnchecked(object? sender, RoutedEventArgs e)
    {
        if (cancel == true)
            return;
        var list = Jsonfile.ToArray();
        string input = list[0];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=false");
        list.Replace<string>(list[0], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }
    private void Clearforcopy_OnChecked(object? sender, RoutedEventArgs e)
    {
        if (cancel == true)
            return;
        var list = Jsonfile.ToArray();
        string input = list[1];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=true");
        list.Replace<string>(list[1], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }

    private void Clearforcopy_OnUnchecked(object? sender, RoutedEventArgs e)
    {
        if (cancel == true)
            return;
        var list = Jsonfile.ToArray();
        string input = list[1];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=false");
        list.Replace<string>(list[1], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }

    private void Clearaftercopy_OnChecked(object? sender, RoutedEventArgs e)
    {  if (cancel == true)
            return;
        var list = Jsonfile.ToArray();
        string input = list[2];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=true");
        list.Replace<string>(list[2], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }

    private void Clearaftercopy_OnUnchecked(object? sender, RoutedEventArgs e)
    {  if (cancel == true)
            return;
        var list = Jsonfile.ToArray();
        string input = list[2];
        string pattern = "=(.*)$";
        string result = Regex.Replace(input, pattern, "=false");
        list.Replace<string>(list[2], result);
        Jsonfile.Clear();
        for (byte a = 0; a <= 2; a++)
        {
            Jsonfile.Add(list[a]);
        }
        File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
    }
    #endregion
    
}