using IncidentDesk.Models;
using IncidentDesk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;

namespace IncidentDesk
{
    public partial class MainWindow : Window
    {
        private List<Incident> incidents = new List<Incident>();
        private IncidentStorage storage = new IncidentStorage();
        private int nextId = 1;

        public MainWindow()
        {
            InitializeComponent();

            cbFilterStatus.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
            cbSeverity.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;

            SeedData();
            RefreshList();
        }

        private void SeedData()
        {
            incidents.Add(new Incident
            {
                Id = nextId++,
                Title = "Suspicious login attempt",
                Severity = "High",
                Category = "Suspicious Login",
                Status = "Open",
                Notes = "Login detected from unknown location."
            });

            incidents.Add(new Incident
            {
                Id = nextId++,
                Title = "Possible phishing email",
                Severity = "Medium",
                Category = "Phishing",
                Status = "In Progress",
                Notes = "User reported a suspicious email."
            });
        }

        private void RefreshList()
        {
            string search = txtSearch.Text.ToLower();
            string filterStatus = GetComboBoxValue(cbFilterStatus);
            string sortBy = GetComboBoxValue(cbSort);

            var results = incidents.Where(i =>
                i.Title.ToLower().Contains(search) ||
                i.Category.ToLower().Contains(search) ||
                i.Notes.ToLower().Contains(search)
            );

            if (filterStatus != "All")
            {
                results = results.Where(i => i.Status == filterStatus);
            }

            if (sortBy == "Severity")
            {
                results = results.OrderBy(i => i.Severity);
            }
            else if (sortBy == "Category")
            {
                results = results.OrderBy(i => i.Category);
            }
            else
            {
                results = results.OrderByDescending(i => i.DateReported);
            }

            lstIncidents.ItemsSource = null;
            lstIncidents.ItemsSource = results.ToList();

            lstIncidents.DisplayMemberPath = "Title";

            UpdateDashboard();
        }

        private void UpdateDashboard()
        {
            int open = incidents.Count(i => i.Status == "Open");
            int resolved = incidents.Count(i => i.Status == "Resolved");
            int high = incidents.Count(i => i.Severity == "High");

            txtDashboard.Text = $"Dashboard: Open: {open} | Resolved: {resolved} | High Priority: {high}";
        }

        private string GetComboBoxValue(ComboBox comboBox)
        {
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            return item?.Content.ToString() ?? "";
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.");
                return;
            }

            Incident incident = new Incident
            {
                Id = nextId++,
                Title = txtTitle.Text,
                Severity = GetComboBoxValue(cbSeverity),
                Category = GetComboBoxValue(cbCategory),
                Status = GetComboBoxValue(cbStatus),
                Notes = txtNotes.Text,
                DateReported = DateTime.Now
            };

            incidents.Add(incident);
            ClearFields();
            RefreshList();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Incident selected = lstIncidents.SelectedItem as Incident;

            if (selected == null)
            {
                MessageBox.Show("Please select an incident to update.");
                return;
            }

            selected.Title = txtTitle.Text;
            selected.Severity = GetComboBoxValue(cbSeverity);
            selected.Category = GetComboBoxValue(cbCategory);
            selected.Status = GetComboBoxValue(cbStatus);
            selected.Notes = txtNotes.Text;

            RefreshList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Incident selected = lstIncidents.SelectedItem as Incident;

            if (selected == null)
            {
                MessageBox.Show("Please select an incident to delete.");
                return;
            }

            incidents.Remove(selected);
            ClearFields();
            RefreshList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                storage.Save(incidents);
                MessageBox.Show("Data saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving data: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                incidents = storage.Load();

                if (incidents.Count > 0)
                {
                    nextId = incidents.Max(i => i.Id) + 1;
                }

                RefreshList();
                MessageBox.Show("Data loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void lstIncidents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Incident selected = lstIncidents.SelectedItem as Incident;

            if (selected == null)
            {
                return;
            }

            txtTitle.Text = selected.Title;
            txtNotes.Text = selected.Notes;

            SetComboBox(cbSeverity, selected.Severity);
            SetComboBox(cbCategory, selected.Category);
            SetComboBox(cbStatus, selected.Status);
        }

        private void SetComboBox(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtNotes.Clear();
            cbSeverity.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshList();
        }

        private void cbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshList();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshList();
        }
    }
}