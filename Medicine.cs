using System;
using System.Collections.Generic;
 
namespace DoctorHandbook.Models
{
    [Serializable]
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Alternatives { get; set; } // взаємозамінність
 
        public Medicine() { }
 
        public Medicine(int id, string name, int quantity, string alternatives)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Alternatives = alternatives;
        }
 
        public override string ToString()
        {
            return $"{Name} (в наявності: {Quantity})";
        }
    }
}
 
