using Microsoft.Win32;
using SalesOrganizer.Generic;
using SalesOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SalesOrganizer.ViewModels
{
    public sealed class InventoryViewModel : NotifyableObject
    {
        // number of columns before we get to the people %'s
        public const int PersonHeaderOffset = 3;

        private readonly MainWindowViewModel m_owner;
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

            string savedPath = RegistryHelper.InventoryFilePath;
            if (!string.IsNullOrEmpty(savedPath))
            {
                FilePath = savedPath;
            }
        }



        private void BrowseExecute(object o)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Select the file containing your inventory information",
                Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*",
            };
            if (m_filePath != null)
            {
                dlg.InitialDirectory = Path.GetDirectoryName(m_filePath);
            }

            if (dlg.ShowDialog() == true)
            {
                FilePath = dlg.FileName;
            }
        }

        private bool LoadInventory()
        {
            Items.Clear();

            if (!File.Exists(m_filePath))
            {
                return LoadFailure("A file with that path does not exist.");
            }

            string[] contents = File.ReadAllLines(m_filePath);
            if (contents.Length < 2)
            {
                return LoadFailure("File contains no data.");
            }

            List<Person> people;
            try
            {
                // name, worth, type, pyritie%, pudding%, crunchy%
                people = contents[0].Split(',').Skip(PersonHeaderOffset).Select(name => new Person(name.Trim())).ToList();
            }
            catch (Exception e)
            {
                return LoadFailure("Error when parsing headers: " + e.Message);
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

                    decimal totalPercent = 0;
                    for (int i = 0; i < people.Count; i++)
                    {
                        decimal percent = decimal.Parse(split[i + PersonHeaderOffset]);
                        item.Earners.Add(people[i], percent);
                        totalPercent += percent;
                    }

                    if (totalPercent != 1)
                        throw new Exception($"Percentages do not add up to 100% ({name}, {type})");

                    Items.Add(item);
                }
            }
            catch (Exception e)
            {
                return LoadFailure($"Error when parsing line {line}: {e.Message}");
            }

            return LoadSuccess(people);
        }

        private bool LoadSuccess(List<Person> people)
        {
            // save the path if it's good
            RegistryHelper.InventoryFilePath = m_filePath;

            LoadError = null;
            m_owner.InventoryLoaded(people);
            return true;
        }

        private bool LoadFailure(string error)
        {
            LoadError = error;
            Items.Clear();
            m_owner.InvalidateResults();
            return false;
        }
    }
}
