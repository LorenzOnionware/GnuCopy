using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        public ObservableCollection<string> folderitems = new ObservableCollection<string>();
        public bool selectionchange = false;
        private bool _eventHandlerBlocked = false;
        private int _blockDuration = 2000;

        public MainWindow()
        {
            DataContext = MainViewmodel.Default;
            InitializeComponent();
            AddItemsToList();
            ListBox1.SelectedIndex = 0;
        }

        #region JsonRead


        private async Task  AddItemsToList()
        {
            string[] listboxitems = ListBox1.Items.OfType<string>().ToArray();
            System.IO.DirectoryInfo Items = new System.IO.DirectoryInfo(System.Reflection.Assembly.GetEntryAssembly().Location+ @"\..\Presets");
            var fs = Items.GetFiles();
            for (var index = 0; index < fs.Length; index++)
            {
                FileInfo f = fs[index];
                if (f.Name.Contains(".json") || f.Name.Contains(".Json"))
                {
                    folderitems.Add(f.Name.ToString());
                }
            }

            ListBox1.Items = folderitems.Distinct();
            ListBox1.SelectedIndex = 0;

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
            var item = ListBox1.SelectedItem;
            if (item == null)
            {
                noitemselected.Text = "Please select a Preset!";
                noitemselected.IsVisible = true;
            }
            else
            {
                
                MainViewmodel.Default.Progressmax = Directory.EnumerateDirectories(Copyfrom.Text, "*", SearchOption.AllDirectories).Count()-1;
                noitemselected.IsVisible = false;
                string itemst = item.ToString();
                var value3 = IndexObject(System.Reflection.Assembly.GetEntryAssembly()?.Location + @"\..\Presets\" + itemst);
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
        private void ListBox1_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_eventHandlerBlocked)
            {
                return;
            }
            string value2 = "";
            var item = ListBox1.SelectedItem;
            string itemst = item.ToString();
            var value = IndexObject(System.Reflection.Assembly.GetEntryAssembly().Location + @"\..\Presets\" + itemst);
            foreach (string i in value)
            {
                value2 += i+"\n";
            }
            JsonIndex.Text = value2;
        }
        #endregion
        
        #region Presets

        
        private void AddPreset_OnClick(object? sender, RoutedEventArgs e)
        {
            if (MainViewmodel.openwindow == false)
            {
                var window = new Project1.pages.AddPreset();
                window.Show();
                MainViewmodel.openwindow = true;
            }
        }
        private void RemovePreset_OnClick(object? sender, RoutedEventArgs e)
        {
          // File.Delete(ListBox1.SelectedIndex.ToString());
        }
        #endregion


        private void Aktualisieren_OnClick(object? sender, RoutedEventArgs e)
        {
            _eventHandlerBlocked = true;
            Task.Delay(_blockDuration).ContinueWith(t => _eventHandlerBlocked = false);
            AddItemsToList();
        }
    }
    
}