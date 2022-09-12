using System;
using Avalonia.Controls;
using Avalonia.Media;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Newtonsoft;
using Newtonsoft.Json;
using static Project1.MoveProcess;

namespace Project1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            DataContext = this;
            AddItemsToList();
            ListBox1.SelectedItem = "Preset1";
        }
        
       #region Variables
       

       #endregion

        #region JsonRead+DataAusschluss

        public ObservableCollection<string> folderitems = new ObservableCollection<string>();
        private void AddItemsToList()
        {
            
            System.IO.DirectoryInfo Items = new System.IO.DirectoryInfo(System.Reflection.Assembly.GetEntryAssembly().Location+ @"\..\Presets");
            foreach (System.IO.FileInfo f in Items.GetFiles())
            {
                if (f.Name.Contains(".json") || f.Name.Contains(".Json"))
                {
                    folderitems.Add(f.Name.ToString());
                }
                
            }
            Debug.WriteLine(folderitems.Count);
            ListBox1.Items = folderitems;
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
        
        private void ProcessStart_Clicked(object sender, RoutedEventArgs e)
        {
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
                noitemselected.IsVisible = false;
                string itemst = item.ToString();
                var value = IndexObject(System.Reflection.Assembly.GetEntryAssembly().Location + @"\..\Presets\" + itemst);
                string a = Copyfrom.Text;
                string b = Copyto.Text;
                var c = ListBox1.SelectedItem;
                if (a != "" || b != "" || a != null || b != null)
                {
                     MoveToo(a, b, value);
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
            string value2 = "";
            var item =ListBox1.SelectedItem;
            string itemst = item.ToString();
            var value = IndexObject(System.Reflection.Assembly.GetEntryAssembly().Location + @"\..\Presets\" + itemst);
            foreach (string i in value)
            {
                value2 += i+"\n";
            }
            JsonIndex.Text = value2;
        }
        #endregion



    }
}