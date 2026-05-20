using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage tabDiseases;
        private TabPage tabMedicines;
        private TabPage tabPrescription;
        private TabPage tabPrescriptionHistory;
 
        public MainForm()
        {
            InitializeComponent();
            DataStore.Instance.LoadAll();
            DataStore.Instance.SeedSampleData();
        }
 
        private void InitializeComponent()
        {
            this.Text = "Довідник лікаря";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.FormClosing += MainForm_FormClosing;
 
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10f);
 
            tabDiseases = new TabPage("Хвороби");
            tabMedicines = new TabPage("Медикаменти");
            tabPrescription = new TabPage("Виписати рецепт");
            tabPrescriptionHistory = new TabPage("Рецепти");
 
            tabControl.TabPages.Add(tabDiseases);
            tabControl.TabPages.Add(tabMedicines);
            tabControl.TabPages.Add(tabPrescription);
            tabControl.TabPages.Add(tabPrescriptionHistory);
 
            // Embed user controls into tabs
            var diseasesPanel = new DiseasesPanel();
            diseasesPanel.Dock = DockStyle.Fill;
            tabDiseases.Controls.Add(diseasesPanel);
 
            var medicinesPanel = new MedicinesPanel();
            medicinesPanel.Dock = DockStyle.Fill;
            tabMedicines.Controls.Add(medicinesPanel);
 
            var prescriptionPanel = new PrescriptionPanel();
            prescriptionPanel.Dock = DockStyle.Fill;
            tabPrescription.Controls.Add(prescriptionPanel);
 
            var historyPanel = new PrescriptionHistoryPanel();
            historyPanel.Dock = DockStyle.Fill;
            tabPrescriptionHistory.Controls.Add(historyPanel);
 
            // Refresh panels when switching tabs
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                foreach (Control c in tabControl.SelectedTab.Controls)
                {
                    if (c is IRefreshable r) r.Refresh();
                }
            };
 
            this.Controls.Add(tabControl);
        }
 
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataStore.Instance.SaveAll();
        }
    }
 
    public interface IRefreshable
    {
        new void Refresh();
    }
}
