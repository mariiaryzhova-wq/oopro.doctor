using System;
 
namespace DoctorHandbook.Models
{
    [Serializable]
    public class PrescriptionItem
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
 
        public PrescriptionItem() { }
 
        public PrescriptionItem(int medicineId, string medicineName, int quantity, bool isAvailable)
        {
            MedicineId = medicineId;
            MedicineName = medicineName;
            Quantity = quantity;
            IsAvailable = isAvailable;
        }
 
        public override string ToString()
        {
            string status = IsAvailable ? "є в наявності" : "немає в наявності";
            return $"{MedicineName} — {Quantity} шт. ({status})";
        }
    }
}
 
