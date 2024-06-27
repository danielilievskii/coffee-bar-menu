using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coffee_bar_demo
{
    public enum ItemType
    {
        Coffee,
        NonCoffee,
        Dessert
    }

    [Serializable]
    public class BarMenuItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public ItemType Category { get; set; }

        public BarMenuItem() { }

        public BarMenuItem(string name, string description, string imagePath, decimal price, ItemType category)
        {
            Name = name;
            Description = description;
            ImagePath = imagePath;
            Price = price;
            Category = category;
        }
    }   
}

