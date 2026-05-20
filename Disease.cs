using System;
using System.Collections.Generic;
 
namespace DoctorHandbook.Models
{
    [Serializable]
    public class Disease
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symptoms { get; set; }
        public string Procedures { get; set; }
        public List<DiseaseMedicine> RecommendedMedicines { get; set; }
 
        public Disease()
        {
            RecommendedMedicines = new List<DiseaseMedicine>();
        }
 
        public Disease(int id, string name, string symptoms, string procedures)
        {
            Id = id;
            Name = name;
            Symptoms = symptoms;
            Procedures = procedures;
            RecommendedMedicines = new List<DiseaseMedicine>();
        }
 
        public override string ToString()
        {
            return Name;
        }
    }
}
