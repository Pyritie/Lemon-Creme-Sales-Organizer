using Microsoft.Win32;
using SalesOrganizer.Generic;
using SalesOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace SalesOrganizer.ViewModels
{
    public sealed class InventoryViewModel : NotifyableObject
    {
        // number of columns before we get to the people %'s
        public const int PersonHeaderOffset = 3;

        private readonly MainWindowViewModel m_owner;
        private readonly FileSystemWatcher m_fileWatcher;
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

            m_fileWatcher = new FileSystemWatcher();
            m_fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            m_fileWatcher.Changed += OnFileChanged;

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
                m_fileWatcher.EnableRaisingEvents = false;
                return LoadFailure("A file with that path does not exist.");
            }

            // as long as the path is good, we want to watch the file
            m_fileWatcher.Path = Path.GetDirectoryName(m_filePath);
            m_fileWatcher.Filter = Path.GetFileName(m_filePath);
            m_fileWatcher.EnableRaisingEvents = true;

            string[] contents;
            try
            {
                contents = File.ReadAllLines(m_filePath);
            }
            catch (Exception e)
            {
                return LoadFailure("Error when reading file: " + e.Message);
            }

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
                    string type = split[2].Trim();

                    if (!decimal.TryParse(split[1], out decimal worth))
                        throw new Exception($"Couldn't parse item worth column ({name}, {type})");

                    var item = new InventoryItem(name, type, worth);

                    decimal totalPercent = 0;
                    for (int i = 0; i < people.Count; i++)
                    {
                        if (!decimal.TryParse(split[i + PersonHeaderOffset], out decimal percent))
                            throw new Exception($"Couldn't parse {people[i]} % column ({name}, {type})");

                        if (percent > 1 || percent < 0)
                            throw new Exception($"{people[i]} % column value must be between 0 and 1, inclusive ({name}, {type})");

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
                return LoadFailure($"Error on line {line + 1}: {e.Message}");
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

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.Replace('\\', '/').Equals(m_filePath?.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase))
            {
                // the file watcher has its own thread so we need to run this on the wpf thread
                Application.Current.Dispatcher.Invoke(LoadInventory);
            }
        }
    }
}
