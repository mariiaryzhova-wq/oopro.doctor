using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class DiseaseEditForm : Form
    {
        public Disease Disease { get; private set; }
 
        private TextBox txtName, txtSymptoms, txtProcedures;
        private ListBox lstMeds;
        private Button btnAddMed, btnRemoveMed, btnOk, btnCancel;
        private Label lblErrors;
 
        public DiseaseEditForm(Disease existing)
        {
            Disease = existing != null
                ? new Disease(existing.Id, existing.Name, existing.Symptoms, existing.Procedures)
                : new Disease();
 
            if (existing != null)
                foreach (var m in existing.RecommendedMedicines)
                    Disease.RecommendedMedicines.Add(new DiseaseMedicine(m.MedicineId, m.MedicineName, m.RequiredQuantity));
 
            InitializeComponent();
 
            if (existing != null)
            {
                txtName.Text = existing.Name;
                txtSymptoms.Text = existing.Symptoms;
                txtProcedures.Text = existing.Procedures;
                RefreshMedList();
            }
 
            this.Text = existing == null ? "Додати хворобу" : "Редагувати хворобу";
        }
 
        private void InitializeComponent()
        {
            this.Size = new Size(520, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);
 
            int y = 15;
 
            var lblName = new Label { Text = "Назва хвороби *:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            txtName = new TextBox { Top = y, Left = 15, Width = 470 };
            y += 35;
 
            var lblSymptoms = new Label { Text = "Симптоми *:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            txtSymptoms = new TextBox { Top = y, Left = 15, Width = 470, Height = 65, Multiline = true, ScrollBars = ScrollBars.Vertical };
            y += 75;
 
            var lblProcedures = new Label { Text = "Процедури:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            txtProcedures = new TextBox { Top = y, Left = 15, Width = 470, Height = 65, Multiline = true, ScrollBars = ScrollBars.Vertical };
            y += 75;
 
            var lblMeds = new Label { Text = "Рекомендовані ліки:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            lstMeds = new ListBox { Top = y, Left = 15, Width = 350, Height = 90 };
            btnAddMed = new Button { Text = "Додати", Top = y, Left = 375, Width = 110 };
            btnRemoveMed = new Button { Text = "Видалити", Top = y + 30, Left = 375, Width = 110 };
            btnAddMed.Click += BtnAddMed_Click;
            btnRemoveMed.Click += BtnRemoveMed_Click;
            y += 100;
 
            lblErrors = new Label { Top = y, Left = 15, Width = 470, ForeColor = Color.Red, AutoSize = false, Height = 20 };
            y += 25;
 
            btnOk = new Button { Text = "OK", Top = y, Left = 310, Width = 80, DialogResult = DialogResult.None };
            btnCancel = new Button { Text = "Скасувати", Top = y, Left = 405, Width = 80, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;
 
            this.Controls.AddRange(new Control[] {
                lblName, txtName, lblSymptoms, txtSymptoms, lblProcedures, txtProcedures,
                lblMeds, lstMeds, btnAddMed, btnRemoveMed, lblErrors, btnOk, btnCancel
            });
 
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
 
        private void RefreshMedList()
        {
            lstMeds.DataSource = null;
            lstMeds.DataSource = new List<DiseaseMedicine>(Disease.RecommendedMedicines);
        }
 
        private void BtnAddMed_Click(object sender, EventArgs e)
        {
            var form = new SelectMedicineForm();
            if (form.ShowDialog() == DialogResult.OK && form.SelectedMedicine != null)
            {
                var med = form.SelectedMedicine;
                Disease.RecommendedMedicines.Add(new DiseaseMedicine(med.Id, med.Name, form.Quantity));
                RefreshMedList();
            }
        }
 
        private void BtnRemoveMed_Click(object sender, EventArgs e)
        {
            if (lstMeds.SelectedItem is DiseaseMedicine dm)
            {
                Disease.RecommendedMedicines.Remove(dm);
                RefreshMedList();
            }
        }
 
        private void BtnOk_Click(object sender, EventArgs e)
        {
            var errors = new System.Text.StringBuilder();
 
            string name = txtName.Text.Trim();
            string symptoms = txtSymptoms.Text.Trim();
 
            if (string.IsNullOrEmpty(name))
                errors.AppendLine("Назва хвороби не може бути порожньою.");
            else if (name.Length > 200)
                errors.AppendLine("Назва не може перевищувати 200 символів.");
 
            if (string.IsNullOrEmpty(symptoms))
                errors.AppendLine("Симптоми не можуть бути порожніми.");
 
            if (errors.Length > 0)
            {
                lblErrors.Text = errors.ToString().Trim();
                return;
            }
 
            Disease.Name = name;
            Disease.Symptoms = symptoms;
            Disease.Procedures = txtProcedures.Text.Trim();
 
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
