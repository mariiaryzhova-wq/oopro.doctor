using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class PrescriptionHistoryPanel : UserControl, IRefreshable
    {
        private TextBox txtSearch;
        private Button btnSearch, btnDelete;
        private ListView lstPrescriptions;
        private RichTextBox rtbDetails;
 
        private List<Prescription> _currentList;
 
        public PrescriptionHistoryPanel()
        {
            InitializeComponent();
            LoadPrescriptions();
        }
 
        private void InitializeComponent()
        {
            this.Font = new Font("Segoe UI", 9.5f);
 
            // TOP toolbar
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(5) };
            txtSearch = new TextBox { Width = 250, Top = 10, Left = 5, PlaceholderText = "Пошук за пацієнтом або діагнозом..." };
            btnSearch = new Button { Text = "Пошук", Width = 80, Top = 8, Left = 265 };
            btnDelete = new Button { Text = "Видалити", Width = 80, Top = 8, Left = 360 };
            btnSearch.Click += (s, e) => LoadPrescriptions(txtSearch.Text);
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadPrescriptions(txtSearch.Text); };
            btnDelete.Click += BtnDelete_Click;
            pnlTop.Controls.AddRange(new Control[] { txtSearch, btnSearch, btnDelete });
 
            // LEFT: list
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 380, Padding = new Padding(5) };
            lstPrescriptions = new ListView
            {
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                GridLines = true,
                View = View.Details,
                MultiSelect = false
            };
            lstPrescriptions.Columns.Add("№", 45);
            lstPrescriptions.Columns.Add("Пацієнт", 160);
            lstPrescriptions.Columns.Add("Дата", 90);
            lstPrescriptions.Columns.Add("Діагноз", 60);
            lstPrescriptions.SelectedIndexChanged += LstPrescriptions_SelectedIndexChanged;
            pnlLeft.Controls.Add(lstPrescriptions);
 
            // RIGHT: details
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };
            var lblDetails = new Label { Text = "Деталі рецепта:", Top = 0, Left = 0, AutoSize = true };
            rtbDetails = new RichTextBox
            {
                Top = 20,
                Left = 0,
                Dock = DockStyle.Bottom,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Courier New", 9f)
            };
            rtbDetails.Height = this.Height - 30;
            pnlRight.Controls.Add(rtbDetails);
            pnlRight.Controls.Add(lblDetails);
 
            this.Controls.Add(pnlRight);
            this.Controls.Add(pnlLeft);
            this.Controls.Add(pnlTop);
        }
 
        private void LoadPrescriptions(string query = "")
        {
            _currentList = DataStore.Instance.SearchPrescriptions(query);
            lstPrescriptions.Items.Clear();
            foreach (var p in _currentList)
            {
                var item = new ListViewItem(p.Id.ToString());
                item.SubItems.Add(p.PatientName);
                item.SubItems.Add(p.Date.ToString("dd.MM.yyyy"));
                item.SubItems.Add(p.DiagnosisName);
                item.Tag = p;
                lstPrescriptions.Items.Add(item);
            }
            rtbDetails.Clear();
        }
 
        private void LstPrescriptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPrescriptions.SelectedItems.Count > 0 && lstPrescriptions.SelectedItems[0].Tag is Prescription p)
                ShowDetails(p);
        }
 
        private void ShowDetails(Prescription p)
        {
            rtbDetails.Clear();
            rtbDetails.AppendText("═══════════════════════════════════\n");
            rtbDetails.AppendText($"         РЕЦЕПТ №{p.Id}\n");
            rtbDetails.AppendText("═══════════════════════════════════\n");
            rtbDetails.AppendText($"Дата:    {p.Date:dd.MM.yyyy HH:mm}\n");
            rtbDetails.AppendText($"Пацієнт: {p.PatientName}\n");
            rtbDetails.AppendText($"Лікар:   {p.DoctorName}\n");
            rtbDetails.AppendText($"Діагноз: {p.DiagnosisName}\n");
            rtbDetails.AppendText("───────────────────────────────────\n");
            rtbDetails.AppendText("Призначені медикаменти:\n");
            foreach (var item in p.Items)
            {
                string avail = item.IsAvailable ? "видано" : "відсутнє";
                rtbDetails.AppendText($"  • {item.MedicineName} — {item.Quantity} шт.  [{avail}]\n");
            }
            rtbDetails.AppendText("═══════════════════════════════════\n");
        }
 
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstPrescriptions.SelectedItems.Count > 0 && lstPrescriptions.SelectedItems[0].Tag is Prescription p)
            {
                if (MessageBox.Show($"Видалити рецепт №{p.Id} пацієнта «{p.PatientName}»?", "Підтвердження",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataStore.Instance.DeletePrescription(p.Id);
                    LoadPrescriptions(txtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Оберіть рецепт для видалення.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
 
        public new void Refresh()
        {
            LoadPrescriptions(txtSearch.Text);
        }
    }
}
