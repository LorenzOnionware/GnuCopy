  using System;
using Avalonia.Controls;
  using System.IO;
using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Reflection;
  using System.Security.Principal;
  using System.Threading.Tasks;
  using Avalonia;
  using Avalonia.Interactivity;
  using Avalonia.Media;
  using Avalonia.Platform;
  using Avalonia.Styling;
  using Microsoft.Win32;
using Newtonsoft.Json;
  using Project1.Viewmodels;
using Avalonia.Markup.Xaml;


  namespace Project1
{
    public partial class MainWindow : Window
    {
        private bool _eventHandlerBlocked = false;
        private int _blockDuration = 500;
        public static string PresetPath = MainViewmodel.PPresetPath;
        private bool _blockMethod = false;
        public static bool  savepath;
        

        public MainWindow()
        { 
          /*  WindowsIdentity identity = WindowsIdentity.GetCurrent(); 
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool Admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            
           
            if (!Admin)
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = "GnuCopy.exe";

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }*/
            InitializeComponent();
            DataContext = MainViewmodel.Default;
            string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
            IOC.Default.GetService<Settings>().Packageformat = 0;
            File.WriteAllText(Path.Combine(SettingsViewmodel.Default.settingspath), ab);
            
            IOC.Default.GetService<WindowClosingService>().RegisterWindow(this);
            
            ThemeChanged(null,null);
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AddItemsToList();
            if (IOC.Default.GetService<Settings>().Savelastpaths)
            {
                if (IOC.Default.GetService<Settings>().MultipleSources)
                {
                    Addsources();
                    MainViewmodel.Default.AddPath();
                }
                else
                { 
                    MainViewmodel.Default.Copyfromtext = IOC.Default.GetService<Settings>().Pathfrom;
                }
                MainViewmodel.Default.Copytotext = IOC.Default.GetService<Settings>().Pathto;
                
            }
            IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
            MainViewmodel.Default.ChageProperties();
            SystemEvents.UserPreferenceChanged += ThemeChanged;

        }


        public void Addsources()
        {
            MainViewmodel.Default.Expanderpaths.Replace((from path in IOC.Default.GetService<Settings>().Sources orderby path select path).ToArray());
        }


        public void ThemeChanged(object sender, UserPreferenceChangedEventArgs e)
        {
#if WINDOWS

            if (Environment.OSVersion.Version.Build >= 22000)
            {

                if (IOC.Default.GetService<Settings>().CustomMica)
                {
                    var mica = IOC.Default.GetService<Settings>().MicaIntensy;
                    byte dark = 235;
                    byte light = 235;
                    switch (mica)
                    {
                        case 0:
                            light = 238;
                            dark = 210;
                            break;
                        case 1 :
                            light = 200;
                            dark = 200;
                            break;
                        case 2:
                            light = 180;
                            dark = 180;
                            break;
                    }
                    if (this.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        this.TransparencyLevelHint =
                            WindowTransparencyLevel.Mica;
                        this.Background =
                            new SolidColorBrush(Color.FromArgb(dark, 30, 30, 30));
                    }
                    else
                    {
                        this.TransparencyLevelHint =
                            WindowTransparencyLevel.Mica;
                        this.Background =
                            new SolidColorBrush(Color.FromArgb(light, 238, 244, 249));
                    }
                }

            }
#endif
        }
        
        #region Presets
        public async Task AddItemsToList()
        {
            var folderitems = new HashSet<string>();
            var files = Directory.EnumerateFiles(PresetPath);
            foreach (var file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".json")
                {
                    folderitems.Add(Path.GetFileNameWithoutExtension(file));
                }
            }

            MainViewmodel.Default.Folderitems.Replace(folderitems);
        }

        #endregion
        
        #region something-other
        private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
        {
            string path = Path.Combine(SettingsViewmodel.Default.settingspath);
            string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
            File.WriteAllText(path,ab);
        }

        #endregion

        private bool busy;

        private void TargetPath_OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            busy = true;
            string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
            IOC.Default.GetService<Settings>().Pathto = IOC.Default.GetService<MainViewmodel>().Copytotext;
            File.WriteAllText(Path.Combine(SettingsViewmodel.Default.settingspath), ab);
            busy = false;
        }
        

        private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            if (!busy)
            {
                busy = true;
                string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
                IOC.Default.GetService<Settings>().Listingart = IOC.Default.GetService<MainViewmodel>().Listing;
                File.WriteAllText(Path.Combine(SettingsViewmodel.Default.settingspath), ab);
                busy = false;
            }
            else
            {
                while (busy)
                {
                    if (!busy)
                    {
                        busy = true;
                        string ab = JsonConvert.SerializeObject(IOC.Default.GetService<Settings>());
                        IOC.Default.GetService<Settings>().Listingart = IOC.Default.GetService<MainViewmodel>().Listing;
                        File.WriteAllText(Path.Combine(SettingsViewmodel.Default.settingspath), ab);
                        busy = false;
                    }
                }
            }
        }
    }
}