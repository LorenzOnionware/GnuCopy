  using System;
using Avalonia.Controls;
  using System.IO;
using System.Collections.Generic;
  using System.Threading.Tasks;
  using Avalonia.Media;
  using Avalonia.Platform;
  using Avalonia.Styling;
  using Microsoft.Win32;
using Newtonsoft.Json;
  using Project1.Viewmodels;


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
            DataContext = MainViewmodel.Default;
            InitializeComponent();
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
            MainViewmodel.Default.Expanderpaths.Replace(IOC.Default.GetService<Settings>().Sources);
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
        
    }
}
