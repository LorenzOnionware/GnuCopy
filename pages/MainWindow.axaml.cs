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
using Avalonia.Threading;
using DynamicData;
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
        public static bool? Listing = false;
        public static bool isediting = false;
        public List<string> ignorefiles = new();
        public List<string> ignorefolder = new();
        public bool selectionchange = false;
        private bool _eventHandlerBlocked = false;
        private int _blockDuration = 500;
        public static string PresetPath = MainViewmodel.PPresetPath;
        private bool _blockMethod = false;
        private static bool  savepath;
        public List<string> Jsonfile => JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(SettingsViewmodel.Default.settingspath));

        public MainWindow()
        {
            DataContext = MainViewmodel.Default;
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Directory.CreateDirectory(PresetPath);
            Directory.CreateDirectory(Path.Combine(PresetPath,"Settings"));
            if (File.Exists(Path.Combine(PresetPath, "Settings", "Settings.json")))
            {
                List<string>? lel = IndexObject(Path.Combine(PresetPath, "Settings", "Settings.json"));
                if (lel == null||lel.Count < 8)
                {
                    File.Delete(Path.Combine(PresetPath, "Settings", "Settings.json"));
                    FirstStart();

                }
                savepath = Readsettings.Read(4);
            }
            else
            {
                FirstStart();
            }
            AddItemsToList();
            AktualisereSetting();
            if (savepath)
            {
                MainViewmodel.Default.Copytotext = Readsettings.read2(6);
                MainViewmodel.Default.Copyfromtext = Readsettings.read2(5);
            }
            
        }

        #region JsonRead

        private async Task FirstStart()
        {
            string[] settings = new[] { "Overrite=true", "clearforcopy=false", "clearaftercopy=false", "listingart=0","savelastpaths=false","pathfrom=","pathto=","packageformat=none"};
            string path = Path.Combine(PresetPath, "Settings", "Settings.json");
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                string json = JsonConvert.SerializeObject(settings);
                await file.WriteAsync(json);
            }

            var files1 = Directory.GetFiles(PresetPath);
            string[] preset1Extensions = new string[]
                { ".exe", ".dll", ".msixbundle", ".msixupload", ".pfx", ".winmd" };
            string[] preset2Extensions = new string[] { ".mp3", ".mp4", ".png", ".txt", ".docx", ".jpg", ".png" };

            await WritePresetToFile("Preset1", preset1Extensions);
            await WritePresetToFile("Preset2", preset2Extensions);
        }

        private async Task WritePresetToFile(string presetName, string[] extensions)
        {
            string path = PresetPath + @"\" + presetName + ".json";
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                string json = JsonConvert.SerializeObject(extensions);
                await file.WriteAsync(json);
            }
        }

        #endregion

        #region Dialogs


        
        private async void CopyDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                MainViewmodel.Default.Copyfromtext = result;
                SettingsChange(5, result);
            }

        }
        private async void PasteDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(this);
            if (result != null)
            {
                MainViewmodel.Default.Copytotext = result;
                SettingsChange(6, result);
            }
        }

        #endregion

        public static List<string> IndexObject(string e)
        {
            List<string> jsonfile = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(e));
            return jsonfile;
        }
        
        #region MoveProcess
        
        private async Task SettingsChange(byte index,object ab)
        {
            var list = Jsonfile.ToArray();
            string input = list[index];
            string pattern = "=(.*)$";
            string result = Regex.Replace(input, pattern, $"={ab}");
            list.Replace<string>(list[index], result);
            Jsonfile.Clear();
            for (byte a = 0; a <= 2; a++)
            {
                Jsonfile.Add(list[a]);
            }
            File.WriteAllText(SettingsViewmodel.Default.settingspath, JsonConvert.SerializeObject(list));
        }
        private async void Copy_OnClick(object? sender, RoutedEventArgs e)
        {
            if (Readsettings.read3(7) != "none")
            {
                var value3 = IndexObject(Path.Combine(PresetPath, MainViewmodel.Default.Selectedlistitem));
                foreach (var value4 in value3)
                {
                    if (value4.Contains('#'))
                    {
                        ignorefolder.Add(Regex.Replace(value4, @"#", ""));
                    }
                    else
                    {
                        ignorefiles.Add(value4);
                    }
                }
                switch (Readsettings.read3(7))
                {
                    case "7Zip" :
                        await copypackaged.Copy(ignorefiles,ignorefolder);
                        StartPackaging(true);
                        break;
                    case "Tar":
                        await copypackaged.Copy(ignorefiles,ignorefolder);
                        StartPackaging(false);
                        break;
                }
            }
            else
            {
                CopyUnPackaged();
            }
        }

        private static void StartPackaging(bool verfahren)
        {
            switch (verfahren)
            {
                case true:
                    goto B;
                    break;
                case false:
                    goto C;
                    break;
            }
            B:
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"));
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext,"Compresseddirection.7z"),CompressionType.LZMA);
            }
            Directory.Delete(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"),true);
            C:
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"));
                archive.SaveTo(Path.Combine(MainViewmodel.Default.Copytotext,"Compresseddirection.tar"),CompressionType.None);
            }
            Directory.Delete(Path.Combine(MainViewmodel.Default.Copyfromtext,"OnionwareTemp"),true);
            
        }

        private async void CopyUnPackaged()
        {
            bool settingschaged = false;
            if (MainViewmodel.Default.Selectedpreset == -1)
            {
                settingschaged = true;
            }
            if (Path.Exists(MainViewmodel.Default.Copyfromtext) && Path.Exists(MainViewmodel.Default.Copytotext))
            {
                var value3 = IndexObject(Path.Combine(PresetPath, MainViewmodel.Default.Selectedlistitem));
                foreach (var value4 in value3)
                {
                    if (value4.Contains('#'))
                    {
                        ignorefolder.Add(Regex.Replace(value4, @"#", ""));
                    }
                    else
                    {
                        ignorefiles.Add(value4);
                    }
                }
                await Copy.Settings(ignorefolder, ignorefiles);
            }
        }
        #endregion

        
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

        private void PresetList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_eventHandlerBlocked)
            {
                return;
            }

            Dispatcher.UIThread.Post(() =>
            {
                string item = MainViewmodel.Default.Selectedlistitem;
                if (item == null)
                {
                    MainViewmodel.Default.jsonindex.Clear();
                    return;
                }

                string itemst = item.ToString();
                var value = IndexObject(Path.Combine(PresetPath,itemst));
                _blockMethod = true;
                Task.Delay(_blockDuration).ContinueWith(t => _blockMethod = false);
                MainViewmodel.Default.jsonindex.Clear();
                foreach (string i in value)
                {
                    MainViewmodel.Default.jsonindex.Add(i);
                }
            });
        }

        private void AddPreset_OnClick(object? sender, RoutedEventArgs e)
        {
            if (MainViewmodel.openwindow == false)
            {
                _blockMethod = true;
                var window = new Project1.pages.AddPreset();
                window.Show();
                MainViewmodel.openwindow = true;
            }
        }
        public static void DeleteSucces()
        {
            MainWindow window1 = new MainWindow();
            window1._eventHandlerBlocked = true;
            Task.Delay(500).ContinueWith(t => window1._eventHandlerBlocked = false);
            window1.AddItemsToList();
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
        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            jsonindexviev.SelectedIndex = -1;
        }

        private void Settingb_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = new Project1.pages.SettingWindow();
            window.Show();
        }

        private void Copyfrom_OnTextChanged(object? sender, TextChangingEventArgs textChangingEventArgs)
        {
            if (string.IsNullOrEmpty(MainViewmodel.Default.Copyfromtext))
            {
                return;
            } 
            SettingsChange(5, MainViewmodel.Default.Copyfromtext);
        }

        private void Copyto_OnTextChanged(object? sender, TextChangingEventArgs textChangingEventArgs)
        {
            if (string.IsNullOrEmpty(MainViewmodel.Default.Copytotext))
            {
               return;
            } 
            SettingsChange(6, MainViewmodel.Default.Copytotext);
        }
        //Edit presets button
       private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            if(PresetList.SelectedIndex == -1 || isediting) 
                return;
            isediting = true;
            var window = new Project1.pages.AddPreset();
            window.Show();
        }

       public static void AktualisereSetting()
       {
           savepath = Readsettings.Read(4);
           Dispatcher.UIThread.Post(() =>
           {
               var setting = Readsettings.Read1(3);
               switch (setting)
               {
                   case 0:
                       Listing = false;
                       MainViewmodel.Default.Presetlistenable  = true;
                       break;
                   case 1:
                       Listing = true;
                       MainViewmodel.Default.Presetlistenable  = true;
                       break;
                   case 2:
                       Listing = null;
                       MainViewmodel.Default.Presetlistenable = false;
                       break;
               }
           });
             
       }
       
       #endregion  
    }
}
