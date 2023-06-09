  using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Collections.Generic;
using System.Linq;
  using System.Reactive.Linq;
  using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
  using Avalonia.Controls.Primitives;
  using Avalonia.Input;
  using Avalonia.Media;
using Avalonia.Skia;
using Avalonia.Styling;
using Avalonia.Threading;
  using CommunityToolkit.Mvvm.DependencyInjection;
  using DynamicData;
using Microsoft.Win32;
using Newtonsoft.Json;
using Project1.Viewmodels;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;


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
            ThemeChanged(null,null);
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AddItemsToList();
            if (IOC.Default.GetService<Settings>().Savelastpaths)
            {
                if (IOC.Default.GetService<Settings>().MultipleSources)
                {
                    Addsources();
                }
                MainViewmodel.Default.Copytotext = IOC.Default.GetService<Settings>().Pathto;
                MainViewmodel.Default.Copyfromtext = IOC.Default.GetService<Settings>().Pathfrom;
                MainViewmodel.Default.AddPath();
            }
            IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
            MainViewmodel.Default.ChageProperties();
            SystemEvents.UserPreferenceChanged += ThemeChanged;

        }

        public void Addsources()
        {
            MainViewmodel.Default.Expanderpaths.Replace(IOC.Default.GetService<Settings>().Sources);
        }
        
        
        private void ThemeChanged(object sender, UserPreferenceChangedEventArgs e)
        {
#if WINDOWS

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                if (this.ActualThemeVariant == ThemeVariant.Dark)
                {
                    this.TransparencyLevelHint =
                        WindowTransparencyLevel.Mica;
                    this.Background =
                        new SolidColorBrush(Color.Parse("#991e1e1e"));
                }
                else
                {
                    this.TransparencyLevelHint =
                        WindowTransparencyLevel.Mica;
                    this.Background =
                        new SolidColorBrush(Color.Parse("#eeeef4f9"));
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
