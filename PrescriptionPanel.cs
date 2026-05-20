using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class PrescriptionPanel : UserControl, IRefreshable
    {
        private TextBox txtPatient, txtDoctor;
        private ComboBox cmbDisease;
        private ListBox lstItems;
        private Button btnCreate, btnClear;
        private RichTextBox rtbPreview;
        private Label lblStatus;
 
        private List<PrescriptionItem> _items;
 
        public PrescriptionPanel()
        {
            _items = new List<PrescriptionItem>();
            InitializeComponent();
            LoadDiseases();
        }
 
        private void InitializeComponent()
        {
            this.Font = new Font("Segoe UI", 9.5f);
 
            // LEFT: form input
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 400, Padding = new Padding(10) };
 
            int y = 10;
 
            var lblPatient = new Label { Text = "ПІБ пацієнта *:", Top = y, Left = 10, AutoSize = true };
            y += 22;
            txtPatient = new TextBox { Top = y, Left = 10, Width = 360 };
            y += 35;
 
            var lblDoctor = new Label { Text = "ПІБ лікаря *:", Top = y, Left = 10, AutoSize = true };
            y += 22;
            txtDoctor = new TextBox { Top = y, Left = 10, Width = 360 };
            y += 35;
 
            var lblDisease = new Label { Text = "Діагноз *:", Top = y, Left = 10, AutoSize = true };
            y += 22;
            cmbDisease = new ComboBox { Top = y, Left = 10, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDisease.SelectedIndexChanged += CmbDisease_SelectedIndexChanged;
            y += 35;
 
            var lblItems = new Label { Text = "Медикаменти у рецепті:", Top = y, Left = 10, AutoSize = true };
            y += 22;
            lstItems = new ListBox { Top = y, Left = 10, Width = 360, Height = 130 };
            y += 140;
 
            lblStatus = new Label { Top = y, Left = 10, Width = 360, AutoSize = false, Height = 40, ForeColor = Color.DarkRed };
            y += 45;
 
            btnCreate = new Button { Text = "Виписати рецепт", Top = y, Left = 10, Width = 160 };
            btnClear = new Button { Text = "Очистити", Top = y, Left = 185, Width = 100 };
            btnCreate.BackColor = Color.FromArgb(40, 167, 69);
            btnCreate.ForeColor = Color.White;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.Click += BtnCreate_Click;
            btnClear.Click += BtnClear_Click;
 
            pnlLeft.Controls.AddRange(new Control[] {
                lblPatient, txtPatient, lblDoctor, txtDoctor, lblDisease, cmbDisease,
                lblItems, lstItems, lblStatus, btnCreate, btnClear
            });
 
            // RIGHT: recipe preview
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            var lblPreview = new Label { Text = "Попередній перегляд рецепта:", Top = 10, Left = 0, AutoSize = true };
            rtbPreview = new RichTextBox { Top = 30, Left = 0, Dock = DockStyle.Bottom, ReadOnly = true, BackColor = Color.White, Font = new Font("Courier New", 9f) };
            rtbPreview.Height = this.Height - 50;
            pnlRight.Controls.Add(rtbPreview);
            pnlRight.Controls.Add(lblPreview);
 
            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);
        }
 
        private void LoadDiseases()
        {
            cmbDisease.DataSource = null;
            cmbDisease.DataSource = DataStore.Instance.Diseases;
            cmbDisease.DisplayMember = "Name";
            if (cmbDisease.Items.Count > 0)
                cmbDisease.SelectedIndex = 0;
        }
 
        private void CmbDisease_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDisease.SelectedItem is Disease d)
            {
                _items.Clear();
 
                foreach (var dm in d.RecommendedMedicines)
                {
                    var med = DataStore.Instance.Medicines.Find(m => m.Id == dm.MedicineId);
                    bool available = med != null && med.Quantity >= dm.RequiredQuantity;
                    _items.Add(new PrescriptionItem(dm.MedicineId, dm.MedicineName, dm.RequiredQuantity, available));
                }
 
                RefreshItemsList();
                UpdatePreview();
            }
        }
 
        private void RefreshItemsList()
        {
            lstItems.DataSource = null;
            lstItems.DataSource = new List<PrescriptionItem>(_items);
        }
 
        private void UpdatePreview()
        {
            if (!(cmbDisease.SelectedItem is Disease d)) return;
 
            rtbPreview.Clear();
            rtbPreview.AppendText("═══════════════════════════════════\n");
            rtbPreview.AppendText("              РЕЦЕПТ\n");
            rtbPreview.AppendText("═══════════════════════════════════\n");
            rtbPreview.AppendText($"Дата: {DateTime.Now:dd.MM.yyyy}\n");
            rtbPreview.AppendText($"Пацієнт: {txtPatient.Text.Trim()}\n");
            rtbPreview.AppendText($"Лікар:   {txtDoctor.Text.Trim()}\n");
            rtbPreview.AppendText($"Діагноз: {d.Name}\n");
            rtbPreview.AppendText("───────────────────────────────────\n");
            rtbPreview.AppendText("Призначені медикаменти:\n");
 
            foreach (var item in _items)
            {
                string avail = item.IsAvailable ? "✓" : "✗ НЕМАЄ";
                rtbPreview.AppendText($"  • {item.MedicineName} — {item.Quantity} шт.  [{avail}]\n");
            }
 
            rtbPreview.AppendText("───────────────────────────────────\n");
            rtbPreview.AppendText("Процедури:\n");
            rtbPreview.AppendText($"  {d.Procedures}\n");
            rtbPreview.AppendText("═══════════════════════════════════\n");
        }
 
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            var errors = new System.Text.StringBuilder();
 
            string patient = txtPatient.Text.Trim();
            string doctor = txtDoctor.Text.Trim();
 
            if (string.IsNullOrEmpty(patient))
                errors.AppendLine("Вкажіть ПІБ пацієнта.");
            if (string.IsNullOrEmpty(doctor))
                errors.AppendLine("Вкажіть ПІБ лікаря.");
            if (cmbDisease.SelectedItem == null)
                errors.AppendLine("Оберіть діагноз.");
 
            if (errors.Length > 0)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = errors.ToString().Trim();
                return;
            }
 
            var disease = cmbDisease.SelectedItem as Disease;
 
            // Check if any medicines are unavailable
            var missing = _items.FindAll(i => !i.IsAvailable);
            if (missing.Count > 0)
            {
                var msg = "Не вистачає наступних медикаментів:\n";
                foreach (var m in missing) msg += $"  • {m.MedicineName} (потрібно: {m.Quantity})\n";
                msg += "\nВиписати рецепт попри відсутність ліків?";
 
                if (MessageBox.Show(msg, "Недостатньо ліків", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }
 
            // Deduct available medicines from stock
            foreach (var item in _items)
            {
                if (item.IsAvailable)
                {
                    DataStore.Instance.AdjustMedicineStock(item.MedicineId, item.Quantity);
                    item.IsAvailable = true;
                }
            }
 
            var prescription = new Prescription(0, patient, doctor, disease.Name);
            prescription.Items.AddRange(_items);
            DataStore.Instance.AddPrescription(prescription);
            DataStore.Instance.SaveAll();
 
            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Text = $"✓ Рецепт #{prescription.Id} успішно виписано!";
 
            UpdatePreview();
        }
 
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtPatient.Text = "";
            txtDoctor.Text = "";
            _items.Clear();
            RefreshItemsList();
            rtbPreview.Clear();
            lblStatus.Text = "";
        }
 
        public new void Refresh()
        {
            LoadDiseases();
        }
    }
}
