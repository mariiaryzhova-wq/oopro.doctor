
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;

namespace DoctorHandbook.Forms
{
    public class MedicinesPanel : UserControl, IRefreshable
    {
        private TextBox txtSearch;
        private Button btnSearch, btnAdd, btnEdit, btnDelete;
        private ListView lstMedicines;
        private Label lblName, lblQuantity, lblAlternatives;
        private TextBox txtName, txtAlternatives;
        private NumericUpDown numQuantity;
        private Button btnAdjust;

        private List<Medicine> _currentList;

        public MedicinesPanel()
        {
            InitializeComponent();
            LoadMedicines();
        }

        private void InitializeComponent()
        {
            this.Font = new Font("Segoe UI", 9.5f);

            // --- TOP TOOLBAR ---
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5) };
            txtSearch = new TextBox { Width = 250, Top = 10, Left = 5, PlaceholderText = "Пошук за назвою або замінниками..." };
            btnSearch = new Button { Text = "Пошук", Width = 80, Top = 8, Left = 265 };
            btnAdd = new Button { Text = "Додати", Width = 80, Top = 8, Left = 360 };
            btnEdit = new Button { Text = "Змінити", Width = 80, Top = 8, Left = 450 };
            btnDelete = new Button { Text = "Видалити", Width = 80, Top = 8, Left = 540 };
            btnSearch.Click += (s, e) => LoadMedicines(txtSearch.Text);
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadMedicines(txtSearch.Text); };
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            pnlTop.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnAdd, btnEdit, btnDelete });

            // --- LEFT: ListView ---
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 420, Padding = new Padding(5) };
            lstMedicines = new ListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                View = View.Details,
                MultiSelect = false
            };
            lstMedicines.Columns.Add("Назва", 220);
            lstMedicines.Columns.Add("В наявності", 100);
            lstMedicines.SelectedIndexChanged += LstMedicines_SelectedIndexChanged;
            pnlLeft.Controls.Add(lstMedicines);

            // --- RIGHT: details + stock adjust ---
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };

            lblName = new Label { Text = "Назва:", Top = 0, Left = 0, AutoSize = true };
            txtName = new TextBox { Top = 20, Left = 0, Width = 350, ReadOnly = true, BackColor = Color.WhiteSmoke };

            lblQuantity = new Label { Text = "Кількість в наявності:", Top = 60, Left = 0, AutoSize = true };
            var numDisplay = new TextBox { Top = 80, Left = 0, Width = 100, ReadOnly = true, BackColor = Color.WhiteSmoke, Name = "numDisplay" };

            lblAlternatives = new Label { Text = "Взаємозамінні препарати:", Top = 120, Left = 0, AutoSize = true };
            txtAlternatives = new TextBox { Top = 140, Left = 0, Width = 350, Multiline = true, Height = 60, ReadOnly = true, BackColor = Color.WhiteSmoke };

            var lblAdjust = new Label { Text = "Коригування запасу:", Top = 220, Left = 0, AutoSize = true };
            numQuantity = new NumericUpDown { Top = 240, Left = 0, Width = 100, Minimum = -9999, Maximum = 9999, Value = 0 };
            btnAdjust = new Button { Text = "Застосувати", Top = 238, Left = 115, Width = 100 };
            btnAdjust.Click += BtnAdjust_Click;

            pnlRight.Controls.AddRange(new Control[] { lblName, txtName, lblQuantity, numDisplay, lblAlternatives, txtAlternatives, lblAdjust, numQuantity, btnAdjust });

            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlTop);
        }

        private void LoadMedicines(string query = "")
        {
            _currentList = DataStore.Instance.SearchMedicines(query);
            lstMedicines.Items.Clear();
            foreach (var m in _currentList)
            {
                var item = new ListViewItem(m.Name);
                item.SubItems.Add(m.Quantity.ToString());
                item.Tag = m;
                if (m.Quantity == 0) item.ForeColor = Color.Red;
                else if (m.Quantity < 10) item.ForeColor = Color.DarkOrange;
                lstMedicines.Items.Add(item);
            }
            ClearDetails();
        }

        private void LstMedicines_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMedicines.SelectedItems.Count > 0 && lstMedicines.SelectedItems[0].Tag is Medicine m)
                ShowDetails(m);
        }

        private void ShowDetails(Medicine m)
        {
            var nameBox = this.Controls.Find("", false);
            // Use the controls directly
            foreach (Control c in this.Controls)
                if (c is Panel p)
                    foreach (Control pc in p.Controls)
                    {
                        if (pc.Name == "numDisplay") pc.Text = m.Quantity.ToString();
                    }

            // find in pnlRight
            foreach (Control c in this.Controls)
            {
                if (c is Panel p && p.Dock == DockStyle.Fill)
                {
                    foreach (Control pc in p.Controls)
                    {
                        if (pc == txtName) txtName.Text = m.Name;
                        if (pc.Name == "numDisplay") pc.Text = m.Quantity.ToString();
                        if (pc == txtAlternatives) txtAlternatives.Text = m.Alternatives ?? "";
                    }
                }
            }

            txtName.Text = m.Name;
            txtAlternatives.Text = m.Alternatives ?? "";
        }

        private void ClearDetails()
        {
            txtName.Text = "";
            txtAlternatives.Text = "";
        }

        private Medicine GetSelectedMedicine()
        {
            if (lstMedicines.SelectedItems.Count > 0)
                return lstMedicines.SelectedItems[0].Tag as Medicine;
            return null;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new MedicineEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                DataStore.Instance.AddMedicine(form.Medicine);
                LoadMedicines(txtSearch.Text);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMedicine();
            if (m != null)
            {
                var form = new MedicineEditForm(m);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    DataStore.Instance.UpdateMedicine(form.Medicine);
                    LoadMedicines(txtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть медикамент для редагування.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMedicine();
            if (m != null)
            {
                if (MessageBox.Show($"Видалити медикамент «{m.Name}»?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataStore.Instance.DeleteMedicine(m.Id);
                    LoadMedicines(txtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть медикамент для видалення.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnAdjust_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMedicine();
            if (m == null)
            {
                MessageBox.Show("Оберіть медикамент.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int delta = (int)numQuantity.Value;
            int newQty = m.Quantity + delta;
            if (newQty < 0)
            {
                MessageBox.Show($"Недостатньо запасу. Поточна кількість: {m.Quantity}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            m.Quantity = newQty;
            DataStore.Instance.UpdateMedicine(m);
            numQuantity.Value = 0;
            LoadMedicines(txtSearch.Text);
            MessageBox.Show($"Запас оновлено. Нова кількість: {m.Quantity}", "Успішно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public new void Refresh()
        {
            LoadMedicines(txtSearch.Text);
        }
    }
}
