using System;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Data;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class SelectMedicineForm : Form
    {
        public Medicine SelectedMedicine { get; private set; }
        public int Quantity { get; private set; }
 
        private ListBox lstMedicines;
        private NumericUpDown numQuantity;
        private Button btnOk, btnCancel;
        private Label lblError;
 
        public SelectMedicineForm()
        {
            InitializeComponent();
            lstMedicines.DataSource = DataStore.Instance.Medicines;
            lstMedicines.DisplayMember = "Name";
        }
 
        private void InitializeComponent()
        {
            this.Text = "Обрати медикамент";
            this.Size = new Size(350, 310);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);
 
            var lblMed = new Label { Text = "Медикамент:", Top = 15, Left = 15, AutoSize = true };
            lstMedicines = new ListBox { Top = 35, Left = 15, Width = 300, Height = 150 };
 
            var lblQty = new Label { Text = "Кількість:", Top = 200, Left = 15, AutoSize = true };
            numQuantity = new NumericUpDown { Top = 220, Left = 15, Width = 100, Minimum = 1, Maximum = 9999, Value = 1 };
 
            lblError = new Label { Top = 248, Left = 15, Width = 300, ForeColor = Color.Red, AutoSize = false, Height = 18 };
 
            btnOk = new Button { Text = "OK", Top = 235, Left = 200, Width = 60, DialogResult = DialogResult.None };
            btnCancel = new Button { Text = "Скасувати", Top = 235, Left = 268, Width = 70, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;
 
            this.Controls.AddRange(new Control[] { lblMed, lstMedicines, lblQty, numQuantity, lblError, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
 
        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (lstMedicines.SelectedItem is Medicine m)
            {
                SelectedMedicine = m;
                Quantity = (int)numQuantity.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblError.Text = "Оберіть медикамент зі списку.";
            }
        }
    }
}
