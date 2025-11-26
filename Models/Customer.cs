using System.Collections.Generic;

namespace shreeji_packaging.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public List<BoxRecord> Records { get; set; } = new List<BoxRecord>();

        public Customer(string name)
        {
            Name = name;
        }
    }
}
