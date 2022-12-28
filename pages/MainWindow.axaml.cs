using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.IO;
using Avalonia.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using Project1.pages;
using Project1.Viewmodels;
using static Project1.FirstFolderFiles;
using static Project1.Files;


namespace Project1
{
    public partial class MainWindow : Window
    {
        public List<string> valuea = new();
        public List<string> ignore = new();
        public bool selectionchange = false;
        private bool _eventHandlerBlocked = false;
        private int _blockDuration = 500;
        private string PresetPath = MainViewmodel.PPresetPath;
        private bool _blockMethod = false;

        public MainWindow()
        {
            DataContext = MainViewmodel.Default;
            InitializeComponent();    
            string Appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string[] directories = Directory.GetDirectories(Appdata);
            if (directories.Any(x => x.Contains(PresetPath))) { }
            else
            {
                Directory.CreateDirectory(PresetPath);
                FirstStart();
            }
            AddItemsToList(false);
            ListBox1.SelectedIndex = 0;

        }

        #region JsonRead


        private async Task FirstStart()
        {
            string[] preset1Extensions = new string[] { ".exe", ".dll", ".msixbundle", ".msixupload", ".pfx", ".winmd" };
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

        #region TextBoxBackend

        private void CopyDialogClicked(object sender, RoutedEventArgs e)
        {
        }


        private void PasteDialogClicked(object sender, RoutedEventArgs e)
        {

        }

        #endregion


        #region MoveProcess

        public static List<string> IndexObject(string e)
        {
            List<string> jsonfile = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(e));
            return jsonfile;
        }

        private async void Copy_OnClick(object? sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Copyfrom.Text)|| string.IsNullOrEmpty(Copyto.Text))return;
            var item = ListBox1.SelectedItem;
            if (item == null)
            {
                noitemselected.Text = "Please select a Preset!";
                noitemselected.IsVisible = true;
            }
            else
            {

                MainViewmodel.Default.Progressmax = Directory.EnumerateDirectories(Copyfrom.Text, "*", SearchOption.AllDirectories).Count() - 1;
                noitemselected.IsVisible = false;
                string itemst = item.ToString();
                var value3 = IndexObject(PresetPath+ @"\" + itemst);
                foreach (var value4 in value3)
                {
                    if (value4.Contains("."))
                    {
                        valuea.Add(value4);
                    }
                    else
                    {
                        ignore.Add(value4);
                    }
                }
                string a = Copyfrom.Text;
                string b = Copyto.Text;
                var c = ListBox1.SelectedItem;
                if (a != "" || b != "" || a != null || b != null)
                {
                    await Task.Run(() => CopyFolder(a, b, valuea, ignore));
                }
                else
                {
                    noitemselected.Text = "Path ist empty or doesn't exist";
                    noitemselected.IsVisible = true;
                }
            }
        }

        #endregion

        #region Presets



        public async Task AddItemsToList(bool a)
        {
            if (a == true)
            {
                for (int value = 0; value <= MainViewmodel.Default.Folderitems.Count; value++)
                {
                    if (MainViewmodel.Default.Folderitems[value] == MainViewmodel.SelectedListItem)
                    {
                        MainViewmodel.Default.Folderitems.RemoveAt(value);
                    }
                }
            }

            var folderitems = new HashSet<string>();
            System.IO.DirectoryInfo Items = new System.IO.DirectoryInfo(PresetPath);
            var fs = Items.GetFiles();
            for (var index = 0; index < fs.Length; index++)
            {
                FileInfo f = fs[index];
                if (f.Name.Contains(".json") || f.Name.Contains(".Json"))
                {
                    folderitems.Add(f.Name.ToString());
                }
            }

            Debug.WriteLine(folderitems.Count());
            MainViewmodel.Default.Folderitems.Clear();
            foreach (var item in folderitems)
                MainViewmodel.Default.Folderitems.Add(item);
            ListBox1.SelectedIndex = 0;
        }

        private void ListBox1_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_eventHandlerBlocked)
            {
                return;
            }

            string value2 = "";
            var item = ListBox1.SelectedItem;
            if (item == null)
            {
                return;
            }

            string itemst = item.ToString();
            var value = IndexObject(PresetPath + @"\" + itemst);
            _blockMethod = true;
            Task.Delay(_blockDuration).ContinueWith(t => _blockMethod = false);
            MainViewmodel.Default.jsonindex.Clear();
            foreach (string i in value)
            {
                MainViewmodel.Default.jsonindex.Add(i);
            }
        }

        private void AddPreset_OnClick(object? sender, RoutedEventArgs e)
        {
            if (MainViewmodel.openwindow == false)
            {
                var window = new Project1.pages.AddPreset();
                window.Show();
                MainViewmodel.openwindow = true;
            }
        }

        private async void RemovePreset_OnClick(object? sender, RoutedEventArgs e)
        {
            if (MainViewmodel.openwindow1 == false)
            {
                MainViewmodel.SelectedListItem = ListBox1.SelectedItem.ToString();
                if (ListBox1.SelectedIndex < 1)
                {
                    ListBox1.SelectedIndex++;
                }
                else
                {
                    ListBox1.SelectedIndex--;
                }

                var window = new Project1.Delete();
                window.Show();
                MainViewmodel.openwindow1 = true;
            }
        }
        public static void DeleteSucces()
        {

            MainWindow window1 = new MainWindow();
            window1._eventHandlerBlocked = true;
            Task.Delay(500).ContinueWith(t => window1._eventHandlerBlocked = false);
            window1.AddItemsToList(true);
        }

        public static void AddSuccess()
        {
            MainWindow window1 = new MainWindow();
            window1._eventHandlerBlocked = true;
            Task.Delay(500).ContinueWith(t => window1._eventHandlerBlocked = false);
            window1.AddItemsToList(false);
        }
        #endregion


        private void Aktualisieren_OnClick(object? sender, RoutedEventArgs e)
        {
            _eventHandlerBlocked = true;
            Task.Delay(_blockDuration).ContinueWith(t => _eventHandlerBlocked = false);
            AddItemsToList(false);
        }
    }
}