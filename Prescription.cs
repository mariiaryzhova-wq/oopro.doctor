using System;
using System.Collections.Generic;
 
namespace DoctorHandbook.Models
{
    [Serializable]
    public class Prescription
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DiagnosisName { get; set; }
        public DateTime Date { get; set; }
        public List<PrescriptionItem> Items { get; set; }
 
        public Prescription()
        {
            Items = new List<PrescriptionItem>();
            Date = DateTime.Now;
        }
 
        public Prescription(int id, string patientName, string doctorName, string diagnosisName)
        {
            Id = id;
            PatientName = patientName;
            DoctorName = doctorName;
            DiagnosisName = diagnosisName;
            Date = DateTime.Now;
            Items = new List<PrescriptionItem>();
        }
 
        public override string ToString()
        {
            return $"Рецепт №{Id} — {PatientName} ({Date:dd.MM.yyyy})";
        }
    }
}
