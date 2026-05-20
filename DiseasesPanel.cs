using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class DiseasesPanel : UserControl, IRefreshable
    {
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private ListBox lstDiseases;
        private Label lblName, lblSymptoms, lblProcedures, lblMedicines;
        private TextBox txtName, txtSymptoms, txtProcedures;
        private ListBox lstMedicines;
        private Button btnAddMed, btnRemoveMed;
 
        private List<Disease> _currentList;
 
        public DiseasesPanel()
        {
            InitializeComponent();
            LoadDiseases();
        }
 
        private void InitializeComponent()
        {
            this.Font = new Font("Segoe UI", 9.5f);
 
            // --- TOP TOOLBAR ---
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5) };
 
            txtSearch = new TextBox { Width = 250, Top = 10, Left = 5, PlaceholderText = "Пошук за назвою або симптомами..." };
            btnSearch = new Button { Text = "Пошук", Width = 80, Top = 8, Left = 265 };
            btnAdd = new Button { Text = "Додати", Width = 80, Top = 8, Left = 360 };
            btnEdit = new Button { Text = "Змінити", Width = 80, Top = 8, Left = 450 };
            btnDelete = new Button { Text = "Видалити", Width = 80, Top = 8, Left = 540 };
 
            btnSearch.Click += (s, e) => LoadDiseases(txtSearch.Text);
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadDiseases(txtSearch.Text); };
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
 
            pnlTop.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnAdd, btnEdit, btnDelete });
 
            // --- LEFT: list ---
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 280, Padding = new Padding(5) };
            lstDiseases = new ListBox { Dock = DockStyle.Fill };
            lstDiseases.SelectedIndexChanged += LstDiseases_SelectedIndexChanged;
            pnlLeft.Controls.Add(lstDiseases);
 
            // --- RIGHT: details ---
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };
 
            lblName = new Label { Text = "Назва:", Top = 0, Left = 0, AutoSize = true };
            txtName = new TextBox { Top = 20, Left = 0, Width = 400, ReadOnly = true, BackColor = Color.WhiteSmoke };
 
            lblSymptoms = new Label { Text = "Симптоми:", Top = 55, Left = 0, AutoSize = true };
            txtSymptoms = new TextBox { Top = 75, Left = 0, Width = 400, Height = 70, Multiline = true, ReadOnly = true, BackColor = Color.WhiteSmoke, ScrollBars = ScrollBars.Vertical };
 
            lblProcedures = new Label { Text = "Процедури:", Top = 160, Left = 0, AutoSize = true };
            txtProcedures = new TextBox { Top = 180, Left = 0, Width = 400, Height = 70, Multiline = true, ReadOnly = true, BackColor = Color.WhiteSmoke, ScrollBars = ScrollBars.Vertical };
 
            lblMedicines = new Label { Text = "Рекомендовані ліки:", Top = 265, Left = 0, AutoSize = true };
            lstMedicines = new ListBox { Top = 285, Left = 0, Width = 400, Height = 120 };
 
            pnlRight.Controls.AddRange(new Control[] { lblName, txtName, lblSymptoms, txtSymptoms, lblProcedures, txtProcedures, lblMedicines, lstMedicines });
 
            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlTop);
        }
 
        private void LoadDiseases(string query = "")
        {
            _currentList = DataStore.Instance.SearchDiseases(query);
            lstDiseases.DataSource = null;
            lstDiseases.DataSource = _currentList;
            lstDiseases.DisplayMember = "Name";
 
            ClearDetails();
        }
 
        private void LstDiseases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDiseases.SelectedItem is Disease d)
                ShowDetails(d);
        }
 
        private void ShowDetails(Disease d)
        {
            txtName.Text = d.Name;
            txtSymptoms.Text = d.Symptoms;
            txtProcedures.Text = d.Procedures;
            lstMedicines.DataSource = null;
            lstMedicines.DataSource = new List<DiseaseMedicine>(d.RecommendedMedicines);
            lstMedicines.DisplayMember = null;
        }
 
        private void ClearDetails()
        {
            txtName.Text = "";
            txtSymptoms.Text = "";
            txtProcedures.Text = "";
            lstMedicines.DataSource = null;
        }
 
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new DiseaseEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                DataStore.Instance.AddDisease(form.Disease);
                LoadDiseases(txtSearch.Text);
            }
        }
 
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (lstDiseases.SelectedItem is Disease d)
            {
                var form = new DiseaseEditForm(d);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DataStore.Instance.UpdateDisease(form.Disease);
                    LoadDiseases(txtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть хворобу для редагування.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
 
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstDiseases.SelectedItem is Disease d)
            {
                if (MessageBox.Show($"Видалити хворобу «{d.Name}»?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataStore.Instance.DeleteDisease(d.Id);
                    LoadDiseases(txtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть хворобу для видалення.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
 
        public new void Refresh()
        {
            LoadDiseases(txtSearch.Text);
        }
    }
}
