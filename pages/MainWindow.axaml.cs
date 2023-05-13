  using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Skia;
using Avalonia.Styling;
using Avalonia.Threading;
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
                MainViewmodel.Default.Copytotext = IOC.Default.GetService<Settings>().Pathto;
                MainViewmodel.Default.Copyfromtext = IOC.Default.GetService<Settings>().Pathfrom;
            }
            IOC.Default.GetService<AktualiselSettingsInUI>().AktualisereSetting();
            SystemEvents.UserPreferenceChanged += ThemeChanged;

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
            System.IO.DirectoryInfo Items = new System.IO.DirectoryInfo(PresetPath);
            var fs = Items.GetFiles();
            for(var index = 0; index < fs.Length; index++)
            {
                FileInfo f = fs[index];
                if (f.Name.Contains(".json") || f.Name.Contains(".Json"))
                {
                    if(File.Exists(fs[index].ToString())) 
                        folderitems.Add(f.Name.ToString());
                }
            }
            MainViewmodel.Default.Folderitems.Clear();
            foreach (var item in folderitems)
                MainViewmodel.Default.Folderitems.Add(item);
        }
        

        public static void AddSuccess()
        {
            MainWindow window1 = new MainWindow();
            window1._eventHandlerBlocked = true;
            Task.Delay(500).ContinueWith(t => window1._eventHandlerBlocked = false);
            window1.AddItemsToList();
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
