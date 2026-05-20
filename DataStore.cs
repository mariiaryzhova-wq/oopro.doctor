using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DoctorHandbook.Models;
 
namespace DoctorHandbook.Data
{
    public class DataStore
    {
        private static DataStore _instance;
        public static DataStore Instance => _instance ?? (_instance = new DataStore());
 
        private readonly string _diseasesFile = "diseases.json";
        private readonly string _medicinesFile = "medicines.json";
        private readonly string _prescriptionsFile = "prescriptions.json";
 
        public List<Disease> Diseases { get; private set; }
        public List<Medicine> Medicines { get; private set; }
        public List<Prescription> Prescriptions { get; private set; }
 
        private int _nextDiseaseId = 1;
        private int _nextMedicineId = 1;
        private int _nextPrescriptionId = 1;
 
        private DataStore()
        {
            Diseases = new List<Disease>();
            Medicines = new List<Medicine>();
            Prescriptions = new List<Prescription>();
        }
 
        // ===================== DISEASES =====================
 
        public void AddDisease(Disease disease)
        {
            disease.Id = _nextDiseaseId++;
            Diseases.Add(disease);
        }
 
        public void UpdateDisease(Disease updated)
        {
            var index = Diseases.FindIndex(d => d.Id == updated.Id);
            if (index >= 0)
                Diseases[index] = updated;
        }
 
        public void DeleteDisease(int id)
        {
            Diseases.RemoveAll(d => d.Id == id);
        }
 
        public List<Disease> SearchDiseases(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Disease>(Diseases);
 
            query = query.Trim().ToLower();
            return Diseases.FindAll(d =>
                d.Name.ToLower().Contains(query) ||
                d.Symptoms.ToLower().Contains(query));
        }
 
        // ===================== MEDICINES =====================
 
        public void AddMedicine(Medicine medicine)
        {
            medicine.Id = _nextMedicineId++;
            Medicines.Add(medicine);
        }
 
        public void UpdateMedicine(Medicine updated)
        {
            var index = Medicines.FindIndex(m => m.Id == updated.Id);
            if (index >= 0)
                Medicines[index] = updated;
        }
 
        public void DeleteMedicine(int id)
        {
            Medicines.RemoveAll(m => m.Id == id);
        }
 
        public List<Medicine> SearchMedicines(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Medicine>(Medicines);
 
            query = query.Trim().ToLower();
            return Medicines.FindAll(m =>
                m.Name.ToLower().Contains(query) ||
                (m.Alternatives != null && m.Alternatives.ToLower().Contains(query)));
        }
 
        public bool AdjustMedicineStock(int medicineId, int quantityUsed)
        {
            var med = Medicines.Find(m => m.Id == medicineId);
            if (med == null) return false;
            if (med.Quantity < quantityUsed) return false;
            med.Quantity -= quantityUsed;
            return true;
        }
 
        // ===================== PRESCRIPTIONS =====================
 
        public void AddPrescription(Prescription prescription)
        {
            prescription.Id = _nextPrescriptionId++;
            Prescriptions.Add(prescription);
        }
 
        public void DeletePrescription(int id)
        {
            Prescriptions.RemoveAll(p => p.Id == id);
        }
 
        public List<Prescription> SearchPrescriptions(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Prescription>(Prescriptions);
 
            query = query.Trim().ToLower();
            return Prescriptions.FindAll(p =>
                p.PatientName.ToLower().Contains(query) ||
                p.DiagnosisName.ToLower().Contains(query));
        }
 
        // ===================== SERIALIZATION =====================
 
        public void SaveAll()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_diseasesFile, JsonSerializer.Serialize(Diseases, options));
            File.WriteAllText(_medicinesFile, JsonSerializer.Serialize(Medicines, options));
            File.WriteAllText(_prescriptionsFile, JsonSerializer.Serialize(Prescriptions, options));
        }
 
        public void LoadAll()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
 
            if (File.Exists(_diseasesFile))
            {
                Diseases = JsonSerializer.Deserialize<List<Disease>>(File.ReadAllText(_diseasesFile), options)
                           ?? new List<Disease>();
                if (Diseases.Count > 0)
                    _nextDiseaseId = Diseases[Diseases.Count - 1].Id + 1;
            }
 
            if (File.Exists(_medicinesFile))
            {
                Medicines = JsonSerializer.Deserialize<List<Medicine>>(File.ReadAllText(_medicinesFile), options)
                            ?? new List<Medicine>();
                if (Medicines.Count > 0)
                    _nextMedicineId = Medicines[Medicines.Count - 1].Id + 1;
            }
 
            if (File.Exists(_prescriptionsFile))
            {
                Prescriptions = JsonSerializer.Deserialize<List<Prescription>>(File.ReadAllText(_prescriptionsFile), options)
                                ?? new List<Prescription>();
                if (Prescriptions.Count > 0)
                    _nextPrescriptionId = Prescriptions[Prescriptions.Count - 1].Id + 1;
            }
        }
 
        public void SeedSampleData()
        {
            if (Medicines.Count > 0) return;
 
            AddMedicine(new Medicine(0, "Аспірин", 50, "Парацетамол, Ібупрофен"));
            AddMedicine(new Medicine(0, "Парацетамол", 100, "Аспірин"));
            AddMedicine(new Medicine(0, "Ібупрофен", 80, "Аспірин, Диклофенак"));
            AddMedicine(new Medicine(0, "Амоксицилін", 30, "Ампіцилін"));
            AddMedicine(new Medicine(0, "Ампіцилін", 20, "Амоксицилін"));
            AddMedicine(new Medicine(0, "Диклофенак", 60, "Ібупрофен"));
            AddMedicine(new Medicine(0, "Но-шпа", 45, "Дротаверин"));
            AddMedicine(new Medicine(0, "Лоратадин", 35, "Цетиризин"));
 
            var d1 = new Disease(0, "Грип", "Висока температура, кашель, нежить, слабкість", "Постільний режим, рясне пиття");
            d1.RecommendedMedicines.Add(new DiseaseMedicine(1, "Аспірин", 10));
            d1.RecommendedMedicines.Add(new DiseaseMedicine(2, "Парацетамол", 15));
            AddDisease(d1);
 
            var d2 = new Disease(0, "Ангіна", "Біль у горлі, температура, набряк мигдалин", "Полоскання горла, антибіотикотерапія");
            d2.RecommendedMedicines.Add(new DiseaseMedicine(4, "Амоксицилін", 14));
            d2.RecommendedMedicines.Add(new DiseaseMedicine(2, "Парацетамол", 10));
            AddDisease(d2);
 
            var d3 = new Disease(0, "Остеохондроз", "Біль у спині, оніміння кінцівок, обмеження руху", "Фізіотерапія, масаж, ЛФК");
            d3.RecommendedMedicines.Add(new DiseaseMedicine(3, "Ібупрофен", 20));
            d3.RecommendedMedicines.Add(new DiseaseMedicine(6, "Диклофенак", 10));
            AddDisease(d3);
        }
    }
}
