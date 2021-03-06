﻿using SalesOrganizer.Generic;
using SalesOrganizer.Models;
using SalesOrganizer.Views;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace SalesOrganizer.ViewModels
{
    public sealed class MainWindowViewModel : NotifyableObject
    {
        private readonly MainWindow m_owner;
        private List<Person> m_people;

        public InventoryViewModel InventoryVM { get; }
        public SalesViewModel SalesVM { get; }

        public MainWindowViewModel(MainWindow owner)
        {
            m_owner = owner;
            InventoryVM = new InventoryViewModel(this);
            SalesVM = new SalesViewModel(this);
        }

        public void InventoryLoaded(List<Person> people)
        {
            m_people = people;

            // clear off any old ones
            int oldColumnCount = m_owner.inventoryTable.Columns.Count;
            int offset = InventoryViewModel.PersonHeaderOffset;
            if (oldColumnCount > offset)
            {
                for (int i = offset; i < oldColumnCount; i++)
                {
                    m_owner.inventoryTable.Columns.RemoveAt(offset);
                }
            }

            // then add new ones
            foreach (var person in m_people)
            {
                m_owner.inventoryTable.Columns.Add(new DataGridTextColumn
                {
                    Header = person.Name,
                    Binding = new Binding($"EarnersByName[{person.Name}]")
                    {
                        StringFormat = "0.##%"
                    }
                });
            }
        }

        public void InvalidateResults()
        {
            // TODO
        }

        public void CalculateResults()
        {
            // TODO: if inventory and sales have good data, do it
        }
    }
}
