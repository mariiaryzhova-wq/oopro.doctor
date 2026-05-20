using System;
 
namespace DoctorHandbook.Models
{
    [Serializable]
    public class DiseaseMedicine
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int RequiredQuantity { get; set; }
 
        public DiseaseMedicine() { }
 
        public DiseaseMedicine(int medicineId, string medicineName, int requiredQuantity)
        {
            MedicineId = medicineId;
            MedicineName = medicineName;
            RequiredQuantity = requiredQuantity;
        }
 
        public override string ToString()
        {
            return $"{MedicineName} — {RequiredQuantity} шт.";
        }
    }
}
