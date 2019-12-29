using Microsoft.Win32;
using SalesOrganizer.Generic;
using SalesOrganizer.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace SalesOrganizer.ViewModels
{
    public sealed class SalesViewModel : NotifyableObject
    {
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
                    LoadSales();
                }
            }
        }

        public string LoadError
        {
            get => m_loadError;
            set => Set(ref m_loadError, value);
        }

        public DelegateCommand BrowseCommand { get; }
        public ObservableCollection<Sale> Sales { get; }


        public SalesViewModel(MainWindowViewModel owner)
        {
            m_owner = owner;

            Sales = new ObservableCollection<Sale>();
            BrowseCommand = new DelegateCommand(BrowseExecute);

            string savedPath = RegistryHelper.SalesFilePath;
            if (!string.IsNullOrEmpty(savedPath))
            {
                FilePath = savedPath;
            }
        }

        private void BrowseExecute(object o)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Select the file containing your sales you want to calculate",
                Filter = "Comma Separated Values (*.csv)|*.csv|All Files (*.*)|*.*"
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

        private bool LoadSales()
        {
            if (!File.Exists(m_filePath))
            {
                return LoadFailure("A file with that path does not exist.");
            }

            string[] contents = File.ReadAllLines(m_filePath);
            if (contents.Length < 2)
            {
                return LoadFailure("File contains no data.");
            }

            // TODO: I have no idea what this file format is going to look like

            return LoadSuccess();
        }

        private bool LoadSuccess()
        {
            // save the path if it's good
            RegistryHelper.SalesFilePath = m_filePath;

            LoadError = null;
            m_owner.CalculateResults();
            return true;
        }

        private bool LoadFailure(string error)
        {
            LoadError = error;
            Sales.Clear();
            m_owner.InvalidateResults();
            return false;
        }
    }
}
