using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        public MainWindow()
        {
            DataContext = MainViewmodel.Default;
            InitializeComponent();
            AddItemsToList();
            ListBox1.SelectedIndex = 0;
        }

        #region JsonRead


        private void AddItemsToList()
        {
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
                var value3 = IndexObject(System.Reflection.Assembly.GetEntryAssembly().Location + @"\..\Presets\" + itemst);
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