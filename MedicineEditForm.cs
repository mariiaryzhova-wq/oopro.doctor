using System;
using System.Drawing;
using System.Windows.Forms;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Forms
{
    public class MedicineEditForm : Form
    {
        public Medicine Medicine { get; private set; }
 
        private TextBox txtName, txtAlternatives;
        private NumericUpDown numQuantity;
        private Label lblError;
        private Button btnOk, btnCancel;
 
        public MedicineEditForm(Medicine existing)
        {
            Medicine = existing != null
                ? new Medicine(existing.Id, existing.Name, existing.Quantity, existing.Alternatives)
                : new Medicine();
 
            InitializeComponent();
 
            if (existing != null)
            {
                txtName.Text = existing.Name;
                numQuantity.Value = existing.Quantity;
                txtAlternatives.Text = existing.Alternatives ?? "";
            }
 
            this.Text = existing == null ? "Додати медикамент" : "Редагувати медикамент";
        }
 
        private void InitializeComponent()
        {
            this.Size = new Size(430, 290);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);
 
            int y = 15;
 
            var lblName = new Label { Text = "Назва медикаменту *:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            txtName = new TextBox { Top = y, Left = 15, Width = 380 };
            y += 35;
 
            var lblQty = new Label { Text = "Кількість в наявності *:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            numQuantity = new NumericUpDown { Top = y, Left = 15, Width = 120, Minimum = 0, Maximum = 99999 };
            y += 35;
 
            var lblAlt = new Label { Text = "Взаємозамінні препарати:", Top = y, Left = 15, AutoSize = true };
            y += 20;
            txtAlternatives = new TextBox { Top = y, Left = 15, Width = 380, Multiline = true, Height = 50, ScrollBars = ScrollBars.Vertical };
            y += 60;
 
            lblError = new Label { Top = y, Left = 15, Width = 380, ForeColor = Color.Red, AutoSize = false, Height = 18 };
            y += 22;
 
            btnOk = new Button { Text = "OK", Top = y, Left = 220, Width = 80, DialogResult = DialogResult.None };
            btnCancel = new Button { Text = "Скасувати", Top = y, Left = 315, Width = 80, DialogResult = DialogResult.Cancel };
            btnOk.Click += BtnOk_Click;
 
            this.Controls.AddRange(new Control[] { lblName, txtName, lblQty, numQuantity, lblAlt, txtAlternatives, lblError, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
 
        private void BtnOk_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
 
            if (string.IsNullOrEmpty(name))
            {
                lblError.Text = "Назва медикаменту не може бути порожньою.";
                return;
            }
            if (name.Length > 200)
            {
                lblError.Text = "Назва не може перевищувати 200 символів.";
                return;
            }
 
            Medicine.Name = name;
            Medicine.Quantity = (int)numQuantity.Value;
            Medicine.Alternatives = txtAlternatives.Text.Trim();
 
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
