using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Shared;

namespace GUI
{
    public class InventoryViewModel : NotifyableObject
    {
        // number of columns before we get to the people %'s
        private const int c_personHeaderOffset = 3;

        private MainWindowViewModel m_owner;
        private string m_filePath;
        private string m_loadError;

        public string FilePath
        {
            get => m_filePath;
            set
            {
                if (Set(ref m_filePath, value))
                {
                    LoadInventory();
                }
            }
        }

        public string LoadError
        {
            get => m_loadError;
            set => Set(ref m_loadError, value);
        }

        public DelegateCommand BrowseCommand { get; }
        public ObservableCollection<InventoryItem> Items { get; }



        public InventoryViewModel(MainWindowViewModel owner)
        {
            m_owner = owner;

            Items = new ObservableCollection<InventoryItem>();
            BrowseCommand = new DelegateCommand(BrowseExecute);
        }



        private void ClearLoadError() => LoadError = null;

        private void BrowseExecute(object o)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Select the file containing your inventory information",
                Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                FilePath = dlg.FileName;
            }
        }

        private void LoadInventory()
        {
            Items.Clear();

            if (!File.Exists(m_filePath))
            {
                LoadError = "A file with that path does not exist.";
                return; 
            }

            string[] contents = File.ReadAllLines(m_filePath);

            if (contents.Length < 2)
            {
                LoadError = "File contains no data.";
                return;
            }

            List<Person> people;
            try
            {
                // name, worth, type, pyritie%, pudding%, crunchy%
                people = contents[0].Split(',').Skip(c_personHeaderOffset).Select(name => new Person(name.Trim())).ToList();
            }
            catch (Exception e)
            {
                LoadError = "Error when parsing headers: " + e.Message;
                return;
            }

            int line = 1;
            try
            {
                for (; line < contents.Length; line++)
                {
                    string[] split = contents[line].Split(',');

                    // latias, 7.50, charm, 1, 0, 0
                    string name = split[0].Trim();
                    decimal worth = decimal.Parse(split[1]);
                    string type = split[2].Trim();

                    var item = new InventoryItem(name, type, worth);

                    for (int i = 0; i < people.Count; i++)
                    {
                        item.Owners.Add(people[i], decimal.Parse(split[i + c_personHeaderOffset]));
                    }

                    Items.Add(item);
                }
            }
            catch (Exception e)
            {
                LoadError = $"Error when parsing line {line}: {e.Message}";
                Items.Clear();
                return;
            }
        }
    }
}
